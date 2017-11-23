# roslyn
First build the mssql server linux adventureworks image.

```
cd src/adventureworks16
docker build . -t=adventureworks16
```
Then, run the image including the 
```
docker run -p 1433:1433 -d adventureworks16
```
Connect SSMS to localhost with SA and restore backup.

Scaffold the ef model.
```
dotnet ef dbcontext scaffold -c Adventureworks -o model "Data Source=127.0.0.1;Initial Catalog=AdventureWorks2016;persist security info=True;user id=sa;password=abc123$%" Microsoft.EntityFrameworkCore.SqlServer --force

dotnet ef dbcontext scaffold -c AdventureworksLogic -o logic "Data Source=127.0.0.1;Initial Catalog=AdventureWorks.Logic;persist security info=True;user id=sa;password=abc123$%" Microsoft.EntityFrameworkCore.SqlServer --force
```

https://docs.microsoft.com/en-us/aspnet/core/mvc/razor-pages/?tabs=visual-studio
https://msdn.microsoft.com/en-us/magazine/mt842512.aspx