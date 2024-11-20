using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EnergomeraIncidentsBot.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddTelegramNotificationHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "telegram_notification_history",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "ИД.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegram_chat_id = table.Column<long>(type: "bigint", nullable: false, comment: "Телеграм чат ИД."),
                    message = table.Column<string>(type: "text", nullable: false, comment: "Сообщение уведомления."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Когда создана сущность."),
                    created_by = table.Column<string>(type: "text", nullable: true, comment: "Кем создана сущность."),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Когда обновлена сушность."),
                    updated_by = table.Column<string>(type: "text", nullable: true, comment: "Кем обновлена сущность."),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Когда удалена сущность."),
                    deleted_by = table.Column<string>(type: "text", nullable: true, comment: "Кем удалена сущность.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_notification_history", x => x.id);
                },
                comment: "История уведомлений в Telegram.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "telegram_notification_history",
                schema: "app");
        }
    }
}
