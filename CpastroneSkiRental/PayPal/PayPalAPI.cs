using CpastroneSkiRental.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CpastroneSkiRental.PayPal
{
    public class PayPalAPI
    {
        /// <summary>
        /// Get account credentials from json file
        /// </summary>
        public IConfiguration configuration { get; }


        public HttpRequestMessage tokenMsg = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");//Http request for access token
        public HttpRequestMessage paymentMsg = new HttpRequestMessage(HttpMethod.Post, "v1/payments/payment");//Http request for paymant
        
        public PayPalAPI(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        /// <summary>
        /// Get redirected to PayPal payment page if response is successful
        /// </summary>
        /// <param name="total"></param>
        /// <param name="currency"></param>
        /// <returns>PayPal URL</returns>
        public async Task<string> GetRedirectURLToPayPal(double total, string currency)
        {
            try
            {

          
                return Task.Run(async () =>
                {
                    HttpClient http = GetPayPalSandBoxAccountHttp();
                    PayPalAPIAccessToken accessToken = await GetPayPalAccessToken(http);
                    PayPalAPIResponse createdPayment = await SendPayPalPayment(http, accessToken, total,currency);
                    return createdPayment.links.First(a => a.rel == "approval_url").href;


                }

                ).Result;
            }
            catch (Exception ex)
            {
                 Console.WriteLine(ex);
                return null;
            }
        }

       
        /// <summary>
        /// Get http client of the sandbox account
        /// </summary>
        /// <returns></returns>
        private HttpClient GetPayPalSandBoxAccountHttp()
        {
            try
            {
            var http =new  HttpClient{
                BaseAddress= new Uri(configuration["PayPal:urlAPI"]),
                Timeout = TimeSpan.FromSeconds(30),
            };

            return http;
            }catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

        }
        /// <summary>
        /// Get access token to PayPal api
        /// </summary>
        /// <param name="http"></param>
        /// <returns>PayPal access token</returns>
        private async Task<PayPalAPIAccessToken> GetPayPalAccessToken(HttpClient http)
        {
            try { 
            byte[] clientCredentials = Encoding.UTF8.GetBytes($"{configuration["PayPal:clientId"]}:{configuration["PayPal:clientSecret"]}");
            tokenMsg.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(clientCredentials));
            var formURL = new Dictionary<string, string>
                {
                    {"grant_type", "client_credentials"}
                };

            tokenMsg.Content = new FormUrlEncodedContent(formURL);

            HttpResponseMessage httpResponseMsg = await http.SendAsync(tokenMsg);
            string responseContent = await httpResponseMsg.Content.ReadAsStringAsync();

            PayPalAPIAccessToken palAcess = JsonConvert.DeserializeObject<PayPalAPIAccessToken>(responseContent);

            return palAcess;
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

        }
        /// <summary>
        /// Send payment request to PayPal api 
        /// </summary>
        /// <param name="http"></param>
        /// <param name="acess"></param>
        /// <param name="total"></param>
        /// <param name="currency"></param>
        /// <returns>PayPalApi response</returns>
        private async Task<PayPalAPIResponse> SendPayPalPayment(HttpClient http, PayPalAPIAccessToken acess, double total,string currency)
        {
            try
            {
            paymentMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", acess.access_token);

            var payment = JObject.FromObject(new
            {
                intent = "sale",
                redirect_urls = new
                {
                    return_url = configuration["PayPal:returnUrl"],
                    cancel_url = configuration["PayPal:cancelUrl"]
                },
                payer = new { payment_method = "paypal"},
                transactions = JArray.FromObject(new[]
                
                {
                    new
                    {
                        amount = new
                        {
                            total ,
                            currency
                        }
                    }

                    })

            });

            paymentMsg.Content = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await http.SendAsync(paymentMsg);

            string content = await responseMessage.Content.ReadAsStringAsync();
            PayPalAPIResponse payPalPayment = JsonConvert.DeserializeObject<PayPalAPIResponse>(content);
            return payPalPayment;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

    }

    /// <summary>
    /// Access token getter for PayPal API
    /// </summary>
    public class PayPalAPIAccessToken
    {
        public string access_token { get; set; }
    }
    /// <summary>
    /// PayPal api response
    /// </summary>
    public class PayPalAPIResponse
    {

        public string id { get; set; }
        public string intent { get; set; }
        public string state { get; set; }
        public Payer payer { get; set; }
        public Transaction[] transactions { get; set; }
        public DateTime create_time { get; set; }
        public Link[] links { get; set; }

        /// <summary>
        /// PayPal payment method
        /// </summary>
        public class Payer
        {
            public string payment_method { get; set; }

        }
        /// <summary>
        /// Current transaction
        /// </summary>
        public class Transaction
        {
            public Amount amount { get; set; }
            public object[] related_resources { get; set; }
        }
        /// <summary>
        /// Amount and currency of the transaction
        /// </summary>
        public class Amount
        {
            public string total { get; set; }
            public string currency { get; set; }
        }
        /// <summary>
        /// PayPalRedirectionLink
        /// </summary>
        public class Link
        {
            public string href { get; set; }
            public string rel { get; set; }
            public string method { get; set; }
        }

    }

}
