@echo off 
set yyyy=%date:~,4%  
set mm=%date:~5,2%  
set day=%date:~8,2%   
set "YYYYmmdd=%yyyy%%mm%%day%" 
set "YYYYmmdd=%YYYYmmdd: =%" 
set hh=%time:~0,2%  
set mi=%time:~3,2%  
set ss=%time:~6,2%   
set "hhmiss=%hh%%mi%%ss%"  
set "hhmiss=%hhmiss: =%"
set "filename=%YYYYmmdd%%hhmiss%"
set "filename=%filename: =%"

f:
cd F:\3.4.1\aspnet-core\src\zyGIS.EntityFrameworkCore
dotnet ef migrations add %filename%
dotnet ef database update
cd..
cd zyGIS.Web.Host
dotnet run --project zyGIS.Web.Host.csproj
exit