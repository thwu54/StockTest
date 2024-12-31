using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StockTest
{
    public class GetStockClass
    {
        public static async Task Get(string StockNo)
        {
            string apiKey = "6UEMBD3GO8PIETH3";
            string symbol = StockNo; // 股票代号
            string url = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={apiKey}";

            //using (HttpClient client = new HttpClient())
            //{
            //    HttpResponseMessage response = await client.GetAsync(url);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        string jsonResponse = await response.Content.ReadAsStringAsync();
            //        JObject data = JObject.Parse(jsonResponse);

            //        // 解析数据示例
            //        var timeSeries = data["Time Series (Daily)"];
            //        foreach (var item in timeSeries)
            //        {
            //            string ss=item.ToString();
            //        }
            //    }
            //    else
            //    {
                    
            //    }
            //}
        }
        public static void GetYahooStockData(string symbol)
        {
            string apiKey = "your_api_key";
            string url = $"https://yahoo-finance1.p.rapidapi.com/market/v2/get-quotes?region=US&symbols={symbol}";
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("X-RapidAPI-Key", apiKey);
                try
                {
                    string response = client.DownloadString(url);
                    Console.WriteLine(response);
                }
                catch (WebException ex)
                {
                    Console.WriteLine($"API Request failed: {ex.Message}");
                }
            }
            
        }
        public static void GetStock(string StockNo)
        {
            string apiKey = "6UEMBD3GO8PIETH3";
            string symbol = StockNo+".TW"; // 股票代号
            string url = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={apiKey}";
            using (WebClient client = new WebClient())
            {
                string jsonResponse= client.DownloadString(url);
                JObject data = JObject.Parse(jsonResponse);

                // 解析数据示例
                var timeSeries = data["Time Series (Daily)"];
                foreach (var item in timeSeries)
                {
                    string ss = item.ToString();
                }
            }
        }
    }
}
