param([int]$Jours=8)
$ErrorActionPreference='Stop'
$ProjectRoot=Split-Path -Parent $PSScriptRoot
Add-Type -AssemblyName System.Windows.Forms,System.Drawing
$source=(@('SelectionNaturelle.cs','Ecosysteme.cs','Diplomatie.cs') | ForEach-Object { Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $ProjectRoot $_) }) -join "`n"
Add-Type -TypeDefinition $source -ReferencedAssemblies System.Windows.Forms,System.Drawing,System.Xml
[Windows.Forms.Application]::EnableVisualStyles()
$records=@()
foreach($shape in @(@{Nom='carre';Colonnes=30;Lignes=30},@{Nom='rectangle';Colonnes=60;Lignes=20},@{Nom='allonge';Colonnes=80;Lignes=10})) {
    $rules=[SelectionNaturelle.Rules]::new(); $rules.MapColumns=$shape.Colonnes; $rules.MapRows=$shape.Lignes
    $world=[SelectionNaturelle.World]::new($rules)
    $arena=[SelectionNaturelle.Arena]::new($world); $arena.Size=[Drawing.Size]::new(1200,750)
    $arena.Camera.Zoom=1.4
    $bitmap=[Drawing.Bitmap]::new(1200,750)
    try { $arena.DrawToBitmap($bitmap,$arena.ClientRectangle); $bitmap.Save((Join-Path $PSScriptRoot ('apercu-chateaux-'+$shape.Nom+'.png'))) } finally { $bitmap.Dispose(); $arena.Dispose() }
    for($day=0;$day -lt $Jours -and $world.Blobs.Count -gt 0;$day++) {
        $world.AdvanceDay(); $last=$world.History[$world.History.Count-1]; $counts=@(0,0,0,0)
        foreach($blob in $world.Blobs) { $counts[$blob.FactionId]++ }
        $records+=[pscustomobject]@{ Terrain=$shape.Nom; Colonnes=$rules.MapColumns; Lignes=$rules.MapRows; Jour=$world.Day; Population=$world.Blobs.Count; Bleu=$counts[0]; Corail=$counts[1]; Vert=$counts[2]; Violet=$counts[3]; Naissances=$last.Births; Morts=$last.Deaths }
    }
}
$records | Export-Csv -NoTypeInformation -Encoding UTF8 -LiteralPath (Join-Path $PSScriptRoot 'scenarios/verification-chateaux.csv')
$records | Format-Table -AutoSize
