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
dotnet ef dbcontext scaffold -c Adventureworks "Data Source=127.0.0.1;Initial Catalog=AdventureWorks2016;persist security info=True;user id=sa;password=abc123$%" Microsoft.EntityFrameworkCore.SqlServer --force
```