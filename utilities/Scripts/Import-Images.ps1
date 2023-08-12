$databaseName = "guexit_game"
$userName = "postgres"
$password = "postgres"
$dbHost = "localhost"

$csvPath = "./Images.csv"

Write-Host 'Reading Images CSV...';

$tempFile = [System.IO.Path]::GetTempFileName()
$csvContent = Import-Csv -Path $csvPath
foreach ($row in $csvContent) {
    $id = $row.Id
    $gameRoomId = $row.GameRoomId
    $url = $row.Url.Replace("'", "''")
    $createdAt = $row.CreatedAt

    $sql = @"
INSERT INTO public."Images" ("Id", "GameRoomId", "Url", "CreatedAt") 
VALUES ('$id', '$gameRoomId', '$url', '$createdAt')
ON CONFLICT ("Id") 
DO UPDATE SET "GameRoomId" = '$gameRoomId', "Url" = '$url', "CreatedAt" = '$createdAt';
"@

    Add-Content -Path $tempFile -Value $sql
}

Write-Host 'Importing Images to database...'

$psqlCommand = "psql -h $dbHost -p 5433 -U $userName -d $databaseName -f $tempFile -W $password" 
Invoke-Expression "$psqlCommand" | Out-Null

Remove-Item -Path $tempFile

Write-Host 'Images imported successfully' 
