using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    internal class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Console Application is up and running...");
            var cookies = new CookieContainer();

            string connectionURL = "http://127.0.0.1:43027"; 

            var connection = new HubConnectionBuilder()
                .WithUrl(connectionURL+ "/alarmHub", options =>
                {
                    options.Cookies = cookies;
                })
                .WithAutomaticReconnect()
                .Build();

            connection.On<string>("Alarm", async (msg) =>
            {
                Console.WriteLine(msg);
            });

            await connection.StartAsync();

            await connection.InvokeAsync("InitSub");
            Console.WriteLine("[CONSOLE] Successfully subscribed to the hub");
            

            _ = Task.Run(() => InputCommandLoop(connectionURL));

            await Task.Delay(-1);
        }


        private static async Task InputCommandLoop(string connectionURL)
        {
            while (true)
            {
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) continue;

                if(input.Equals("exit", StringComparison.OrdinalIgnoreCase)) Environment.Exit(0);

                if (!input.StartsWith("/"))
                {
                    Console.WriteLine("[Console] Bad input");
                    continue;
                }

                string[] parts = input.Split(' ');
                string command = parts[0].Substring(1).ToLower();

                if (command.Equals("sensors"))
                {
                    try
                    {
                        HttpResponseMessage response = await httpClient.GetAsync($"{connectionURL}/api/sensor");
                        if (response.IsSuccessStatusCode)
                        {
                            string jsonResult = await response.Content.ReadAsStringAsync();

                            try
                            {
                                var jsonDocument = System.Text.Json.JsonDocument.Parse(jsonResult);
                                string prettyJson = System.Text.Json.JsonSerializer.Serialize(jsonDocument, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                                Console.WriteLine($"[Console] HTTP Request successful:\n{prettyJson}");
                            }
                            catch
                            {
                                Console.WriteLine($"[Console] HTTP Request unsuccessful:\n{jsonResult}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[Console] HTTP Request failed with status code: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Console] Error sending HTTP request: {ex.Message}");
                    }
                }else if (command.Equals("block"))
                {
                    if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
                    {
                        Console.WriteLine("[Console] Usage: /block <sensor-id>");
                        continue;
                    }

                    string sensorId = parts[1].Trim();

                    try
                    {
                        var jsonContent = new StringContent($"\"{sensorId}\"", Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await httpClient.PostAsync($"{connectionURL}/api/ingestion/block", jsonContent);

                        if (response.IsSuccessStatusCode)
                        {
                            string result = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"[Console] Block successful:\n{result}");
                        }
                        else
                        {
                            Console.WriteLine($"[Console] Block failed with status code:\n{response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Console] Error sending block request: {ex.Message}");
                    }
                }
            }
        }
    }
}
