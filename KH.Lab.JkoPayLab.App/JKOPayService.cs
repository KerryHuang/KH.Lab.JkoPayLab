using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace KH.Lab.JkoPayLab.App
{
    public class JKOPayService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "ad114d1f58e0698e25094b631483f0b1bc566ac4b3a1693f7a914591d3519f5d";
        private readonly string _apiSecret = "c47dbc649b063efa5ad5ed05c5550b51624e5f5bf4e000788441d4697fa47121";
        private readonly string _jkoApiUrl = "https://uat-onlinepay.jkopay.app";

        public JKOPayService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> CreateOrderAsync()
        {
            var orderData = new
            {
                store_id = "a89094fa-925f-11ed-8c25-0050568403ed",
                platform_order_id = DateTime.Now.ToString("yyMMddHHmmssFFFFFFF"),
                currency = "TWD",
                total_price = 10,
                final_price = 10,
                unredeem = 10,
                escrow = false,
                payment_type = "onetime",
                result_url = "https://vip1.car1.com.tw/Car1Pay/Car1Pay/JkoPay/PayResult",
                result_display_url = "https://vip1.car1.com.tw/Car1Pay/Car1Pay/JkoPay/PayResultDisplay"
            };

            string jsonOrderData = JsonConvert.SerializeObject(orderData);
            string digest = GenerateDigest(jsonOrderData);

            var request = new HttpRequestMessage(HttpMethod.Post, _jkoApiUrl + "/platform/entry")
            {
                Content = new StringContent(jsonOrderData, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("API-KEY", _apiKey);
            request.Headers.Add("DIGEST", digest);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            else
            {
                return $"Error: {response.StatusCode}";
            }
        }

        private string GenerateDigest(string data)
        {
            //using (var sha256 = SHA256.Create())
            //{
            //    var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data + _apiSecret));
            //    return BitConverter.ToString(hash).Replace("-", "").ToLower();
            //}

            var dataBytes = Encoding.UTF8.GetBytes(data);
            var keyBytes = Encoding.UTF8.GetBytes(_apiSecret);
            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(dataBytes);
            return Convert.ToHexString(hash).ToLower();
        }
    }
}
