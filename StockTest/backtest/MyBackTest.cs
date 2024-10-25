using My;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.Analysis; 
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Data;
using MathNet.Numerics.Statistics; 
using MathNet.Numerics.LinearAlgebra;
using Accord.Math;
using MathNet.Numerics;
using ZedGraph;
namespace StockTest
{
    public class MyBackTest
    {
        public static double CalculateEuclideanDistance(double[] features1, double[] features2)
        {
            return Distance.Euclidean(features1, features2);
        }

        public static double CalculateCosineSimilarity(double[] vectorA, double[] vectorB)
        {
            //return Distance.Cosine(features1, features2);

            double dotProduct = vectorA.Zip(vectorB, (a, b) => a * b).Sum();

            // 计算向量的范数（长度）
            double magnitudeA = Math.Sqrt(vectorA.Sum(a => a * a));
            double magnitudeB = Math.Sqrt(vectorB.Sum(b => b * b));

            if (magnitudeA * magnitudeB == 0)
                return 0;

            // 计算余弦相似度
            return dotProduct / (magnitudeA * magnitudeB);
        }

        public static double CalculateCosineSimilarity2(double[] vectorA, double[] vectorB)
        {
            // 計算權重，讓數列的最後一筆數據比重更高
            double[] weights = new double[vectorA.Length];
            double totalWeight = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = Math.Pow(2, i); // 這裡使用指數權重，你可以根據需要調整
                totalWeight += weights[i];
            }

            // 計算加權的點積
            double weightedDotProduct = 0;
            for (int i = 0; i < vectorA.Length; i++)
            {
                weightedDotProduct += vectorA[i] * vectorB[i] * weights[i];
            }

            // 計算加權的向量范數（長度）
            double weightedMagnitudeA = 0;
            double weightedMagnitudeB = 0;
            for (int i = 0; i < vectorA.Length; i++)
            {
                weightedMagnitudeA += vectorA[i] * vectorA[i] * weights[i];
                weightedMagnitudeB += vectorB[i] * vectorB[i] * weights[i];
            }
            weightedMagnitudeA = Math.Sqrt(weightedMagnitudeA);
            weightedMagnitudeB = Math.Sqrt(weightedMagnitudeB);

            if (weightedMagnitudeA * weightedMagnitudeB == 0)
                return 0;

            // 計算加權的餘弦相似度
            return weightedDotProduct / (weightedMagnitudeA * weightedMagnitudeB);
        }

        public static double[] ExtractFeatures(List<Kline> klines, int start, int end)
        {
            var features = new List<double>();
            var closes = klines.GetRange(start, end - start + 1).Select(k => k.close).ToArray();
            var sma5 = CalculateSMA(klines, 5).Skip(start).Take(end - start + 1).ToArray();
            var sma20 = CalculateSMA(klines, 20).Skip(start).Take(end - start + 1).ToArray();
            var sma60 = CalculateSMA(klines, 60).Skip(start).Take(end - start + 1).ToArray();
            var sma120 = CalculateSMA(klines, 120).Skip(start).Take(end - start + 1).ToArray();
            //var rsi = CalculateRSI(klines, 14).Skip(start).Take(end - start + 1).ToArray();

            features.AddRange(closes);
            features.AddRange(sma5);
            features.AddRange(sma20);
            features.AddRange(sma60);
            features.AddRange(sma120);
            //features.AddRange(rsi);

            return features.ToArray();
        }

        public static double[] CalculateSMA(List<Kline> klines, int period)
        {
            double[] sma = new double[klines.Count];
            for (int i = 0; i < klines.Count; i++)
            {
                if (i < period - 1)
                {
                    sma[i] = double.NaN;
                }
                else
                {
                    double sum = 0;
                    for (int j = 0; j < period; j++)
                    {
                        sum += klines[i - j].close;
                    }
                    sma[i] = sum / period;
                }
            }
            return sma;
        }

        public static double[] CalculateRSI(List<Kline> klines, int period)
        {
            double[] rsi = new double[klines.Count];
            double gain = 0, loss = 0;

            for (int i = 1; i <= period; i++)
            {
                double change = klines[i].close - klines[i - 1].close;
                if (change > 0) gain += change;
                else loss -= change;
            }

            gain /= period;
            loss /= period;

            rsi[period] = 100 - 100 / (1 + gain / loss);

            for (int i = period + 1; i < klines.Count; i++)
            {
                double change = klines[i].close - klines[i - 1].close;
                if (change > 0)
                {
                    gain = (gain * (period - 1) + change) / period;
                    loss = (loss * (period - 1)) / period;
                }
                else
                {
                    gain = (gain * (period - 1)) / period;
                    loss = (loss * (period - 1) - change) / period;
                }

                rsi[i] = 100 - 100 / (1 + gain / loss);
            }

            return rsi;
        }

        public static double[] Normalize(double[] values)
        {
            var filteredValues = values.Where(v => !double.IsNaN(v) && !double.IsInfinity(v)).ToArray();

            // 如果過濾後沒有值，返回全零數組
            if (filteredValues.Length == 0)
            {
                return values.Select(v => 0.0).ToArray();
            }

            var mean = filteredValues.Mean();
            var stdDev = filteredValues.StandardDeviation();
            if (stdDev == 0)
            {
                // 如果標準差為零，返回全零數組
                return values.Select(v => 0.0).ToArray();
            }
            return values.Select(v => (v - mean) / stdDev).ToArray();
        }

        public static void GetOneJob(string gaunit, string stockno, string txtPeriod, string start, string end)
        {
            DateTime sDate = new DateTime(int.Parse(start.Substring(0, 4)), 1, 1);
            DateTime eDate = new DateTime(int.Parse(end.Substring(0, 4)), 1, 1);
            DataFrame df = MyGraphic.GetDateData(stockno, txtPeriod.ToString(), sDate, eDate);

            GASignal GASiganals = new GASignal(stockno);
            List<int> GAUNIT = gaunit.Split(',').Select(int.Parse).ToList();
            double fitness = GASiganals.GenProfit(GAUNIT, df);
            FunLog.Write2OneFileAppend(stockno + "," + GASiganals.Profit.ToString() + "," + GASiganals.TradeCount.ToString(), "Reaslt");
            //string datastr = "LastGain:" + GASiganals.LastGain.ToString() + "  LastMoney:" + (GASiganals.LastMoney / 10000).ToString() + "  DropDown:" + GASiganals.DropDown.ToString();

            //GetMaxDrowDown();
            return;
        }

        public static List<Kline> ReadKlines(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture))) 
            {
                return new List<Kline>(csv.GetRecords<Kline>());
            }
        }

        public static void DataTableToCsv(DataTable dataTable, string filePath)
        {
            StringBuilder csvContent = new StringBuilder();

            // 寫入標題行
            string[] columnNames = dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
            csvContent.AppendLine(string.Join(",", columnNames));

            // 寫入資料行
            foreach (DataRow row in dataTable.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                csvContent.AppendLine(string.Join(",", fields));
            }

            // 將內容寫入 CSV 檔案
            File.WriteAllText(filePath, csvContent.ToString());
        }

        public static List<string> GetStockList()
        {
            string[] files = FunLog.GetFolderFile("database");
            List<string> stockList = new List<string>();
            foreach (string item in files)
            {
                string[] parts = item.Split('_');
                if (parts.Length > 0)
                {
                    string stockno = parts[0];
                    if (!stockList.Contains(stockno))
                        stockList.Add(stockno);
                }
            } 
            return stockList;
        }

        public static string GetSimStock(string StockNo)
        {
            List<string> stockList = GetStockList();
            double maxSim = 1000;
            double bestLag = 0;
            string simStock = "";
            foreach (string stk in stockList)
            {
                if(stk!= StockNo)
                {
                    double[] sim= GetSim(StockNo, stk);
                    if (sim[0] < maxSim && sim[2]!=0)
                    {
                        maxSim = sim[0];
                        simStock = stk;
                        bestLag = sim[2];
                    }
                }
            } 
            return simStock + "_" + maxSim.ToString() + "_" + bestLag.ToString();
        }

        public static double[] CrossCorrelate(double[] x, double[] y)
        {
            int n = x.Length;
            double[] result = new double[2 * n - 1];

            for (int lag = -n + 1; lag < n; lag++)
            {
                double sum = 0;
                for (int i = 0; i < n; i++)
                {
                    int j = i + lag;
                    if (j >= 0 && j < n)
                    {
                        sum += x[i] * y[j];
                    }
                }
                result[lag + n - 1] = sum;
            }

            return result;
        }

        public static int FindBestLag(double[] x, double[] y)
        {
            double[] crossCorrelation = CrossCorrelate(x, y);
            int bestLag = 0;
            double maxCorrelation = double.MinValue;

            for (int i = 0; i < crossCorrelation.Length; i++)
            {
                Console.WriteLine($"Lag: {i - x.Length + 1}, Correlation: {crossCorrelation[i]}");
                if (crossCorrelation[i] > maxCorrelation)
                {
                    maxCorrelation = crossCorrelation[i];
                    bestLag = i - x.Length + 1;
                }
            }

            Console.WriteLine($"Best Lag: {bestLag}, Max Correlation: {maxCorrelation}");
            return bestLag;
        }

        public static double[] ApplyLag(double[] data, int lag)
        {
            double[] result = new double[data.Length];
            if (lag > 0)
            {
                Array.Copy(data, 0, result, lag, data.Length - lag);
            }
            else if (lag < 0)
            {
                Array.Copy(data, -lag, result, 0, data.Length + lag);
            }
            else
            {
                Array.Copy(data, result, data.Length);
            }
            return result;
        }

        public static double[] GetSim(string StockNo1,string StockNo2)
        {
            string filename = StockNo1 + "_W-SAT_20200101_20250101";
            List<Kline> klines1 = ReadKlines("database\\" + filename + ".csv");
            filename = StockNo2 + "_W-SAT_20200101_20250101";
            List<Kline> klines2 = ReadKlines("database\\" + filename + ".csv"); 
            int N = 40; // 最近N天
            if(klines1.Count < N || klines2.Count < N)
                return new double[] { 0, 0 };
            var features1 = ExtractFeatures(klines1, klines1.Count - N, klines1.Count - 1);
            var features2 = ExtractFeatures(klines2, klines2.Count - N, klines2.Count - 1);
            var normalizedFeatures1 = Normalize(features1);
            var normalizedFeatures2 = Normalize(features2);
            int bestLag = FindBestLag(normalizedFeatures1, normalizedFeatures2);
            var laggedFeatures2 = ApplyLag(normalizedFeatures2, bestLag);

            var euclideanDistance = CalculateEuclideanDistance(normalizedFeatures1, laggedFeatures2);
            var cosineSimilarity = CalculateCosineSimilarity(normalizedFeatures1, laggedFeatures2);

            return new double[] { euclideanDistance, cosineSimilarity,(double) bestLag };
        }
    }
}
