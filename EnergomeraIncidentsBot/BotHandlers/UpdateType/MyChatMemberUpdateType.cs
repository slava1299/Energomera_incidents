using BotFramework.Attributes;
using BotFramework.Base;
using BotFramework.Enums;
using EnergomeraIncidentsBot.BotHandlers.State;
using EnergomeraIncidentsBot.Db.Entities;
using EnergomeraIncidentsBot.Db.Repository;
using EnergomeraIncidentsBot.Services.ExternalDb;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EnergomeraIncidentsBot.BotHandlers.UpdateType;

[BotPriorityHandler(Telegram.Bot.Types.Enums.UpdateType.MyChatMember)]
public class MyChatMemberUpdateType : BaseBotPriorityHandler
{
    private readonly DbRepository _repository;
    private readonly IExternalDbRepository _externalDbRepository;
    
    public MyChatMemberUpdateType(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _repository = serviceProvider.GetRequiredService<DbRepository>();
        _externalDbRepository = serviceProvider.GetRequiredService<IExternalDbRepository>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        ChatMemberUpdated info = update.MyChatMember!;

        // Пользователь открыл доступ боту.
        if (info.OldChatMember.Status == ChatMemberStatus.Kicked &&
            info.NewChatMember.Status == ChatMemberStatus.Member)
        {
            return;
        }

        // Пользователь заблокировал бота.
        if (info.OldChatMember.Status == ChatMemberStatus.Member &&
            info.NewChatMember.Status == ChatMemberStatus.Kicked)
        {
            await ResetUser(User.TelegramId);
        }
        
        return;
    }

    /// <summary>
    /// Пользователь заблокировал бота, сбросить данные по пользователю.
    /// </summary>
    /// <returns></returns>
    private async Task ResetUser(long userTelegramId)
    {
        AppUser? appUser = await _repository.GetAppUser(userTelegramId);
        
        if(appUser is null) return;

        await _repository.ResetAppUser(userTelegramId);
        
        if (string.IsNullOrEmpty(appUser.Email) == false)
        {
            await _externalDbRepository.RemoveTelegramUser(appUser.Email!);
        }
        
        Chat.States.Set(StartState.Name, ChatStateSetterType.SetRoot);
        await BotDbContext.SaveChangesAsync();
    }
}