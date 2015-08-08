using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace ProxyTesting
{
    class ProxyTest
    {
        private static int testNo { get; set; }
        private static int successfulResults { get; set; }


        public static void TestProxies(DataTable _proxylistDT, string testUrl, int maxThreads)
        {
            Console.WriteLine("Testing.....");
            Parallel.For(0, _proxylistDT.Rows.Count, new ParallelOptions { MaxDegreeOfParallelism = maxThreads },
              i =>
              {
                  testNo++;
                  Console.Write(string.Format("\r{0} / {1}", testNo, _proxylistDT.Rows.Count));
                  Request(_proxylistDT.Rows[i], testUrl);
              });
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nSuccessful results: {0}", successfulResults);
            Console.ForegroundColor = ConsoleColor.White;

        }
        private static void Request(DataRow _dtRow, string testURL)
        {
            try
            {

                string proxyip = _dtRow["IP"].ToString();
                int port = Convert.ToInt32(_dtRow["Port"].ToString());
                Stopwatch sw = new Stopwatch();
                sw.Start();

                HttpWebRequest myHttpWebRequest = null;     //Declare an HTTP-specific implementation of the WebRequest class.
                HttpWebResponse myHttpWebResponse = null;   //Declare an HTTP-specific implementation of the WebResponse class

                //Create Request
                myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(testURL);
                myHttpWebRequest.Method = "GET";
                myHttpWebRequest.ContentType = "text/xml; encoding='utf-8'";

                //why the hell wait more than 5 seconds....
                myHttpWebRequest.Timeout = 5000;
                myHttpWebRequest.ReadWriteTimeout = 5000;

                //set proxy
                WebProxy proxy = new WebProxy(proxyip, port);
                myHttpWebRequest.Proxy = proxy;


                //Get Response
                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

                sw.Stop();

                int testResult = 0;
                if (sw.Elapsed < new TimeSpan(0, 0, 1))
                    testResult = 1;
                else if (sw.Elapsed < new TimeSpan(0, 0, 2))
                    testResult = 2;
                else if (sw.Elapsed < new TimeSpan(0, 0, 3))
                    testResult = 3;
                else if (sw.Elapsed < new TimeSpan(0, 0, 4))
                    testResult = 4;
                else if (sw.Elapsed < new TimeSpan(0, 0, 5))
                    testResult = 5;

                ProxyWorld.sqlProcedures.InsertProxyResults.InsertResult(proxyip, port, DateTime.Now, "PhyregoldProxyTester", sw.Elapsed, testResult);
                successfulResults++;
            }
            catch
            {

            }
        }



        public static void ProcessTestResults()
        {


        }
    }
}
