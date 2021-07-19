using System;
using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

void OpenLinkInBrowser(string url)
{
    try
    {
        Process.Start(url);
    }
    catch
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url.Replace("&", "^&")}") { CreateNoWindow = true });
        }
        else
        {
            throw;
        }
    }
}

void CopyToCLipboard(string text)
{
    Thread thread = new Thread(() => Clipboard.SetText(text));
    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();
    thread.Join();
}

string rtx3080ApiLink = "https://api.nvidia.partners/edge/product/search?page=1&limit=9&locale=fr-fr&category=GPU&gpu=RTX%203080";

var client = new System.Net.Http.HttpClient();

var response = await client.GetAsync(rtx3080ApiLink);

Console.WriteLine(response.StatusCode);

if (response.IsSuccessStatusCode)
{
    var body = await response.Content.ReadAsStringAsync();

    var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(body);

    var ldlcLink = apiResponse.SearchedProducts.FeaturedProduct.Retailers[0].PurchaseLink;

    Console.WriteLine(ldlcLink);

    CopyToCLipboard(ldlcLink);

    OpenLinkInBrowser(ldlcLink);
}
