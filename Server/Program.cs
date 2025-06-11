using Grpc.Core;
using GrpcSecureAuth;
using System.IO;

class Program
{
    const int Port = 50051;

    static void Main()
    {
        var cert = File.ReadAllText("server.crt");
        var key = File.ReadAllText("server.key");

        var server = new Server
        {
            Services = { AuthService.BindService(new AuthServiceImpl()) },
            Ports = {
                new ServerPort("localhost", Port, new SslServerCredentials(
                    new[] { new KeyCertificatePair(cert, key) }))
            }
        };

        server.Start();
        Console.WriteLine($"Server listening securely on port {Port}");
        Console.ReadKey();
        server.ShutdownAsync().Wait();
    }
}