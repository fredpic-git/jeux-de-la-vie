param([ValidateRange(1,100000)][int]$Nombre=1000,[ValidateRange(1,10000)][int]$Jours=200,[int]$PremiereGraine=0,[ValidateRange(1,64)][int]$Travailleurs=4,[string[]]$Scenarios=@('actuel'),[string]$Sortie='statistiques-penurie')
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Windows.Forms,System.Drawing
$source=(@('SelectionNaturelle.cs','Ecosysteme.cs','Diplomatie.cs','StatistiquesPenurie.cs') | ForEach-Object { Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $PSScriptRoot $_) }) -join "`n"
Add-Type -TypeDefinition $source -ReferencedAssemblies System.Windows.Forms,System.Drawing,System.Xml,System.Core
$destination=Join-Path $PSScriptRoot $Sortie
New-Item -ItemType Directory -Force -Path $destination | Out-Null
Get-FileHash -Algorithm SHA256 -LiteralPath (Join-Path $PSScriptRoot 'SelectionNaturelle.cs'),(Join-Path $PSScriptRoot 'Ecosysteme.cs'),(Join-Path $PSScriptRoot 'Diplomatie.cs') | Select-Object Path,Hash | Export-Csv -NoTypeInformation -Encoding UTF8 -LiteralPath (Join-Path $destination 'version-moteur.csv')
$chrono=[Diagnostics.Stopwatch]::StartNew()
foreach($scenario in $Scenarios) { @{Nombre=$Nombre;Jours=$Jours;PremiereGraine=$PremiereGraine;Travailleurs=$Travailleurs;Scenario=$scenario} | ConvertTo-Json | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $destination ("protocole-"+$scenario+".json")); [SelectionNaturelle.ScarcityBatch]::Run($destination,$scenario,$Nombre,$Jours,$PremiereGraine,$Travailleurs) }
Write-Host "Durée : $($chrono.Elapsed). Résultats : $destination"
