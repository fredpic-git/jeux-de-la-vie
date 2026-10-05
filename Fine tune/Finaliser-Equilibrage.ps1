param([string]$Avant='equilibrage/avant-valide',[string]$Apres='equilibrage/apres-valide',[int]$Nombre=250,[int]$Jours=40,[string]$Sortie='equilibrage/rapport')
$ErrorActionPreference='Stop'
$ProjectRoot=Split-Path -Parent $PSScriptRoot
$names=@('sans-mutations','vitesse','trois-traits','rarefaction','penurie','conflit-maga','grand-monde')
$previous=-1
do {
    $complete=0
    foreach($lot in @($Avant,$Apres)) {
        foreach($name in $names) {
            $path=Join-Path $PSScriptRoot ($lot+'/runs-'+$name+'.csv')
            if((Test-Path -LiteralPath $path) -and @(Import-Csv -LiteralPath $path).Count -eq $Nombre) { $complete++ }
        }
    }
    if($complete -ne $previous) { Write-Host "$complete/14 lots complets ; attente de la fin des calculs."; $previous=$complete }
    if($complete -lt 14) { Start-Sleep -Seconds 10 }
} while($complete -lt 14)
& (Join-Path $PSScriptRoot 'Generer-Rapport-Equilibrage.ps1') -Avant $Avant -Apres $Apres -Nombre $Nombre -Jours $Jours -Sortie $Sortie
