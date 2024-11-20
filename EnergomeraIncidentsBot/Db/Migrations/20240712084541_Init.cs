using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EnergomeraIncidentsBot.Db.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.CreateTable(
                name: "app_users",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "ИД.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegram_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "Telegram идентификатор пользователя."),
                    telegram_chat_id = table.Column<long>(type: "bigint", nullable: false, comment: "Telegram идентификатор чата с пользователем."),
                    telegram_username = table.Column<string>(type: "text", nullable: true, comment: "Telegram ник."),
                    fio = table.Column<string>(type: "text", nullable: true, comment: "ФИО сотрудника."),
                    email = table.Column<string>(type: "text", nullable: true, comment: "Рабочий email сотрудника."),
                    is_registered = table.Column<bool>(type: "boolean", nullable: false, comment: "Пользователь зарегистрирован в боте?"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, comment: "Активен ли пользователь в боте?"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Когда создана сущность."),
                    created_by = table.Column<string>(type: "text", nullable: true, comment: "Кем создана сущность."),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Когда обновлена сушность."),
                    updated_by = table.Column<string>(type: "text", nullable: true, comment: "Кем обновлена сущность."),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Когда удалена сущность."),
                    deleted_by = table.Column<string>(type: "text", nullable: true, comment: "Кем удалена сущность.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_users", x => x.id);
                },
                comment: "Таблица сотрудников в боте.");

            migrationBuilder.CreateTable(
                name: "app_users_confirmation_codes",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "ИД.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegram_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "Telegram ИД пользвоателя."),
                    code = table.Column<string>(type: "text", nullable: false, comment: "Код подтверждения."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Когда создана сущность."),
                    created_by = table.Column<string>(type: "text", nullable: true, comment: "Кем создана сущность."),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Когда обновлена сушность."),
                    updated_by = table.Column<string>(type: "text", nullable: true, comment: "Кем обновлена сущность."),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Когда удалена сущность."),
                    deleted_by = table.Column<string>(type: "text", nullable: true, comment: "Кем удалена сущность.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_users_confirmation_codes", x => x.id);
                },
                comment: "Таблица кодов подтверждения для пользователей.");

            migrationBuilder.CreateTable(
                name: "email_queue",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "ИД.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "text", nullable: false, comment: "Email адрес."),
                    subject = table.Column<string>(type: "text", nullable: true, comment: "Заголовок письма."),
                    message = table.Column<string>(type: "text", nullable: false, comment: "Сообщение."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Когда создана сущность."),
                    created_by = table.Column<string>(type: "text", nullable: true, comment: "Кем создана сущность."),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Когда обновлена сушность."),
                    updated_by = table.Column<string>(type: "text", nullable: true, comment: "Кем обновлена сущность."),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Когда удалена сущность."),
                    deleted_by = table.Column<string>(type: "text", nullable: true, comment: "Кем удалена сущность.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_email_queue", x => x.id);
                },
                comment: "Элемент email рассылки.");

            migrationBuilder.CreateTable(
                name: "incident_fail_notifications",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "ИД.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status = table.Column<string>(type: "text", nullable: true, comment: "Статус"),
                    area = table.Column<string>(type: "text", nullable: true, comment: "Участок"),
                    incident_number = table.Column<string>(type: "text", nullable: false, comment: "Номер инцидента"),
                    incident_date_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, comment: "Дата появления инцидента"),
                    incident_level = table.Column<int>(type: "integer", nullable: true, comment: "Уровень инцидента"),
                    author = table.Column<string>(type: "text", nullable: true, comment: "Автор"),
                    comission_leader = table.Column<string>(type: "text", nullable: true, comment: "Лидер комиссии"),
                    product_code = table.Column<string>(type: "text", nullable: true, comment: "Код изделия"),
                    product_name = table.Column<string>(type: "text", nullable: true, comment: "Наименование изделия"),
                    complementary_product_code = table.Column<string>(type: "text", nullable: true, comment: "Код комплектующего изделия"),
                    complementary_product_name = table.Column<string>(type: "text", nullable: true, comment: "Наименование комплектующего изделия"),
                    problem_description = table.Column<string>(type: "text", nullable: true, comment: "Описание Несоответствия"),
                    problem_name = table.Column<string>(type: "text", nullable: true, comment: "Наименование Дефекта"),
                    npcount_for_shift = table.Column<int>(type: "integer", nullable: true, comment: "Количество НП За Смену"),
                    complementary_count_for_shift = table.Column<int>(type: "integer", nullable: true, comment: "Количество Изделий За Смену"),
                    defect_percent = table.Column<decimal>(type: "numeric", nullable: true, comment: "% дефектов"),
                    executor = table.Column<string>(type: "text", nullable: true, comment: "ФИО сотрудника (исполнителя)"),
                    norm = table.Column<int>(type: "integer", nullable: true, comment: "norm"),
                    executor_telegram_username = table.Column<string>(type: "text", nullable: true, comment: "@telegram сотрудника"),
                    executor_email = table.Column<string>(type: "text", nullable: false, comment: "Емайл сотрудника (исполнителя)"),
                    director = table.Column<string>(type: "text", nullable: true, comment: "ФИО руководителя"),
                    director_email = table.Column<string>(type: "text", nullable: true, comment: "Емайл руководителя"),
                    director_telegram_username = table.Column<string>(type: "text", nullable: true, comment: "Телеграм руководителя."),
                    time = table.Column<int>(type: "integer", nullable: true, comment: "Время со старта инцидента"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Когда создана сущность."),
                    created_by = table.Column<string>(type: "text", nullable: true, comment: "Кем создана сущность."),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Когда обновлена сушность."),
                    updated_by = table.Column<string>(type: "text", nullable: true, comment: "Кем обновлена сущность."),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Когда удалена сущность."),
                    deleted_by = table.Column<string>(type: "text", nullable: true, comment: "Кем удалена сущность.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_incident_fail_notifications", x => x.id);
                },
                comment: "Уведомление о незавершении инцидента для пользователя. ");

            migrationBuilder.CreateTable(
                name: "incident_prevent_notifications",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "ИД.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    incident_number = table.Column<string>(type: "text", nullable: false, comment: "Номер"),
                    incident_date_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, comment: "Дата"),
                    author = table.Column<string>(type: "text", nullable: true, comment: "Автор"),
                    author_email = table.Column<string>(type: "text", nullable: true, comment: "Автор_почта"),
                    incident_level = table.Column<int>(type: "integer", nullable: true, comment: "Уровень"),
                    area = table.Column<string>(type: "text", nullable: true, comment: "Участок"),
                    manufactory = table.Column<string>(type: "text", nullable: true, comment: "Цех"),
                    description = table.Column<string>(type: "text", nullable: true, comment: "Описание"),
                    product_code = table.Column<string>(type: "text", nullable: true, comment: "Код_изделия"),
                    product_name = table.Column<string>(type: "text", nullable: true, comment: "Наименование_изделия"),
                    defect = table.Column<string>(type: "text", nullable: true, comment: "Дефект"),
                    consumer_responsible_person = table.Column<string>(type: "text", nullable: true, comment: "Ответственный_от_потребителя"),
                    consumer_responsible_person_email = table.Column<string>(type: "text", nullable: true, comment: "Ответственный_от_потребителя_почта"),
                    supplier_responsible_person = table.Column<string>(type: "text", nullable: true, comment: "ОтветственныйОтПоставщика"),
                    supplier_responsible_person_email = table.Column<string>(type: "text", nullable: true, comment: "ОтветственныйОтПоставщика_Почта"),
                    normative = table.Column<int>(type: "integer", nullable: true, comment: "Норматив"),
                    response_time = table.Column<int>(type: "integer", nullable: true, comment: "Время_отклика"),
                    comission_leader = table.Column<string>(type: "text", nullable: true, comment: "Лидер"),
                    comission_leader_email = table.Column<string>(type: "text", nullable: true, comment: "Лидер_почта"),
                    deputy_technology_director = table.Column<string>(type: "text", nullable: true, comment: "Зам_начальник_по_технологии"),
                    deputy_technology_director_email = table.Column<string>(type: "text", nullable: true, comment: "Зам_начальник_по_технологии_почта"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Когда создана сущность."),
                    created_by = table.Column<string>(type: "text", nullable: true, comment: "Кем создана сущность."),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Когда обновлена сушность."),
                    updated_by = table.Column<string>(type: "text", nullable: true, comment: "Кем обновлена сущность."),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Когда удалена сущность."),
                    deleted_by = table.Column<string>(type: "text", nullable: true, comment: "Кем удалена сущность.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_incident_prevent_notifications", x => x.id);
                },
                comment: "Уведомление о купировании инцидента.");

            migrationBuilder.CreateTable(
                name: "incidents",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "ИД.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    executor = table.Column<string>(type: "text", nullable: true, comment: "Исполнитель"),
                    executor_email = table.Column<string>(type: "text", nullable: false, comment: "Почта исполнителя"),
                    executor_telegram_username = table.Column<string>(type: "text", nullable: true, comment: "Аккаунт telegram"),
                    director = table.Column<string>(type: "text", nullable: true, comment: "ФИО руководителя"),
                    director_email = table.Column<string>(type: "text", nullable: true, comment: "Почта руководителя"),
                    area = table.Column<string>(type: "text", nullable: true, comment: "Участок"),
                    incident_number = table.Column<string>(type: "text", nullable: false, comment: "Номер инцидента"),
                    incident_date_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, comment: "Дата появления инцидента"),
                    incident_level = table.Column<int>(type: "integer", nullable: true, comment: "Уровень инцидента"),
                    author = table.Column<string>(type: "text", nullable: true, comment: "Автор"),
                    comission_leader = table.Column<string>(type: "text", nullable: true, comment: "Лидер комиссии"),
                    product_code = table.Column<string>(type: "text", nullable: true, comment: "Код изделия"),
                    product_name = table.Column<string>(type: "text", nullable: true, comment: "Наименование изделия"),
                    complementary_product_code = table.Column<string>(type: "text", nullable: true, comment: "Код комплектующего изделия"),
                    complementary_product_name = table.Column<string>(type: "text", nullable: true, comment: "Наименование комплектующего изделия"),
                    problem_description = table.Column<string>(type: "text", nullable: true, comment: "Описание Несоответствия"),
                    problem_name = table.Column<string>(type: "text", nullable: true, comment: "Наименование Дефекта"),
                    npcount_for_shift = table.Column<int>(type: "integer", nullable: true, comment: "Количество НП За Смену"),
                    complementary_count_for_shift = table.Column<int>(type: "integer", nullable: true, comment: "Количество Изделий За Смену"),
                    defect_percent = table.Column<decimal>(type: "numeric", nullable: true, comment: "% дефектов"),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Когда создана сущность."),
                    created_by = table.Column<string>(type: "text", nullable: true, comment: "Кем создана сущность."),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Когда обновлена сушность."),
                    updated_by = table.Column<string>(type: "text", nullable: true, comment: "Кем обновлена сущность."),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Когда удалена сущность."),
                    deleted_by = table.Column<string>(type: "text", nullable: true, comment: "Кем удалена сущность.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_incidents", x => x.id);
                },
                comment: "Таблица инцидентов");

            migrationBuilder.CreateTable(
                name: "not_registered_user_notifications",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "ИД.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "text", nullable: true, comment: "Email пользователя."),
                    name = table.Column<string>(type: "text", nullable: true, comment: "ФИО пользователя."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Когда создана сущность."),
                    created_by = table.Column<string>(type: "text", nullable: true, comment: "Кем создана сущность."),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Когда обновлена сушность."),
                    updated_by = table.Column<string>(type: "text", nullable: true, comment: "Кем обновлена сущность."),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Когда удалена сущность."),
                    deleted_by = table.Column<string>(type: "text", nullable: true, comment: "Кем удалена сущность.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_not_registered_user_notifications", x => x.id);
                },
                comment: "Уведомление о незарегистрированном пользователе. ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "app_users",
                schema: "app");

            migrationBuilder.DropTable(
                name: "app_users_confirmation_codes",
                schema: "app");

            migrationBuilder.DropTable(
                name: "email_queue",
                schema: "app");

            migrationBuilder.DropTable(
                name: "incident_fail_notifications",
                schema: "app");

            migrationBuilder.DropTable(
                name: "incident_prevent_notifications",
                schema: "app");

            migrationBuilder.DropTable(
                name: "incidents",
                schema: "app");

            migrationBuilder.DropTable(
                name: "not_registered_user_notifications",
                schema: "app");
        }
    }
}
