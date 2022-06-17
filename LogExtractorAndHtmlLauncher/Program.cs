/* ***************************************************************
* AzureDevOps Monitoring Application
* © 2022, CCH Incorporated.  All rights reserved.
* Author: Vivek Singh
*****************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

namespace LogsExtractor
{
    class Program
    {
        static void Main(string[] args)
        {

            TestExecutionResult ter = new TestExecutionResult();
            var directory = new DirectoryInfo(ConfigurationSettings.AppSettings.Get("LogFolderLocation"));
            var directories = directory.EnumerateDirectories()
                                .OrderByDescending(d => d.CreationTime)
                                .Select(d => d.Name)
                                .ToList();
            var directoryName = string.Empty;
            for (int folderIndex = 0; folderIndex <= directories.Count; folderIndex++)
            {
                HtmlDocument html = new HtmlDocument();
                html.OptionOutputAsXml = true;
                directoryName = directories[folderIndex];
                string[] filePaths = Directory.GetFiles(ConfigurationSettings.AppSettings.Get("LogFolderLocation") + directoryName);
                if (File.Exists(filePaths[0]))
                {
                    string fileName = filePaths[0];
                    html.Load(fileName);
                    var htmlBody = html.ParsedText;
                    var pathToAttachment = fileName;
                    HtmlNode doc = html.DocumentNode;
                    var reportParams = new List<Tuple<string, string>>();

                    var findContent = doc.SelectNodes("//*[contains(text(),'Func1 start')]/parent::*/td[@class='timestamp']");
                    foreach (var item in findContent)
                    {
                        reportParams.Add(Tuple.Create("Func1 start", item.InnerHtml));
                    }

                    findContent = doc.SelectNodes("//*[contains(text(),'Func1 end')]/parent::*/td[@class='timestamp']");
                    foreach (var item in findContent)
                    {
                        reportParams.Add(Tuple.Create("Func1 end", item.InnerHtml));
                    }


                    findContent = doc.SelectNodes("//*[contains(text(),'Func2 start')]/parent::*/td[@class='timestamp']");
                    foreach (var item in findContent)
                    {
                        reportParams.Add(Tuple.Create("Func2 start", item.InnerHtml));
                    }

                    findContent = doc.SelectNodes("//*[contains(text(),'Func2 end')]/parent::*/td[@class='timestamp']");
                    foreach (var item in findContent)
                    {
                        reportParams.Add(Tuple.Create("Func2 end", item.InnerHtml));
                    }

                    findContent = doc.SelectNodes("//*[contains(text(),'Func3 start')]/parent::*/td[@class='timestamp']");
                    foreach (var item in findContent)
                    {
                        reportParams.Add(Tuple.Create("Func3 start", item.InnerHtml));
                    }

                    findContent = doc.SelectNodes("//*[contains(text(),'Func3 end')]/parent::*/td[@class='timestamp']");
                    foreach (var item in findContent)
                    {
                        reportParams.Add(Tuple.Create("Func3 end", item.InnerHtml));
                    }


                    findContent = doc.SelectNodes("//*[contains(text(),'Func4 start')]/parent::*/td[@class='timestamp']");
                    foreach (var item in findContent)
                    {
                        reportParams.Add(Tuple.Create("Func4 start", item.InnerHtml));
                    }

                    findContent = doc.SelectNodes("//*[contains(text(),'Func4 end')]/parent::*/td[@class='timestamp']");
                    foreach (var item in findContent)
                    {
                        reportParams.Add(Tuple.Create("Func4 end", item.InnerHtml));
                    }

                    var deltaForNewAdvisorAccountCreation = DateTime.Parse(reportParams[1].Item2).Subtract(DateTime.Parse(reportParams[0].Item2)).TotalSeconds;
                    var deltaForAccountActivation = DateTime.Parse(reportParams[3].Item2).Subtract(DateTime.Parse(reportParams[2].Item2)).TotalSeconds;
                    var deltaForOrganizationCreationForAdvisor = DateTime.Parse(reportParams[5].Item2).Subtract(DateTime.Parse(reportParams[4].Item2)).TotalSeconds;
                    var deltaForAddingWidgetForDashboard = DateTime.Parse(reportParams[7].Item2).Subtract(DateTime.Parse(reportParams[6].Item2)).TotalSeconds;

                    ter.NewAdvisorAccountCreation = Convert.ToInt32(deltaForNewAdvisorAccountCreation);
                    ter.AccountActivation = Convert.ToInt32(deltaForAccountActivation);
                    ter.OrganizationCreationForAdvisor = Convert.ToInt32(deltaForOrganizationCreationForAdvisor);
                    ter.AddingWidgetForDashboard = Convert.ToInt32(deltaForAddingWidgetForDashboard);

                    string json = JsonConvert.SerializeObject(ter);
                    Console.WriteLine(json);
                    File.WriteAllText(@"TestExecutionResult.json", json);
                    Console.WriteLine("json file saved...");
                    if (Directory.Exists(@"C:\inetpub\wwwroot"))
                    {
                        File.Copy("TestExecutionResult.json", @"C:\inetpub\wwwroot\TestExecutionResult.json", true);
                        File.Copy("StackedColumnChart.html", @"C:\inetpub\wwwroot\StackedColumnChart.html", true);
                    }
                    else
                    {
                        Console.WriteLine("You need apache server to be installed on your system which has publishing folder structure in the format - \"C:\\apache\\htdocs\" for the chart to be plotted using google visualisation by this application");
                        Console.ReadKey();
                    }
                    break;
                }
                else
                {
                    var message = "The output doesn't have any report file generated";
                    Console.WriteLine(message);
                }
            }

            if (Directory.Exists(@"C:\inetpub\wwwroot"))
            {
                Process.Start($"https://{GetLocalIPAddress()}/StackedColumnChart.html");
            }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters available");
        }
    }
}
