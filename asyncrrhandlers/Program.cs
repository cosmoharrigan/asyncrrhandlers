using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace asyncrrhandlers
{
    class Program
    {
        private const int NUMBER_OF_CLIENTS = 100;
        private const int PROCESSING_TIME = 5;
        
        static void Main(string[] args)
        {
            //DateTime starttime = DateTime.Now;
            Thread serverThread = new Thread(runServer);
            serverThread.Start();

            Thread.Sleep(1000);
            for (int i = 1; i <= NUMBER_OF_CLIENTS; i++)
            {
                Thread clientThread = new Thread(runClient);
                clientThread.Start(i);
            }
            //DateTime endtime = DateTime.Now;
            //TimeSpan elapsed = endtime - starttime;
            //Console.WriteLine("Total time elapsed: %d seconds\nNumber of requests processed: %s\nIndividual request processing time: %s seconds\nParallel request processing time: %.4f seconds", elapsed.Seconds, NUMBER_OF_CLIENTS, PROCESSING_TIME, (float)elapsed.Seconds / NUMBER_OF_CLIENTS);
        }

        private static void runServer()
        {
            MyServer server = new MyServer();
            server.Run();
        }

        private static void runClient(object id)
        {
            MyClient client = new MyClient((int) id);
            client.Run();
        }
    }
}
