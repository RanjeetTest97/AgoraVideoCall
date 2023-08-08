using Agora.Rtc;
using Agora_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
#pragma warning disable CS0105 // The using directive for 'Agora.Rtc' appeared previously in this namespace
using Agora.Rtc;
#pragma warning restore CS0105 // The using directive for 'Agora.Rtc' appeared previously in this namespace
using System.Net.Http;
using System.Collections.Specialized;
using System.Text;
using Newtonsoft.Json;
#pragma warning disable CS0105 // The using directive for 'Agora_App.Models' appeared previously in this namespace
using Agora_App.Models;
#pragma warning restore CS0105 // The using directive for 'Agora_App.Models' appeared previously in this namespace
using System.Net;
using System.IO;

namespace Agora_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static readonly HttpClient httpClient = new HttpClient();
        private static string BaseURL = "https://api.agora.io/dev/v1/kicking-rule";
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Agorademo()
        {

            var body = new AgoraModel
            {

                appid = "2d574648e5e4422085535cda8a06ff08",
                cname = "Test",
                uid = "589517928",
                ip = "",
                time = "60",
                time_in_seconds = "360",
                privileges = new string[] { "join_channel" }
            };

            // calling header
            GenerateHeader(body);

            return View();
        }

        public static void GenerateHeader(AgoraModel model)
        {
            // Customer ID
            string customerKey = "78c123ac4a394924aa8e110c6b1c0d68";
            // Customer secret
            string customerSecret = "13b8866e962740c6829246a86d2574f8";
            // Concatenate customer key and customer secret and use base64 to encode the concatenated string
            string plainCredential = customerKey + ":" + customerSecret;

            // Encode with base64
            var plainTextBytes = Encoding.UTF8.GetBytes(plainCredential);
            string encodedCredential = Convert.ToBase64String(plainTextBytes);
            // Create authorization header
            string authorizationHeader = "Authorization: Basic " + encodedCredential;

            // Create request object
            WebRequest request = WebRequest.Create("https://api.agora.io/dev/v1/projects");
            request.Method = "GET";

            request.Headers.Add(authorizationHeader);
            request.ContentType = "application/json";


            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Console.WriteLine(responseFromServer);
            }

            WebRequest Newrequest = WebRequest.Create(BaseURL);
            Newrequest.Method = "POST";

            Newrequest.Headers.Add(authorizationHeader);
            Newrequest.ContentType = "application/json";


            using (var streamWriter = new StreamWriter(Newrequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(model);

                streamWriter.Write(json);
            }
            WebResponse _response = Newrequest.GetResponse();
            Console.WriteLine(((HttpWebResponse)_response).StatusDescription);

            using (Stream dataStream1 = _response.GetResponseStream())
            {
                StreamReader reader1 = new StreamReader(dataStream1);
                string responseFromServer1 = reader1.ReadToEnd();
                Console.WriteLine(responseFromServer1);
            }
            response.Close();
        }

        //public static async Task PostData(AgoraModel model)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(BaseURL);
        //        var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        //        content.Headers()
        //        var result = await client.PostAsync(BaseURL, content);
        //        result.EnsureSuccessStatusCode();
        //    }
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
