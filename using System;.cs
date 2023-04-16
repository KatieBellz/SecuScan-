using System;
using System.Net.Http;

class ZapScanner 
{
    static void Main(string[] args) 
    {
        string targetUrl = "http://example.com";
        string zapApiKey = "your_api_key";
        string zapApiUrl = "http://localhost:8080/JSON/";

        HttpClient httpClient = new HttpClient();

        // Start a new ZAP session
        var response = httpClient.GetAsync(zapApiUrl + "core/action/newSession/?apikey=" + zapApiKey + "&name=MySession").Result;
        var sessionId = response.Content.ReadAsStringAsync().Result;

        // Access the spider API to crawl the target website
        response = httpClient.GetAsync(zapApiUrl + "spider/action/scan/?apikey=" + zapApiKey + "&url=" + targetUrl).Result;
        var spiderScanId = response.Content.ReadAsStringAsync().Result;

        // Access the scanner API to scan the target website for vulnerabilities
        response = httpClient.GetAsync(zapApiUrl + "ascan/action/scan/?apikey=" + zapApiKey + "&url=" + targetUrl).Result;
        var scanId = response.Content.ReadAsStringAsync().Result;

        // Check the status of the scan until it's complete
        while (true) 
        {
            response = httpClient.GetAsync(zapApiUrl + "ascan/view/status/?apikey=" + zapApiKey + "&scanId=" + scanId).Result;
            var status = response.Content.ReadAsStringAsync().Result;
            if (status == "100") 
            {
                break;
            }
            System.Threading.Thread.Sleep(1000);
        }

        // Generate a report of the vulnerabilities found
        response = httpClient.GetAsync(zapApiUrl + "core/view/alerts/?apikey=" + zapApiKey + "&baseurl=" + targetUrl).Result;
        var report = response.Content.ReadAsStringAsync().Result;

        Console.WriteLine(report);

        // Shutdown the ZAP session
        response = httpClient.GetAsync(zapApiUrl + "core/action/shutdown/?apikey=" + zapApiKey).Result;

    }
}
