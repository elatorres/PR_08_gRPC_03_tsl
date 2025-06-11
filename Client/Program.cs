using Grpc.Core;
using GrpcSecureAuth;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    // Where to find our server CN=localhost
    const string Host = "localhost";
    const int Port = 50051;

    static async Task Main()
    {
        // Get our openssl certificate ...
        var cert = File.ReadAllText("server.crt");
        var credentials = new SslCredentials(cert);
        
        // open a communications channel through our gRPC Service
        var channel = new Channel(Host, Port, credentials);
        var client = new AuthService.AuthServiceClient(channel);
        
        // AuthenticateAsync returns a string. RPC in general and gRPC in particular are thought as synchronous programming.
        var auth = await client.AuthenticateAsync(new AuthRequest { Username = "admin", Password = "pass123" });

        // Show the resulting message. Either OK or not OK
        Console.WriteLine($"Auth message: {auth.Message}");
        // Get the authentication token used to keep the authenticated connection
        if (!string.IsNullOrEmpty(auth.Token))
        {
            // If authenticated OK then ask for the user's profile (with authenticated token)
            var profile = await client.GetProfileAsync(new ProfileRequest { Token = auth.Token });
            Console.WriteLine($"Name: {profile.Name}, Age: {profile.Age}, Email: {profile.Email}");
        }

        // Shut down connection when done.
        await channel.ShutdownAsync();
    }
}