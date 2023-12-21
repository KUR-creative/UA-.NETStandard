/*
using System.Threading.Tasks;
using System;
using Opc.Ua;
using Opc.Ua.Server;
using Opc.Ua.Configuration;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // Create and configure the application configuration
        ApplicationInstance application = new ApplicationInstance();
        application.ApplicationName = "UA-.NETStandard Server";
        application.ApplicationType = ApplicationType.Server;
        application.ConfigSectionName = "UA-.NETStandardServer";

        // Create and configure the server
        StandardServer server = new StandardServer(application);

        server.Started += Server_Started;

        // Create a variable node
        VariableNode variableNode = new VariableNode() {
            NodeId = new NodeId(1, "the.answer"),
            BrowseName = new QualifiedName(1, "the answer"),
            DisplayName = new LocalizedText("en-US", "the answer"),
            Value = new Variant(42),
            DataType = DataTypeIds.Int32
        };

        // Add the variable node to the server's address space
        server.CurrentInstance.AddressSpace.AddChild(variableNode);

        try
        {
            // Start the server
            server.Start();

            Console.WriteLine("Server started. Press Enter to exit.");
            Console.ReadLine();
        }
        finally
        {
            // Stop the server
            server.Stop();
        }
    }

    private static void Server_Started(IServerBase obj)
    {
        Console.WriteLine("Server is running on port {0}", obj.BaseAddress.Port);
    }
}
*/