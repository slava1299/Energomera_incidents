using BotFramework.Attributes;
using BotFramework.Enums;
using BotFramework.Utils;
using EnergomeraIncidentsBot.App;
using EnergomeraIncidentsBot.Db.Entities;
using EnergomeraIncidentsBot.Db.Repository;
using EnergomeraIncidentsBot.Resources;
using EnergomeraIncidentsBot.Services.ConfirmationCode;
using EnergomeraIncidentsBot.Services.Email;
using EnergomeraIncidentsBot.Services.ExternalDb;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EnergomeraIncidentsBot.BotHandlers.State;

[BotState(stateName:Name, version: 2.0f)]
public class ConfirmCodeState : BaseIncidentsBotState
{
    public const string Name = "ConfirmCodeState";
    
    private readonly ConfirmCodeStateResources _r;
    private readonly IExternalDbRepository _externalRepository;
    private readonly IConfirmationCodeService _confirmationCodeService;
    private readonly IEmailService _emailService;
    private readonly DbRepository _dbRepository;
    
    public ConfirmCodeState(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Expected(Telegram.Bot.Types.Enums.UpdateType.Message);
        ExpectedMessage(MessageType.Text);
        NotExpectedMessage = R.InputEmailState.InputEmail;
        _r = R.ConfirmCodeState;
        _externalRepository = serviceProvider.GetRequiredService<IExternalDbRepository>();
        _confirmationCodeService = serviceProvider.GetRequiredService<IConfirmationCodeService>();
        _emailService = serviceProvider.GetRequiredService<IEmailService>();
        _dbRepository = serviceProvider.GetRequiredService<DbRepository>();
    }

    public override async Task HandleMessage(Message message)
    {
        string input = message.Text ?? "";
        if (string.IsNullOrEmpty(input))
        {
            await FailConfirmation();
            return;
        }

        bool isCodeConfirm = await _confirmationCodeService.CheckCode(User.TelegramId, input);

        if (isCodeConfirm == false)
        {
            await FailConfirmation();
            return;
        }
        
        // Пользователь подтвердил код
        // Добавить пользователя, пометить как зарегистрированного.
        // Перевести на главное состояние.

        string email = User.AdditionalProperties.Get("email");
        string fio = User.AdditionalProperties.Get("fio");

        AppUser appUser = new()
        {
            Email = email,
            Fio = fio,
            IsRegistered = true,
            TelegramUsername = User.TelegramUsername,
            TelegramUserId = User.TelegramId,
            TelegramChatId = Chat.TelegramId!.Value,
            IsActive = true,
        };
        await _dbRepository.AddAppUser(appUser);
        await _externalRepository.AddTelegramUser(fio, email, User.TelegramUsername);

        MarkupBuilder<ReplyKeyboardMarkup> mb = new();
        mb.NewRow().Add(R.MainState.ActiveIncidentsBtn);
        await Answer(_r.SuccessConfirmation, replyMarkup: mb.Build());
        
        // Переходим в главное состояние.
        await ChangeState(MainState.Name, ChatStateSetterType.SetRoot);
        
        // // Убираем у пользователя свойства
        // User.AdditionalProperties.Remove("email");
        // User.AdditionalProperties.Remove("fio");
        // await BotDbContext.SaveChangesAsync();
    }

    private async Task FailConfirmation()
    {
        await Answer(_r.FailConfirmation);
        Chat.States.GoBack(ChatStateGoBackType.GoToPrevious);
        await BotDbContext.SaveChangesAsync();

        if (Chat.States.CurrentState == InputEmailState.Name)
        {
            await Answer(R.InputEmailState.InputEmail);
        }
        else
        {
            await Answer(R.InputFioState.InputFio);
        }
    }
}