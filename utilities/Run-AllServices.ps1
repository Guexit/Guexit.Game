
# docker-compose --file "../../TryGuessIt.IdentityProvider" up  -d 

Start-Process dotnet.exe "../../TryGuessIt.Game/src/TryGuessIt.Game.WebApi/bin/Debug/net7.0/TryGuessIt.Game.WebApi.dll"
Start-Process dotnet.exe "../../TryGuessIt.IdentityProvider/src/TryGuessIt.IdentityProvider.WebApi/bin/Debug/net7.0/TryGuessIt.IdentityProvider.WebApi.dll"
Start-Process dotnet.exe "../../TryGuessIt.BackendForFrontend/TryGuessIt.BackendForFrontend.WebApi/bin/Debug/net7.0/TryGuessIt.BackendForFrontend.WebApi.dll"