using System.Diagnostics;
using BotFramework.Db;
using BotFramework.Utils.ExceptionHandler;
using Microsoft.AspNetCore.Diagnostics;
using Telegram.Bot;

namespace EnergomeraIncidentsBot.App;

public class ExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        IServiceProvider serviceProvider = httpContext.RequestServices;

        await HandleBotException(serviceProvider, exception, cancellationToken);
        
        return false;
    }

    public async Task HandleBotException(IServiceProvider serviceProvider, Exception exception, CancellationToken? cancellationToken = null)
    {
        BotExceptionHandler botHandler = new();
        BotExceptionHandlerArgs args = new(exception, serviceProvider);
        await botHandler.Handle(args, cancellationToken);
    }
}