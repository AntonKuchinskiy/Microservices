# Microservices
![image](https://user-images.githubusercontent.com/42771805/159183633-ed13e57c-6d03-41b2-b13f-d208404f1ad1.png)
![image](https://user-images.githubusercontent.com/42771805/159184281-9126fb8c-a9f1-42e7-b210-242137311354.png)

dotnet new classlib -n Play.Common -f net6.0
dotnet new webapi -n Play.Catalog.Service -f net6.0
dotnet nuget add source C:\Study\Microservices\packages -n PlayEconomy
docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo
docker ps
docher rm 4360ebbb631029bbbd6801e801e753b22048cb88ecf0350d6aa1a67c5904892c
