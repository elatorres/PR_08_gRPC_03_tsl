using Grpc.Core;
using GrpcSecureAuth;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    const string Host = "localhost";
    const int Port = 50051;

    static async Task Main()
    {
        var cert = File.ReadAllText("server.crt");
        var credentials = new SslCredentials(cert);

        var channel = new Channel(Host, Port, credentials);
        var client = new AuthService.AuthServiceClient(channel);

        var auth = await client.AuthenticateAsync(new AuthRequest { Username = "admin", Password = "pass123" });

        Console.WriteLine($"Auth message: {auth.Message}");
        if (!string.IsNullOrEmpty(auth.Token))
        {
            var profile = await client.GetProfileAsync(new ProfileRequest { Token = auth.Token });
            Console.WriteLine($"Name: {profile.Name}, Age: {profile.Age}, Email: {profile.Email}");
        }

        await channel.ShutdownAsync();
    }
}