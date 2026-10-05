param([int]$Nombre=250,[int]$Jours=100,[int]$PremiereGraine=0,[int]$Travailleurs=4,[int]$PremierScenario=0,[int]$DernierScenario=6,[string]$Sortie='equilibrage/avant')
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Windows.Forms,System.Drawing
$source=(@('SelectionNaturelle.cs','Ecosysteme.cs','Diplomatie.cs','EquilibrageBatch.cs') | ForEach-Object { Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $PSScriptRoot $_) }) -join "`n"
Add-Type -TypeDefinition $source -ReferencedAssemblies System.Windows.Forms,System.Drawing,System.Xml,System.Core
$destination=Join-Path $PSScriptRoot $Sortie
New-Item -ItemType Directory -Force -Path $destination | Out-Null
@{Nombre=$Nombre;Jours=$Jours;PremiereGraine=$PremiereGraine;Travailleurs=$Travailleurs;PremierScenario=$PremierScenario;DernierScenario=$DernierScenario} | ConvertTo-Json | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $destination 'protocole.json')
$archive=Join-Path $destination 'moteur'
New-Item -ItemType Directory -Force -Path $archive | Out-Null
foreach($file in @('SelectionNaturelle.cs','Ecosysteme.cs','Diplomatie.cs','EquilibrageBatch.cs','Equilibrer-Scenarios.ps1')) { Copy-Item -LiteralPath (Join-Path $PSScriptRoot $file) -Destination (Join-Path $archive $file) }
$watch=[Diagnostics.Stopwatch]::StartNew()
[SelectionNaturelle.BalanceBatch]::Run($destination,$Nombre,$Jours,$PremiereGraine,$Travailleurs,$PremierScenario,$DernierScenario)
Write-Host "Duree : $($watch.Elapsed)"
