using System.Net.Http.Json;
using System.Security.Cryptography; 


var client = new HttpClient();
string gatewayUrl = "https://localhost:7000";


Console.WriteLine("=================================================");
Console.WriteLine("   SIMULATOR...     ");
Console.WriteLine("=================================================\n");

var sensorTasks = Enumerable.Range(1, 5)
    .Select(index => StartSingleSensorAsync(client, gatewayUrl, index))
    .ToList();

await Task.WhenAll(sensorTasks);


async Task StartSingleSensorAsync(HttpClient httpClient, string url, int instanceNumber)
{
    Guid mySensorId = Guid.NewGuid();
    string name = $"Sensor: {instanceNumber}";

    using RSA rsa = RSA.Create(2048);

    string publicKeyBase64 = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo());

    // string privateKeyBase64 = Convert.ToBase64String(rsa.ExportPkcs8PrivateKey());

    var registrationPayload = new
    {
        Id = mySensorId,
        Quality = DataQuality.GOOD, 
        PublicKey = publicKeyBase64 
    };

    try
    {
        var registerResponse = await httpClient.PostAsJsonAsync($"{url}/api/sensor/register", registrationPayload);

        if (!registerResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"[Sensor {instanceNumber}] Interrupted registration. Status: {registerResponse.StatusCode}");
            return;
        }
        Console.WriteLine($"[Sensor {instanceNumber}] Succesfully registrated (ID: {mySensorId.ToString().Substring(0, 8)}...)");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Sensor {instanceNumber}] Error: {ex.Message}");
        return;
    }

    await Task.Delay(new Random().Next(500, 3000));

    int messageId = 1;
    var random = new Random();

    while (true)
    {
        double simulatedTemp = random.Next(1, 100) > 90 ? random.Next(85, 102) : random.Next(20, 26);
        int alarmPriority = simulatedTemp > 80 ? random.Next(1, 4) : 0;

        var ingestionPayload = new
        {
            SensorId = mySensorId,
            Temperature = Math.Round(simulatedTemp, 2),
            MessageId = messageId++,
            Timestamp = DateTime.UtcNow,
            AlarmPriority = alarmPriority
        };

        try
        {
            var response = await httpClient.PostAsJsonAsync($"{url}/api/ingestion/ingest", ingestionPayload);

            string alertTag = alarmPriority > 0 ? $"[ALARM P{alarmPriority}] " : "";
            Console.WriteLine($"[Sensor {instanceNumber}] {alertTag}Sent: {simulatedTemp}°C | Status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Sensor {instanceNumber}] Error: {ex.Message}");
        }

        await Task.Delay(TimeSpan.FromSeconds(10));
    }
}

enum DataQuality { GOOD, BAD, UNCERTAIN }
