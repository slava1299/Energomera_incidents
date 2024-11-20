using BotFramework.Base;
using BotFramework.Enums;
using EnergomeraIncidentsBot.Resources;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EnergomeraIncidentsBot.BotHandlers.Command;

public class BaseIncidentBotCommand : BaseBotCommand
{
    protected IServiceProvider ServiceProvider;
    protected readonly BotResources R;

    public BaseIncidentBotCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        this.ServiceProvider = serviceProvider;
        R = serviceProvider.GetRequiredService<IOptions<BotResources>>().Value;
    }
    

    protected Task Answer(string text, ParseMode parseMode = ParseMode.Html, IReplyMarkup replyMarkup = default)
    {
        return BotClient.SendTextMessageAsync(Chat.ChatId, text, parseMode: parseMode, replyMarkup: replyMarkup);
    }
    
    public async Task ChangeState(string stateName, ChatStateSetterType setterType = ChatStateSetterType.ChangeCurrent)
    {
        Chat.States.Set(stateName, setterType);
        await BotDbContext.SaveChangesAsync();
    }

    public override Task HandleBotRequest(Update update)
    {
        throw new NotImplementedException();
    }
}