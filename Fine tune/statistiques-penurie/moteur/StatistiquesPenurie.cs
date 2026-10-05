namespace SelectionNaturelle
{
    using System;
    using System.IO;
    using System.Text;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    public class ScarcityResult
    {
        public int Seed,Days,Steps,LastMask,LastCount,Individual=-1,SoleFaction=-1,SoleFactionDay,Raids,CoalitionStrikes;
        public string Outcome; public int[] FinalCounts=new int[4]; public double Speed,Size,Sense; public int SkinA=-1,SkinB=-1,Sex=-1,Job=-1;
        public string Csv()
        {
            return string.Format(CultureInfo.InvariantCulture,"{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15:F6},{16:F6},{17:F6},{18},{19},{20},{21}",Seed,Outcome,Days,Steps,Individual,SoleFaction,SoleFactionDay,LastMask,LastCount,FinalCounts[0],FinalCounts[1],FinalCounts[2],FinalCounts[3],Raids,CoalitionStrikes,Speed,Size,Sense,SkinA,SkinB,Sex,Job);
        }
    }
    public static class ScarcityBatch
    {
        static Rules Setup(int seed,string scenario)
        {
            Rules r=Rules.Preset(4); r.Seed=seed;
            if(scenario!="actuel") r.InitialPopulation=24;
            if(scenario=="neutre") r.EcosystemEnabled=false;
            if(scenario=="sans_raids") { r.RaidChance=0; r.AlliancesEnabled=false; }
            return r;
        }
        static ScarcityResult One(int seed,string scenario,int maxDays)
        {
            World w=new World(Setup(seed,scenario));
            if(scenario=="rotation") foreach(Blob b in w.Blobs) { if(b.FactionId==0) b.FactionId=2; else if(b.FactionId==2) b.FactionId=3; else if(b.FactionId==3) b.FactionId=0; }
            ScarcityResult result=new ScarcityResult { Seed=seed };
            for(;;)
            {
                int count=0,mask=0; Blob last=null; int[] counts=new int[4];
                foreach(Blob b in w.Blobs) if(!b.Dead) { count++; mask|=1<<b.FactionId; counts[b.FactionId]++; last=b; }
                result.FinalCounts=counts; result.Days=w.Day; result.Raids=w.Raids; result.CoalitionStrikes=w.CoalitionStrikes;
                if(count==0) { result.Outcome="extinction"; break; }
                result.LastCount=count; result.LastMask=mask;
                if(result.SoleFaction<0 && (mask&(mask-1))==0) { result.SoleFaction=last.FactionId; result.SoleFactionDay=w.Day; }
                if(count==1)
                {
                    result.Outcome="dernier_individu"; result.Individual=last.FactionId;
                    result.Speed=last.Speed; result.Size=last.Size; result.Sense=last.Sense; result.SkinA=last.Genes.SkinA; result.SkinB=last.Genes.SkinB; result.Sex=(int)last.Sex; result.Job=(int)last.Job; break;
                }
                if(w.DayFinished && w.Day>=maxDays) { result.Outcome="limite_jours"; break; }
                if(w.DayFinished) w.History.Clear();
                w.Step(); result.Steps++;
            }
            return result;
        }
        public static void Run(string directory,string scenario,int runs,int maxDays,int startSeed,int workers)
        {
            if(scenario!="actuel" && scenario!="equilibre" && scenario!="rotation" && scenario!="neutre" && scenario!="sans_raids") throw new ArgumentException("Scénario inconnu");
            Directory.CreateDirectory(directory); Setup(startSeed,scenario).Save(Path.Combine(directory,"regles-"+scenario+".xml"));
            ScarcityResult[] results=new ScarcityResult[runs]; int completed=0;
            Parallel.For(0,runs,new ParallelOptions { MaxDegreeOfParallelism=workers },delegate(int i)
            {
                results[i]=One(startSeed+i,scenario,maxDays); int n=Interlocked.Increment(ref completed);
                if(n%25==0 || n==runs) Console.WriteLine(scenario+" : "+n+" / "+runs);
            });
            StringBuilder csv=new StringBuilder("graine,resultat,jour,pas,faction_dernier_individu,premiere_faction_seule,jour_faction_seule,masque_dernier_groupe,effectif_dernier_groupe,bleus_restants,magas_restants,verts_restants,violets_restants,raids,attaques_coalition,vitesse,taille,perception,peau_a,peau_b,sexe,metier\r\n");
            foreach(ScarcityResult r in results) csv.AppendLine(r.Csv());
            File.WriteAllText(Path.Combine(directory,scenario+".csv"),csv.ToString(),new UTF8Encoding(true));
            int[] individuals=new int[4],factions=new int[4]; int extinctions=0,censored=0;
            foreach(ScarcityResult r in results) { if(r.Individual>=0) individuals[r.Individual]++; if(r.SoleFaction>=0) factions[r.SoleFaction]++; if(r.Outcome=="extinction") extinctions++; if(r.Outcome=="limite_jours") censored++; }
            Console.WriteLine(scenario+" individus="+string.Join("/",Array.ConvertAll(individuals,delegate(int n) { return n.ToString(); }))+" factions="+string.Join("/",Array.ConvertAll(factions,delegate(int n) { return n.ToString(); }))+" extinctions="+extinctions+" limite="+censored);
        }
    }
}
