# Play.Inventory.Contracts
Inventory contracts used by Play Economy services.

# Create and publish package
```powershell
$version="1.0.3"
$owner="RafaelJCamara"
$gh_pat="[PERSONAL ACCESS TOKEN HERE]"

dotnet pack src\Play.Inventory.Contracts\ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/Play.Inventory -o ..\packages

dotnet nuget push ..\packages\Play.Inventory.Contracts.$version.nupkg --api-key $gh_pat --source "github"
```