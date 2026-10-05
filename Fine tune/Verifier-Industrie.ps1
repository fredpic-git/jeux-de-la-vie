param([int]$Jours=6)
$ErrorActionPreference='Stop'
$ProjectRoot=Split-Path -Parent $PSScriptRoot
Add-Type -AssemblyName System.Windows.Forms,System.Drawing
$source=(@('SelectionNaturelle.cs','Ecosysteme.cs','Diplomatie.cs') | ForEach-Object { Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $ProjectRoot $_) }) -join "`n"
Add-Type -TypeDefinition $source -ReferencedAssemblies System.Windows.Forms,System.Drawing,System.Xml
$records=@()
foreach($preset in @(2,5)) {
    $world=[SelectionNaturelle.World]::new([SelectionNaturelle.Rules]::Preset($preset))
    for($day=0;$day -lt $Jours -and $world.Blobs.Count -gt 0;$day++) {
        $world.AdvanceDay()
        for($faction=0;$faction -lt 4;$faction++) {
            $camp=$world.Camps[$faction]; $equipped=0; $raiders=0
            foreach($blob in $world.Blobs) { if($blob.FactionId -eq $faction) { if($blob.Weapon -gt 0) { $equipped++ }; if($blob.RaidTarget -ge 0) { $raiders++ } } }
            $records+=[pscustomobject]@{ Mode=$preset; Jour=$world.Day; Faction=$camp.Nom; CharbonConsomme=$camp.ConsumedToday[4]; FerProduit=$camp.ProducedToday[6]; ArmesProduites=$camp.ProducedToday[7]; ArmesStock=$camp.Stock[7]; ArmesEquipees=$equipped; Reserves=$camp.Reserves; RaidsCumul=$world.Raids; Combattants=$raiders }
        }
    }
}
$records | Export-Csv -NoTypeInformation -Encoding UTF8 -LiteralPath (Join-Path $PSScriptRoot 'scenarios/verification-industrie.csv')
$records | Format-Table -AutoSize
