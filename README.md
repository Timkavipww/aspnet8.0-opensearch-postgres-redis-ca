TO CREATE MIGRATIONS FROM FOLDER ./
```
dotnet ef migrations add InitialCreate2 \
--project Infrastructure \
--startup-project webapi \
--output-dir Persistence/Migrations
```

dotnet ef migrations add InitialCreate2 --project Infrastructure --startup-project webapi --output-dir Persistence/Migrations