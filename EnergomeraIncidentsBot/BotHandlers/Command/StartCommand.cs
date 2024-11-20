using BotFramework.Attributes;
using BotFramework.Extensions;
using BotFramework.Utils;
using EnergomeraIncidentsBot.BotHandlers.State;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace EnergomeraIncidentsBot.BotHandlers.Command;

[BotCommand(command:"/start", version: 2.0f)]
public class StartCommand : BaseIncidentBotCommand
{
    public StartCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        
    }

    public override async Task HandleBotRequest(Update update)
    {
        //await Answer("Привет");
        await Answer(R.InputFioState.InputFio);
        await ChangeState(InputFioState.Name);
    }
}