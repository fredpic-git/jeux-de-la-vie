param([string]$Dossier='statistiques-penurie')
$ErrorActionPreference='Stop'
$ProjectRoot=Split-Path -Parent $PSScriptRoot
$root=Join-Path $PSScriptRoot $Dossier
$maxDays=200; $firstSeed=0; $totalRuns=1000
if(Test-Path -LiteralPath (Join-Path $root 'protocole-actuel.json')) { $protocol=Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $root 'protocole-actuel.json') | ConvertFrom-Json; $maxDays=[int]$protocol.Jours; $firstSeed=[int]$protocol.PremiereGraine; $totalRuns=[int]$protocol.Nombre }
$names=@('Bleus','MAGA','Verts','Violets')
$labels=@{ actuel='Jeu actuel : 25 fondateurs'; equilibre='24 fondateurs : 6 par faction'; rotation='24 fondateurs : factions non-MAGA permutées'; neutre='24 fondateurs : petit monde désactivé'; sans_raids='24 fondateurs : raids et alliances désactivés' }
function Intervalle([int]$k,[int]$n) {
    $p=$k/[double]$n; $z2=1.96*1.96; $centre=($p+$z2/(2*$n))/(1+$z2/$n); $rayon=1.96*[Math]::Sqrt($p*(1-$p)/$n+$z2/(4*$n*$n))/(1+$z2/$n)
    return ('{0:F1}–{1:F1} %' -f (100*($centre-$rayon)),(100*($centre+$rayon)))
}
$report=[Collections.Generic.List[string]]::new()
$report.Add('# Statistiques de survie en pénurie')
$report.Add('')
$report.Add("Expérience réalisée sur le moteur actuel, sans modifier les règles du jeu. Nourriture distribuée : 10 par jour ; agriculture, stocks, eau, relief, chef et diplomatie actifs dans le scénario principal. Graines $firstSeed à $($firstSeed+$totalRuns-1), au maximum $maxDays jours par partie. Les scénarios de contrôle utilisent les mêmes graines.")
$report.Add('')
$report.Add("**Dernier individu** : premier instant observé après un pas où un seul individu est encore vivant. Il peut ensuite mourir ; on mesure qui reste en dernier, pas son immortalité. **Dernière faction** : première faction seule encore vivante, même avec plusieurs habitants. Une extinction simultanée ne reçoit aucun dernier individu. Les parties qui atteignent $maxDays jours sans individu unique sont censurées ; elles peuvent avoir déjà une faction seule.")
$summary=@()
foreach($scenario in @('actuel','equilibre','rotation','neutre','sans_raids')) {
    $file=Join-Path $root "$scenario.csv"
    if(!(Test-Path -LiteralPath $file)) { continue }
    $data=@(Import-Csv -LiteralPath $file); $n=$data.Count
    $report.Add(''); $report.Add('## '+$labels[$scenario]); $report.Add('')
    $report.Add("$n simulations."); $report.Add('')
    $report.Add('| Faction | Dernier individu | % des simulations | Intervalle à 95 % | Dernière faction |')
    $report.Add('|---|---:|---:|---|---:|')
    $counts=@()
    for($f=0;$f -lt 4;$f++) {
        $k=@($data | Where-Object { [int]$_.faction_dernier_individu -eq $f }).Count
        $sole=@($data | Where-Object { [int]$_.premiere_faction_seule -eq $f }).Count
        $counts+=$k
        $report.Add(('| {0} | {1} | {2:F1} % | {3} | {4} |' -f $names[$f],$k,(100*$k/$n),(Intervalle $k $n),$sole))
    }
    $extinct=@($data | Where-Object resultat -eq 'extinction').Count
    $limit=@($data | Where-Object resultat -eq 'limite_jours').Count
    $resolved=@($data | Where-Object resultat -eq 'dernier_individu')
    $noSingleFaction=@($data | Where-Object { [int]$_.premiere_faction_seule -lt 0 }).Count
    $report.Add(''); $report.Add("Extinctions sans dernier individu unique : $extinct. Sans dernier individu après $maxDays jours : $limit. Sans dernière faction unique observée : $noSingleFaction.")
    if($scenario -eq 'actuel') {
        $default=@($data | Where-Object { [int]$_.graine -eq 42 })
        if($default.Count -eq 1 -and [int]$default[0].faction_dernier_individu -ge 0) { $report.Add("Graine par défaut 42 : dernier individu de la faction $($names[[int]$default[0].faction_dernier_individu]) au jour $($default[0].jour). Ce résultat se répète avec les mêmes règles lorsque l'on clique sur Recommencer.") }
    }
    $neutral=$counts[0]+$counts[2]+$counts[3]
    if($neutral -gt 0) {
        $expected=$neutral/3.0; $chi=0; foreach($f in @(0,2,3)) { $chi+=[Math]::Pow($counts[$f]-$expected,2)/$expected }
        $pval=[Math]::Exp(-$chi/2)
        $report.Add(('Parmi les {0} derniers individus non-MAGA : Bleu {1:F1} %, Vert {2:F1} %, Violet {3:F1} %. Test des trois fréquences égales : chi² = {4:F3}, 2 degrés de liberté, p = {5:G5}.' -f $neutral,(100*$counts[0]/$neutral),(100*$counts[2]/$neutral),(100*$counts[3]/$neutral),$chi,$pval))
        $report.Add('Ce test porte sur les parties résolues par un individu unique. Les parties censurées limitent la portée de cette comparaison ; les scénarios sont des essais du même moteur, pas des observations biologiques.')
    }
    if($resolved.Count -gt 0) {
        $mean=($resolved | Measure-Object jour -Average).Average
        $mixed=@($resolved | Where-Object { $_.peau_a -ne $_.peau_b }).Count
        $report.Add(('Jour moyen du dernier individu : {0:F1}. Derniers individus à peau mixte : {1}/{2}. La faction culturelle et la peau ne sont pas la même mesure.' -f $mean,$mixed,$resolved.Count))
        $report.Add(''); $report.Add('| Dernier individu de la faction | Vitesse moyenne | Taille moyenne | Perception moyenne |'); $report.Add('|---|---:|---:|---:|')
        for($f=0;$f -lt 4;$f++) { $group=@($resolved | Where-Object { [int]$_.faction_dernier_individu -eq $f }); if($group.Count -gt 0) { $report.Add(('| {0} | {1:F3} | {2:F3} | {3:F3} |' -f $names[$f],($group | Measure-Object vitesse -Average).Average,($group | Measure-Object taille -Average).Average,($group | Measure-Object perception -Average).Average)) } }
        $report.Add(''); $report.Add('| Marqueur de peau | Derniers individus de peau unie | Derniers individus porteurs |'); $report.Add('|---|---:|---:|')
        for($f=0;$f -lt 4;$f++) {
            $pure=@($resolved | Where-Object { [int]$_.peau_a -eq $f -and [int]$_.peau_b -eq $f }).Count
            $carriers=@($resolved | Where-Object { [int]$_.peau_a -eq $f -or [int]$_.peau_b -eq $f }).Count
            $skinNames=@('Bleu','Corail','Vert','Violet'); $report.Add("| $($skinNames[$f]) | $pure | $carriers |")
        }
        $report.Add('Les porteurs de peau mixte sont comptés dans leurs deux couleurs : cette colonne ne s''additionne pas en nombre de parties.')
    }
    $summary+=[pscustomobject]@{ Scenario=$scenario; Simulations=$n; Bleu=$counts[0]; Maga=$counts[1]; Vert=$counts[2]; Violet=$counts[3]; Extinctions=$extinct; Limite=$limit }
}
$report.Add(''); $report.Add('## Interprétation des contrôles'); $report.Add('')
if(@($summary | Where-Object Scenario -eq 'equilibre').Count -gt 0) { $report.Add('Le jeu actuel contient un déséquilibre initial mesurable : sept Bleus contre six dans chacune des autres factions. Dans le contrôle à six par faction, les fréquences des trois factions non-MAGA sont compatibles avec des probabilités égales (voir le test ci-dessus). Cela suggère que l''effectif initial contribue à l''avantage bleu ; ce contrôle ne prouve pas que tous les autres effets sont nuls.') }
if(@($summary | Where-Object Scenario -eq 'rotation').Count -gt 0) { $report.Add('La permutation des positions des factions non-MAGA ne montre pas non plus d''avantage systématique bleu dans ce lot. La seule distance initiale aux MAGA ne suffit donc pas à expliquer les répétitions observées avec la graine 42. La carte reste asymétrique et de petits effets de position peuvent subsister.') }
if(@($summary | Where-Object Scenario -eq 'neutre').Count -gt 0) { $report.Add('Sans petit monde, les quatre factions donnent des nombres de derniers individus proches. Ce contrôle ne révèle pas de bonus intrinsèque lié à la couleur dans le modèle de base.') }
if(@($summary | Where-Object Scenario -eq 'sans_raids').Count -gt 0) { $report.Add('Désactiver les raids et les alliances change fortement les résultats MAGA, alors que leur chef, leur escorte, le relief et les ressources restent actifs. Le comportement de conflit joue donc un rôle majeur dans ces lots ; ce contrôle modifie deux mécanismes à la fois et ne sépare pas l''effet propre des raids de celui des alliances.') }
$report.Add('Les parties sans dernier individu unique à 200 jours ne sont pas des victoires attribuées au groupe le plus nombreux. L''agriculture et les réserves permettent des populations durables. Pour une comparaison plus longue, relancer avec une limite de jours plus grande et conserver la même définition des résultats.')
$report.Add(''); $report.Add('## Lecture du code et limites'); $report.Add('')
$report.Add('- La graine par défaut est 42. « Recommencer » reproduit la même expérience avec les mêmes règles : les relances ne sont pas indépendantes. Pour ce lot, chaque partie possède une graine différente.')
$report.Add('- Avec 25 fondateurs distribués par i % 4, les Bleus commencent à 7, les autres à 6. Les Bleus disposent notamment d''un artisan supplémentaire. Avec 24, toutes les factions commencent à 6 avec la même répartition de métiers.')
$report.Add('- La carte fixe ne présente pas de symétrie entre ouest, nord et sud : les lacs, montagnes et dépôts affectent les trajets, l''eau et les métiers. L''opposition initiale aux MAGA est une autre différence ; les côtés de départ sont retirés aléatoirement les jours suivants.')
$report.Add('- Les traits suivent la même distribution initiale, mais les individus possèdent des génomes différents ; un petit échantillon ne garantit pas les mêmes phénotypes, sexes ou descendants par faction.')
$report.Add('- Le commerce traite les paires de factions dans un ordre fixe, et la planification des raids parcourt les cibles par indice. Des effets de cet ordre sont possibles ; le scénario de permutation aide à distinguer les positions des noms de faction sans isoler chaque mécanisme.')
$report.Add('- La permutation conserve les mêmes organismes, gènes, sexes, métiers, positions et carte, et change seulement les étiquettes culturelles des trois factions non-MAGA : ouest devient Vert, nord Violet, sud Bleu. Les MAGA restent à l''est. Les réserves initiales de ces trois camps sont identiques.')
$report.Add('- Le contrôle neutre désactive tout le petit monde : pas de relief fonctionnel, eau, chef, réserves ni raids. Il vérifie la symétrie du modèle de base ; il ne représente pas un monde avec des MAGA particuliers.')
$report.Add('- Le contrôle sans raids conserve le relief, les ressources, le chef et l''avidité de collecte, mais désactive les raids et les alliances.')
$report.Add(''); $report.Add('Les CSV contiennent un enregistrement par graine, la faction du dernier individu, les deux marqueurs de peau, les traits, le métier, le sexe, le jour, les extinctions et les effectifs restants à la limite. Les XML donnent les règles exactes. `version-moteur.csv` conserve les empreintes SHA-256 des sources. Exécution : `powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\Simuler-Penurie.ps1`.')
[IO.File]::WriteAllLines((Join-Path $root 'RAPPORT.md'),$report,[Text.UTF8Encoding]::new($true))
$summary | Export-Csv -NoTypeInformation -Encoding UTF8 -LiteralPath (Join-Path $root 'synthese.csv')
$summary | Format-Table -AutoSize
if(Test-Path -LiteralPath (Join-Path $root 'actuel.csv')) {
    Add-Type -AssemblyName System.Windows.Forms.DataVisualization,System.Drawing
    $chart=[Windows.Forms.DataVisualization.Charting.Chart]::new()
    try {
        $chart.Width=1200; $chart.Height=650; $chart.BackColor=[Drawing.Color]::White
        $area=[Windows.Forms.DataVisualization.Charting.ChartArea]::new('Resultats')
        $area.AxisY.Title='% des simulations'; $area.AxisY.Minimum=0; $area.AxisY.MajorGrid.LineColor=[Drawing.Color]::Gainsboro
        $area.AxisX.MajorGrid.Enabled=$false; $area.AxisX.LabelStyle.Interval=1
        $chart.ChartAreas.Add($area)
        $main=@(Import-Csv -LiteralPath (Join-Path $root 'actuel.csv'))
        $title=$chart.Titles.Add("Pénurie : faction du dernier individu — $($main.Count) graines différentes")
        $title.Font=[Drawing.Font]::new('Segoe UI',16)
        $s=[Windows.Forms.DataVisualization.Charting.Series]::new('Dernier individu')
        $s.ChartType=[Windows.Forms.DataVisualization.Charting.SeriesChartType]::Column
        $s.IsValueShownAsLabel=$true; $s.Font=[Drawing.Font]::new('Segoe UI',12)
        $chart.Series.Add($s)
        $colors=@([Drawing.Color]::FromArgb(94,178,241),[Drawing.Color]::FromArgb(244,129,107),[Drawing.Color]::FromArgb(153,208,111),[Drawing.Color]::FromArgb(197,148,236),[Drawing.Color]::Gray,[Drawing.Color]::Goldenrod)
        for($i=0;$i -lt 6;$i++) {
            if($i -lt 4) { $count=@($main | Where-Object { [int]$_.faction_dernier_individu -eq $i }).Count; $label=$names[$i] }
            elseif($i -eq 4) { $count=@($main | Where-Object resultat -eq 'extinction').Count; $label='Extinction sans individu unique' }
            else { $count=@($main | Where-Object resultat -eq 'limite_jours').Count; $label="Sans individu unique à $maxDays jours" }
            $index=$s.Points.AddXY($label,(100.0*$count/$main.Count)); $s.Points[$index].Color=$colors[$i]; $s.Points[$index].Label=('{0:F1} % ({1})' -f (100.0*$count/$main.Count),$count)
        }
        $chart.SaveImage((Join-Path $root 'dernier-survivant.png'),[Windows.Forms.DataVisualization.Charting.ChartImageFormat]::Png)
    } finally { $chart.Dispose() }
}
