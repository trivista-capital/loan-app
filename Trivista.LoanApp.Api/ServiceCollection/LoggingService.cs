using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Trivista.LoanApp.Api.ServiceCollection;

public static class LoggingService
{
    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsLocal() || builder.Environment.IsStaging())
        {
            builder.Host.UseSerilog((ctx, services, lc) => lc.WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .ReadFrom.Configuration(ctx.Configuration)
                .ReadFrom.Services(services), true);
        }
        else
        {
            // var telemetryConfiguration = builder.Services.BuildServiceProvider().GetRequiredService<TelemetryConfiguration>();
            //
            // builder.Host.UseSerilog((ctx, lc) 
            //     => 
            //     lc.WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Events)
            //         .MinimumLevel.Verbose()
            //         .Filter.ByIncludingOnly(e => e.Level >= LogEventLevel.Error)
            //         .Filter.ByIncludingOnly(e => e.Level >= LogEventLevel.Information)
            //         .Filter.ByIncludingOnly(e => e.Level >= LogEventLevel.Warning)
            //         .Filter.ByIncludingOnly(e => e.Level >= LogEventLevel.Fatal)
            //         .Enrich.FromLogContext()
            //         .ReadFrom.Configuration(ctx.Configuration), preserveStaticLogger: true);   
        }
        return builder;
    }
}