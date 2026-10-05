param([int]$Jours=8)
$ErrorActionPreference='Stop'
$ProjectRoot=Split-Path -Parent $PSScriptRoot
Add-Type -AssemblyName System.Windows.Forms,System.Drawing
$source=(@('SelectionNaturelle.cs','Ecosysteme.cs','Diplomatie.cs') | ForEach-Object { Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $ProjectRoot $_) }) -join "`n"
Add-Type -TypeDefinition $source -ReferencedAssemblies System.Windows.Forms,System.Drawing,System.Xml
[Windows.Forms.Application]::EnableVisualStyles()
$records=@()
foreach($seed in @(0,1,42)) {
    $rules=[SelectionNaturelle.Rules]::new(); $rules.Seed=$seed
    $world=[SelectionNaturelle.World]::new($rules)
    for($day=0;$day -lt $Jours -and $world.Blobs.Count -gt 0;$day++) {
        $world.AdvanceDay(); $last=$world.History[$world.History.Count-1]
        $records+=[pscustomobject]@{ Graine=$seed; Jour=$world.Day; Population=$world.Blobs.Count; Naissances=$last.Births; Morts=$last.Deaths }
    }
}
$records | Export-Csv -NoTypeInformation -Encoding UTF8 -LiteralPath (Join-Path $PSScriptRoot 'scenarios/verification-demographie.csv')
$records | Format-Table -AutoSize
$rules=[SelectionNaturelle.Rules]::new(); $rules.TornadoesEnabled=$true; $rules.UnequalResources=$true
$rules.FactionSize=25; $world=[SelectionNaturelle.World]::new($rules)
for($tick=0;$tick -lt 80;$tick++) { $world.Step() }
$arena=[SelectionNaturelle.Arena]::new($world); $arena.Size=[Drawing.Size]::new(1000,650)
$arena.Camera.Zoom=1.5
$bitmap=[Drawing.Bitmap]::new(1000,650)
try { $arena.DrawToBitmap($bitmap,$arena.ClientRectangle); $bitmap.Save((Join-Path $PSScriptRoot 'apercu-tornades.png')) } finally { $bitmap.Dispose(); $arena.Dispose() }
