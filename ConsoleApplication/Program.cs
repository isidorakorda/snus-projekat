using System;
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

            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5052/alarmHub")
                .WithAutomaticReconnect()
                .Build();

            connection.On<string>("Alarm", async (msg) =>
            {
                Console.WriteLine(msg);
            });

            await connection.StartAsync();

            await connection.InvokeAsync("InitSub");
            Console.WriteLine("[CONSOLE] Successfully subscribed to the hub");
            Console.ReadLine();
        }
    }
}
