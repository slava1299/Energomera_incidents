using BotFramework.Base;
using BotFramework.Enums;
using BotFramework.Utils;
using EnergomeraIncidentsBot.Db;
using EnergomeraIncidentsBot.Resources;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EnergomeraIncidentsBot.BotHandlers.State;

public class BaseIncidentsBotState : BaseBotState
{
    protected AppDbContext Db;
    
    protected string NotExpectedMessage { get; set; } 
    protected IServiceProvider ServiceProvider;
    protected readonly BotResources R;
    private readonly List<Telegram.Bot.Types.Enums.UpdateType> ExpectedUpdates = new ();
    private readonly List<MessageType> ExpectedMessageTypes = new ();
    
    public BaseIncidentsBotState(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        this.ServiceProvider = serviceProvider;
        R = serviceProvider.GetRequiredService<IOptions<BotResources>>().Value;
        Db = serviceProvider.GetRequiredService<AppDbContext>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        if (IsExpectedUpdate(update) == false)
        {
            await UnexpectedUpdateHandler();
            return;
        }
        
        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message &&
            IsExpectedMessageType(update.Message) == false)
        {
            await UnexpectedUpdateHandler();
            return;
        }

        switch (update.Type)
        {
            case Telegram.Bot.Types.Enums.UpdateType.Message:
                await HandleMessage(update.Message!);
                break;
            case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                await HandleCallbackQuery(update.CallbackQuery!);
                break;
        }
    }

    public virtual async Task UnexpectedUpdateHandler()
    {
        await BotClient.SendTextMessageAsync(Chat.ChatId, NotExpectedMessage);
    }
    
    public virtual async Task HandleMessage(Message message)
    {
        throw new NotImplementedException();
    }
    
    public virtual async Task HandleCallbackQuery(CallbackQuery callbackQuery)
    {
        throw new NotImplementedException();
        return;
    }

    /// <summary>
    /// Заполняем ожидаемые типы запросов для состояния.
    /// </summary>
    /// <param name="types"></param>
    protected void Expected(params Telegram.Bot.Types.Enums.UpdateType[] types)
    {
        foreach (var type in types)
        {
            ExpectedUpdates.Add(type);
        }
    }
    
    /// <summary>
    /// Заполняем ожидаемые типы сообщений для состояния.
    /// </summary>
    /// <param name="types"></param>
    protected void ExpectedMessage(params MessageType[] types)
    {
        foreach (var type in types)
        {
            ExpectedMessageTypes.Add(type);
        }
    }

    private bool IsExpectedUpdate(Update update)
    {
        return ExpectedUpdates.Any() == false || ExpectedUpdates.Contains(update.Type);
    }
    
    private bool IsExpectedMessageType(Message message)
    {
        return ExpectedMessageTypes.Any() == false || ExpectedMessageTypes.Contains(message.Type);
    }

    protected Task Answer(string text, ParseMode parseMode = ParseMode.Html, IReplyMarkup replyMarkup = default)
    {
        return BotClient.SendTextMessageAsync(Chat.ChatId, text:text, parseMode:parseMode, replyMarkup: replyMarkup);
    }
    
    public async Task ChangeState(string stateName, ChatStateSetterType setterType = ChatStateSetterType.ChangeCurrent)
    {
        Chat.States.Set(stateName, setterType);
        BotDbContext.Update(Chat);
        await BotDbContext.SaveChangesAsync();
    }
}