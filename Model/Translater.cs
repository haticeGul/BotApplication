using System;

namespace BotApplication
{
    public class Translater
    {
        //public static string TranslateText(string text, string targetLanguageCode,
        //          string sourceLanguageCode)
        //{

        //    TranslationClient client = TranslationClient.CreateFromApiKey("AIzaSyDYsHZkzqb5xw3nOjlZuxaoVOp56yIGZZ0");
        //    var response = client.TranslateText(text, targetLanguageCode,
        //        sourceLanguageCode);
        //    return response.TranslatedText;
        //}



        public static string TranslateTextToEnglishRest(string text)
        {
            System.Net.WebClient client = new System.Net.WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            client.Headers.Add("User-Agent",@"Mozilla/5.0 (Windows NT 6.1; rv:6.0.2) Gecko/20100101 Firefox/6.0.2 FirePHP/0.6");
            client.Headers.Add("Accept", @"text /html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            string translatedText =
                client.DownloadString(
                    new Uri("https://translate.googleapis.com/translate_a/single?client=gtx&sl=tr&tl=en&dt=t&q=" +

                            System.Uri.EscapeUriString(text)));
            int getIndexResult = translatedText.IndexOf('"' + "," + '"');
            translatedText = translatedText.Substring(3, getIndexResult);
            translatedText = translatedText.Trim().Replace('"' + "," + '"', "").Replace("\"", "").Trim();

            return translatedText;
        }


    }
}
