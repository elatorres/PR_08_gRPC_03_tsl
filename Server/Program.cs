using Grpc.Core;
using GrpcSecureAuth;
using System.IO;

class Program
{
    const int Port = 50051;

    static void Main()
    {
        // Read the private key
        var cert = File.ReadAllText("server.crt");
        // Read the certificate
        var key = File.ReadAllText("server.key");
        // Both sould be in the directory with the server executable.

        // Let's create a gRPC Server with TSL encryption using cert and key
        var server = new Server
        {
            Services = { AuthService.BindService(new AuthServiceImpl()) },
            Ports = {
                new ServerPort("localhost", Port, new SslServerCredentials(
                    new[] { new KeyCertificatePair(cert, key) }))
            }
        };

        server.Start();  // Start the Server
        Console.WriteLine($"Server listening securely on port {Port}");
        Console.WriteLine($"Press any key to terminate Server...");
        Console.ReadKey();
        server.ShutdownAsync().Wait();
    }
}