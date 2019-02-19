using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;


namespace BotApplication
{
    /// <summary>
    /// invokes api for client requests
    /// </summary>
    public class ApiInvoker
    {
        static readonly ApiInvoker _instance = new ApiInvoker();

        public static ApiInvoker Instance
        {
            get
            {
                return _instance;
            }
        }

        private ApiInvoker()
        {
            //initialize
        }

        /// <summary>
        /// for  requests which has a T typed object parameter
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<ApiResponse<Tres>> InvokeApi<Treq, Tres>(Uri url, Treq requestObject)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var reqParams = RequestParser.GetRequestParameters(requestObject);

                var content = new FormUrlEncodedContent(reqParams);

                var apiToken = HttpContext.Current.Request.Cookies["AccessToken"];

                if (apiToken != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", "Bearer", apiToken.Value));
                }

                // Get Token
                using (var response = await client.PostAsync(url, content))
                {
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.Unauthorized:
                            break;
                        case System.Net.HttpStatusCode.OK:
                            break;

                    }

                    var data = await response.Content.ReadAsStringAsync();
                    var json = GetParsedData<Tres>(data);

                    //if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    //{
                    //    throw new HttpException((int)response.StatusCode, json.GetValue("error_description").ToString());
                    //}

                    //var access_token = json.GetValue("access_token").ToString();

                    return new ApiResponse<Tres> { ResponseCode = response.StatusCode, ResponseData = json };
                }
            }

        }

        /// <summary>
        /// for parameterless requests
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<ApiResponse<Tres>> InvokeApi<Tres>(Uri url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var apiToken = HttpContext.Current.Request.Cookies["AccessToken"];

                if (apiToken != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", "Bearer", apiToken.Value));
                }

                // Get Token
                using (var response = await client.PostAsync(url, null))
                {
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.Unauthorized:
                            break;
                        case System.Net.HttpStatusCode.OK:
                            break;

                    }

                    var data = await response.Content.ReadAsStringAsync();



                    var json = GetParsedData<Tres>(data);//JObject.Parse(data);

                    return new ApiResponse<Tres> { ResponseCode = response.StatusCode, ResponseData = json };
                }
            }

        }

        /// <summary>
        /// returns generic type from gven json string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private T GetParsedData<T>(string data)
        {
            #region sample json string

            /* 
            {
             "$d":
                [
                 {
                   "Text": "Foo",
                   "Value": "foo"
                 },
                 {
                   "Text": "Bar",
                   "Value": "bar"
                 }
               ]
             }
           */

            #endregion sample json string

            T parsedObject;

            if (data[0] != '{') // primitive type returned  like int,string
            {
                return (T)Convert.ChangeType(data, typeof(T));
            }

            var rawJson = JObject.Parse(data);

            string arraySignature = "$d";

            if (data.Substring(2, 2) == arraySignature)  // is this data is actually an array?
            {
                var json = rawJson.GetValue("$d").ToString(); // first remove the top element named '$d'

                JArray jsonArray = JArray.Parse(json);

                parsedObject = jsonArray.ToObject<T>();

                return parsedObject;
            }

            parsedObject = rawJson.ToObject<T>();

            return parsedObject;
        }
    }

    public class ApiResponse<T> : BaseResponse
    {
        public HttpStatusCode ResponseCode { get; set; }

        T responseData;
        public T ResponseData
        {
            get
            {
                return this.responseData;
            }
            set
            {
                this.responseData = value;
            }
        }

    }
    public abstract class BaseResponse
    {
        public string ResponseMessage { get; set; }
    }
    public static class RequestParser
    {
        public static Dictionary<string, string> GetRequestParameters<T>(T requestObject)
        {
            Dictionary<string, string> reqDic = new Dictionary<string, string>();
            foreach (PropertyInfo pi in requestObject.GetType().GetProperties())
            {
                string value = pi.GetValue(requestObject, null).ToString();
                reqDic.Add(pi.Name, GetValueWithValidatedChars(value));
            }

            return reqDic;
        }

        /// <summary>
        /// web api do not accept chars=> requestPathInvalidCharacters="<,>,*,%,:,&"
        /// this method replaces invalid chars with the valid ones
        /// </summary>
        /// <param name="apiParameter">parameter we pass to web api</param>
        /// <returns></returns>
        private static string GetValueWithValidatedChars(string apiParameter)
        {
            string validatedText = apiParameter.Replace('<', '(').Replace('>', ')').Replace('*', '-').Replace('%', '-').Replace(':', '-').Replace('&', '-');

            return validatedText;
        }
    }

    public class GeneralResponse
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}

