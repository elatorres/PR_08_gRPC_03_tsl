using Grpc.Core;
using GrpcSecureAuth;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

// This is my service with the remote functions
public class AuthServiceImpl : AuthService.AuthServiceBase
{
    // my token's dictionary where I collect my authentication tokens
    private readonly ConcurrentDictionary<string, string> _tokens = new();

    // Remote Function Authenticate
    public override Task<AuthResponse> Authenticate(AuthRequest request, ServerCallContext context)
    {
        // if you are user admin with this password the you will be authenticated
        // Change this to have several users, each with a different authentication token.
        if (request.Username == "admin" && request.Password == "pass123")
        {
            // Create a token
            var token = Guid.NewGuid().ToString();
            // Associate it with username
            _tokens[token] = request.Username;
            // return generic task with token and message
            return Task.FromResult(new AuthResponse { Token = token, Message = "Login successful!" });
        }
        // return generic task with empty token and invalid message
        return Task.FromResult(new AuthResponse { Token = "", Message = "Invalid credentials." });
        // another option is to raise an exception that is transfer it to the client.
        // throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid token."));
    }

    // Remote Function GetProfile
    public override Task<ProfileResponse> GetProfile(ProfileRequest request, ServerCallContext context)
    {
        // If authenticated ...
        if (!_tokens.ContainsKey(request.Token))
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid token"));
        // Return answer ... 
        return Task.FromResult(new ProfileResponse
        {
            Name = "Alice Admin",
            Age = 30,
            Email = "admin@example.com"
        });
        // Check these answers with auth.proto description
    }
}