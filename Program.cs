using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;

class Program
{
    //洛阳2023.7月检测未关机 C#多线程快Ping
    static void Main()
    {
        // 获取当前日期  
        DateTime today = DateTime.Today;  

        string outputFile = @"d:\noshutdown\"+ today.Year+today.Month+today.Day + ".txt";


        // 创建文件并清空内容
        File.WriteAllText(outputFile, "");

        //读取需要ping的ip网段
        string iniFile = "ipConfig.ini";
        List<string> subnetList = ReadIniFile(iniFile);
        

        // 创建线程数组
        Thread[] threads = new Thread[subnetList.Count];

        for (int i = 0; i < subnetList.Count; i++)
        {
            string subnet = subnetList[i];

            // 创建线程，并传递ping操作的方法和参数
            threads[i] = new Thread(() =>
            {
                PingSubnet(subnet, outputFile);
            });

            // 启动线程
            threads[i].Start();
        }

        // 等待所有线程执行完毕
        foreach (var thread in threads)
        {
            thread.Join();
        }

        Console.WriteLine("Ping操作已完成，结果已写入到result.txt文件。");
    }

    static void PingSubnet(string subnet, string outputFile)
    {
        for (int i = 0; i <= 255; i++)
        {
            string ipAddress = subnet + "." + Convert.ToString(i);

            using (Ping ping = new Ping())
            {
                PingReply reply = ping.Send(ipAddress,500);

                if (reply.Status == IPStatus.Success)
                {
                    Console.WriteLine("IP 地址 {0} 可达", ipAddress);

                    WriteLineToFile(outputFile, ipAddress + " is reachable.");
                }
            }
        }
    }

    static void WriteLineToFile(string file, string line)
    {
        lock (file)
        {
            using (StreamWriter writer = File.AppendText(file))
            {
                writer.WriteLine(line);
            }
        }
    }

    static List<string> ReadIniFile(string file)
    {
        List<string> ipList = new List<string>();

        using (StreamReader reader = new StreamReader(file))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                // 忽略空行和注释行
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("//"))
                    continue;

                // 仅读取IP段
                if (line.StartsWith("[") && line.EndsWith("]"))
                    continue;

                ipList.Add(line.Trim());
            }
        }

        return ipList;
    }
}