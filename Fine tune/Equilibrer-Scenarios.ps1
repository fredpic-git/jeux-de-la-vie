param([int]$Nombre=250,[int]$Jours=40,[int]$PremiereGraine=0,[int]$Travailleurs=8,[int]$PremierScenario=0,[int]$DernierScenario=6,[string]$Sortie='equilibrage/apres',[string]$Moteur=(Split-Path -Parent $PSScriptRoot),[switch]$Reference)
$ErrorActionPreference='Stop'
$ProjectRoot=Split-Path -Parent $PSScriptRoot
Add-Type -AssemblyName System.Windows.Forms,System.Drawing
$source=(@('SelectionNaturelle.cs','Ecosysteme.cs','Diplomatie.cs') | ForEach-Object { Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $Moteur $_) }) -join "`n"
$source+="`n"+(Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $PSScriptRoot 'EquilibrageBatch.cs'))
$source+='namespace SelectionNaturelle { public static class BatchProgram { public static void Main(string[] args) { BalanceBatch.Run(args[0],int.Parse(args[1]),int.Parse(args[2]),int.Parse(args[3]),int.Parse(args[4]),int.Parse(args[5]),int.Parse(args[6]),bool.Parse(args[7])); } } }'
$compiler=New-Object CodeDom.Compiler.CompilerParameters
$compiler.CompilerOptions='/optimize+'
$compiler.ReferencedAssemblies.AddRange(@('System.dll','System.Core.dll','System.Windows.Forms.dll','System.Drawing.dll','System.Xml.dll'))
$destination=Join-Path $PSScriptRoot $Sortie
New-Item -ItemType Directory -Force -Path $destination | Out-Null
$executable=Join-Path $destination ('calcul-'+[Guid]::NewGuid().ToString('N')+'.exe')
$compiler.OutputAssembly=$executable
$compiler.GenerateExecutable=$true
$provider=New-Object Microsoft.CSharp.CSharpCodeProvider
$compilation=$provider.CompileAssemblyFromSource($compiler,[string[]]@($source))
if($compilation.Errors.HasErrors) { throw (($compilation.Errors | ForEach-Object ToString) -join "`n") }
[IO.File]::WriteAllText(($executable+'.config'),'<configuration><runtime><gcServer enabled="true"/><gcConcurrent enabled="true"/></runtime></configuration>')
@{Nombre=$Nombre;Jours=$Jours;PremiereGraine=$PremiereGraine;Travailleurs=$Travailleurs;PremierScenario=$PremierScenario;DernierScenario=$DernierScenario;Reference=[bool]$Reference} | ConvertTo-Json | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $destination 'protocole.json')
$archive=Join-Path $destination 'moteur'
New-Item -ItemType Directory -Force -Path $archive | Out-Null
foreach($file in @('SelectionNaturelle.cs','Ecosysteme.cs','Diplomatie.cs')) { Copy-Item -LiteralPath (Join-Path $Moteur $file) -Destination (Join-Path $archive $file) }
foreach($file in @('EquilibrageBatch.cs','Equilibrer-Scenarios.ps1')) { Copy-Item -LiteralPath (Join-Path $PSScriptRoot $file) -Destination (Join-Path $archive $file) }
$watch=[Diagnostics.Stopwatch]::StartNew()
& $executable $destination $Nombre $Jours $PremiereGraine $Travailleurs $PremierScenario $DernierScenario ([bool]$Reference)
if($LASTEXITCODE -ne 0) { throw "Le calcul a echoue : $LASTEXITCODE" }
Write-Host "Duree : $($watch.Elapsed)"
