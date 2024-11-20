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
public class InputFioState : BaseIncidentsBotState
{
    public const string Name = "InputFioState";
    
    private readonly InputFioStateResources _r;
    private readonly IExternalDbRepository _repos;
    private readonly IConfirmationCodeService _confirmationCodeService;
    private readonly IEmailService _emailService;
    
    public InputFioState(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Expected(Telegram.Bot.Types.Enums.UpdateType.Message);
        ExpectedMessage(MessageType.Text);
        NotExpectedMessage = R.InputFioState.InputFio;
        _r = R.InputFioState;
        _repos = serviceProvider.GetRequiredService<IExternalDbRepository>();
        _emailService = serviceProvider.GetRequiredService<IEmailService>();
        _confirmationCodeService = serviceProvider.GetRequiredService<IConfirmationCodeService>();
    }

    public override async Task HandleMessage(Message message)
    {
        string input = message.Text ?? "";
        if (string.IsNullOrEmpty(input) || input.Length > AppConstants.MaxFioLength)
        {
            await InputAgain();
            return;
        }

        int count = await _repos.CheckFioDouble(input);

        if (count == 0)
        {
            await Answer(string.Format(_r.NotFoundEmployee, input));
            return;
        }

        if (count > 1)
        {
            await Answer(string.Format(_r.FoundManyEmployees, input));
            await ChangeState(InputEmailState.Name, ChatStateSetterType.ChangeCurrent);
            return;
        }

        string email = (await _repos.GetEmailByFio(input))!;
        string? fio = (await _repos.GetFioByEmail(email));
        
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
        await Answer(_r.InputFio);
    }
}