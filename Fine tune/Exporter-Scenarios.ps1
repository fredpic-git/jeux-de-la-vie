$ErrorActionPreference='Stop'
$ProjectRoot=Split-Path -Parent $PSScriptRoot
Add-Type -AssemblyName System.Windows.Forms,System.Drawing
$source=(@('SelectionNaturelle.cs','Ecosysteme.cs','Diplomatie.cs') | ForEach-Object { Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $ProjectRoot $_) }) -join "`n"
Add-Type -TypeDefinition $source -ReferencedAssemblies System.Windows.Forms,System.Drawing,System.Xml
$names=@('equilibre-sans-mutations','equilibre-vitesse','equilibre-trois-traits','rarefaction','penurie','conflit-maga','grand-monde')
for($i=0;$i -lt $names.Count;$i++) { [SelectionNaturelle.Rules]::Preset($i).Save((Join-Path $ProjectRoot ('scenarios/'+$names[$i]+'.xml'))) }
Write-Host 'Les sept scenarios ont ete exportes.'
