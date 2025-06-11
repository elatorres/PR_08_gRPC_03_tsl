using Grpc.Core;
using GrpcSecureAuth;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class AuthServiceImpl : AuthService.AuthServiceBase
{
    private readonly ConcurrentDictionary<string, string> _tokens = new();

    public override Task<AuthResponse> Authenticate(AuthRequest request, ServerCallContext context)
    {
        if (request.Username == "admin" && request.Password == "pass123")
        {
            var token = Guid.NewGuid().ToString();
            _tokens[token] = request.Username;
            return Task.FromResult(new AuthResponse { Token = token, Message = "Login successful!" });
        }
        return Task.FromResult(new AuthResponse { Token = "", Message = "Invalid credentials." });
    }

    public override Task<ProfileResponse> GetProfile(ProfileRequest request, ServerCallContext context)
    {
        if (!_tokens.ContainsKey(request.Token))
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid token"));

        return Task.FromResult(new ProfileResponse
        {
            Name = "Alice Admin",
            Age = 30,
            Email = "admin@example.com"
        });
    }
}