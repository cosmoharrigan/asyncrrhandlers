using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMQ;

namespace asyncrrhandlers
{
    public class RequestHandler
    {
        private const int PROCESSING_TIME = 5;
        private Context _context;
        private string _id;
        private string _msg;

        public RequestHandler(Context context, string id, string msg)
        {
            _context = context;
            _id = id;
            _msg = msg;
        }

        public async Task start()
        {
            ZMQ.Socket worker = _context.Socket(SocketType.DEALER);
            worker.Connect("inproc://backend");
            Console.WriteLine("Request handler started to process " + _msg);
            await Task.Delay(PROCESSING_TIME * 1000);
            worker.Send(_id, Encoding.Unicode, ZMQ.SendRecvOpt.SNDMORE);
            worker.Send(_msg, Encoding.Unicode);
            Console.WriteLine("Request handler quitting");
        }
    }
}
