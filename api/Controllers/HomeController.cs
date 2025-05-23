using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
public class HomeController : Controller
{
    // GET
    [HttpGet("/")]
    public string Index()
    {
        return  "api is working...";
    }
    
    [HttpGet("/login/{data}")]
    public IActionResult Login(string data)
    {
        if (data == "is-34fiot")
        {
            return Ok(new { Name = "Valdyslav", Surname = "Bakunets", Year=2, Group = "IS-34" });
        }
        return NotFound("User with this login not found");
    }

    [HttpGet("/api/crypto/{name}")]
    public async Task<IActionResult> GetCryptoInfo(string name)
    {
        string apiKey = "74c9e22a3c8b5426915d290af8032edc4daac44eded7af8a5473a186e6bc7a8e";
        string url = $"https://rest.coincap.io/v3/assets/{name}?apiKey={apiKey}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        using var client = new HttpClient();

        var response = await client.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return NotFound("Wrong name");

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, "Failed to fetch data from CoinCap");

        var text = await response.Content.ReadAsStringAsync();
        var cryptoData = JsonNode.Parse(text);

        var html = @$"
    <table border='1' cellpadding='5'>
        <thead>
            <tr>
                <th>Ідентифікатор</th>
                <th>Рейтинг</th>
                <th>Символічне позначення</th>
                <th>Назва</th>
                <th>Доступна кількість для торгівлі</th>
                <th>Обіг</th>
                <th>Капіталізація в доларах</th>
                <th>Ціна в доларах</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>{cryptoData["data"]?["id"]}</td>
                <td>{cryptoData["data"]?["rank"]}</td>
                <td>{cryptoData["data"]?["symbol"]}</td>
                <td>{cryptoData["data"]?["name"]}</td>
                <td>{cryptoData["data"]?["supply"]}</td>
                <td>{cryptoData["data"]?["maxSupply"]}</td>
                <td>{cryptoData["data"]?["marketCapUsd"]}</td>
                <td>{cryptoData["data"]?["priceUsd"]}</td>
            </tr>
        </tbody>
    </table>";

        return Content(html, "text/html", Encoding.UTF8);
    }

}