using System.Reflection;
using BotFramework.Utils;
using EnergomeraIncidentsBot.App;
using EnergomeraIncidentsBot.App.Options;
using EnergomeraIncidentsBot.Db.Entities;
using EnergomeraIncidentsBot.Quartz.EmailQueueSender;
using EnergomeraIncidentsBot.Quartz.ExpiredIncidentTimeNotification;
using EnergomeraIncidentsBot.Quartz.MasTelegramAlarmsStart;
using EnergomeraIncidentsBot.Quartz.PreventIncidentNotification;
using EnergomeraIncidentsBot.Resources;
using Mapster;
using MapsterMapper;
using Quartz;

namespace EnergomeraIncidentsBot.Extensions;

public static class IServiceCollectionExtension
{
    /// <summary>
    /// Зарегистрировать класс ресурсов бота. 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="resourcesFilePath"></param>
    /// <returns></returns>
    public static BotResources ConfigureBotResources(this IServiceCollection services, string resourcesFilePath)
    {
        if (resourcesFilePath == null) throw new ArgumentNullException(nameof(resourcesFilePath));
        
        string json = File.ReadAllText(resourcesFilePath);
        BotResourcesBuilder resourcesBuilder = new(json);
        json = resourcesBuilder.Build();

        Stream jsonStream = StreamHelper.GenerateStreamFromString(json);
        var resourcesConfigBuilder = new ConfigurationBuilder().AddJsonStream(jsonStream);
        IConfiguration resourcesConfiguration = resourcesConfigBuilder.Build();
        
        services.Configure<BotResources>(resourcesConfiguration);
        return resourcesConfiguration.Get<BotResources>();
    }
    
    /// <summary>
    /// Расширение для регистрации заданий и триггеров.
    /// </summary>
    /// <param name="services">Коллекция служб.</param>
    /// <param name="quartzOptions">Настройки Quartz.</param>
    public static void AddQuartzJobs(this IServiceCollection services, AppQuartzOptions options)
    {
        services
            .AddQuartz(quartzConfigurator =>
            {
                quartzConfigurator.UseMicrosoftDependencyInjectionJobFactory();
                
                // Добавить задачу получения инцидентов
                quartzConfigurator.AddJob<MasTelegramAlarmsStartJob>(jobConfigurator =>
                {
                    jobConfigurator.WithIdentity(AppConstants.QuartzKeys.MasTelegramAlarmsStartJobKey);
                });
                quartzConfigurator.AddTrigger(triggerConfigurator =>
                {
#if DEBUG
                    triggerConfigurator.ForJob(AppConstants.QuartzKeys.MasTelegramAlarmsStartJobKey)
                        .WithIdentity(AppConstants.QuartzKeys.MasTelegramAlarmsStartTriggerKey)
                        .StartNow();
                        //.StartAt(DateTimeOffset.MaxValue); 
#elif RELEASE
                     triggerConfigurator.ForJob(AppConstants.QuartzKeys.MasTelegramAlarmsStartJobKey)
                        .WithIdentity(AppConstants.QuartzKeys.MasTelegramAlarmsStartTriggerKey)
                        .WithCronSchedule(options.MasTelegramAlarmsStartJobCron);
#endif
                });
                

                // Добавить задачу уведомления об истекающих инцидентах
                quartzConfigurator.AddJob<ExpiredIncidentTimeNotificationJob>(jobConfigurator =>
                {
                    jobConfigurator.WithIdentity(AppConstants.QuartzKeys.ExpiredIncidentTimeNotificationJobKey);
                });
                quartzConfigurator.AddTrigger(triggerConfigurator =>
                {
#if DEBUG
                    triggerConfigurator.ForJob(AppConstants.QuartzKeys.ExpiredIncidentTimeNotificationJobKey)
                        .WithIdentity(AppConstants.QuartzKeys.ExpiredIncidentTimeNotificationTriggerKey)
                        //.StartNow();
                        .StartAt(DateTimeOffset.MaxValue); 
#elif RELEASE
                    triggerConfigurator.ForJob(AppConstants.QuartzKeys.ExpiredIncidentTimeNotificationJobKey)
                        .WithIdentity(AppConstants.QuartzKeys.ExpiredIncidentTimeNotificationTriggerKey)
                        .WithCronSchedule(options.ExpiredIncidentTimeNotificationJobCron);
#endif
                });
                
                
                // Добавить задачу уведомления о срочном предотвращении инцидента.
                quartzConfigurator.AddJob<PreventIncidentNotificationJob>(jobConfigurator =>
                {
                    jobConfigurator.WithIdentity(AppConstants.QuartzKeys.PreventIncidentNotificationJobKey);
                });
                quartzConfigurator.AddTrigger(triggerConfigurator =>
                {
#if DEBUG
                    triggerConfigurator.ForJob(AppConstants.QuartzKeys.PreventIncidentNotificationJobKey)
                        .WithIdentity(AppConstants.QuartzKeys.PreventIncidentNotificationTriggerKey)
                        //.StartNow();
                        .StartAt(DateTimeOffset.MaxValue); 
#elif RELEASE
                    triggerConfigurator.ForJob(AppConstants.QuartzKeys.PreventIncidentNotificationJobKey)
                        .WithIdentity(AppConstants.QuartzKeys.PreventIncidentNotificationTriggerKey)
                        .WithCronSchedule(options.PreventIncidentNotificationJobCron);
#endif
                });
                
                
                // Добавить задачу отправки емайл из очереди.
                quartzConfigurator.AddJob<EmailQueueSenderJob>(jobConfigurator =>
                {
                    jobConfigurator.WithIdentity(AppConstants.QuartzKeys.EmailQueueSenderJobKey);
                });
                quartzConfigurator.AddTrigger(triggerConfigurator =>
                {
#if DEBUG
                    triggerConfigurator.ForJob(AppConstants.QuartzKeys.EmailQueueSenderJobKey)
                        .WithIdentity(AppConstants.QuartzKeys.EmailQueueSenderTriggerKey)
                        //.StartNow();
                        .StartAt(DateTimeOffset.MaxValue); 
#elif RELEASE
                    triggerConfigurator.ForJob(AppConstants.QuartzKeys.EmailQueueSenderJobKey)
                        .WithIdentity(AppConstants.QuartzKeys.EmailQueueSenderTriggerKey)
                        .WithCronSchedule(options.EmailQueueSenderJobCron);
#endif
                });
            });
        
        
        
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true; 
        });
    }
    
    /// <summary>
    /// Расширения для регистрации конфигурации и сопоставления Mapster в коллекции служб.
    /// </summary>
    /// <param name="services">Коллекция служб.</param>
    public static void AddMapster(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(
            Assembly.GetExecutingAssembly());
        config.RequireExplicitMapping = false;
        config.RequireDestinationMemberSource = false;

        // config
        //     .When((srcType, destType, _) => srcType == destType)
        //     .Ignore("Id");

        config.When((srcType, destType, _) => true)
            .IgnoreNullValues(true);
        
        config
            .When((srcType, destType, _) => srcType == typeof(IBaseEntity) == false && destType == typeof(IBaseEntity))
            .Ignore("Id",
                nameof(IBaseEntity.CreatedAt),
                nameof(IBaseEntity.CreatedBy),
                nameof(IBaseEntity.UpdatedAt),
                nameof(IBaseEntity.UpdatedBy),
                nameof(IBaseEntity.DeletedAt),
                nameof(IBaseEntity.DeletedBy));
        
        var mapperConfig = new Mapper(config);
        services.AddSingleton<IMapper>(mapperConfig);
    }
}