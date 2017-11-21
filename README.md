# roslyn
First build the mssql server linux adventureworks image.

```
cd src/aventureworks16
docker build . -t=adventureworks16
```
Then, run the image including the 
```
docker run -p 1433:1433 -d adventureworks16
```
Connect SSMS to localhost with SA.