# EF Operations

## Add Migration

dotnet ef migrations add [migrationName] --project .\HouseManagement.Infrastructure\ --startup-project .\HouseManagement.API\

## Database update

dotnet ef database update --project .\HouseManagement.Infrastructure\ --startup-project .\HouseManagement.API\
