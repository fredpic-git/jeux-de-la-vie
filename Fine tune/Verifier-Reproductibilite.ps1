$ErrorActionPreference='Stop'
$ProjectRoot=Split-Path -Parent $PSScriptRoot
Add-Type -AssemblyName System.Windows.Forms,System.Drawing
$old=(@('SelectionNaturelle.cs','Ecosysteme.cs','Diplomatie.cs') | ForEach-Object { Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path (Join-Path $PSScriptRoot 'equilibrage/reference-originale/moteur') $_) }) -join "`n"
$old=$old.Replace('namespace SelectionNaturelle','namespace MoteurOriginal')
$current=(@('SelectionNaturelle.cs','Ecosysteme.cs','Diplomatie.cs') | ForEach-Object { Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $ProjectRoot $_) }) -join "`n"
$current=$current.Substring($current.IndexOf('namespace SelectionNaturelle'))
$verifier=@'
namespace SelectionNaturelle {
using System.Threading.Tasks;
public static class ExactReplayTests {
 public static void Run() {
  int[] scenarios={0,2,4,5,6};
  Parallel.For(0,10,new ParallelOptions {MaxDegreeOfParallelism=4},delegate(int index) {
   int preset=scenarios[index/2],seed=index%2==0?0:42;
   Rules rules=Rules.Preset(preset); rules.Seed=seed; rules.ReproductionFood=2; rules.TransporterBagSlots=3; rules.AgricultureWater=2; rules.PrioritizeWater=false; rules.EconomicDiplomacy=false; rules.EnergySitesPerQuarter=3; rules.EnergyPerSite=3; rules.MaxReserves=8; rules.CastleRepairsUseResources=false; rules.CastleRepairPerNight=5;
   MoteurOriginal.Rules reference=MoteurOriginal.Rules.Preset(preset); reference.Seed=seed;
   World a=new World(rules); MoteurOriginal.World b=new MoteurOriginal.World(reference);
   while(a.Day<=3) {
    if(a.Day!=b.Day || a.Tick!=b.Tick || a.Blobs.Count!=b.Blobs.Count || a.Raids!=b.Raids || a.CoalitionStrikes!=b.CoalitionStrikes) throw new Exception("Entete divergente "+preset+"/"+seed+" J"+a.Day+" T"+a.Tick);
    for(int i=0;i<a.Blobs.Count;i++) {
     Blob x=a.Blobs[i]; MoteurOriginal.Blob y=b.Blobs[i];
     if(x.Id!=y.Id || x.X!=y.X || x.Y!=y.Y || x.Heading!=y.Heading || x.Energy!=y.Energy || x.Food!=y.Food || x.Water!=y.Water || x.Dead!=y.Dead || x.Home!=y.Home || x.Returning!=y.Returning || x.Weapon!=y.Weapon || x.RaidTarget!=y.RaidTarget || x.FactionId!=y.FactionId || (x.Mate==null?0:x.Mate.Id)!=(y.Mate==null?0:y.Mate.Id)) throw new Exception("Creature divergente "+preset+"/"+seed+" J"+a.Day+" T"+a.Tick+" #"+x.Id);
     for(int k=0;k<8;k++) if(x.Bag[k]!=y.Bag[k]) throw new Exception("Sac divergent");
    }
    for(int f=0;f<4;f++) { if(a.Camps[f].Fortification!=b.Camps[f].Fortification) throw new Exception("Fortification divergente"); for(int k=0;k<8;k++) if(a.Camps[f].Stock[k]!=b.Camps[f].Stock[k]) throw new Exception("Stocks divergents"); for(int z=0;z<4;z++) if(a.Relations[f,z]!=b.Relations[f,z]) throw new Exception("Confiance divergente"); }
    if(a.DayFinished && a.Day==3) break; a.Step(); b.Step();
   }
   Console.WriteLine("Rejeu exact : scenario "+preset+", graine "+seed+", trois jours et toutes les positions / stocks / combats identiques.");
  });
 }
}
}
'@
$compiler=New-Object CodeDom.Compiler.CompilerParameters
$compiler.CompilerOptions='/optimize+'
$compiler.ReferencedAssemblies.AddRange(@('System.dll','System.Core.dll','System.Windows.Forms.dll','System.Drawing.dll','System.Xml.dll'))
Add-Type -TypeDefinition ($old+"`n"+$current+"`n"+$verifier) -CompilerParameters $compiler
[SelectionNaturelle.ExactReplayTests]::Run()
