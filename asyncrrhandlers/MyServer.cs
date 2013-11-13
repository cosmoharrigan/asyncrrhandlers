using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMQ;

namespace asyncrrhandlers
{
    public class MyServer
    {
        ZMQ.Socket frontend;
        ZMQ.Socket backend;
        ZMQ.Context context;

        public void Run()
        {
            context = new ZMQ.Context(1);
            frontend = context.Socket(SocketType.ROUTER);
            frontend.Bind("tcp://*:5570");

            backend = context.Socket(SocketType.DEALER);
            backend.Bind("inproc://backend");
            
            PollItem[] pollItems = new PollItem[2];
            pollItems[0] = frontend.CreatePollItem(IOMultiPlex.POLLIN);
            pollItems[0].PollInHandler += new PollHandler(FrontendPollInHandler);
            pollItems[1] = backend.CreatePollItem(IOMultiPlex.POLLIN);
            pollItems[1].PollInHandler += new PollHandler(BackendPollInHandler);
            while (true)
            {
                context.Poll(pollItems, -1);
            }            
        }

        private async void FrontendPollInHandler(Socket socket, IOMultiPlex revents)
        {
            //  Process all parts of the message
            bool isProcessing = true;
            while (isProcessing)
            {
                string _id = socket.Recv(Encoding.Unicode);
                string message = socket.Recv(Encoding.Unicode);
                Console.WriteLine("Server received " + message);
                RequestHandler handler = new RequestHandler(context, _id + "", message + "");
                await handler.start();
                isProcessing = socket.RcvMore;
            }
        }

        private void BackendPollInHandler(Socket socket, IOMultiPlex revents)
        {
            //  Process all parts of the message
            bool isProcessing = true;
            while (isProcessing)
            {
                string _id = socket.Recv(Encoding.Unicode);
                string message = socket.Recv(Encoding.Unicode);
                Console.WriteLine("Server sending to frontend " + message);
                frontend.Send(_id, Encoding.Unicode, SendRecvOpt.SNDMORE);
                frontend.Send(message, Encoding.Unicode);
                isProcessing = socket.RcvMore;
            }
        }
    }
}
