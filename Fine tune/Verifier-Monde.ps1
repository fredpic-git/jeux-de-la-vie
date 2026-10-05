$ErrorActionPreference='Stop'
$ProjectRoot=Split-Path -Parent $PSScriptRoot
Add-Type -AssemblyName System.Windows.Forms,System.Drawing
$source=(Get-Content -Raw -Encoding UTF8 "$ProjectRoot\SelectionNaturelle.cs")+"`n"+(Get-Content -Raw -Encoding UTF8 "$ProjectRoot\Ecosysteme.cs")
$source += "`n" + (Get-Content -Raw -Encoding UTF8 "$ProjectRoot\Diplomatie.cs")
Add-Type -TypeDefinition $source -ReferencedAssemblies System.Windows.Forms,System.Drawing,System.Xml
[System.Windows.Forms.Application]::EnableVisualStyles()
[SelectionNaturelle.Tests]::Run()
$world=[SelectionNaturelle.World]::new([SelectionNaturelle.Rules]::new())
for($i=0;$i -lt 12 -and $world.Blobs.Count -gt 0;$i++) { $world.AdvanceDay(); Write-Host "Jour $($world.Day) : $($world.Blobs.Count) habitants" }
$world=[SelectionNaturelle.World]::new([SelectionNaturelle.Rules]::new())
$world.Blobs.Clear()
for($i=0;$i -lt 3;$i++) {
    $blob=[SelectionNaturelle.Blob]::new(1,1.3,1)
    $blob.Id=$i+1; $blob.FactionId=1; $blob.X=125+$i*25; $blob.Y=150; $blob.Heading=[Math]::PI/2; $blob.IsLeader=($i -eq 0)
    $world.Blobs.Add($blob)
}
$arena=[SelectionNaturelle.Arena]::new($world)
$arena.Size=[Drawing.Size]::new(1100,700)
$arena.Camera.Yaw=0; $arena.Camera.Elevation=.4; $arena.Camera.Zoom=3.5
$bitmap=[Drawing.Bitmap]::new(1100,700)
try { $arena.DrawToBitmap($bitmap,[Drawing.Rectangle]::new(0,0,1100,700)); $bitmap.Save("$PSScriptRoot\apercu-chef-maga.png") } finally { $bitmap.Dispose(); $arena.Dispose() }
$panel=[SelectionNaturelle.EconomyPanel]::new($world)
$panel.Size=[Drawing.Size]::new(1180,800)
$bitmap=[Drawing.Bitmap]::new(1180,800)
try { $panel.DrawToBitmap($bitmap,$panel.ClientRectangle); $bitmap.Save("$PSScriptRoot\apercu-ressources.png") } finally { $bitmap.Dispose(); $panel.Dispose() }
$world=[SelectionNaturelle.World]::new([SelectionNaturelle.Rules]::new())
$world.ChangeTrust(0,2,35); $world.ChangeTrust(0,3,30); $world.ChangeTrust(2,3,28)
$world.ChangeTrust(0,1,-50); $world.ChangeTrust(2,1,-45); $world.ChangeTrust(3,1,-38)
$world.PlanCoalitionRaids()
$panel=[SelectionNaturelle.DiplomacyPanel]::new($world)
$panel.Size=[Drawing.Size]::new(1180,850)
$bitmap=[Drawing.Bitmap]::new(1180,850)
try { $panel.DrawToBitmap($bitmap,$panel.ClientRectangle); $bitmap.Save("$PSScriptRoot\apercu-diplomatie.png") } finally { $bitmap.Dispose(); $panel.Dispose() }
