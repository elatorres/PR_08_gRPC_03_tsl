# Instructions for gRPC with TSL encryption

We are going to simulate an **authentication with user and password using encryption** in the communication.

=============================================

Create our solution with two projects

```
PR_08_gRPC_03_tsl/
├── Server/
│   ├── Protos/
│   │   └── auth.proto
│   ├── Program.cs
│   └── AuthServiceImpl.cs
├── Client/
│   ├── Protos/
│   │   └── auth.proto
│   └── Program.cs
├── server.crt	// Estos los vamos a crear después.
└── server.key
```

=============================================

Our .proto is as follows:

```
syntax = "proto3";

option csharp_namespace = "GrpcSecureAuth";

service AuthService {
  rpc Authenticate (AuthRequest) returns (AuthResponse);
  rpc GetProfile (ProfileRequest) returns (ProfileResponse);
}

message AuthRequest {
  string username = 1;
  string password = 2;
}

message AuthResponse {
  string token = 1;
  string message = 2;
}

message ProfileRequest {
  string token = 1;
}

message ProfileResponse {
  string name = 1;
  int32 age = 2;
  string email = 3;
}
```

=============================================

We generate a self-signed certificate using the openssl utilities https://www.openssl.org/
https://stackoverflow.com/questions/10175812/how-can-i-generate-a-self-signed-ssl-certificate-using-openssl

```
openssl req -x509 -nodes -newkey rsa:2048 -keyout server.key -out server.crt -days 365 -subj "/CN=localhost"
```

This generates a certificate valid for one year.
There it is important the declaration of the CN that must match the url used to connect. If it is different, you must use that one. For example: 192.168.1.25 or my_server_grpc.
This procedure generates two files: server.key (Private key, not shared the server must access) and server.crt (certificate, delivered to the clients)
Then we must copy them to the directories where the executables are.
To verify the certificate we can do:

```
openssl x509 -in server.crt -text -noout
```

To make the host (the physical machine) trust the self-signed certificate, assuming it is a linux of the Debian/Ubuntu family, we can run:

```
sudo cp server.crt /usr/local/share/ca-certificates/
sudo update-ca-certificates
```

=============================================

We write our codes as the ones delivered with this project.

```
Server/Program.cs
Server/AuthServiceImpl.cs
Client/Program.cs
```

In our case the .proto is the same for both projects. But we still leave them in two directories for each project. In other cases they could have small differences for example when the namespace of the server and the client are different.

=============================================

The libraries that we need are the following:

```
dotnet add package Grpc.Net.Client
dotnet add package Grpc.Tools
dotnet add package Google.Protobuf
dotnet add package Grpc.Core
```

These are not the last ones. But we use them for compatibility and simplicity of the example.

=============================================
Verify that in Server.csproj and Client.csproj is the reference to Protos/auth.proto or similar.Por ejemplo:

```
    <ItemGroup>
        <Protobuf Include="Protos/auth.proto" GrpcServices="Client" />
    </ItemGroup>
```

To run this example we need to place the private key and the certificate in the directory of the executable Server. And the certificate on the directory of the executable Client.

If everything is OK then building and running the Server first and then the client woukd show the demonstration.

Regards.
