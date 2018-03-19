# Serilog.Sinks.AmazonSimpleEmailService

Sends log events by Amazon SES.

**Package** - [Serilog.Sinks.AmazonSimpleEmailService](https://www.nuget.org/packages/Serilog.Sinks.AmazonSimpleEmailService)
| **Platforms** - .NET 4.5, .NET Standard 1.3

```powershell
Install-Package Serilog.Sinks.AmazonSimpleEmailService
```

To configure the standard sink, use the standard extensions on the LoggerConfiguration instance.

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

This sink uses the AWS SDKs to communicate with AWS services.  It is recommended to use the standard AWS SDK credential discovery process to avoid additional maintenance of configuration (via AppSettings or otherwise).

See [Configuring AWS Credentials](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-config-creds.html) for more information