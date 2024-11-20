dotnet ef migrations add AddTelegramNotificationHistoryTable -o Db/Migrations --context AppDbContext

dotnet ef database update --context AppDbContext

dotnet ef migrations remove --context AppDbContext