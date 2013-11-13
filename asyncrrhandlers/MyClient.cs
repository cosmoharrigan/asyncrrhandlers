using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMQ;

namespace asyncrrhandlers
{
    public class MyClient
    {
        private int _identity;
        ZMQ.Socket dealer;
        ZMQ.Context context;

        public MyClient(int identity)
        {
            _identity = identity;
        }

        public void Run()
        {
            context = new ZMQ.Context(1);
            dealer = context.Socket(SocketType.DEALER);
            dealer.StringToIdentity(_identity.ToString(), Encoding.Unicode);
            dealer.Connect("tcp://localhost:5570");            

            Console.WriteLine("Client " + _identity + " started" + dealer.IdentityToString(Encoding.Unicode));
            dealer.Send("[request from client " + _identity.ToString() + "]", Encoding.Unicode);
            Console.WriteLine("Req from client " + _identity.ToString() + " sent.");

            PollItem[] pollItems = new PollItem[1];
            pollItems[0] = dealer.CreatePollItem(IOMultiPlex.POLLIN);            
            pollItems[0].PollInHandler += new PollHandler(BackendPollInHandler);
            while (true)
            {            
                context.Poll(pollItems, -1);
            }
        }

        private void BackendPollInHandler(Socket socket, IOMultiPlex revents)
        {
            bool isProcessing = true;
            while (isProcessing)
            {
                if (socket.IdentityToString(Encoding.Unicode) == _identity.ToString())
                {
                    string message = socket.Recv(Encoding.Unicode);
                    Console.WriteLine("Client " + _identity.ToString() + " received reply: " + message);
                }
                isProcessing = socket.RcvMore;
            }
        }
    }
}
