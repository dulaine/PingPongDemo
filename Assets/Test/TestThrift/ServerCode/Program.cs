using System;
using System.Collections.Generic;
using System.Text;
using Thrift.Server;
using Thrift.Transport;
using Thrift.Protocol;
using Thrift;
using Test;
using System.Threading;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TestHandler handler = new TestHandler();
                CommunicateTest.Processor processor = new CommunicateTest.Processor(handler);
                TServerTransport transport = new TServerSocket(9090);
                TServer server = new TSimpleServer(processor, transport);
                Console.WriteLine("启动……");
                server.Serve();
            }
            catch (Exception x)
            {
                Console.WriteLine(x.StackTrace);
            }
            Console.WriteLine("done.");
        }
    }

    class TestHandler : CommunicateTest.Iface
    {
        public string echo(string _s)
        {
            Console.WriteLine(DateTime.Now);
            Thread.Sleep(1500);
            Console.WriteLine(DateTime.Now);
            return "haha " + _s;
        }
    }
}
