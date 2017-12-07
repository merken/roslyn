# roslyn code samples

Welfare
=======

To run the welfare projects, first build the mssql server linux unemloyment docker image.

```
cd src/welfare/docker
docker build . -t=unemployment
```
Then, run the image 
```
docker run -p 1433:1433 -d unemployment
```
Connect SSMS to 127.0.0.1 with SA (password: abc123$%).

Run the db creation script src/welfare/welfare.sql/createdb.sql

Run the schema creation script src/welfare/welfare.sql/createschema.sql
