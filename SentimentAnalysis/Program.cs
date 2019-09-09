using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SampleSentiment
{
    public class Program
    {
        public class TextAnalyticsSentimentClient
        {
            //You can get the reqeust url by going to: 
            //https://centralus.dev.cognitive.microsoft.com/docs/services/TextAnalytics-v3-0-preview/operations/56f30ceeeda5650db055a3c9
            //and clicking on the region (e.g. Central US). 
            private static readonly string textAnalyticsUrl = "https://devnxt-textanalysis.cognitiveservices.azure.com/text/analytics/v2.1/sentiment";

            private static readonly string textAnalyticsKey = "15b9e04e816d4558a3e896a768dc6433";

            public static async Task<SentimentResponse> SentimentPreviewPredictAsync(TextAnalyticsBatchInput inputDocuments)
            {
                //Uri newuri = new Uri(textAnalyticsUrl);

                //WebRequest objwebRequest = WebRequest.Create(newuri);
                //objwebRequest.Headers.Add("Ocp-Apim-Subscription-Key", textAnalyticsKey);
                //HttpWebResponse objwebResponse = (HttpWebResponse)objwebRequest.GetResponse();
                //StreamReader objStreamReader = new StreamReader(objwebResponse.GetResponseStream());
                //string sResponse = objStreamReader.ReadToEnd();

                //List<SentimentResponse> dataList = JsonConvert.DeserializeObject<List<SentimentResponse>>(sResponse);

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", textAnalyticsKey);
                    var httpContent = new StringContent(JsonConvert.SerializeObject(inputDocuments), Encoding.UTF8, "application/json");
                    
                    var httpResponse = await httpClient.PostAsync(new Uri(textAnalyticsUrl), httpContent);
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();

                    if (!httpResponse.StatusCode.Equals(HttpStatusCode.OK) || httpResponse.Content == null)
                    {
                        throw new Exception(responseContent);
                    }
                    var test= JsonConvert.DeserializeObject(responseContent);
                   
                   
                    JObject jObject = JObject.Parse(responseContent);
          
                    string score = (string)jObject.SelectToken("documents[0].score");

                    
                        

                    //JArray signInNames = (JArray)jObject.SelectToken("documents");
                    //foreach (JToken signInName in signInNames)
                    //{
                    //    type = (string)signInName.SelectToken("score");                        

                    //}

                    SentimentResponse sR = JsonConvert.DeserializeObject<SentimentResponse>(responseContent, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                    double scoreCompare = Convert.ToDouble(score);
                    if (scoreCompare > .5)
                        sR.Documents[0].Sentiment = "Positive";
                    
                    else
                        sR.Documents[0].Sentiment = "Negative";

                    return sR;
                }
            }
        }

        public static void Main(string[] args)
        {
            var count = 1;
            
            
            Console.WriteLine("=============== Sentiment Analysis ================\n");
            Console.WriteLine("The process of computationally identifying and categorizing opinions\nexpressed by the user, especially in order to determine whether the\nuser's attitude towards a particular topic, product, etc. is positive, negative, or neutral.\n");
            Console.WriteLine("=============== Start of process ==================\n");
            while(true)
            {
                Console.WriteLine("=============== Prediction Attempt:" + count++ + "===============");
                //Console.WriteLine("Prediction Attempt : " + count++);
                Console.WriteLine("Enter the Sentiment: ");
                var inputSentiment = Console.ReadLine();
                var inputDocuments = new TextAnalyticsBatchInput()
                {

                    Documents = new List<TextAnalyticsInput>()
                {
                    new TextAnalyticsInput()
                    {
                        Id = "1",

                        Text = inputSentiment

                      
                    }

                }
                };

                var sentimentPrediction = TextAnalyticsSentimentClient.SentimentPreviewPredictAsync(inputDocuments).Result;
                Console.WriteLine("====================================================================================");
                Console.WriteLine("\nSentiment = " + sentimentPrediction.Documents[0].Sentiment + "\nScore = " + sentimentPrediction.Documents[0].Score);
                Console.WriteLine("\n====================================================================================\n\n");
            }

        }

        public class SentimentResponse
        {
            public IList<DocumentSentiment> Documents { get; set; }
           
            public IList<ErrorRecord> Errors { get; set; }

            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
            public RequestStatistics Statistics { get; set; }
        }

        public class TextAnalyticsBatchInput 
        {
            public IList<TextAnalyticsInput> Documents { get; set; }
        }

        public class TextAnalyticsInput
        {
            public string Id { get; set; }
            public string Text { get; set; }
            public string LanguageCode { get; set; } = "en";

        }

        public class DocumentSentiment
        {
            public DocumentSentiment(
                string id,
                string sentiment,
                //DocumentSentimentLabel sentiment,
                //SentimentConfidenceScoreLabel documentSentimentScores,
                //IEnumerable<SentenceSentiment> sentencesSentiment,
                string score
                )
            {
                Id = id;

                Sentiment = sentiment;

                //DocumentScores = documentSentimentScores;

                //Sentences = sentencesSentiment;

                Score = score;
            }
            public string Id { get; set; }
            //public DocumentSentimentLabel Sentiment { get; set; }
            //public SentimentConfidenceScoreLabel DocumentScores { get; set; }
            //public IEnumerable<SentenceSentiment> Sentences { get; set; }
            public string Score { get; set; }
            public string Sentiment { get; set; }
        }

        public enum DocumentSentimentLabel
        {
            Positive,

            Neutral,

            Negative,

            Mixed
        }

        public enum SentenceSentimentLabel
        {
            Positive,

            Neutral,

            Negative
        }

        public class SentimentConfidenceScoreLabel
        {
            public double Positive { get; set; }

            public double Neutral { get; set; }

            public double Negative { get; set; }
            
        }

        public class ErrorRecord
        {
            public string Id { get; set; }
            public string Message { get; set; }
        }

        public class SentenceSentiment
        {
            public SentenceSentimentLabel Sentiment { get; set; }
            public SentimentConfidenceScoreLabel SentenceScores { get; set; }
            public int Offset { get; set; }
            public int Length { get; set; }
            public string[] Warnings { get; set; }
        }

        public class RequestStatistics
        {
            public int DocumentsCount { get; set; }
            public int ValidDocumentsCount { get; set; }    
            public int ErroneousDocumentsCount { get; set; }
            public long TransactionsCount { get; set; }
        }
    }
}
