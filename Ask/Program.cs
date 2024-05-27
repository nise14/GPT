using System.Text;
using System.Text.Json;

if (args.Length > 0)
{
    HttpClient client = new HttpClient();

    client.DefaultRequestHeaders.Add("authorization", "Bearer API KEY");

    var data = new
    {
        model = "gpt-3.5-turbo",
        temperature = 0.3,
        messages = new[]{
            new{
                role = "user",
                content = args[0]
            }
        }
    };

    var jsonSerializer = JsonSerializer.Serialize(data);
    var content = new StringContent(jsonSerializer, Encoding.UTF8, "application/json");

    HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

    string responseString = await response.Content.ReadAsStringAsync();

    try
    {
        var dyData = JsonSerializer.Deserialize<dynamic>(responseString);

        string guess = GuessCommand(dyData!.choices[0].text);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"---> My guess at the command prompt is: {guess}");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"---> Could not deserialize the JSON: {ex.Message}");
    }

    // Console.WriteLine(responseString);
}
else
{
    Console.WriteLine("---> You need to provide some input");
}

string GuessCommand(string raw){
    Console.WriteLine("---> GPT-3 API Returned Text: ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine(raw);

    var lastIndex = raw.LastIndexOf('\n');

    string guess = raw.Substring(lastIndex+1);

    Console.ResetColor();

    TextCopy.ClipboardService.SetText(guess);

    return guess;
}