using System;
using System.IO;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using Quickstarts;

namespace Playground
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            TextWriter output = Console.Out;
            //Uri serverUrl = new Uri("opc.tcp://localhost:4840");
            //Uri serverUrl = new Uri("opc.tcp://localhost:62541/Quickstarts/ReferenceServer");
            //Uri serverUrl = new Uri("opc.tcp://localhost:4840");
            Uri serverUrl = new Uri(args[0]);

            ApplicationInstance app = new ApplicationInstance
            {
                ApplicationName = "testCsharpClient",
                ApplicationType = ApplicationType.Client,
                ConfigSectionName = "Quickstarts.ReferenceClient",
                CertificatePasswordProvider = new CertificatePasswordProvider(null)
            };

            var config = await app.LoadApplicationConfiguration(silent: false)
                .ConfigureAwait(false);
            using (UAClient client = new UAClient(
                app.ApplicationConfiguration, output,
                ClientBase.ValidateResponse
                )
            {
                AutoAccept = true,
                SessionLifeTime = 60_000,
            })
            {
                bool connected = false;
                connected = await client.ConnectAsync(
                    serverUrl.ToString()).ConfigureAwait(false);
                output.WriteLine($"Is connected? {connected}");

                //var nodeId = new NodeId("the.answer", 1);

                ReadValueIdCollection nodesToRead = new ReadValueIdCollection()
                {
                    // Value of ServerStatus
                    new ReadValueId() { NodeId = Variables.Server_ServerStatus, AttributeId = Attributes.Value },
                    // BrowseName of ServerStatus_StartTime
                    new ReadValueId() { NodeId = Variables.Server_ServerStatus_StartTime, AttributeId = Attributes.BrowseName },
                    // Value of ServerStatus_StartTime
                    new ReadValueId() { NodeId = Variables.Server_ServerStatus_StartTime, AttributeId = Attributes.Value },
                    new ReadValueId() { NodeId = new NodeId("the.answer", 1), AttributeId = Attributes.Value },
                    new ReadValueId() { NodeId = new NodeId("the.answer", 2), AttributeId = Attributes.Value },
                };
                client.Session.Read(
                    null,
                    0,
                    TimestampsToReturn.Both,
                    nodesToRead,
                    out DataValueCollection resultsValues,
                    out DiagnosticInfoCollection diagnosticInfos);

                foreach (DataValue result in resultsValues)
                {
                    output.WriteLine("Read Value = {0} , StatusCode = {1}",
                        result.Value, result.StatusCode);
                }
            }
        }
    }
}