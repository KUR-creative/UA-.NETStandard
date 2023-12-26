using System;
using System.IO;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Configuration;
using Quickstarts; // UAServer
using Quickstarts.ReferenceServer; // ReferenceServer

namespace Playground
{
    public class Program
    {
        static int inc(int x)
        {
            return x + 1;
        }

        //public static void Main(string[] args)
        public static async Task Main(string[] args)
        {
            // Minimal server code that communicate with ProgramOrigin client
            TextWriter output = Console.Out;

            var server = new UAServer<ReferenceServer>(output)
            {
                AutoAccept = true,
            };
            await server.LoadAsync("testCsharpServer", "Quickstarts.ReferenceServer").ConfigureAwait(false);
            output.WriteLine("loading complete");

            await server.CheckCertificateAsync(false).ConfigureAwait(false);
            output.WriteLine("cert is checked(accept all)");

            server.Create(Quickstarts.Servers.Utils.NodeManagerFactories);
            output.WriteLine("Created and added the node managers");

            await server.StartAsync().ConfigureAwait(false);
            output.WriteLine("Server started");

            var quitEvent = ConsoleUtils.CtrlCHandler();
            bool ctrlc = quitEvent.WaitOne(-1);

        }
    }
}