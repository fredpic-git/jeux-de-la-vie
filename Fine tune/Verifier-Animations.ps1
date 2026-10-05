param([int]$Frames=10)
$ErrorActionPreference='Stop'
$ProjectRoot=Split-Path -Parent $PSScriptRoot
Add-Type -AssemblyName System.Windows.Forms,System.Drawing
$source=(@('SelectionNaturelle.cs','Ecosysteme.cs','Diplomatie.cs') | ForEach-Object { Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $ProjectRoot $_) }) -join "`n"
Add-Type -TypeDefinition $source -ReferencedAssemblies System.Windows.Forms,System.Drawing,System.Xml
$world=[SelectionNaturelle.World]::new([SelectionNaturelle.Rules]::new())
for($tick=0;$tick -lt 60;$tick++) { $world.Step() }
$arena=[SelectionNaturelle.Arena]::new($world); $arena.Size=[Drawing.Size]::new(1000,650)
$bitmap=[Drawing.Bitmap]::new(1000,650)
try {
    $arena.DrawToBitmap($bitmap,$arena.ClientRectangle)
    $watch=[Diagnostics.Stopwatch]::StartNew()
    for($frame=0;$frame -lt $Frames;$frame++) { $arena.InvalidateScene(); $arena.DrawToBitmap($bitmap,$arena.ClientRectangle) }
    $full=$watch.Elapsed.TotalMilliseconds/$Frames
    $effects=$arena.GetType().GetMethod('InvalidateEffects',[Reflection.BindingFlags]'Instance,NonPublic')
    $visual=$world.GetType().GetMethod('VisualAction',[Reflection.BindingFlags]'Instance,NonPublic')
    $visual.Invoke($world,@($world.Blobs[0],3,-1,1)) | Out-Null
    $visual.Invoke($world,@($world.Blobs[1],1,3,1)) | Out-Null
    $depositor=[SelectionNaturelle.Blob]::new(1,1,1); $depositor.X=$world.Camps[3].X; $depositor.Y=$world.Camps[3].Y
    $visual.Invoke($world,@($depositor,4,2,5)) | Out-Null
    $watch.Restart()
    for($frame=0;$frame -lt $Frames;$frame++) { $effects.Invoke($arena,@()) | Out-Null; $arena.DrawToBitmap($bitmap,$arena.ClientRectangle) }
    $cached=$watch.Elapsed.TotalMilliseconds/$Frames
    [pscustomobject]@{ RenduCompletMs=[Math]::Round($full,2); AnimationsAvecCacheMs=[Math]::Round($cached,2) } | Format-List
    $bitmap.Save((Join-Path $PSScriptRoot 'apercu-animations-legeres.png'))
} finally { $bitmap.Dispose(); $arena.Dispose() }
$world.AdvanceDay()
$economy=[SelectionNaturelle.EconomyPanel]::new($world); $economy.Size=[Drawing.Size]::new(1180,900)
$bitmap=[Drawing.Bitmap]::new(1180,900)
try { $economy.DrawToBitmap($bitmap,$economy.ClientRectangle); $bitmap.Save((Join-Path $PSScriptRoot 'apercu-production-armes.png')) } finally { $bitmap.Dispose(); $economy.Dispose() }
