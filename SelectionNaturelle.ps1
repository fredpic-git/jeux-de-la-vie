param([switch]$Test, [string]$Apercu = '')
$ErrorActionPreference = 'Stop'
try {
    Add-Type -AssemblyName System.Windows.Forms
    Add-Type -AssemblyName System.Drawing
    $source = Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $PSScriptRoot 'SelectionNaturelle.cs')
    $source += "`n" + (Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $PSScriptRoot 'Ecosysteme.cs'))
    $source += "`n" + (Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $PSScriptRoot 'Diplomatie.cs'))
    Add-Type -TypeDefinition $source -ReferencedAssemblies System.Windows.Forms,System.Drawing,System.Xml
    [System.Windows.Forms.Application]::EnableVisualStyles()
    if ($Test) {
        [SelectionNaturelle.Tests]::Run()
    } elseif ($Apercu) {
        [SelectionNaturelle.Tests]::Render($Apercu)
    } else {
        [System.Windows.Forms.Application]::Run([SelectionNaturelle.MainWindow]::new())
    }
} catch {
    Write-Host "Erreur au lancement : $_" -ForegroundColor Red
    exit 1
}
