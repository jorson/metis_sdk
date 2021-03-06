﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Metis.ClientSdkConsole
{
    class Program
    {
        static string[] urls = {
                               "/v3/course/unitcourse/stat/list?uids=0&uids=0&uids=0&uids=0&uids=0&uids=0&uids=0&uids=0&uids=0&uids=0&accesstoken={0}",
                                "/v3/resource/resourcecatalog/list?appId=78&rcids=2578&rcids=2579&rcids=2580&rcids=2581&accesstoken={0}",
                                "/v3/course/unitcourse/stat?unitId=0&accesstoken={0}",
                                "/v3/course/unitexercise/get?unitId=2519&unitExerciseId=576&accesstoken={0}",
                                "/v3/course/unitpaper/list?unitId=2519&upids=0&upids=0&upids=0&upids=0&upids=0&upids=0&upids=0&upids=0&upids=0&upids=0&accesstoken={0}",
                                "/v3/course/unitquestion/list?unitId=0&uqids=0&uqids=0&uqids=0&uqids=0&uqids=0&uqids=0&uqids=0&uqids=0&uqids=0&uqids=0&accesstoken={0}",
                                "/v3/course/unitcourse/list?uids=1735&uids=1734&uids=1733&uids=1732&uids=1731&uids=1730&uids=1729&uids=1728&uids=1727&uids=1726&accesstoken={0}",
                                "/v3/course/unitresource/search?accesstoken={0}",
                                "/v3/course/unitvideo/list?unitId=2519&uvids=0&uvids=0&accesstoken={0}",
                                "/v3/course/unitword/search?accesstoken={0}",
                                "/v3/course/unitvideo/query?unitId=2519&unitVideoId=0&accesstoken={0}",
                                "/v3/course/unitword/query?unitId=0&unitWordId=0&accesstoken={0}",
                                "/v3/course/unitvideo/get?unitId=2519&unitVideoId=0&accesstoken={0}",
                                "/v3/course/catalog/tree?unitId=2519&accesstoken={0}",
                                "/v3/course/unitexercise/get?unitId=0&unitExerciseId=576&accesstoken={0}",
                                "/v3/course/unitword/datanode/unitpaper?unitId=0&unitPaperId=0&accesstoken={0}",
                                "/v3/resource/video/object/create?bucketId=158&title=20140612040615&fileName=原20140612040615.mp4&fileSize=1000&lastModifyTime=2014/6/12+16:06:15&accesstoken={0}",
                                "/v3/course/catalog/tree?unitId=2519&accesstoken={0}"
                               };
        static string accessToken = "bdf1b1c9f1bf4eb89f4483802f00482a";
        static string httpUrl = "http://dev.cloud.91open.ty.nd";


        static void Main(string[] args)
        {
            //Console.WriteLine("127.0.0.1:" + IPToNumber("127.0.0.1"));
            //Console.WriteLine("192.168.206.40:" + IPToNumber("192.168.206.40"));
            //Console.WriteLine("121.207.105.15:" + IPToNumber("121.207.105.15"));
            //Console.WriteLine("255.255.255.255:" + IPToNumber("255.255.255.255"));
            //Console.WriteLine("0.0.0.0:" + IPToNumber("0.0.0.0"));
            for (int i = 0; i < 5; i++)
            {
                Thread thread = new Thread(new ThreadStart(SendMessage));
                thread.IsBackground = true;
                thread.Start();
            }
            Console.Read();
        }

        static void BlockCollectionDemo()
        {
            BlockingCollection<string> collection = new BlockingCollection<string>(
                new ConcurrentQueue<string>(), 2000);
            
        }

        static long IPToNumber(string strIPAddress)
        {
            //将目标IP地址字符串strIPAddress转换为数字
            string[] arrayIP = strIPAddress.Split('.');
            UInt32 sip1 = UInt32.Parse(arrayIP[0]);
            UInt32 sip2 = UInt32.Parse(arrayIP[1]);
            UInt32 sip3 = UInt32.Parse(arrayIP[2]);
            UInt32 sip4 = UInt32.Parse(arrayIP[3]);
            long tmpIpNumber;
            tmpIpNumber = sip1 * 256 * 256 * 256 + sip2 * 256 * 256 + sip3 * 256 + sip4;
            return tmpIpNumber;
        }

        static void SendMessage()
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime = DateTime.Now.AddHours(5);
            Random random = new Random();
            int urlLength = urls.Length - 1;
            Random random2 = new Random();
            int sleep = 0;
            
            using(System.Net.WebClient client = new System.Net.WebClient())
            {
                string requestUrl = String.Empty, result = String.Empty;
                int id = 0;
                while (DateTime.Now < endTime)
                {
                    id = random.Next(urlLength);
                    sleep = random2.Next(100, 300);
                    requestUrl = httpUrl + String.Format(urls[id], accessToken);
                    try
                    {
                        result = client.DownloadString(requestUrl);
                        Console.WriteLine("Thread Id:" + Thread.CurrentThread.ManagedThreadId + ";SendOver:" + 
                           DateTime.Now.Subtract(startTime).TotalMinutes + ";Result Length:" + result.Length);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Error!" + ex.Message);
                        continue;
                    }

                    Thread.Sleep(sleep);
                }
            }            
        }
    }
}
