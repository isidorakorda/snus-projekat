<<<<<<< HEAD
﻿using Microsoft.AspNetCore.SignalR.Client;
using ConsoleApplication.DTO;
using System;
=======
﻿using System;
>>>>>>> parent of 16aea24 (Merge pull request #10 from isidorakorda/feature/deployment)
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace ConsoleApplication
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Console Application is up and running...");
<<<<<<< HEAD
            var cookies = new CookieContainer();

            string connectionURL = "http://10.199.219.221:8080";

            var connection = new HubConnectionBuilder()
                .WithUrl(connectionURL + "/alarmHub", options =>
                {
                    options.Cookies = cookies;
                })
=======

            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5052/alarmHub")
>>>>>>> parent of 16aea24 (Merge pull request #10 from isidorakorda/feature/deployment)
                .WithAutomaticReconnect()
                .Build();

            connection.On<AlarmDTO>("Alarm", async (dto) =>
            {
                var originalColor = Console.ForegroundColor;

                switch (dto.Priority)
                {
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    default:
                        break;
                }
                Console.WriteLine(dto.Message);
                Console.ForegroundColor = originalColor;
            });
            Console.ForegroundColor = ConsoleColor.Red;
            while (true)
            {
                try
                {
                    await connection.StartAsync();
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Can't connect to server. Trying again");
                    continue;
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            await connection.InvokeAsync("InitSub");
            Console.WriteLine("[CONSOLE] Successfully subscribed to the hub");
<<<<<<< HEAD
            Console.ForegroundColor = ConsoleColor.White;

            _ = Task.Run(() => InputCommandLoop(connectionURL));

            await Task.Delay(-1);
        }


        private static async Task InputCommandLoop(string connectionURL)
        {
            while (true)
            {
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) continue;

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) Environment.Exit(0);

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
                }
                else if (command.Equals("block"))
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
=======
            Console.ReadLine();
>>>>>>> parent of 16aea24 (Merge pull request #10 from isidorakorda/feature/deployment)
        }
    }
}
