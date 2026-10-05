param([string]$Moteur=(Join-Path $PSScriptRoot 'equilibrage/avant/moteur'),[int]$Jours=3)
$ErrorActionPreference='Stop'
$ProjectRoot=Split-Path -Parent $PSScriptRoot
$source=(@('SelectionNaturelle.cs','Ecosysteme.cs','Diplomatie.cs') | ForEach-Object { Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $Moteur $_) }) -join "`n"
$source=$source.Replace('public const double Extent=300;','public const double Extent=300; public long[] Profile=new long[8];')
$source=$source.Replace('if(b.Dead || b.Home) continue; if(UseCastles', 'if(b.Dead || b.Home) continue; long stamp=System.Diagnostics.Stopwatch.GetTimestamp(),mark; if(UseCastles')
$markers=@('double reach=Rules.SenseRadius*b.Sense;','double workHeading;','double walkingSpeed=','MoveBlobIndex(b); WorkerAction(b);','// A single food item','if(b.Energy>0 && b.RaidTarget','if(b.Returning && b.Food>=Rules.SurvivalFood','DiplomacyContacts(b);')
for($i=0;$i -lt $markers.Count;$i++) {
 $marker=$markers[$i]
 $source=$source.Replace($marker, ('mark=System.Diagnostics.Stopwatch.GetTimestamp(); Profile['+$i+']+=mark-stamp; stamp=mark; '+$marker))
}
$source+='namespace SelectionNaturelle { class ProfileProgram { static void Main(string[] args) { World w=new World(Rules.Preset(2)); for(int i=0;i<int.Parse(args[0]);i++) w.AdvanceDay(); Console.WriteLine("PROFILE COST/MATE,QUERY,WORKDIR,MOVE,WORKACTION,FOOD,MATE,HOME: "+string.Join("/",Array.ConvertAll(w.Profile,delegate(long n){ return (n/(double)System.Diagnostics.Stopwatch.Frequency).ToString("F3"); }))); } } }'
$compiler=New-Object CodeDom.Compiler.CompilerParameters
$compiler.CompilerOptions='/optimize+'
$compiler.ReferencedAssemblies.AddRange(@('System.dll','System.Core.dll','System.Windows.Forms.dll','System.Drawing.dll','System.Xml.dll'))
$compiler.OutputAssembly=Join-Path $PSScriptRoot ('equilibrage/profile-'+[Guid]::NewGuid().ToString('N')+'.exe')
$compiler.GenerateExecutable=$true
$provider=New-Object Microsoft.CSharp.CSharpCodeProvider
$result=$provider.CompileAssemblyFromSource($compiler,[string[]]@($source))
if($result.Errors.HasErrors) { throw (($result.Errors | ForEach-Object ToString) -join "`n") }
& $compiler.OutputAssembly $Jours
