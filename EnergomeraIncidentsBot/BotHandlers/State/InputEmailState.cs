using BotFramework.Attributes;
using BotFramework.Enums;
using EnergomeraIncidentsBot.App;
using EnergomeraIncidentsBot.Resources;
using EnergomeraIncidentsBot.Services.ConfirmationCode;
using EnergomeraIncidentsBot.Services.Email;
using EnergomeraIncidentsBot.Services.ExternalDb;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EnergomeraIncidentsBot.BotHandlers.State;

[BotState(stateName:Name, version: 2.0f)]
public class InputEmailState : BaseIncidentsBotState
{
    public const string Name = "InputEmailState";
    
    private readonly InputEmailStateResources _r;
    private readonly IExternalDbRepository _repos;
    private readonly IConfirmationCodeService _confirmationCodeService;
    private readonly IEmailService _emailService;
    
    public InputEmailState(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Expected(Telegram.Bot.Types.Enums.UpdateType.Message);
        ExpectedMessage(MessageType.Text);
        NotExpectedMessage = R.InputEmailState.InputEmail;
        _r = R.InputEmailState;
        _repos = serviceProvider.GetRequiredService<IExternalDbRepository>();
        _confirmationCodeService = serviceProvider.GetRequiredService<IConfirmationCodeService>();
    }

    public override async Task HandleMessage(Message message)
    {
        string input = message.Text ?? "";
        if (string.IsNullOrEmpty(input))
        {
            await InputAgain();
            return;
        }

        string email = input.ToLower().Trim(' ');

        if (email.EndsWith(AppConstants.EnergomeraEmailPostfix) == false)
        {
            await Answer(string.Format(_r.NotFoundEmail, email));
            return;
        }

        bool emailExists = await _repos.IsEmailExists(email);

        if (emailExists == false)
        {
            await Answer(string.Format(_r.NotFoundEmail, email));
            return;
        }

        string? fio = await _repos.GetFioByEmail(email);
        
        // отправляем код на почту, переводим на состояние подтверждения кода.
        var newCode = await _confirmationCodeService.CreateCode(User.TelegramId);
        await _emailService.SendEmailAsync(email, "Код подтверждения", newCode.Code);

        User.AdditionalProperties.Set("email", email);
        User.AdditionalProperties.Set("fio", fio ?? "");
        await BotDbContext.SaveChangesAsync();
        
        await ChangeState(ConfirmCodeState.Name, ChatStateSetterType.SetNext);
        await Answer(R.ConfirmCodeState.InputCode);
    }

    private async Task InputAgain()
    {
        await Answer(_r.InputEmail);
    }
}