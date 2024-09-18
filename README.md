# 街口支付

根據街口支付的「訂單創建 API」文件，要在 .NET 8 中實作此功能，需要處理以下幾個關鍵點：

1. **API 路徑與方法**：使用 `POST` 方法向街口的 API 路徑發送請求。

2. 必要參數

   - `store_id` (商家 ID)
   - `platform_order_id` (訂單 ID)
   - `total_price` (訂單總金額)
   - `final_price` (實際消費金額)
   - `currency` (貨幣，通常為 TWD)
   - `result_url` (消費者付款完成後的回調 URL)

3. 加密與簽章

   街口 API 需要加簽與加密處理，具體步驟包括生成 

   ```
   API-KEY
   ```

    和 

   ```
   DIGEST
   ```

   [JKOS](https://open-doc.jkos.com/?docs=線上支付onlinepay/api列表/訂單創建-api)
   [JKOS](https://open-doc.jkos.com/?docs=線上支付onlinepay)



### .NET 8 實作範例

以下是 .NET 8 C# 的程式碼範例，展示如何創建訂單並發送到街口 API：

```csharp
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class JKOPayService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "your_api_key";
    private readonly string _apiSecret = "your_api_secret";
    private readonly string _jkoApiUrl = "https://test-onlinepay.jkopay.com/web/entry";

    public JKOPayService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<string> CreateOrderAsync()
    {
        var orderData = new
        {
            store_id = "your_store_id",
            platform_order_id = "order12345",
            currency = "TWD",
            total_price = 1000,
            final_price = 1000,
            escrow = false,
            payment_type = "onetime",
            result_url = "https://yourwebsite.com/payment/callback"
        };

        string jsonOrderData = JsonConvert.SerializeObject(orderData);
        string digest = GenerateDigest(jsonOrderData);

        var request = new HttpRequestMessage(HttpMethod.Post, _jkoApiUrl)
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
         var dataBytes = Encoding.UTF8.GetBytes(data);
         var keyBytes = Encoding.UTF8.GetBytes(_apiSecret);
         using var hmac = new HMACSHA256(keyBytes);
         var hash = hmac.ComputeHash(dataBytes);
         return Convert.ToHexString(hash).ToLower();
    }
}
```

### 說明：

1. **訂單資料**：`orderData` 包含必要的參數如 `store_id`、`platform_order_id`、`total_price`、`final_price` 等。
2. **加密處理**：使用 SHA256 對訂單資料加密來生成 `DIGEST`，這是 API 請求的必要步驟之一。
3. **API 請求**：發送 `POST` 請求至街口 API，並且處理回應結果。

您可以根據自己的實際需求對此程式碼進行調整，如處理更多訂單屬性、回調 URL 等

[JKOS](https://open-doc.jkos.com/?docs=線上支付onlinepay/api列表/訂單創建-api)
[JKOS](https://open-doc.jkos.com/?docs=線上支付onlinepay/api列表)
[JKOS](https://open-doc.jkos.com/?docs=線上支付onlinepay)


完整 API 文件請參考[街口開放 API 文件](https://open-doc.jkos.com/)。