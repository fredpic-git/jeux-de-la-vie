namespace SelectionNaturelle
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    public partial class Rules
    {
        [Category("Diplomatie"),DisplayName("Deux blocs permanents"),Description("Deux alliances de deux factions tirées au hasard, confiance fixée à +100 entre alliés et −100 entre blocs. Nécessite les alliances actives.")]
        public bool PermanentBlocs { get; set; }
        bool economicDiplomacy=true;
        [Category("Diplomatie"),DisplayName("Réaction à la pénurie et aux agressions"),Description("Les factions en manque de nourriture se rapprochent contre une faction qui accumule davantage de ressources. Les raids injustifiés donnent une réputation d'agression qui bloque les alliances avec leur auteur.")]
        public bool EconomicDiplomacy { get { return economicDiplomacy; } set { economicDiplomacy=value; } }
        bool alliancesEnabled=true,diplomaticContactsEnabled=true; int allianceTrust=20,allianceBreak=5,raidHostility=-20,raidSquadSize=2;
        [Category("Diplomatie"),DisplayName("Rencontres pacifiques diplomatiques"),Description("Une rencontre pacifique entre deux factions ajoute 2 de confiance, au maximum une fois par paire de factions et par jour. Fonctionne sans MAGA.")]
        public bool DiplomaticContactsEnabled { get { return diplomaticContactsEnabled; } set { diplomaticContactsEnabled=value; } }
        [Category("Diplomatie"),DisplayName("Activer les alliances")] public bool AlliancesEnabled { get { return alliancesEnabled; } set { alliancesEnabled=value; } }
        [Category("Diplomatie"),DisplayName("Confiance pour former une alliance")] public int AllianceTrust { get { return allianceTrust; } set { allianceTrust=value; } }
        [Category("Diplomatie"),DisplayName("Rupture si confiance inférieure à")] public int AllianceBreak { get { return allianceBreak; } set { allianceBreak=value; } }
        [Category("Diplomatie"),DisplayName("Hostilité pour un raid commun")] public int RaidHostility { get { return raidHostility; } set { raidHostility=value; } }
        [Category("Diplomatie"),DisplayName("Combattants par faction alliée")] public int RaidSquadSize { get { return raidSquadSize; } set { raidSquadSize=value; } }
        void ValidateDiplomacy() { if(AllianceTrust<1 || AllianceTrust>100 || AllianceBreak<-100 || AllianceBreak>=AllianceTrust || RaidHostility<-100 || RaidHostility>=AllianceBreak || RaidSquadSize<1 || RaidSquadSize>5) throw new ArgumentException("Diplomatie : formation 1–100, rupture < formation, hostilité < rupture (minimum −100), combattants 1–5."); }
    }
    public class RaidPlan
    {
        public int Target,VictimId,LastAttack=-60,Strikes; public double X,Y; public bool Castle; public List<int> Members=new List<int>(); public List<Blob> Fighters=new List<Blob>();
    }
    public partial class World
    {
        public bool[,] Alliances=new bool[4,4]; public List<RaidPlan> RaidPlans=new List<RaidPlan>(); public int CoalitionStrikes;
        int[,] contactDays=new int[4,4];
        public int[] Aggression=new int[4];
        int[] diplomaticBlocs;
        void ResetDiplomacy() { Aggression=new int[4]; diplomaticBlocs=null; Alliances=new bool[4,4]; contactDays=new int[4,4]; RaidPlans.Clear(); CoalitionStrikes=0; }
        void ApplyPermanentBlocs() {
            if(!Rules.PermanentBlocs || !Rules.EcosystemEnabled || !Rules.AlliancesEnabled) return;
            if(diplomaticBlocs==null) {
                int partner=random.Next(1,4); diplomaticBlocs=new int[] { 0,1,1,1 }; diplomaticBlocs[partner]=0;
                List<string> first=new List<string>(),second=new List<string>();
                for(int i=0;i<4;i++) (diplomaticBlocs[i]==0?first:second).Add(Camps[i].Nom);
                Log("Blocs permanents : "+string.Join(" + ",first.ToArray())+" contre "+string.Join(" + ",second.ToArray()));
            }
            for(int a=0;a<4;a++) for(int b=a+1;b<4;b++) Relations[a,b]=Relations[b,a]=diplomaticBlocs[a]==diplomaticBlocs[b]?100:-100;
        }
        internal bool PeacefulContact(Blob a,Blob b)
        {
            if(!Rules.EcosystemEnabled || !Rules.AlliancesEnabled || !Rules.DiplomaticContactsEnabled || a.FactionId==b.FactionId || a.Dead || b.Dead || a.Home || b.Home || Distance(a.X,a.Y,b.X,b.Y)>12 || Relations[a.FactionId,b.FactionId]<=Rules.RaidHostility || CanHunt(a,b) || CanHunt(b,a)) return false;
            int f=a.FactionId,z=b.FactionId;
            if(contactDays[f,z]==Day) return false;
            contactDays[f,z]=contactDays[z,f]=Day; ChangeTrust(f,z,2); UpdateDiplomacy(); return true;
        }
        void DiplomacyContacts(Blob b)
        {
            if(!Rules.DiplomaticContactsEnabled || !Rules.AlliancesEnabled || !Rules.EcosystemEnabled || b.Dead || b.Home) return;
            bool possible=false; for(int f=0;f<4;f++) if(f!=b.FactionId && contactDays[b.FactionId,f]!=Day && Relations[b.FactionId,f]>Rules.RaidHostility) { possible=true; break; } if(!possible) return;
            if(!UseSpatialIndex || Blobs.Count<100) { foreach(Blob other in interactionOrder) if(other.FactionId!=b.FactionId && contactDays[b.FactionId,other.FactionId]!=Day) PeacefulContact(b,other); return; }
            for(int x=CellX(b.X-12);x<=CellX(b.X+12);x++) for(int y=CellY(b.Y-12);y<=CellY(b.Y+12);y++) {
                List<Blob> cell; if(!blobCells.TryGetValue(x+GridColumns*y,out cell)) continue;
                foreach(Blob other in cell) if(other.FactionId!=b.FactionId && contactDays[b.FactionId,other.FactionId]!=Day) PeacefulContact(b,other);
            }
        }
        public void ChangeTrust(int a,int b,int delta) { if(a==b) return; if(Rules.PermanentBlocs && Rules.EcosystemEnabled && Rules.AlliancesEnabled) { ApplyPermanentBlocs(); return; } Relations[a,b]=Relations[b,a]=Math.Max(-100,Math.Min(100,Relations[a,b]+delta)); }
        public bool Allied(int a,int b) { return Rules.EcosystemEnabled && Rules.AlliancesEnabled && Alliances[a,b]; }
        internal bool CanHunt(Blob hunter,Blob prey) { return (!Rules.EcosystemEnabled || hunter.FactionId!=prey.FactionId && !Allied(hunter.FactionId,prey.FactionId)) && CanEat(hunter,prey,Rules); }
        public void UpdateDiplomacy()
        {
            ApplyPermanentBlocs();
            if(Rules.EconomicDiplomacy && !Rules.PermanentBlocs && Rules.EcosystemEnabled && Rules.AlliancesEnabled) for(int a=0;a<4;a++) if(Aggression[a]>=6) for(int b=0;b<4;b++) if(a!=b) {
                bool sameExpedition=false; foreach(RaidPlan plan in RaidPlans) if(plan.Members.Contains(a) && plan.Members.Contains(b)) { sameExpedition=true; break; }
                if(!sameExpedition) Relations[a,b]=Relations[b,a]=Math.Min(Relations[a,b],Rules.RaidHostility);
            }
            bool[] alive=new bool[4]; foreach(Blob b in Blobs) if(!b.Dead) alive[b.FactionId]=true;
            for(int a=0;a<4;a++) for(int b=a+1;b<4;b++)
            {
                bool previous=Alliances[a,b]; bool next=Rules.EcosystemEnabled && Rules.AlliancesEnabled && alive[a] && alive[b] && Relations[a,b]>=(previous?Rules.AllianceBreak:Rules.AllianceTrust);
                Alliances[a,b]=Alliances[b,a]=next;
                if(next!=previous) Log((next?"Alliance : ":"Alliance rompue : ")+Camps[a].Nom+" + "+Camps[b].Nom);
            }
        }
        void CommonThreat(int aggressor,int victim,bool raid=false)
        {
            bool offensive=Rules.EconomicDiplomacy && raid && Aggression[victim]<6;
            if(offensive) Aggression[aggressor]=Math.Min(100,Aggression[aggressor]+3);
            ChangeTrust(aggressor,victim,-6);
            for(int f=0;f<4;f++) if(f!=aggressor && f!=victim)
            {
                ChangeTrust(aggressor,f,-2);
                if(Relations[f,aggressor]<=Rules.RaidHostility) ChangeTrust(f,victim,offensive?6:3);
            }
            UpdateDiplomacy();
        }
        void EconomicDiplomacyNight(List<Blob> survivors) {
            if(!Rules.EcosystemEnabled || !Rules.AlliancesEnabled || !Rules.EconomicDiplomacy || Rules.PermanentBlocs) return;
            for(int f=0;f<4;f++) Aggression[f]=Math.Max(0,Aggression[f]-1);
            int[] population=new int[4]; foreach(Blob b in survivors) population[b.FactionId]++;
            double[] wealth=new double[4]; bool[] scarce=new bool[4];
            for(int f=0;f<4;f++) if(population[f]>0) {
                Camp c=Camps[f]; wealth[f]=(5.0*c.Stock[0]+.25*c.Stock[1]+.5*c.Stock[2]+c.Stock[3]+c.Stock[4]+.25*c.Stock[5]+2*c.Stock[6]+4*c.Stock[7])/population[f];
                scarce[f]=c.Stock[0]+FoodToday/4.0<population[f]*Rules.SurvivalFood*.8;
            }
            for(int target=0;target<4;target++) if(population[target]>0) {
                List<int> deprived=new List<int>();
                for(int f=0;f<4;f++) if(f!=target && population[f]>0 && scarce[f] && wealth[target]>Math.Max(1,wealth[f])*1.35) deprived.Add(f);
                if(deprived.Count<2) continue;
                foreach(int f in deprived) { Relations[f,target]=Relations[target,f]=Math.Min(0,Relations[f,target]); ChangeTrust(f,target,-8); }
                foreach(int f in deprived) foreach(int z in deprived) if(f<z) ChangeTrust(f,z,4);
                Log("Tensions de pénurie : "+Camps[target].Nom+" concentre les ressources ; rapprochement des factions en manque.");
            }
            UpdateDiplomacy();
        }
        public void PlanCoalitionRaids()
        {
            foreach(Blob b in Blobs) b.RaidTarget=-1;
            RaidPlans.Clear(); UpdateDiplomacy(); if(!Rules.EcosystemEnabled || !Rules.AlliancesEnabled) return;
            bool[] assigned=new bool[4];
            for(int target=0;target<4;target++)
            {
                Blob victim=null; foreach(Blob b in Blobs) if(b.FactionId==target && !b.Dead) { victim=b; break; }
                if(victim==null) continue;
                List<int> members=new List<int>();
                for(int mask=1;mask<16;mask++)
                {
                    List<int> candidate=new List<int>(); bool compatible=true;
                    for(int a=0;a<4;a++) if((mask&(1<<a))!=0)
                    {
                        if(a==target || assigned[a] || Allied(a,target) || Relations[a,target]>Rules.RaidHostility) compatible=false;
                        foreach(int ally in candidate) if(!Allied(a,ally)) compatible=false;
                        candidate.Add(a);
                    }
                    if(compatible && candidate.Count>members.Count) members=candidate;
                }
                if(members.Count<2) continue;
                RaidPlan plan=new RaidPlan { Target=target,VictimId=victim.Id,X=UseCastles?Camps[target].X:victim.X,Y=UseCastles?Camps[target].Y:victim.Y,Castle=UseCastles };
                foreach(int a in members)
                {
                    int n=0; foreach(Blob b in Blobs) if(b.FactionId==a && !b.Dead && !b.IsLeader && b.GuardLeaderId==0 && b.Food>=Rules.SurvivalFood && HasWater(b,Rules) && n<Rules.RaidSquadSize) { plan.Fighters.Add(b); n++; }
                    if(n>0) plan.Members.Add(a);
                }
                if(plan.Members.Count<2) continue;
                if(UseCastles) foreach(Blob fighter in plan.Fighters) fighter.RaidTarget=target;
                foreach(int a in plan.Members) assigned[a]=true;
                RaidPlans.Add(plan); Log("Raid organisé : "+MemberNames(plan)+" contre "+Camps[target].Nom);
            }
        }
        public string MemberNames(RaidPlan plan) { List<string> names=new List<string>(); foreach(int a in plan.Members) names.Add(Camps[a].Nom); return string.Join(" + ",names.ToArray()); }
        public string RaidStatus(RaidPlan plan) { return !ValidPlan(plan)?"annulé":DayFinished || Tick>=Rules.DayLength*.65?"terminé":"rassemblement / attaque"; }
        bool ValidPlan(RaidPlan p)
        {
            foreach(int a in p.Members) { if(Allied(a,p.Target) || Relations[a,p.Target]>Rules.RaidHostility) return false; foreach(int b in p.Members) if(a!=b && !Allied(a,b)) return false; }
            return Rules.EcosystemEnabled && Rules.AlliancesEnabled;
        }
        double ReturnSafety(Blob b) { return UseCastles && b.RaidTarget>=0?Math.Min(1.15,Rules.ReturnMargin):Rules.ReturnMargin; }
        bool ReadyRaider(Blob b) { return !b.Dead && !b.Home && !b.Returning && b.Mate==null && b.Food>=Rules.SurvivalFood && HasWater(b,Rules) && Tick<Rules.DayLength*.65 && b.Energy>Cost(b,Rules)*MovementMultiplier(b)*(HomeDistance(b)/Math.Max(.2,b.Speed)*MovementMultiplier(b)*ReturnSafety(b)+25); }
        void PlanMagaRaids()
        {
            if(!UseCastles || !Rules.TrumpFactionEnabled || Rules.RaidChance<=0) return;
            List<Blob> candidates=new List<Blob>(); foreach(Blob b in Blobs) if(b.FactionId==1 && !b.IsLeader && b.GuardLeaderId==0 && b.RaidTarget<0 && (b.Job==Profession.Mineur || b.Job==Profession.Transporteur) && b.Food>=Rules.SurvivalFood && HasWater(b,Rules)) candidates.Add(b);
            candidates.Sort(delegate(Blob a,Blob b) { int weapons=b.Weapon.CompareTo(a.Weapon); return weapons!=0?weapons:(Cost(a,Rules)/a.Speed).CompareTo(Cost(b,Rules)/b.Speed); });
            int assigned=0; foreach(Blob b in candidates) {
                int target=CastleTarget(b); if(target<0) continue;
                double travel=Math.Max(0,Distance(b.X,b.Y,Camps[target].X,Camps[target].Y)-CastleRadius-8)/Math.Max(.2,b.Speed);
                if(travel+45>=Rules.DayLength*.65 || b.Energy<Cost(b,Rules)*(travel*2.4+30)) continue;
                b.RaidTarget=target; assigned++; if(assigned>=Rules.RaidSquadSize) break;
            }
            if(assigned>0) Log("Expédition MAGA : "+assigned+" combattants affectés aux châteaux adverses.");
        }
        bool RaidDirection(Blob b,out double heading)
        {
            heading=0; if(!ReadyRaider(b)) return false;
            foreach(RaidPlan p in RaidPlans) if(p.Fighters.Contains(b) && ValidPlan(p))
            {
                if(p.Castle) { heading=Math.Atan2(Camps[p.Target].Y-b.Y,Camps[p.Target].X-b.X); return true; }
                Blob victim=null; double nearest=double.MaxValue;
                foreach(Blob v in Blobs) if(!v.Dead && !v.Home && v.FactionId==p.Target) { double dist=Distance(v.X,v.Y,p.X,p.Y); if(dist<nearest) { nearest=dist; victim=v; } }
                if(victim==null) return false;
                p.VictimId=victim.Id; p.X=victim.X; p.Y=victim.Y;
                heading=Math.Atan2(p.Y-b.Y,p.X-b.X); return true;
            }
            return false;
        }
        public bool TryCoalitionStrike(RaidPlan p)
        {
            if(!ValidPlan(p) || Tick-p.LastAttack<45) return false;
            if(p.Castle) {
                List<Blob> fighters=new List<Blob>(); HashSet<int> participating=new HashSet<int>();
                foreach(Blob b in p.Fighters) if(ReadyRaider(b) && Distance(b.X,b.Y,Camps[p.Target].X,Camps[p.Target].Y)<=CastleRadius+8) { fighters.Add(b); participating.Add(b.FactionId); }
                if(participating.Count<2) return false;
                p.LastAttack=Tick; StrikeCastle(p.Target,fighters,8); Raids++; CoalitionStrikes++; p.Strikes++;
                foreach(int a in participating) CommonThreat(a,p.Target,true);
                foreach(int a in participating) foreach(int z in participating) if(a<z) ChangeTrust(a,z,2);
                Log("Assaut du château : "+MemberNames(p)+" → "+Camps[p.Target].Nom); return true;
            }
            Blob victim=null; foreach(Blob v in Blobs) if(v.Id==p.VictimId && v.FactionId==p.Target && !v.Dead && !v.Home) victim=v;
            if(victim==null) return false;
            List<Blob> nearby=new List<Blob>(); HashSet<int> factions=new HashSet<int>();
            foreach(Blob b in p.Fighters) if(ReadyRaider(b) && Distance(b.X,b.Y,victim.X,victim.Y)<12) { nearby.Add(b); factions.Add(b.FactionId); }
            if(factions.Count<2) return false;
            p.LastAttack=Tick;
            if(ProtectedLeader(victim,nearby[0])) { Log("Les gardes ont repoussé le raid contre le chef."); return false; }
            double damage=0; foreach(Blob b in nearby) { damage+=RaidDamage(b,Rules,8); b.Energy=Math.Max(0,b.Energy-6); if(victim.Food>0) { victim.Food--; b.Food++; } }
            victim.Energy=Math.Max(0,victim.Energy-damage); if(victim.Energy==0) victim.Dead=true;
            Raids++; CoalitionStrikes++; p.Strikes++;
            foreach(int a in factions) CommonThreat(a,p.Target,true);
            foreach(int a in factions) foreach(int z in factions) if(a<z) ChangeTrust(a,z,2);
            Log("Raid exécuté : "+MemberNames(p)+" → "+Camps[p.Target].Nom+" (#"+victim.Id+")"); return true;
        }
        void CoalitionAction(Blob b) { foreach(RaidPlan p in RaidPlans) if(p.Fighters.Contains(b)) TryCoalitionStrike(p); }
        int CastleTarget(Blob raider)
        {
            int target=-1; double nearest=double.MaxValue;
            for(int f=0;f<4;f++) if(f!=raider.FactionId && !Allied(raider.FactionId,f)) {
                int stock=0; foreach(int n in Camps[f].Stock) stock+=n; if(stock==0) continue;
                double distance=Distance(raider.X,raider.Y,Camps[f].X,Camps[f].Y); if(distance<nearest) { nearest=distance; target=f; }
            }
            return target;
        }
        bool MagaCastleDirection(Blob b,out double heading)
        {
            heading=0; if(!Rules.TrumpFactionEnabled || b.FactionId!=1 || b.IsLeader || b.GuardLeaderId!=0 || Rules.RaidChance<=0 || b.RaidTarget<0 || Allied(b.FactionId,b.RaidTarget) || !ReadyRaider(b)) return false;
            heading=Math.Atan2(Camps[b.RaidTarget].Y-b.Y,Camps[b.RaidTarget].X-b.X); return true;
        }
        internal void StrikeCastle(int target,List<Blob> fighters,double baseDamage)
        {
            Camp castle=Camps[target]; int defenders=0; foreach(Blob b in Blobs) if(!b.Dead && b.FactionId==target && Distance(b.X,b.Y,castle.X,castle.Y)<=CastleRadius+8) defenders++;
            double damage=0; foreach(Blob b in fighters) { damage+=RaidDamage(b,Rules,baseDamage); b.Energy=Math.Max(0,b.Energy-6); }
            castle.Fortification=Math.Max(0,castle.Fortification-damage/(1+Math.Min(10,defenders)*.2));
            if(castle.Fortification>0) return;
            foreach(Blob b in fighters) {
                int carried=0; foreach(int n in b.Bag) carried+=n;
                int slots=Math.Max(0,BagCapacity(b,Rules)-carried);
                for(int i=0;i<slots;i++) {
                    int kind=-1; for(int k=0;k<8;k++) if(castle.Stock[k]>0 && (kind<0 || castle.Stock[k]>castle.Stock[kind])) kind=k;
                    if(kind<0) break; castle.Stock[kind]--; b.Bag[kind]++;
                }
                int load=0; foreach(int n in b.Bag) load+=n; if(load>=BagCapacity(b,Rules)) b.Returning=true;
            }
        }
    }
    public static class DiplomacyTests
    {
        static void Assert(bool b,string s) { if(!b) throw new Exception(s); Console.WriteLine("OK : "+s); }
        static World Scenario()
        {
            World w=new World(new Rules(true) { RaidChance=0,Predation=false }); w.Blobs.Clear();
            for(int f=0;f<4;f++) w.Blobs.Add(new Blob(1,1,1) { Id=100+f,FactionId=f,Food=2,Water=1,Energy=900,X=150+f,Y=150 });
            w.ChangeTrust(0,2,25); w.ChangeTrust(0,1,-35); w.ChangeTrust(2,1,-35); w.PlanCoalitionRaids(); return w;
        }
        public static void Run()
        {
            World polarized=new World(Rules.Preset(6)),repeat=new World(Rules.Preset(6)); int pairs=0;
            for(int a=0;a<4;a++) for(int b=a+1;b<4;b++) {
                int trust=polarized.Relations[a,b]; Assert(trust==100 || trust==-100,"blocs : relations initiales verrouillées");
                Assert(trust==repeat.Relations[a,b],"blocs : tirage reproductible avec la même graine");
                if(trust==100) { pairs++; Assert(polarized.Allied(a,b),"blocs : alliance immédiate"); }
                polarized.ChangeTrust(a,b,trust>0?-200:200); polarized.UpdateDiplomacy(); Assert(polarized.Relations[a,b]==trust,"blocs : échanges et attaques ne changent pas les liens");
            }
            Assert(pairs==2 && polarized.RaidPlans.Count>0,"deux alliances permanentes et raids de coalition organisés");
            polarized.Rules.PermanentBlocs=false; polarized.ChangeTrust(0,1,polarized.Relations[0,1]>0?-10:10); Assert(Math.Abs(polarized.Relations[0,1])==90,"désactivation : confiance évolutive restaurée");
            World neutral=new World(new Rules { Predation=false,CastlesEnabled=false }); neutral.Blobs.Clear();
            Blob friendlyA=new Blob(1,1,1) { Id=1,FactionId=0,X=150,Y=150,Food=2,Water=1,Energy=900 },friendlyB=new Blob(1,1,1) { Id=2,FactionId=2,X=151,Y=150,Food=2,Water=1,Energy=900 };
            neutral.Blobs.Add(friendlyA); neutral.Blobs.Add(friendlyB);
            Assert(neutral.PeacefulContact(friendlyA,friendlyB) && neutral.Relations[0,2]==2 && !neutral.PeacefulContact(friendlyB,friendlyA),"rencontre sans MAGA : confiance symétrique, une fois par paire et par jour");
            for(int day=1;day<10;day++) { friendlyA.Home=friendlyB.Home=true; friendlyA.Food=friendlyB.Food=1; neutral.FinishDay(); neutral.Step(); friendlyA.Home=friendlyB.Home=false; friendlyA.X=150; friendlyB.X=151; friendlyA.Y=friendlyB.Y=150; neutral.PeacefulContact(friendlyA,friendlyB); }
            Assert(neutral.Allied(0,2),"alliance formée sans MAGA après dix jours de rencontres pacifiques");
            World automatic=new World(new Rules { Predation=false }); automatic.Blobs.Clear();
            automatic.Blobs.Add(new Blob(1,1,1) { Id=1,FactionId=0,X=150,Y=150,Food=1,Water=1,Energy=900 }); automatic.Blobs.Add(new Blob(1,1,1) { Id=2,FactionId=2,X=151,Y=150,Food=1,Water=1,Energy=900 });
            automatic.Step(); Assert(automatic.Relations[0,2]==2,"rencontres diplomatiques exécutées dans un pas normal de simulation");
            automatic.Rules.AlliancesEnabled=false; automatic.Day++; Assert(!automatic.PeacefulContact(automatic.Blobs[0],automatic.Blobs[1]),"rencontres diplomatiques désactivées avec les alliances");
            World aggression=new World(new Rules { FoodPerDay=0 }); aggression.Blobs.Clear(); aggression.Foods.Clear();
            Blob large=new Blob(1,4,1) { Id=1,FactionId=0,X=150,Y=150,Energy=900,Water=1 },small=new Blob(1,1,1) { Id=2,FactionId=2,X=150,Y=150,Energy=900,Water=1 };
            aggression.Blobs.Add(large); aggression.Blobs.Add(small); aggression.Step();
            Assert(small.Dead && aggression.Predations==1 && aggression.Relations[0,2]==-6,"prédation ordinaire sans MAGA dégrade la confiance");
            World w=Scenario(); Assert(w.Allied(0,2) && w.Allied(2,0) && !w.Allied(0,1),"alliance symétrique fondée sur la confiance");
            Assert(w.RaidPlans.Count==1 && w.RaidPlans[0].Target==1 && w.RaidPlans[0].Members.Count==2,"raid commun organisé contre une cible hostile aux deux alliés");
            RaidPlan p=w.RaidPlans[0]; Blob victim=w.Blobs[1]; int food=victim.Food; double energy=victim.Energy;
            p.Fighters[1].X=250; Assert(!w.TryCoalitionStrike(p) && victim.Energy==energy,"pas de raid coordonné sans deux factions au contact");
            p.Fighters[1].X=152; Assert(w.TryCoalitionStrike(p) && victim.Energy<energy && victim.Food<food && w.CoalitionStrikes==1,"attaque commune avec dégâts et transfert réel de nourriture");
            Assert(!w.TryCoalitionStrike(p) && w.CoalitionStrikes==1,"délai entre deux attaques de la même coalition");
            World sized=Scenario(); RaidPlan sizedPlan=sized.RaidPlans[0];
            sizedPlan.Fighters[0].Size=2; sizedPlan.Fighters[0].Weapon=1;
            foreach(Blob b in sized.Blobs) b.Energy=10000;
            double before=sized.Blobs[1].Energy;
            Assert(sized.TryCoalitionStrike(sizedPlan) && Math.Abs(before-sized.Blobs[1].Energy-(8*2*1.6+8))<.000001,"raid commun : somme des dégâts selon la taille et les armes de chaque combattant");
            w.ChangeTrust(0,2,-18); w.UpdateDiplomacy(); Assert(w.Allied(0,2),"alliance maintenue sous le seuil de formation mais au-dessus du seuil de rupture");
            w.ChangeTrust(0,2,-20); w.UpdateDiplomacy(); w.Tick=60; Assert(!w.Allied(0,2) && !w.TryCoalitionStrike(p),"rupture de l'alliance annule les attaques du plan existant");
            w=Scenario(); Blob hunter=w.Blobs[0],ally=w.Blobs[2]; hunter.Size=4; w.Rules.Predation=true;
            Assert(!w.CanHunt(hunter,ally),"pas de prédation entre alliés");
            w.Rules.AlliancesEnabled=false; w.PlanCoalitionRaids(); Assert(w.RaidPlans.Count==0 && !w.Allied(0,2),"alliances et raids communs désactivables");
            w=Scenario(); w.Blobs[1].Home=true; Assert(!w.TryCoalitionStrike(w.RaidPlans[0]),"les raids respectent les abris");
            w=Scenario(); w.ChangeTrust(2,1,30); w.PlanCoalitionRaids(); Assert(w.RaidPlans.Count==0,"un seul allié hostile ne suffit pas à déclarer un raid commun");
            w=Scenario(); foreach(Blob b in w.Blobs) b.Energy=1; Assert(!w.TryCoalitionStrike(w.RaidPlans[0]),"la survie passe avant un raid sans énergie suffisante");
            w=Scenario(); foreach(Blob b in w.Blobs) b.Home=true; w.FinishDay(); w.Step();
            Assert(w.RaidPlans.Count==1 && w.RaidPlans[0].Fighters.Count>=2,"plans de raids créés automatiquement à l'aube dans la simulation");
            Rules r=new Rules(true) { AllianceBreak=30 }; bool rejected=false; try { r.Validate(); } catch(ArgumentException) { rejected=true; } Assert(rejected,"validation des seuils diplomatiques incohérents");
            string file=System.IO.Path.GetTempFileName(); try { r=new Rules(true) { AllianceTrust=35,RaidHostility=-40,DiplomaticContactsEnabled=true }; r.Save(file); r=Rules.Load(file); Assert(r.AllianceTrust==35 && r.RaidHostility==-40 && r.DiplomaticContactsEnabled,"réglages diplomatiques conservés en XML"); } finally { System.IO.File.Delete(file); }
            w=Scenario(); using(DiplomacyPanel panel=new DiplomacyPanel(w)) { panel.Size=new Size(1100,850); using(Bitmap bitmap=new Bitmap(1100,850)) { panel.DrawToBitmap(bitmap,panel.ClientRectangle); } }
        }
    }
    public class DiplomacyPanel : Control
    {
        readonly World world;
        public DiplomacyPanel(World w) { world=w; Dock=DockStyle.Fill; MinimumSize=new Size(900,850); DoubleBuffered=true; BackColor=Color.FromArgb(24,34,48); Font=new Font("Segoe UI",10); }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g=e.Graphics; g.Clear(BackColor); g.DrawString("CONFIANCE ET ALLIANCES — factions, indépendamment de la peau",Font,Brushes.White,20,15);
            int cw=Math.Max(135,(Width-170)/4);
            for(int a=0;a<4;a++) { g.DrawString(world.Camps[a].Nom,Font,Brushes.White,20,85+a*65); g.DrawString(world.Camps[a].Nom,Font,Brushes.White,160+a*cw,55);
                for(int b=0;b<4;b++) { Rectangle r=new Rectangle(150+b*cw,80+a*65,cw-8,55); Color c=a==b?Color.FromArgb(50,62,76):world.Allied(a,b)?Color.FromArgb(36,113,87):world.Relations[a,b]<0?Color.FromArgb(123,55,55):Color.FromArgb(58,77,101); using(Brush brush=new SolidBrush(c)) g.FillRectangle(brush,r); g.DrawString(a==b?"—":world.Relations[a,b]+(world.Allied(a,b)?"  ALLIÉS":""),Font,Brushes.White,r.X+10,r.Y+16); }
            }
            string rules="Diplomatie : "+(!world.Rules.EcosystemEnabled?"petit monde désactivé":!world.Rules.AlliancesEnabled?"alliances désactivées":"active, avec ou sans MAGA")+"\nFormation ≥ "+world.Rules.AllianceTrust+" · rupture < "+world.Rules.AllianceBreak+" · cible commune ≤ "+world.Rules.RaidHostility+"\nRencontres pacifiques : "+(world.Rules.DiplomaticContactsEnabled?"+2 par paire et par jour":"désactivées")+" ; commerce : +2 par transfert.\nPrédation et raids dégradent la confiance ; menace commune : rapprochement.\nRaids coordonnés exécutés : "+world.CoalitionStrikes;
            if(world.Rules.PermanentBlocs && world.Rules.EcosystemEnabled && world.Rules.AlliancesEnabled) rules="DEUX BLOCS PERMANENTS\nAlliés : +100, alliance indéfectible. Adversaires : −100, hostilité irréconciliable.\nLes rencontres, échanges et attaques ne modifient pas ces liens.\nRaids coordonnés exécutés : "+world.CoalitionStrikes;
            g.DrawString(rules,Font,Brushes.White,new RectangleF(20,355,Width-40,105));
            int y=470; g.DrawString("OPÉRATIONS DU JOUR",Font,Brushes.Gold,20,y); y+=28;
            if(world.RaidPlans.Count==0) { g.DrawString("Aucun raid commun organisé.",Font,Brushes.White,20,y); y+=25; }
            foreach(RaidPlan p in world.RaidPlans) { g.DrawString(world.MemberNames(p)+" → "+(p.Castle?"château ":"")+world.Camps[p.Target].Nom+" · "+p.Fighters.Count+" engagés · "+p.Strikes+" attaques · "+world.RaidStatus(p),Font,Brushes.White,20,y); y+=25; }
            y+=20; g.DrawString("JOURNAL DIPLOMATIQUE",Font,Brushes.Gold,20,y); y+=28;
            foreach(string s in world.Journal) { if(y>Height-25) break; g.DrawString(s,Font,Brushes.White,20,y); y+=23; }
        }
    }
}
