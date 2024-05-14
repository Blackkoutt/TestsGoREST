using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    private static readonly string token = "1e23c93ad583515789525806933184dd7486d5cb9977d780ce8098df1e9650ea";
    private static readonly string URL = "https://gorest.co.in/public/v2/users";

    static async Task Main()
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //await GetAllUsers();
        string id = "";
        try
        {
            PrintHTTPMethod("POST", null);
            id = await PostUser("BSK User", "bsk.user@test.com", "male", "active");
            PrintHTTPMethod("GET", id);
            await GetOneUser(id);
            PrintHTTPMethod("DELETE", id);
            await DeleteUser(id);          
            PrintHTTPMethod("GET", id);
            await GetOneUser(id);
        }     
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }
        catch(Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message);
        }
    }

    private async static Task GetAllUsers()
    {
        try
        {
            using HttpResponseMessage response = await client.GetAsync(URL);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status code: {(int)response.StatusCode} ({response.StatusCode})\n");
            Console.WriteLine("Response body: ");
            ConvertStringToJSONAndPrint(responseBody);
        }
        catch (Exception e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }
    }
    private async static Task GetOneUser(string user_id)
    {
        using HttpResponseMessage response = await client.GetAsync($"{URL}/{user_id}");
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Status code: {(int)response.StatusCode} ({response.StatusCode})\n");
        Console.WriteLine("Response body: ");
        ConvertStringToJSONAndPrint(responseBody);
    }
    private async static Task DeleteUser(string user_id)
    {
        using HttpResponseMessage response = await client.DeleteAsync($"{URL}/{user_id}");
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Status code: {(int)response.StatusCode} ({response.StatusCode})");
        Console.WriteLine("Response body: ");
        ConvertStringToJSONAndPrint(responseBody);
    }
    private async static Task<string> PostUser(string name, string email, string gender, string status)
    {
        var user = new
        {
            name = name,
            email = email,
            gender = gender,
            status = status
        };

        string json = JsonSerializer.Serialize(user);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        using HttpResponseMessage response = await client.PostAsync(URL, content);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Status code: {(int)response.StatusCode} ({response.StatusCode})\n");
        Console.WriteLine("Request body: ");
        string jsonString = JsonSerializer.Serialize(user, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(jsonString);
        Console.WriteLine("Response body: ");
        ConvertStringToJSONAndPrint(responseBody);        
        string id = responseBody.Substring(responseBody.IndexOf(":") + 1, (responseBody.IndexOf(",") - responseBody.IndexOf(":")) - 1 );
        return id;
    }
    private static void ConvertStringToJSONAndPrint(string textString)
    {
        try
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(textString);
            string jsonString = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(jsonString);
        }
        catch(JsonException)
        {
            Console.WriteLine(textString);
        }
        catch (Exception ex) 
        {
            Console.WriteLine("Error: " + ex.ToString());
        }
    }
    private static void PrintHTTPMethod(string method, string? id)
    {
        Console.WriteLine("\n-------------------------------------------------------------------\n");
        switch (method)
        {
            case "GET":
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                }
            case "POST":
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                }
            case "DELETE":
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                }
        }
        Console.Write(method);
        Console.ResetColor();
        if (id != null)
        {
            Console.Write($" User: {URL}/{id}\n");
        }
        else
        {
            Console.Write($" User: {URL}\n");
        }

    }
}