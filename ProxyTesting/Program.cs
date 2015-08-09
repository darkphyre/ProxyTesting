using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Threading;

namespace ProxyTesting
{
    class Program
    {
        static void Main(string[] args)
        {

            string file = @"C:\proxylist.txt";
            //read from files if needes
            if(System.IO.File.Exists(file))
                ProxyTest.ReadTextFileCreateNewRecords(@"C:\proxylist.txt");
            //start statistics
            Statistics("START");


            //grab all proxies from database
            DataTable allProxies = ProxyWorld.sqlProcedures.GrabProxyList.GrabAllProxies();

            //this is the website which we want to test against -- must be full address (http://)
            string website = "http://www.google.com";

            //this is the max # of threads we want to execute at a single time
            int maxThreads = 50;

            //lets test
            ProxyTest.TestProxies(allProxies, website, maxThreads);

            //end statistics
            Statistics("END");


            //if you do not want to clear out crappy results comment this line out
            ProxyWorld.sqlProcedures.SQLCommands.StoredProcedure.sp_DeleteFailedProxyTests();
            Console.ReadLine();

        }

        private static System.Diagnostics.Stopwatch statisticsSW { get; set; }
        private static void Statistics(string StartOrEnd)
        {
            //statistic stop watch
            switch (StartOrEnd.ToUpper())
            {
                case "START":
                    Console.WriteLine(string.Format("Starting @: {0}", DateTime.Now.ToString("mm/dd/yyy - hh:MM:ss")));
                    statisticsSW = new System.Diagnostics.Stopwatch();
                    statisticsSW.Start();
                    break;
                case "END":
                    //end statistic stopwatch
                    Console.WriteLine(string.Format("Ending @: {0}", DateTime.Now.ToString("mm/dd/yyy - hh:MM:ss")));
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Format("Total Time: {0}", statisticsSW.Elapsed));
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }
    }
}
