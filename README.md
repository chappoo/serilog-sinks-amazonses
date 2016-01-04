# Serilog.Sinks.AmazonSimpleEmailService

Sends log events by Amazon SES.

**Package** - [Serilog.Sinks.AmazonSimpleEmailService](https://www.nuget.org/packages/Serilog.Sinks.AmazonSimpleEmailService)
| **Platforms** - .NET 4.5

```powershell
Install-Package Serilog.Sinks.AmazonSimpleEmailService
```

```csharp
var logger = new LoggerConfiguration()
	.WriteTo.AmazonSimpleEmailService(new AmazonSimpleEmailServiceConfig
    {
        FromEmail = "no-reply@myapp.com",
        EmailSubject = "MyApp Warnings / Errors",
        AwsAccessKeyId = "AWSACCESSKEY",
        AwsSecretKey = "AWSSECRETKEY",
        ToEmail = "test@test.com"
    }, restrictedToMinimumLevel: LogEventLevel.Warning)
    .CreateLogger();
```
