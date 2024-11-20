using System.Linq.Expressions;
using BotFramework.Db.Entity;
using BotFramework.Extensions;
using EnergomeraIncidentsBot.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

namespace EnergomeraIncidentsBot.Db;

public partial class AppDbContext
{
        /// <summary>
    /// Таблицы, свойства, ключи, внеш. ключи, индексы переводит в нижний регистр в БД.
    /// </summary>
    protected void SetAllToSnakeCase(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            entityType.SetTableName(entityType.GetTableName().ToSnakeCase());

            foreach (var property in entityType.GetProperties())
            {
                var schema = entityType.GetSchema();
                var tableName = entityType.GetTableName();
                var storeObjectIdentifier = StoreObjectIdentifier.Table(tableName, schema);
                property.SetColumnName(property.GetColumnName(storeObjectIdentifier).ToSnakeCase());
            }
            
            foreach (var key in entityType.GetKeys())
                key.SetName(key.GetName().ToSnakeCase());

            foreach (var key in entityType.GetForeignKeys())
                key.SetConstraintName(key.GetConstraintName().ToSnakeCase());

            foreach (var index in entityType.GetIndexes())
                index.SetDatabaseName(index.GetDatabaseName().ToSnakeCase());
        }
    }

    /// <summary>
    /// Задать наименование таблиц и схемы для таблиц.
    /// </summary>
    private void SetSchemasToTables(ModelBuilder builder)
    {
        builder.Entity<AppUser>().ToTable("app_users", AppSchema);
        builder.Entity<AppUserConfirmationCode>().ToTable("app_users_confirmation_codes", AppSchema);
        builder.Entity<Incident>().ToTable("incidents", AppSchema);
        builder.Entity<IncidentFailNotification>().ToTable("incident_fail_notifications", AppSchema);
        builder.Entity<IncidentPreventNotification>().ToTable("incident_prevent_notifications", AppSchema);
        builder.Entity<NotRegisteredUserNotification>().ToTable("not_registered_user_notifications", AppSchema);
        builder.Entity<EmailQueueItem>().ToTable("email_queue", AppSchema);
        builder.Entity<TelegramNotificationHistoryItem>().ToTable("telegram_notification_history", AppSchema);
    }
    
    /// <summary>
    /// Настройка фильтров запросов.
    /// </summary>
    public static void SetFilters(ModelBuilder modelBuilder)
    {
        var entities = modelBuilder.Model
            .GetEntityTypes()
            //.Where(e => e.ClrType.BaseType == typeof(IBaseEntity))
            .Select(e => e.ClrType);
        
        Expression<Func<IBaseEntity, bool>> 
            expression = del => del.DeletedAt == null;

        foreach (var e in entities)
        {
            ParameterExpression p = Expression.Parameter(e);
            Expression body =
                ReplacingExpressionVisitor
                    .Replace(expression.Parameters.Single(),
                        p, expression.Body);

            modelBuilder.Entity(e)
                .HasQueryFilter(
                    Expression.Lambda(body, p));
        }
    }

    public static void ConfigureEntities(ModelBuilder builder)
    {
        
    }
}