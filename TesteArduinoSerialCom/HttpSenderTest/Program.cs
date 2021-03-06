﻿using System;
using System.IO;
using System.Net;
using System.Threading;

namespace HttpSenderTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "HTTP SENDER TEST";
            Console.SetWindowSize(150, 30);

            String strHostName = string.Empty;
            strHostName = Dns.GetHostName();
            Console.WriteLine("Local Machine's Host Name: " + strHostName);

            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] ips = ipEntry.AddressList;

#if true
            Console.WriteLine("Select the IP");
            foreach (var i in ips)
                Console.WriteLine(i);
            var chosen = Console.ReadKey().KeyChar.ToString();
            var ipIndex = int.Parse(chosen);
            var ip = ips[ipIndex];
#else
            var ip = ips[1];
            Thread.Sleep(3000);
#endif

            Console.WriteLine();
            Console.WriteLine("IP: " + ip);

            var url1 = "http://localhost:8081/testapi";
            var url3 = $"http://{ip}:8081/testapi";
            var url4 = $"http://{Environment.MachineName}:8081/testapi";

            var arduinoUrl1 = "http://esp8266/";
            var arduinoUrl2 = "http://192.168.0.103/";
            var arduinoUrl3 = "http://esp8266/inline";
            var arduinoUrl4 = "http://192.168.0.103/inline";

            while (true) {
                SendRequest(url1);
                SendRequest(url3);
                SendRequest(url4);
                //SendRequest(arduinoUrl1);
                //SendRequest(arduinoUrl2);
                //SendRequest(arduinoUrl3);
                //SendRequest(arduinoUrl4);
                Console.ReadKey(); 
            }
        }

        private static void SendRequest(string url)
        {
            try {
                Console.WriteLine($"------------------------------- START {url} -----------------------------------------");
                WebRequest request = WebRequest.Create(url);
                // Set the Method property of the request to GET.
                request.Method = "GET";
                // Get the response.
                WebResponse response = request.GetResponse();
                // Get the stream containing content returned by the server.
                var dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                Console.WriteLine($"RESPONSE FROM SERVER");
                Console.WriteLine(responseFromServer);
                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();
                Console.WriteLine($"------------------------------- OK END {url} -----------------------------------------");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            } catch (Exception ex) {
                Console.WriteLine(ex);
                Console.WriteLine($"------------------------------- EXCEPTION END {url} -----------------------------------------");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }
        

    }
}