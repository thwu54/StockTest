using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace My
{
    // 定義數據類
    public class DataPoint
    {
        [LoadColumn(0)] // Y 欄位的索引，這裡假設 Y 欄位在最後一列
        public double Label;

        public double[] Features; // 動態建立 X 欄位的屬性
    }

    // 定義預測結果類別
    public class MyPrediction
    {
        [ColumnName("Features")]
        public double Features;
    }

    public class MLClass
    {
        public static void Fit(DataTable dt)
        {
            // 創建 MLContext
            var context = new MLContext();

            // 載入資料
            var data = context.Data.LoadFromEnumerable<DataPoint>(LoadData(dt));

            // 切割資料集
            var trainTestData = context.Data.TrainTestSplit(data, testFraction: 0.2);
            var trainData = trainTestData.TrainSet;
            var testData = trainTestData.TestSet;

            // 動態取得 X 欄位的數量
            var xColumnCount = GetXColumnCount<DataPoint>();

            // 建立管線
            var pipeline = context.Transforms.Conversion.MapValueToKey("Label")
                .Append(context.Transforms.Conversion.MapKeyToValue("Label"))
                .Append(context.Transforms.Concatenate("Features", nameof(DataPoint.Features)))
                .Append(context.Transforms.NormalizeMinMax("Features"))
                .Append(context.Regression.Trainers.Sdca(labelColumnName: "Label", maximumNumberOfIterations: 100));

            // 訓練模型
            var model = pipeline.Fit(trainData);

            // 預測測試資料
            var predictions = model.Transform(testData);

            // 評估模型
            var metrics = context.Regression.Evaluate(predictions);

            Console.WriteLine($"Mean Absolute Error: {metrics.MeanAbsoluteError}");
            Console.WriteLine($"Root Mean Squared Error: {metrics.RootMeanSquaredError}");

            // 進行預測
            var newData = new DataPoint { Features = new double[xColumnCount] }; // 初始化 Features 陣列
            var predictionEngine = context.Model.CreatePredictionEngine<DataPoint, MyPrediction>(model);
            var prediction = predictionEngine.Predict(newData);

            Console.WriteLine($"預測結果: {prediction.Features}");
        }

        // 修改 LoadData 方法
        public static IEnumerable<DataPoint> LoadData(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                var label = Convert.ToSingle(row[dataTable.Columns.Count - 1]); // 最後一列是 Y 欄位

                var featureValues = new List<double>();
                for (int i = 0; i < dataTable.Columns.Count - 1; i++) // 前面的是 X 欄位
                {
                    featureValues.Add(Convert.ToSingle(row[i]));
                }

                yield return new DataPoint
                {
                    Features = featureValues.ToArray(),
                    Label = label
                };
            }
        }

        // 動態取得 X 欄位的數量
        public static int GetXColumnCount<T>()
        {
            return typeof(T).GetProperties()
                .Where(p => p.Name.StartsWith("X"))
                .Count();
        }
    }
}
