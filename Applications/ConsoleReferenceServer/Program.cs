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
            // Minimal server code that communicate with ProgramOrigin client
            var server = new UAServer<MyServer>(output)
            {
                AutoAccept = true,
            };
            await server.LoadAsync("testCsharpServer", "Quickstarts.ReferenceServer").ConfigureAwait(false);
            output.WriteLine("loading complete");

            await server.CheckCertificateAsync(false).ConfigureAwait(false);
            output.WriteLine("cert is checked(accept all)");

            server.Create(Quickstarts.Servers.Utils.NodeManagerFactories);
            // Applications/Quickstarts.Servers/Utils.cs에 정의됨
            // GetNodeManagerFactories는 INodeManagerFactory를 구현한
            // 모든 팩토리를 가져온다고? 도대체 왜 이런 짓을 하는거야?
            // 그냥 팩토리 클래스의 ctor로 만들면 안 될까? 왜 이러는데?
            output.WriteLine("Created and added the node managers");

            await server.StartAsync().ConfigureAwait(false);
            output.WriteLine("Server started");

            var quitEvent = ConsoleUtils.CtrlCHandler();
            bool ctrlc = quitEvent.WaitOne(-1);
        }

        public static async Task TryingMainWithoutFramework(string[] args)
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

            // Add node(The answer 42) // factory 없이 해 보자?
            // ReferenceServer에서 ReferenceNodeManager, factory 이런 거 쓰는 듯?
            // 파일: Applications/Quickstarts.Servers/ReferenceServer/ReferenceNodeManager
            // 저 파일에 서버가 보내는 것들이 정의되어 있다. 후...

            // INodeManager2에서 인터페이스 정의한 것을
            // Opc.Ua.Server의 CustomNodeManager2에서 구현
            // CustomNodeManager2을 상속하여 ReferenceNodeManager를 만든다
            output.WriteLine(Quickstarts.Servers.Utils.NodeManagerFactories);
            output.WriteLine(Quickstarts.Servers.Utils.NodeManagerFactories.Count);
            foreach (var factory in Quickstarts.Servers.Utils.NodeManagerFactories)
            {
                output.WriteLine(factory);
            }
        }

        public static async Task MinimalServerMain(string[] args)
        {
            TextWriter output = Console.Out;
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
            // Applications/Quickstarts.Servers/Utils.cs에 정의됨
            // GetNodeManagerFactories는 INodeManagerFactory를 구현한
            // 모든 팩토리를 가져온다고? 도대체 왜 이런 짓을 하는거야?
            // 그냥 팩토리 클래스의 ctor로 만들면 안 될까? 왜 이러는데?
            output.WriteLine("Created and added the node managers");

            await server.StartAsync().ConfigureAwait(false);
            output.WriteLine("Server started");

            var quitEvent = ConsoleUtils.CtrlCHandler();
            bool ctrlc = quitEvent.WaitOne(-1);
        }
    }
}