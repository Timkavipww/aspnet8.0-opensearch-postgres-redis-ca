TO CREATE MIGRATIONS FROM FOLDER ./
```
dotnet ef migrations add InitialCreate \
--project Infrastructure \
--startup-project webapi \
--output-dir Persistence/Migrations
```