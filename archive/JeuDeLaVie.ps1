param([switch]$Test)
$ErrorActionPreference = 'Stop'
try {
    Add-Type -AssemblyName System.Windows.Forms
    Add-Type -AssemblyName System.Drawing
    Add-Type -Path (Join-Path $PSScriptRoot 'JeuDeLaVie.cs') -ReferencedAssemblies System.Windows.Forms,System.Drawing
    if ($Test) {
        [JeuDeLaVie.Verification]::Run()
    } else {
        [System.Windows.Forms.Application]::EnableVisualStyles()
        [System.Windows.Forms.Application]::Run([JeuDeLaVie.LifeWindow]::new())
    }
} catch {
    Write-Host "Impossible de lancer le jeu : $_" -ForegroundColor Red
    exit 1
}
