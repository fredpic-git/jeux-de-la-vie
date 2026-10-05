namespace SelectionNaturelle {
using System; using System.IO; using System.Text; using System.Globalization; using System.Threading; using System.Threading.Tasks;
public static class BalanceBatch {
    public static readonly string[] Names={"sans-mutations","vitesse","trois-traits","rarefaction","penurie","conflit-maga","grand-monde"};
    static string N(double n) { return n.ToString("0.######",CultureInfo.InvariantCulture); }
    static Rules Setup(int preset,bool baseline) {
        Rules r=Rules.Preset(preset);
        if(baseline) { r.EconomicDiplomacy=false; r.TransporterBagSlots=3; r.AgricultureWater=2; r.PrioritizeWater=false; r.ReproductionFood=2; r.SizePower=3; r.EnergySitesPerQuarter=3; r.EnergyPerSite=3; r.MaxReserves=8; r.CastleRepairsUseResources=false; r.CastleRepairPerNight=5; }
        return r;
    }
    public static void Run(string directory,int runs,int days,int seedStart,int workers,int first,int last,bool baseline=false) {
        Directory.CreateDirectory(directory);
        for(int preset=first;preset<=last;preset++) {
            int scenario=preset,complete=0; Setup(preset,baseline).Save(Path.Combine(directory,"regles-"+Names[preset]+".xml"));
            string[] summaries=new string[runs];
            using(StreamWriter daily=new StreamWriter(Path.Combine(directory,"jours-"+Names[preset]+".csv"),false,new UTF8Encoding(true))) {
                daily.WriteLine("graine,jour,faction,population,naissances_total,morts_total,predations_total,vitesse,taille,perception,armes_equipees,reserves,capacite,defenses_fin,defenses_min_jour,raids_cumul,coalitions_cumul,plans,confiance_0,confiance_1,confiance_2,confiance_3,allies,nourriture_aube"+ResourceColumns());
                object gate=new object();
                Parallel.For(0,runs,new ParallelOptions { MaxDegreeOfParallelism=workers },delegate(int index) {
                    Rules rules=Setup(scenario,baseline); rules.Seed=seedStart+index; World w=new World(rules);
                    int firstSole=-1,soleDay=0,raidDays=0,coalitionDays=0,planDays=0,damagedDays=0,zeroDays=0,peakArmed=0,peakPopulation=0,births=0,deaths=0,predations=0;
                    double minDefense=100; int[] finalCounts=new int[4]; int[] production=new int[8]; int priorRaids=0,priorCoalitions=0; StringBuilder rows=new StringBuilder();
                    for(int day=1;day<=days;day++) {
                        if(runs<=2 || index==0 && day%10==1) Console.WriteLine(Names[scenario]+" graine "+rules.Seed+" jour "+day+" population "+w.Blobs.Count);
                        double[] minimum={100,100,100,100}; bool ran=w.Blobs.Count>0;
                        if(ran) {
                            if(w.DayFinished) w.Step();
                            int planned=w.RaidPlans.Count; if(planned>0) planDays++;
                            while(!w.DayFinished && w.Blobs.Count>0) {
                                w.Step(); for(int f=0;f<4;f++) minimum[f]=Math.Min(minimum[f],w.Camps[f].Fortification);
                            }
                        } else for(int f=0;f<4;f++) minimum[f]=w.Camps[f].Fortification;
                        if(w.Raids>priorRaids) raidDays++; if(w.CoalitionStrikes>priorCoalitions) coalitionDays++; priorRaids=w.Raids; priorCoalitions=w.CoalitionStrikes;
                        DayRecord record=ran && w.History.Count>0?w.History[w.History.Count-1]:null;
                        if(record!=null) { births+=record.Births; deaths+=record.Deaths; predations+=record.Predations; }
                        int[] counts=new int[4],armed=new int[4]; double[] speed=new double[4],size=new double[4],sense=new double[4]; int[,] bags=new int[4,8];
                        foreach(Blob b in w.Blobs) if(!b.Dead) { int f=b.FactionId; counts[f]++; if(b.Weapon>0) armed[f]++; speed[f]+=b.Speed; size[f]+=b.Size; sense[f]+=b.Sense; for(int k=0;k<8;k++) bags[f,k]+=b.Bag[k]; }
                        int mask=0,total=0,totalArmed=0; bool damaged=false,zero=false;
                        for(int f=0;f<4;f++) { if(counts[f]>0) mask|=1<<f; total+=counts[f]; totalArmed+=armed[f]; minDefense=Math.Min(minDefense,minimum[f]); if(minimum[f]<99.999) damaged=true; if(minimum[f]<.001) zero=true; }
                        if(damaged) damagedDays++; if(zero) zeroDays++; peakPopulation=Math.Max(peakPopulation,total); peakArmed=Math.Max(peakArmed,totalArmed);
                        if(firstSole<0 && mask!=0 && (mask&(mask-1))==0) { for(int f=0;f<4;f++) if(counts[f]>0) firstSole=f; soleDay=day; }
                        int[] map=w.MapResourceQuantities();
                        for(int f=0;f<4;f++) {
                            Camp c=w.Camps[f]; int allies=0; for(int z=0;z<4;z++) if(z!=f && w.Allied(f,z)) allies++;
                            rows.Append(rules.Seed+","+day+","+f+","+counts[f]+","+(record==null?0:record.Births)+","+(record==null?0:record.Deaths)+","+(record==null?0:record.Predations)+","+N(counts[f]==0?0:speed[f]/counts[f])+","+N(counts[f]==0?0:size[f]/counts[f])+","+N(counts[f]==0?0:sense[f]/counts[f])+","+armed[f]+","+c.Reserves+","+c.Capacite+","+N(c.Fortification)+","+N(minimum[f])+","+w.Raids+","+w.CoalitionStrikes+","+(ran?w.RaidPlans.Count:0)+","+w.Relations[f,0]+","+w.Relations[f,1]+","+w.Relations[f,2]+","+w.Relations[f,3]+","+allies+","+w.FoodToday);
                            for(int k=0;k<8;k++) { int made=ran?c.ProducedToday[k]:0; production[k]+=made; rows.Append(","+map[k]+","+c.Stock[k]+","+made+","+(ran?c.ConsumedToday[k]:0)+","+bags[f,k]); }
                            rows.AppendLine();
                        }
                        finalCounts=counts; w.History.Clear(); w.ResourceHistory.Clear();
                    }
                    int finalMask=0,finalPopulation=0,winner=-1; for(int f=0;f<4;f++) { finalPopulation+=finalCounts[f]; if(finalCounts[f]>0) { finalMask|=1<<f; winner=f; } }
                    string outcome=finalMask==0?"extinction":(finalMask&(finalMask-1))==0?"faction_seule":"coexistence_limite"; if(outcome!="faction_seule") winner=-1;
                    summaries[index]=rules.Seed+","+outcome+","+winner+","+firstSole+","+soleDay+","+finalPopulation+","+string.Join(",",Array.ConvertAll(finalCounts,delegate(int n) {return n.ToString();}))+","+peakPopulation+","+births+","+deaths+","+predations+","+w.Raids+","+w.CoalitionStrikes+","+raidDays+","+coalitionDays+","+planDays+","+damagedDays+","+zeroDays+","+N(minDefense)+","+peakArmed+","+production[7]+","+production[6]+","+production[5]+","+production[0];
                    lock(gate) { daily.Write(rows.ToString()); daily.Flush(); File.AppendAllText(Path.Combine(directory,"terminees-"+Names[scenario]+".csv"),summaries[index]+"\r\n",new UTF8Encoding(true)); }
                    int done=Interlocked.Increment(ref complete); if(done%25==0 || done==runs) Console.WriteLine(Names[scenario]+" : "+done+"/"+runs);
                });
            }
            File.WriteAllText(Path.Combine(directory,"runs-"+Names[preset]+".csv"),"graine,resultat,gagnant,premiere_faction_seule,jour_faction_seule,population_finale,bleus,corail,verts,violets,population_max,naissances,morts,predations,raids,coalitions,jours_raids,jours_coalitions,jours_plans,jours_chateaux_endommages,jours_chateaux_zero,defenses_min,armes_equipees_max,armes_produites,fer_produit,energie_produite,nourriture_produite\r\n"+string.Join("\r\n",summaries)+"\r\n",new UTF8Encoding(true));
        }
    }
    static string ResourceColumns() { StringBuilder s=new StringBuilder(); string[] names={"nourriture","eau","terre","minerai","charbon","energie","fer","armes"}; foreach(string n in names) s.Append(",carte_"+n+",stock_"+n+",production_"+n+",consommation_"+n+",sac_"+n); return s.ToString(); }
}
}
