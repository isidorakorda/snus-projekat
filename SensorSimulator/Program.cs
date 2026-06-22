using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography; 


var client = new HttpClient();
object lck = new object();
string gatewayUrl = Environment.GetEnvironmentVariable("GATEWAY_URL") ?? "https://localhost:7000";
string podName = Environment.GetEnvironmentVariable("POD_NAME") ?? "sensor-0";

Console.WriteLine("   SIMULATOR...     ");

/*var sensorTasks = Enumerable.Range(1, 5)
    .Select(index => StartSingleSensorAsync(client, gatewayUrl, index))
    .ToList();

await Task.WhenAll(sensorTasks);*/


await StartSingleSensorAsync(client, gatewayUrl, int.Parse(podName.Split('-')[1]));

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
        PublicKey = publicKeyBase64, 
        PodName = podName
    };
    bool isRegistered = false;

    while (!isRegistered)
    {
        try
        {
            var registerResponse = await httpClient.PostAsJsonAsync($"{url}/api/sensor/register", registrationPayload);

            if (registerResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"[Sensor {instanceNumber}] Succesfully registrated (ID: {mySensorId.ToString().Substring(0, 8)}...)");
                isRegistered = true;
            }
            else
            {
                Console.WriteLine($"[Sensor {instanceNumber}] Registration refused. Status: {registerResponse.StatusCode}. Retrying in 5s...");
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Sensor {instanceNumber}] Error: {ex.Message}");
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
    

    

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

            if (alarmPriority > 0)
                LogAlarmInColor(ingestionPayload.SensorId, ingestionPayload.Temperature, alarmPriority);
            else
            {
                lock (lck)
                {
                    Console.WriteLine($"[Sensor {instanceNumber}] Sent: {simulatedTemp}°C | Status: {response.StatusCode}");
                }
            }
        }
        catch (Exception ex)
        {
            lock (lck)
            {
                Console.WriteLine($"[Sensor {instanceNumber}] Error: {ex.Message}");
            }
        }

        await Task.Delay(TimeSpan.FromSeconds(5));
    }
}

void LogAlarmInColor(Guid sensorId, double temp, int priority)
{
    lock (lck)
    {
        var originalColor = Console.ForegroundColor;

        Console.ForegroundColor = priority switch
        {
            1 => ConsoleColor.Yellow,
            2 => ConsoleColor.DarkYellow,
            3 => ConsoleColor.Red,
            _ => originalColor
        };

        Console.WriteLine($"[ ALARM - PRIORITY {priority}] : Sensor: {sensorId} | Temperature: {temp}");
        Console.ForegroundColor = originalColor;
    }
}
    

enum DataQuality { GOOD, BAD, UNCERTAIN }

