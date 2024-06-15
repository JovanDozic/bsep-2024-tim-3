using Serilog.Core;
using Serilog.Events;
using Marketing_system.BL.Contracts.IService;
using System.Threading.Tasks;

public class SignalRSink : ILogEventSink
{
    private readonly IAlertService _alertService;

    public SignalRSink(IAlertService alertService)
    {
        _alertService = alertService;
    }

    public void Emit(LogEvent logEvent)
    {
        if (logEvent.Level == LogEventLevel.Error || logEvent.Level == LogEventLevel.Fatal)
        {
            var message = logEvent.RenderMessage();
            Task.Run(() => _alertService.AlertAsync(message)).GetAwaiter().GetResult();
        }
    }
}
