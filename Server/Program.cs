using Microsoft.EntityFrameworkCore;
using ProtoBuf.Grpc.Server;
using Server.Database;
using Server.Services;
using System.IO.Compression;

var argParseResult = ArgumentParsing.Parsing.parseArguments(args);

if (argParseResult.IsError)
{
    Environment.Exit(argParseResult.ErrorValue);
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddGrpc();
builder.Services.AddCodeFirstGrpc(config => { config.ResponseCompressionLevel = CompressionLevel.Optimal; });

builder.Services.AddSwaggerGen();

builder.Services.AddPooledDbContextFactory<ServerDbContext>(options => options.LogTo(s => { return; }, LogLevel.Critical).UseSqlServer(argParseResult.ResultValue.DbConnectionString));

var app = builder.Build();

if (app.Services.GetService(typeof(IDbContextFactory<ServerDbContext>)) is not IDbContextFactory<ServerDbContext> dbContextFactory)
{
    throw new InvalidOperationException("Application startup failed IDbContextFactory<ServerDbContext> was missing!");
}

using (var dbContext = dbContextFactory.CreateDbContext())
{
    dbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE [UserMessageStore]");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseGrpcWeb();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.MapGrpcService<MessageService>().EnableGrpcWeb();

app.Run();
