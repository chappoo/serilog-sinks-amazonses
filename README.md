# Serilog.Sinks.AmazonSimpleEmailService

Sends log events by Amazon SES.

**Package** - [Serilog.Sinks.AmazonSimpleEmailService](https://www.nuget.org/packages/Serilog.Sinks.AmazonSimpleEmailService)
| **Platforms** - .NET 4.5

```powershell
Install-Package Serilog.Sinks.AmazonSimpleEmailService
```

```csharp
var logger = new LoggerConfiguration()
    .WriteTo.AmazonSimpleEmailService(
        new AmazonSimpleEmailServiceClient(),
        emailFrom = "no-reply@myapp.com",
        emailTo = "test@test.com",
        emailSubject = "MyApp Warnings / Errors",
        restrictedToMinimumLevel: LogEventLevel.Warning
    )
    .CreateLogger();
```
