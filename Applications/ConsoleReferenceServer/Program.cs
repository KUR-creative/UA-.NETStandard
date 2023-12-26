using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Opc.Ua;
using Opc.Ua.Configuration;
using Quickstarts; // UAServer
using Quickstarts.ReferenceServer; // ReferenceServer

namespace Playground
{
    public class Program
    {
        static void PrintObjectMembers(object obj)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            Console.WriteLine($"Members of {type.Name}:");

            foreach (var property in properties)
            {
                object value = property.GetValue(obj);
                Console.WriteLine($"Property {property.Name}: {value}");
            }

            foreach (var field in fields)
            {
                object value = field.GetValue(obj);
                Console.WriteLine($"Field {field.Name}: {value}");
            }
        }
        static int inc(int x)
        {
            return x + 1;
        }

        //public static void Main(string[] args)
        public static async Task Main(string[] args)
        {
            TextWriter output = Console.Out;

            ApplicationInstance app = new ApplicationInstance
            {
                ApplicationName = "testCsharpServer",
                ApplicationType = ApplicationType.Server,
                ConfigSectionName = "Quickstarts.ReferenceServer",
                CertificatePasswordProvider = new CertificatePasswordProvider(null)
            };

            output.WriteLine("test");
            var config = await app.LoadApplicationConfiguration(false)
                .ConfigureAwait(false); // 내부적으로는 ConfigSectionName으로 xml 파일을 읽어옴. 대체 왜 이렇게 하는데..
            // PrintObjectMembers(config); // 또한 xml의 값을 가져오는 IoC를 하는 듯하다.

            // Accept all without cert
            //output.WriteLine($"test {config.SecurityConfiguration.AutoAcceptUntrustedCertificates}"); // false
            config.SecurityConfiguration.AutoAcceptUntrustedCertificates = true;
            //output.WriteLine($"test {config.SecurityConfiguration.AutoAcceptUntrustedCertificates}"); // true
            bool haveAppCertificate = await app.CheckApplicationInstanceCertificate(
                false, minimumKeySize: 0).ConfigureAwait(false);
            output.WriteLine($"have app cert? = {haveAppCertificate}");
            output.WriteLine("cert is checked(accept all)");

            // Add node(The answer 42) // factory 없이 해 보자

            if (args.Length == 0) return; ///////////////////////////
            // Minimal server code that communicate with ProgramOrigin client
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