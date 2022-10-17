using Frontend;
using Grpc.Net.Client.Web;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Lib.Shared.Interfaces;
using ProtoBuf.Grpc.Client;
using Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<MessageService>();

builder.Services.AddScoped(services =>
{
    // Erstelle neuen HttpClient mit GrpcWebHandler
    var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));

    // Konfiguriere die Server URL
    var channel = GrpcChannel.ForAddress(builder.HostEnvironment.BaseAddress,
        new GrpcChannelOptions { HttpClient = httpClient });

    // Erstelle einen gRPC Service, der zur Kommunkation genutzt wird
    return channel.CreateGrpcService<IMessageService>();
});

await builder.Build().RunAsync();
