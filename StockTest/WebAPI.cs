using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;//nuget Newtonsoft.Json
namespace My
{
    class WebAPI
    {
        public class GetGroupList
        {
            public int ID { get; set; }

            public string Name { get; set; }

            public int Age { get; set; }

            public string Sex { get; set; }
        }
        private static T GetInstance<T>()
        {
            T obj;
            if (typeof(T).Name == "String")
            {
                obj = (T)Activator.CreateInstance(typeof(T), "".ToCharArray());
            }
            else
            {
                obj = (T)Activator.CreateInstance(typeof(T));
            }
            return obj;
        }
        public static Tuple<string, string> HttpWebRequest_Get<T>(string url)
        {
            string sErrMsg = "";
            string temp = "";
            T result = GetInstance<T>();
            dynamic stuff=null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Get;
                request.ContentType = "application/json";
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var stream = response.GetResponseStream())
                        using (var reader = new StreamReader(stream))
                        {
                            temp = reader.ReadToEnd();
                            //result = JsonConvert.DeserializeObject<T>(temp);
                            stuff = JsonConvert.DeserializeObject(temp);

                        }
                    }
                    else
                    {
                        sErrMsg = "HttpWebRequest_Get Error.Url:" + url + ".StatusCode:" + response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream rb_data = response.GetResponseStream())
                    using (var reader = new StreamReader(rb_data))
                    {
                        sErrMsg = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                sErrMsg = "HttpWebRequest_Get Error.Url:" + url + ".Error Message:" + ex.Message;
            }
            return Tuple.Create(sErrMsg, temp);
        }
    }
}
