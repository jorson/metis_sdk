using System;
using Metis.ClientSdk;
using Metis.ClientSdk.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;
using System.Security.Cryptography;
using System.Text;

namespace Metis.ClientSdkTest
{
    [TestClass]
    public class SdkTest
    {
        [TestInitialize]
        public void Setup()
        {
//            GathererContext.Setup();
        }

        [TestMethod]
        public void CreateApiCallEntryTest()
        {
            ApiCallEntity entry = new ApiCallEntity()
            {
                AccessToken = "accesstoken",
                CallClientId = 10,
                CallUrl = "/login",
                ClientId = 7,
                LogType = "api",
                UserId = 13123
            };

            Assert.AreEqual(entry.CallClientId, 10);
        }

        [TestMethod]
        public void CsvSerializerSingleTest()
        {
            ApiCallEntity entry = new ApiCallEntity()
            {
                AccessToken = "accesstoken",
                CallClientId = 10,
                CallUrl = "/login",
                ClientId = 7,
                LogType = "api",
                UserId = 13123
            };
            CsvSerializer serializer = new CsvSerializer();
            string result = serializer.SerializeSingle<ApiCallEntity>(entry);

            System.Diagnostics.Debug.WriteLine(result);
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void CsvSerializerMutilTest()
        {
            List<ApiCallEntity> entry = new List<ApiCallEntity>();
            for (int i = 0; i < 5; i++)
            {
                entry.Add(new ApiCallEntity()
                {
                    AccessToken = "accesstoken",
                    CallClientId = 10,
                    CallUrl = "/login",
                    ClientId = 7,
                    LogType = "api",
                    UserId = 13123
                });
            }
            CsvSerializer serializer = new CsvSerializer();
            string result = serializer.SerializeMutil<ApiCallEntity>(entry);

            System.Diagnostics.Debug.WriteLine(result);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SendLogTest()
        {
            Random rand = new Random();

            for (int i = 0; i < 10000; i++)
            {
#if DEBUG
                GathererContext.Current.AppendApiCall(GathererContext.AccessToken, "/login", 19, rand.Next(), rand.Next(), rand.Next(), rand.Next());
#else 
#endif
                Console.WriteLine("Send Message At " + i);
                Thread.Sleep(200);
            }
            System.Diagnostics.Debug.WriteLine("Send Over");
            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public void Md5Test()
        {
            System.Text.UTF8Encoding utf8 = new System.Text.UTF8Encoding();
            var bytes = utf8.GetBytes("");

            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var results = md5.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < results.Length; i++)
            {
                sb.Append(results[i].ToString("x").PadLeft(2, '0'));
            }

            Console.WriteLine(sb.ToString());
            Assert.AreEqual(1, 1);
        } 
    }
}
