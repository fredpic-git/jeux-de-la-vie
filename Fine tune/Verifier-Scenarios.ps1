param([int]$Jours=3)
$ErrorActionPreference='Stop'
$ProjectRoot=Split-Path -Parent $PSScriptRoot
Add-Type -AssemblyName System.Windows.Forms,System.Drawing
$source=(@('SelectionNaturelle.cs','Ecosysteme.cs','Diplomatie.cs') | ForEach-Object { Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $ProjectRoot $_) }) -join "`n"
Add-Type -TypeDefinition $source -ReferencedAssemblies System.Windows.Forms,System.Drawing,System.Xml
[Windows.Forms.Application]::EnableVisualStyles()
$directory=Join-Path $PSScriptRoot 'scenarios'
New-Item -ItemType Directory -Force -Path $directory | Out-Null
$names=@('equilibre-sans-mutations','equilibre-vitesse','equilibre-trois-traits','rarefaction','penurie','conflit-maga','grand-monde')
for($i=0;$i -lt $names.Count;$i++) { [SelectionNaturelle.Rules]::Preset($i).Save((Join-Path (Join-Path $ProjectRoot 'scenarios') ($names[$i]+'.xml'))) }
$watch=[Diagnostics.Stopwatch]::StartNew()
$world=[SelectionNaturelle.World]::new([SelectionNaturelle.Rules]::new())
$records=@()
for($day=0;$day -lt $Jours -and $world.Blobs.Count -gt 0;$day++) {
    $world.AdvanceDay()
    $counts=@(0,0,0,0)
    foreach($blob in $world.Blobs) { $counts[$blob.FactionId]++ }
    $last=$world.History[$world.History.Count-1]
    $alliances=0
    for($a=0;$a -lt 4;$a++) { for($b=$a+1;$b -lt 4;$b++) { if($world.Allied($a,$b)) { $alliances++ } } }
    $records+=[pscustomobject]@{ Jour=$world.Day; Bleu=$counts[0]; Corail=$counts[1]; Vert=$counts[2]; Violet=$counts[3]; Naissances=$last.Births; Morts=$last.Deaths; Raids=$world.Raids; Alliances=$alliances; ConfianceBleuCorail=$world.Relations[0,1]; ConfianceBleuVert=$world.Relations[0,2]; ConfianceBleuViolet=$world.Relations[0,3]; ConfianceCorailVert=$world.Relations[1,2]; ConfianceCorailViolet=$world.Relations[1,3]; ConfianceVertViolet=$world.Relations[2,3] }
}
$records | Export-Csv -NoTypeInformation -Encoding UTF8 -LiteralPath (Join-Path $directory 'verification-equilibre.csv')
$records | Format-Table -AutoSize
Write-Host "Duree : $($watch.Elapsed)"
$window=[SelectionNaturelle.MainWindow]::new()
$root=$window.Controls[0]; $window.Controls.Remove($root); $root.Font=$window.Font; $root.Size=$window.ClientSize; $root.CreateControl(); $root.PerformLayout()
$bitmap=[Drawing.Bitmap]::new($root.Width,$root.Height)
try { $root.DrawToBitmap($bitmap,[Drawing.Rectangle]::new(0,0,$root.Width,$root.Height)); $bitmap.Save((Join-Path $PSScriptRoot 'apercu-equilibre.png')) } finally { $bitmap.Dispose(); $root.Dispose(); $window.Dispose() }
$economy=[SelectionNaturelle.EconomyPanel]::new([SelectionNaturelle.World]::new([SelectionNaturelle.Rules]::new()))
$economy.Size=[Drawing.Size]::new(1180,800)
$bitmap=[Drawing.Bitmap]::new(1180,800)
try { $economy.DrawToBitmap($bitmap,$economy.ClientRectangle); $bitmap.Save((Join-Path $PSScriptRoot 'apercu-ressources-equilibre.png')) } finally { $bitmap.Dispose(); $economy.Dispose() }
