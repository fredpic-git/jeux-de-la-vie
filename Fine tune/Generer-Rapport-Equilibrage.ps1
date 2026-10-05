param([string]$Avant='equilibrage/avant-valide',[string]$Apres='equilibrage/apres-valide',[int]$Nombre=250,[int]$Jours=40,[string]$Sortie='equilibrage/rapport')
$ErrorActionPreference='Stop'
$ProjectRoot=Split-Path -Parent $PSScriptRoot
$source=Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $PSScriptRoot 'RapportEquilibrage.cs')
Add-Type -TypeDefinition $source -ReferencedAssemblies System.Core,System.Web.Extensions
[AnalyseEquilibrage.Report]::Create((Join-Path $PSScriptRoot $Avant),(Join-Path $PSScriptRoot $Apres),(Join-Path $PSScriptRoot $Sortie),$Nombre,$Jours,(Join-Path $PSScriptRoot 'Rapport-Equilibrage.template.html'))
Write-Host 'Rapport genere et integrite des 14 lots verifiee.'
