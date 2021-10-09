# 1670
Project Employee Training Manamement System. The system is based on ASP.NET MVC Web Application (.NET Framework 4.7.2)

# Debug issue 
* Could not find a part of the path '.\WebApp\bin\roslyn\csc.exe'.
Open PMS and run:
  * Install-Package Microsoft.Net.Compilers -r

* Cannot attach the file '.\WebApp\App_Data\aspnet-WebApp-*.mdf
Root cause: the local database is not existed.
Solution: Recreate local database. OPEN PMS and run:
  * sqllocaldb start mssqllocaldb
  * mkdir WebApp/App_Data
  * update-database
* If the problem is not resolved run:
  * sqllocaldb stop mssqllocaldb
  * sqllocaldb delete mssqllocaldb
  * sqllocaldb create mssqllocaldb
then run the first solution again


