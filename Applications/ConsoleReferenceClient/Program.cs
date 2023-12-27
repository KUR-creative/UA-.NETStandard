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
        static int inc(int x)
        {
            return x + 1;
        }

        ///public static async Task Main(string[] args)
        public static async Task Main(string[] args)
        {
            TextWriter output = Console.Out;
            //Uri serverUrl = new Uri("opc.tcp://localhost:4840");
            //Uri serverUrl = new Uri("opc.tcp://localhost:62541/Quickstarts/ReferenceServer");
            //Uri serverUrl = new Uri("opc.tcp://localhost:4840");
            Uri serverUrl = new Uri(args[0]);

            //ApplicationInstance.MessageDlg = new ApplicationMessageDlg(output);
            ApplicationInstance app = new ApplicationInstance {
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
                ) {
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
                    //new ReadValueId() { NodeId = new NodeId("root", 0), AttributeId = Attributes.Value },
                    //new ReadValueId() { NodeId = new NodeId("root", 1), AttributeId = Attributes.Value },
                    //new ReadValueId() { NodeId = new NodeId("root", 2), AttributeId = Attributes.Value },
                    //new ReadValueId() { NodeId = new NodeId("the.answer", 0), AttributeId = Attributes.Value },
                    //new ReadValueId() { NodeId = new NodeId("the.answer", 1), AttributeId = Attributes.Value },
                    new ReadValueId() { NodeId = new NodeId("the.answer", 2), AttributeId = Attributes.Value },
                    //new ReadValueId() { NodeId = new NodeId("the.answer", 3), AttributeId = Attributes.Value },
                    //new ReadValueId() { NodeId = new NodeId("the.answer", 4), AttributeId = Attributes.Value },
                    //new ReadValueId() { NodeId = new NodeId("the.answer", 5), AttributeId = Attributes.Value },
                    //new ReadValueId() { NodeId = new NodeId("ns=2;s=Scalar_Static_Int32"), AttributeId = Attributes.Value },
                    //new ReadValueId() { NodeId = new NodeId("Scalar_Static_Int16", 2), AttributeId = Attributes.Value },
                    //new ReadValueId() { NodeId = new NodeId("Scalar_Static_Decimal", 2), AttributeId = Attributes.Value },
                    //new ReadValueId() { NodeId = new NodeId("Scalar_Static_Int32", 2), AttributeId = Attributes.Value },
                    //new ReadValueId() { NodeId = new NodeId("Scalar_Static_Int64", 2), AttributeId = Attributes.Value },
                    //new ReadValueId() { NodeId = new NodeId("Scalar", 1), AttributeId = Attributes.Value },
                    //new ReadValueId() { NodeId = new NodeId("Scalar_Instructions", 0), AttributeId = Attributes.Value },
                    //new ReadValueId() { NodeId = new NodeId("Scalar_Instructions", 1), AttributeId = Attributes.Value },
                    //new ReadValueId() { NodeId = new NodeId("Scalar_Instructions", 2), AttributeId = Attributes.Value },
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

                /*
                // Browse
                Browser browser = new Browser(client.Session);
                browser.BrowseDirection = BrowseDirection.Forward;
                //browser.NodeClassMask = (int)NodeClass.Object | (int)NodeClass.Variable;
                browser.NodeClassMask = (int)NodeClass.Variable;
                browser.ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences;
                browser.IncludeSubtypes = true;
                NodeId nodeToBrowse = ObjectIds.Server;

                // Call Browse service
                output.WriteLine("Browsing {0} node...", nodeToBrowse);
                ReferenceDescriptionCollection browseResults = browser.Browse(nodeToBrowse);

                // Display the results
                output.WriteLine("Browse returned {0} results:", browseResults.Count);

                foreach (ReferenceDescription result in browseResults)
                {
                    output.WriteLine("     DisplayName = {0}, NodeClass = {1}", result.DisplayName.Text, result.NodeClass);
                }
                */
            }
        }
    }
}

/*
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace Quickstarts.ConsoleReferenceClient
{
    /// <summary>
    /// The program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main entry point.
        /// </summary>
        public static async Task Main(string[] args)
        {
            TextWriter output = Console.Out;
            output.WriteLine("OPC UA Console Reference Client");

            output.WriteLine("OPC UA library: {0} @ {1} -- {2}",
                Utils.GetAssemblyBuildNumber(),
                Utils.GetAssemblyTimestamp().ToString("G", CultureInfo.InvariantCulture),
                Utils.GetAssemblySoftwareVersion());

            // The application name and config file names
            var applicationName = "ConsoleReferenceClient";
            var configSectionName = "Quickstarts.ReferenceClient";
            var usage = $"Usage: dotnet {applicationName}.dll [OPTIONS]";

            // command line options
            bool showHelp = false;
            bool autoAccept = false;
            string username = null;
            string userpassword = null;
            bool logConsole = false;
            bool appLog = false;
            bool renewCertificate = false;
            bool loadTypes = false;
            bool browseall = false;
            bool fetchall = false;
            bool jsonvalues = false;
            bool verbose = false;
            bool subscribe = false;
            bool noSecurity = false;
            string password = null;
            int timeout = Timeout.Infinite;
            string logFile = null;
            string reverseConnectUrlString = null;

            Mono.Options.OptionSet options = new Mono.Options.OptionSet {
                usage,
                { "h|help", "show this message and exit", h => showHelp = h != null },
                { "a|autoaccept", "auto accept certificates (for testing only)", a => autoAccept = a != null },
                { "nsec|nosecurity", "select endpoint with security NONE, least secure if unavailable", s => noSecurity = s != null },
                { "un|username=", "the name of the user identity for the connection", (string u) => username = u },
                { "up|userpassword=", "the password of the user identity for the connection", (string u) => userpassword = u },
                { "c|console", "log to console", c => logConsole = c != null },
                { "l|log", "log app output", c => appLog = c != null },
                { "p|password=", "optional password for private key", (string p) => password = p },
                { "r|renew", "renew application certificate", r => renewCertificate = r != null },
                { "t|timeout=", "timeout in seconds to exit application", (int t) => timeout = t * 1000 },
                { "logfile=", "custom file name for log output", l => { if (l != null) { logFile = l; } } },
                { "lt|loadtypes", "Load custom types", lt => { if (lt != null) loadTypes = true; } },
                { "b|browseall", "Browse all references", b => { if (b != null) browseall = true; } },
                { "f|fetchall", "Fetch all nodes", f => { if (f != null) fetchall = true; } },
                { "j|json", "Output all Values as JSON", j => { if (j != null) jsonvalues = true; } },
                { "v|verbose", "Verbose output", v => { if (v != null) verbose = true; } },
                { "s|subscribe", "Subscribe", s => { if (s != null) subscribe = true; } },
                { "rc|reverseconnect=", "Connect using the reverse connect endpoint. (e.g. rc=opc.tcp://localhost:65300)", (string url) => reverseConnectUrlString = url},
            };

            ReverseConnectManager reverseConnectManager = null;

            try
            {
                // parse command line and set options
                var extraArg = ConsoleUtils.ProcessCommandLine(output, args, options, ref showHelp, "REFCLIENT", false);

                // connect Url?
                Uri serverUrl = new Uri("opc.tcp://localhost:62541/Quickstarts/ReferenceServer");
                if (!string.IsNullOrEmpty(extraArg))
                {
                    serverUrl = new Uri(extraArg);
                }

                // log console output to logger
                if (logConsole && appLog)
                {
                    output = new LogWriter();
                }

                // Define the UA Client application
                ApplicationInstance.MessageDlg = new ApplicationMessageDlg(output);
                CertificatePasswordProvider PasswordProvider = new CertificatePasswordProvider(password);
                ApplicationInstance application = new ApplicationInstance {
                    ApplicationName = applicationName,
                    ApplicationType = ApplicationType.Client,
                    ConfigSectionName = configSectionName,
                    CertificatePasswordProvider = PasswordProvider
                };

                // load the application configuration.
                var config = await application.LoadApplicationConfiguration(silent: false).ConfigureAwait(false);

                // override logfile
                if (logFile != null)
                {
                    var logFilePath = config.TraceConfiguration.OutputFilePath;
                    var filename = Path.GetFileNameWithoutExtension(logFilePath);
                    config.TraceConfiguration.OutputFilePath = logFilePath.Replace(filename, logFile);
                    config.TraceConfiguration.DeleteOnLoad = true;
                    config.TraceConfiguration.ApplySettings();
                }

                // setup the logging
                ConsoleUtils.ConfigureLogging(config, applicationName, logConsole, LogLevel.Information);

                // delete old certificate
                if (renewCertificate)
                {
                    await application.DeleteApplicationInstanceCertificate().ConfigureAwait(false);
                }

                // check the application certificate.
                bool haveAppCertificate = await application.CheckApplicationInstanceCertificate(false, minimumKeySize: 0).ConfigureAwait(false);
                if (!haveAppCertificate)
                {
                    throw new ErrorExitException("Application instance certificate invalid!", ExitCode.ErrorCertificate);
                }

                if (reverseConnectUrlString != null)
                {
                    // start the reverse connection manager
                    output.WriteLine("Create reverse connection endpoint at {0}.", reverseConnectUrlString);
                    reverseConnectManager = new ReverseConnectManager();
                    reverseConnectManager.AddEndpoint(new Uri(reverseConnectUrlString));
                    reverseConnectManager.StartService(config);
                }

                // wait for timeout or Ctrl-C
                var quitCTS = new CancellationTokenSource();
                var quitEvent = ConsoleUtils.CtrlCHandler(quitCTS);

                // connect to a server until application stops
                bool quit = false;
                DateTime start = DateTime.UtcNow;
                int waitTime = int.MaxValue;
                do
                {
                    if (timeout > 0)
                    {
                        waitTime = timeout - (int)DateTime.UtcNow.Subtract(start).TotalMilliseconds;
                        if (waitTime <= 0)
                        {
                            break;
                        }
                    }

                    // create the UA Client object and connect to configured server.

                    using (UAClient uaClient = new UAClient(application.ApplicationConfiguration, reverseConnectManager, output, ClientBase.ValidateResponse) {
                        AutoAccept = autoAccept,
                        SessionLifeTime = 60_000,
                    })
                    {
                        // set user identity
                        if (!String.IsNullOrEmpty(username))
                        {
                            uaClient.UserIdentity = new UserIdentity(username, userpassword ?? string.Empty);
                        }

                        bool connected = await uaClient.ConnectAsync(serverUrl.ToString(), !noSecurity, quitCTS.Token).ConfigureAwait(false);
                        if (connected)
                        {
                            output.WriteLine("Connected! Ctrl-C to quit.");

                            // enable subscription transfer
                            uaClient.ReconnectPeriod = 1000;
                            uaClient.ReconnectPeriodExponentialBackoff = 10000;
                            uaClient.Session.MinPublishRequestCount = 3;
                            uaClient.Session.TransferSubscriptionsOnReconnect = true;
                            var samples = new ClientSamples(output, ClientBase.ValidateResponse, quitEvent, verbose);
                            if (loadTypes)
                            {
                                await samples.LoadTypeSystemAsync(uaClient.Session).ConfigureAwait(false);
                            }

                            if (browseall || fetchall || jsonvalues)
                            {
                                NodeIdCollection variableIds = null;
                                ReferenceDescriptionCollection referenceDescriptions = null;
                                if (browseall)
                                {
                                    referenceDescriptions =
                                        await samples.BrowseFullAddressSpaceAsync(uaClient, Objects.RootFolder).ConfigureAwait(false);
                                    variableIds = new NodeIdCollection(referenceDescriptions
                                        .Where(r => r.NodeClass == NodeClass.Variable && r.TypeDefinition.NamespaceIndex != 0)
                                        .Select(r => ExpandedNodeId.ToNodeId(r.NodeId, uaClient.Session.NamespaceUris)));
                                }

                                IList<INode> allNodes = null;
                                if (fetchall)
                                {
                                    allNodes = await samples.FetchAllNodesNodeCacheAsync(uaClient, Objects.RootFolder, true, true, false).ConfigureAwait(false);
                                    variableIds = new NodeIdCollection(allNodes
                                        .Where(r => r.NodeClass == NodeClass.Variable && r is VariableNode && ((VariableNode)r).DataType.NamespaceIndex != 0)
                                        .Select(r => ExpandedNodeId.ToNodeId(r.NodeId, uaClient.Session.NamespaceUris)));
                                }

                                if (jsonvalues && variableIds != null)
                                {
                                    var (allValues, results) = await samples.ReadAllValuesAsync(uaClient, variableIds).ConfigureAwait(false);
                                }

                                if (subscribe && (browseall || fetchall))
                                {
                                    // subscribe to 100 random variables
                                    const int MaxVariables = 100;
                                    NodeCollection variables = new NodeCollection();
                                    Random random = new Random(62541);
                                    if (fetchall)
                                    {
                                        variables.AddRange(allNodes
                                            .Where(r => r.NodeClass == NodeClass.Variable && r.NodeId.NamespaceIndex > 1)
                                            .Select(r => ((VariableNode)r))
                                            .OrderBy(o => random.Next())
                                            .Take(MaxVariables));
                                    }
                                    else if (browseall)
                                    {
                                        var variableReferences = referenceDescriptions
                                            .Where(r => r.NodeClass == NodeClass.Variable && r.NodeId.NamespaceIndex > 1)
                                            .Select(r => r.NodeId)
                                            .OrderBy(o => random.Next())
                                            .Take(MaxVariables)
                                            .ToList();
                                        variables.AddRange(uaClient.Session.NodeCache.Find(variableReferences).Cast<Node>());
                                    }

                                    await samples.SubscribeAllValuesAsync(uaClient,
                                        variableIds: new NodeCollection(variables),
                                        samplingInterval: 1000,
                                        publishingInterval: 5000,
                                        queueSize: 10,
                                        lifetimeCount: 12,
                                        keepAliveCount: 2).ConfigureAwait(false);

                                    // Wait for DataChange notifications from MonitoredItems
                                    output.WriteLine("Subscribed to {0} variables. Press Ctrl-C to exit.", MaxVariables);
                                    quit = quitEvent.WaitOne(timeout > 0 ? waitTime : Timeout.Infinite);
                                }
                                else
                                {
                                    quit = true;
                                }
                            }
                            else
                            {
                                // Run tests for available methods on reference server.
                                samples.ReadNodes(uaClient.Session);
                                samples.WriteNodes(uaClient.Session);
                                samples.Browse(uaClient.Session);
                                samples.CallMethod(uaClient.Session);
                                samples.SubscribeToDataChanges(uaClient.Session, 120_000);

                                output.WriteLine("Waiting...");

                                // Wait for some DataChange notifications from MonitoredItems
                                quit = quitEvent.WaitOne(timeout > 0 ? waitTime : 30_000);
                            }

                            output.WriteLine("Client disconnected.");

                            uaClient.Disconnect();
                        }
                        else
                        {
                            output.WriteLine("Could not connect to server! Retry in 10 seconds or Ctrl-C to quit.");
                            quit = quitEvent.WaitOne(Math.Min(10_000, waitTime));
                        }
                    }

                } while (!quit);

                output.WriteLine("Client stopped.");
            }
            catch (Exception ex)
            {
                output.WriteLine(ex.Message);
            }
            finally
            {
                Utils.SilentDispose(reverseConnectManager);
                output.Close();
            }
        }
    }
}

*/