using BotFramework.Attributes;
using EnergomeraIncidentsBot.App;
using EnergomeraIncidentsBot.Resources;
using EnergomeraIncidentsBot.Services.ExternalDb;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EnergomeraIncidentsBot.BotHandlers.State;

[BotState(stateName:Name, version: 2.0f)]
public class StartState : BaseIncidentsBotState
{
    public const string Name = "StartState";
    
    public StartState(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Expected(Telegram.Bot.Types.Enums.UpdateType.Message);
        ExpectedMessage(MessageType.Text);
        NotExpectedMessage = R.InputFioState.InputFio;
    }

    public override async Task HandleMessage(Message message)
    {
        //await Answer("Привет");
        await Answer(R.InputFioState.InputFio);
        await ChangeState(InputFioState.Name);
    }
}