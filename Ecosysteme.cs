namespace SelectionNaturelle
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    public enum Profession { Agriculteur, Mineur, Artisan, Transporteur }
    public enum Ressource { Nourriture, Eau, Terre, Minerai, Charbon, Energie, Fer, Armes }
    public class Camp
    {
        public int[] ProducedToday=new int[8],ConsumedToday=new int[8],LastProduced=new int[8],LastConsumed=new int[8];
        public double X,Y,Fortification=100;
        public double LowestFortification=100; public int Assaults;
        public string Nom; public int[] Stock=new int[8]; public int Capacite=40, Reserves;
        public bool Consume(int food,int water,int soil,int ore,int coal,int energy,int iron)
        {
            int[] needs={food,water,soil,ore,coal,energy,iron,0};
            for(int i=0;i<8;i++) if(Stock[i]<needs[i]) return false;
            for(int i=0;i<8;i++) { Stock[i]-=needs[i]; ConsumedToday[i]+=needs[i]; } return true;
        }
        public void Add(int kind,int n) { Stock[kind]=Math.Min(Capacite,Stock[kind]+n); }
    }
    public class Depot { public double X,Y; public int Kind,Quantite; }
    public class ResourceSample { public long Step; public int Day,Tick; public int[] Quantities; }
    public class VisualEvent { public long Id; public double X,Y; public Blob Actor,Partner; public int Kind,Resource=-1,Amount=1; }
    public static class ResourceDisplay
    {
        public static readonly string[] Names={"Nourriture","Eau","Terre","Minerai","Charbon","Énergie","Fer","Armes"};
        public static Color ColorFor(int kind) { return kind==0?Color.Goldenrod:kind==1?Color.DeepSkyBlue:kind==2?Color.Peru:kind==3 || kind==6?Color.Silver:kind==4?Color.FromArgb(145,130,150):kind==7?Color.IndianRed:Color.Gold; }
    }
    public partial class Rules
    {
        int mapColumns=30,mapRows=30; bool castlesEnabled=true;
        [Category("Terrain"),DisplayName("Colonnes (10–80)"),Description("Une case mesure 10 unités. Maximum 3 600 cases au total. Changer les dimensions demande une nouvelle expérience.")]
        public int MapColumns { get { return mapColumns; } set { mapColumns=value; } }
        [Category("Terrain"),DisplayName("Lignes (10–80)")]
        public int MapRows { get { return mapRows; } set { mapRows=value; } }
        [Category("Monde et ressources"),DisplayName("Châteaux de faction"),Description("Chaque faction dort dans son château, qui contient ses stocks. Les raids visent ces stocks.")]
        public bool CastlesEnabled { get { return castlesEnabled; } set { castlesEnabled=value; } }
        [Category("Monde et ressources"),DisplayName("Tornades activées"),Description("Une tornade mobile par jour dans une zone tirée au hasard. Dégâts hors des abris et destruction de ressources dans un rayon de 12 unités.")]
        public bool TornadoesEnabled { get; set; }
        [Category("Monde et ressources"),DisplayName("Répartition inégale des ressources"),Description("Concentre environ 70 % de la nourriture dans une moitié de carte choisie à chaque aube et multiplie les gisements de cette moitié par 1,5, les autres par 0,5. Le relief ne change pas.")]
        public bool UnequalResources { get; set; }
        bool ecosystemEnabled=true,leaderEnabled=false,trumpFactionEnabled=false,balancedFounders=true,symmetricMap=true,scaleResources=true; int waterForLife=1; double mountainDifficulty=1.7,weaponBonus=.6,raidChance=0;
        int transporterBagSlots=8,agricultureWater=1;
        int energySitesPerQuarter=1,energyPerSite=2,maxReserves=4,castleRepairPerNight=3; bool castleRepairsUseResources=true;
        [Category("Monde et ressources"),DisplayName("Gisements d'énergie par quart de carte")]
        public int EnergySitesPerQuarter { get { return energySitesPerQuarter; } set { energySitesPerQuarter=value; } }
        [Category("Monde et ressources"),DisplayName("Énergie par gisement (avant adaptation)")]
        public int EnergyPerSite { get { return energyPerSite; } set { energyPerSite=value; } }
        [Category("Monde et ressources"),DisplayName("Maximum de réserves supplémentaires")]
        public int MaxReserves { get { return maxReserves; } set { maxReserves=value; } }
        [Category("Terrain"),DisplayName("Réparation du château par nuit (%)")]
        public int CastleRepairPerNight { get { return castleRepairPerNight; } set { castleRepairPerNight=value; } }
        [Category("Terrain"),DisplayName("Réparations consomment fer et énergie")]
        public bool CastleRepairsUseResources { get { return castleRepairsUseResources; } set { castleRepairsUseResources=value; } }
        bool prioritizeWater=true;
        [Category("Monde et ressources"),DisplayName("Priorité au transport d'eau")]
        public bool PrioritizeWater { get { return prioritizeWater; } set { prioritizeWater=value; } }
        [Category("Monde et ressources"),DisplayName("Places du sac des transporteurs")]
        public int TransporterBagSlots { get { return transporterBagSlots; } set { transporterBagSlots=value; } }
        [Category("Monde et ressources"),DisplayName("Eau par recette agricole (3 nourritures)")]
        public int AgricultureWater { get { return agricultureWater; } set { agricultureWater=value; } }
        [Category("Factions"),DisplayName("Activer la faction MAGA spéciale")] public bool TrumpFactionEnabled { get { return trumpFactionEnabled; } set { trumpFactionEnabled=value; } }
        [Category("5. Nouvelle expérience"),DisplayName("Équilibrer les fondateurs"),Description("Même ensemble de génomes, sexes et métiers dans chaque faction. Choisir un multiple de quatre pour des effectifs égaux.")] public bool BalancedFounders { get { return balancedFounders; } set { balancedFounders=value; } }
        [Category("Monde et ressources"),DisplayName("Carte symétrique")] public bool SymmetricMap { get { return symmetricMap; } set { symmetricMap=value; } }
        [Category("Monde et ressources"),DisplayName("Adapter les réserves et gisements à la population")] public bool ScaleResources { get { return scaleResources; } set { scaleResources=value; } }
        [Category("Monde et ressources"),DisplayName("Activer le petit monde")] public bool EcosystemEnabled { get { return ecosystemEnabled; } set { ecosystemEnabled=value; } }
        [Category("Monde et ressources"),DisplayName("Eau nécessaire par jour")] public int WaterForLife { get { return waterForLife; } set { waterForLife=value; } }
        [Category("Monde et ressources"),DisplayName("Difficulté des montagnes")] public double MountainDifficulty { get { return mountainDifficulty; } set { mountainDifficulty=value; } }
        [Category("Factions"),DisplayName("Chef trumpiste et protection")] public bool LeaderEnabled { get { return leaderEnabled; } set { leaderEnabled=value; } }
        [Category("Factions"),DisplayName("Probabilité de raid au contact")] public double RaidChance { get { return raidChance; } set { raidChance=value; } }
        [Category("Factions"),DisplayName("Bonus de force par arme")] public double WeaponBonus { get { return weaponBonus; } set { weaponBonus=value; } }
        void ValidateEcosystem() { ValidateDiplomacy(); if(EnergySitesPerQuarter<1 || EnergySitesPerQuarter>8 || EnergyPerSite<1 || EnergyPerSite>20 || MaxReserves<0 || MaxReserves>16 || CastleRepairPerNight<0 || CastleRepairPerNight>20) throw new ArgumentException("Production et stockage : gisements 1-8, energie 1-20, reserves 0-16, reparation 0-20."); if(TransporterBagSlots<1 || TransporterBagSlots>20 || AgricultureWater<0 || AgricultureWater>5) throw new ArgumentException("Transport : 1–20 places ; eau agricole : 0–5."); if(MapColumns<10 || MapColumns>80 || MapRows<10 || MapRows>80 || MapColumns*MapRows>3600) throw new ArgumentException("Terrain : 10 à 80 lignes et colonnes, maximum 3 600 cases."); if(WaterForLife<0 || WaterForLife>5 || double.IsNaN(MountainDifficulty) || MountainDifficulty<1 || MountainDifficulty>5 || double.IsNaN(RaidChance) || RaidChance<0 || RaidChance>1 || double.IsNaN(WeaponBonus) || WeaponBonus<0 || WeaponBonus>3) throw new ArgumentException("Règles du monde invalides : eau 0–5, montagne 1–5, raid 0–1, arme 0–3."); }
    }
    public partial class Blob
    {
        public int RaidTarget=-1;
        internal int SpatialRank;
        public bool DeathEffectReported;
        public int FactionId,Water,Weapon,GuardLeaderId; public Profession Job; public bool IsLeader;
        public int[] Bag=new int[8]; public int LastAction=-50;
        public bool HasOrangeCrest(Rules rules) { return rules.EcosystemEnabled && rules.TrumpFactionEnabled && rules.LeaderEnabled && FactionId==1 && IsLeader && !Dead; }
    }
    public partial class World
    {
        public readonly List<VisualEvent> VisualEvents=new List<VisualEvent>(); public int VisualEpoch; long visualId;
        internal void VisualAction(Blob b,int kind,int resource=-1,int amount=1) {
            VisualEvents.Add(new VisualEvent { Id=++visualId,X=b.X,Y=b.Y,Actor=b,Partner=kind==3?b.Mate:null,Kind=kind,Resource=resource,Amount=amount });
            if(VisualEvents.Count>64) {
                int hearts=0; foreach(VisualEvent visual in VisualEvents) if(visual.Kind==3) hearts++;
                int remove=VisualEvents.FindIndex(delegate(VisualEvent visual) { return hearts>8?visual.Kind==3:visual.Kind!=3; }); VisualEvents.RemoveAt(remove<0?0:remove);
            }
        }
        void NotifyDeath(Blob b) { if(b.Dead && !b.DeathEffectReported) { b.DeathEffectReported=true; VisualAction(b,2); } }
        public List<ResourceSample> ResourceHistory=new List<ResourceSample>();
        public int[] ResourceDailyStart=new int[8]; long resourceStep;
        public int[] MapResourceQuantities()
        {
            int[] quantities=new int[8]; foreach(Food f in Foods) if(!f.Eaten) quantities[0]++;
            if(Rules.EcosystemEnabled) foreach(Depot d in Depots) quantities[d.Kind]+=Math.Max(0,d.Quantite);
            return quantities;
        }
        void SampleResources(bool dawn)
        {
            int[] quantities=MapResourceQuantities(); if(dawn) ResourceDailyStart=(int[])quantities.Clone();
            ResourceHistory.Add(new ResourceSample { Step=resourceStep,Day=Day,Tick=Tick,Quantities=quantities });
            if(ResourceHistory.Count>6000) ResourceHistory.RemoveRange(0,1000);
        }
        public double Width { get { return Rules.MapColumns*10; } }
        public double Height { get { return Rules.MapRows*10; } }
        public bool UseCastles { get { return Rules.EcosystemEnabled && Rules.CastlesEnabled; } }
        public const double CastleRadius=14;
        void PositionCastles()
        {
            Camps[0].X=Width*.15; Camps[0].Y=Height*.5; Camps[1].X=Width*.85; Camps[1].Y=Height*.5;
            Camps[2].X=Width*.5; Camps[2].Y=Height*.15; Camps[3].X=Width*.5; Camps[3].Y=Height*.85;
        }
        void PositionCastleResidents()
        {
            int[] ranks=new int[4];
            foreach(Blob b in Blobs) {
                int rank=ranks[b.FactionId]++; double angle=rank*2.399963229728653,radius=4+8*Math.Sqrt((rank%100)/99.0);
                Camp c=Camps[b.FactionId]; b.X=Math.Max(1,Math.Min(Width-1,c.X+Math.Cos(angle)*radius)); b.Y=Math.Max(1,Math.Min(Height-1,c.Y+Math.Sin(angle)*radius));
                b.Heading=Math.Atan2(Height*.5-c.Y,Width*.5-c.X)+(rank%7-3)*.12;
            }
        }
        public bool TornadoActive; public double TornadoX,TornadoY;
        public int TornadoDeaths,WeatherFoodDestroyed; int richSide;
        double stormCenterX,stormCenterY,stormPhase;
        void StartWeatherDay()
        {
            TornadoActive=Rules.TornadoesEnabled; TornadoDeaths=WeatherFoodDestroyed=0;
            if(!TornadoActive) return;
            stormCenterX=(.2+random.NextDouble()*.6)*Width; stormCenterY=(.2+random.NextDouble()*.6)*Height; stormPhase=random.NextDouble()*Math.PI*2;
            UpdateTornadoPosition();
        }
        void UpdateTornadoPosition()
        {
            TornadoX=stormCenterX+Math.Min(35,Math.Min(Width,Height)*.15)*Math.Cos(stormPhase+Tick*.012);
            TornadoY=stormCenterY+Math.Min(35,Math.Min(Width,Height)*.15)*Math.Sin(stormPhase+Tick*.012);
        }
        internal void StepWeather()
        {
            if(!TornadoActive) return; UpdateTornadoPosition();
            foreach(Blob b in Blobs) if(!b.Dead && !b.Home && Distance(b.X,b.Y,TornadoX,TornadoY)<12) {
                b.Energy=Math.Max(0,b.Energy-12); if(b.Energy==0) { b.Dead=true; TornadoDeaths++; }
            }
            foreach(Food f in Foods) if(!f.Eaten && Distance(f.X,f.Y,TornadoX,TornadoY)<12) { f.Eaten=true; WeatherFoodDestroyed++; }
            foreach(Depot d in Depots) if(d.Quantite>0 && Distance(d.X,d.Y,TornadoX,TornadoY)<12) d.Quantite=0;
        }
        bool RichHalf(double x,double y) { return richSide==0?x<Width/2:richSide==1?x>=Width/2:richSide==2?y<Height/2:y>=Height/2; }
        public Camp[] Camps; public List<Depot> Depots=new List<Depot>(); public int[,] Relations=new int[4,4];
        public int SunkFood,Raids,ThirstDeaths,LeaderMeals,Protections; public List<string> Journal=new List<string>();
        public static bool Lake(double x,double y) { return Distance(x,y,208,90)<26 || Distance(x,y,82,212)<25; }
        public static double HeightAt(double x,double y) { return Math.Max(0,32*(1-Distance(x,y,92,80)/48))+Math.Max(0,38*(1-Distance(x,y,214,215)/45)); }
        bool CastleGround(double x,double y) { if(!UseCastles || Camps==null) return false; foreach(Camp c in Camps) if(Distance(x,y,c.X,c.Y)<CastleRadius+5) return true; return false; }
        public bool IsLake(double x,double y) { if(CastleGround(x,y)) return false; x=x*300/Width; y=y*300/Height; return Rules.SymmetricMap?Distance(x,y,70,150)<18 || Distance(x,y,230,150)<18 || Distance(x,y,150,70)<18 || Distance(x,y,150,230)<18:Lake(x,y); }
        public double TerrainHeight(double x,double y)
        {
            if(CastleGround(x,y)) return 0; x=x*300/Width; y=y*300/Height;
            if(!Rules.SymmetricMap) return HeightAt(x,y);
            double height=0; for(int i=0;i<4;i++) { double cx=i%2==0?90:210,cy=i<2?90:210; height=Math.Max(height,28*Math.Max(0,1-Distance(x,y,cx,cy)/34)); } return height;
        }
        internal int ResourceAmount(int n) { return Rules.ScaleResources?Math.Max(n,(n*Rules.InitialPopulation+23)/24):n; }
        void ResetEcosystem()
        {
            VisualEvents.Clear(); visualId=0; VisualEpoch++;
            ResourceHistory.Clear(); ResourceDailyStart=new int[8]; resourceStep=0;
            Camps=new Camp[4]; string[] names={"Bleus",Rules.TrumpFactionEnabled?"Trumpistes":"Corail","Verts","Violets"};
            for(int i=0;i<4;i++) { Camps[i]=new Camp { Nom=names[i],Capacite=ResourceAmount(40) }; Camps[i].Stock=new int[] {ResourceAmount(12),ResourceAmount(20),ResourceAmount(6),ResourceAmount(4),ResourceAmount(3),ResourceAmount(8),0,0}; }
            if(Rules.ScaleResources) foreach(Camp camp in Camps) camp.Stock[0]=Math.Min(camp.Capacite,((Rules.InitialPopulation+3)/4)*Rules.SurvivalFood);
            PositionCastles();
            ResetDiplomacy(); Depots.Clear(); Journal.Clear(); Relations=new int[4,4]; Raids=ThirstDeaths=LeaderMeals=Protections=SunkFood=0;
        }
        void Log(string s) { Journal.Insert(0,"J"+Day+" : "+s); if(Journal.Count>12) Journal.RemoveAt(12); }
        void BeginEcosystemDay()
        {
            if(!Rules.EcosystemEnabled) return;
            foreach(Camp camp in Camps) { camp.ProducedToday=new int[8]; camp.ConsumedToday=new int[8]; } Camps[1].Nom=Rules.TrumpFactionEnabled?"Trumpistes":"Corail";
            SunkFood=0; foreach(Food f in Foods) if(IsLake(f.X,f.Y)) { f.Eaten=true; SunkFood++; }
            Depots.Clear();
            if(Rules.SymmetricMap) SpawnSymmetricDepots(); else {
            for(int i=0;i<32;i++) { double x=20+random.NextDouble()*260,y=20+random.NextDouble()*260; if(!Lake(x,y)) Depots.Add(new Depot { X=x,Y=y,Kind=2,Quantite=4 }); }
            for(int i=0;i<18;i++) { double cx=i%2==0?92:214,cy=i%2==0?80:215; Depots.Add(new Depot { X=cx+random.NextDouble()*50-25,Y=cy+random.NextDouble()*50-25,Kind=i%3==0?4:3,Quantite=4 }); }
            for(int i=0;i<10;i++) Depots.Add(new Depot { X=25+random.NextDouble()*250,Y=25+random.NextDouble()*250,Kind=5,Quantite=3 });
            for(int i=0;i<32;i++) { double a=i*Math.PI/16; double cx=i<16?208:82,cy=i<16?90:212; Depots.Add(new Depot { X=cx+Math.Cos(a)*29,Y=cy+Math.Sin(a)*29,Kind=1,Quantite=12 }); }
            foreach(Depot d in Depots) { d.X=d.X*Width/300; d.Y=d.Y*Height/300; d.Quantite=ResourceAmount(d.Quantite); }
            }
            if(Rules.UnequalResources) foreach(Depot d in Depots) d.Quantite=Math.Max(1,(int)Math.Round(d.Quantite*(RichHalf(d.X,d.Y)?1.5:.5)));
            Blob leader=null; foreach(Blob b in Blobs) { if(b.FactionId==1 && !b.Dead && b.IsLeader) leader=b; if(!Rules.LeaderEnabled || !Rules.TrumpFactionEnabled) b.IsLeader=false; }
            if(Rules.TrumpFactionEnabled && Rules.LeaderEnabled && leader==null) foreach(Blob b in Blobs) if(b.FactionId==1 && !b.Dead) { b.IsLeader=true; Log("Nouveau chef trumpiste #"+b.Id); break; }
            leader=null; foreach(Blob b in Blobs) { b.GuardLeaderId=0; if(b.IsLeader && Rules.LeaderEnabled) leader=b; }
            int guards=0; if(leader!=null) foreach(Blob b in Blobs) if(b!=leader && b.FactionId==1 && guards++<2) b.GuardLeaderId=leader.Id;
        }
        void WakeWorker(Blob b)
        {
            if(!Rules.EcosystemEnabled) return;
            b.Water=0; b.Bag=new int[8]; b.LastAction=-50; Camp c=Camps[b.FactionId];
            int ration=b.IsLeader?Math.Max(Rules.ReproductionFood,Rules.SurvivalFood):Rules.SurvivalFood;
            if(c.Stock[0]>=ration) { c.Stock[0]-=ration; b.Food=ration; if(b.IsLeader) LeaderMeals++; }
            if(c.Stock[1]>=Rules.WaterForLife) { c.Stock[1]-=Rules.WaterForLife; b.Water=Rules.WaterForLife; }
            if(b.Weapon==0 && c.Stock[7]>0) { c.Stock[7]--; b.Weapon=1; }
        }
        public static int FoodGoal(Blob b,Rules r) { return b.RaidTarget>=0?r.SurvivalFood:r.ReproductionFood; }
        public static int BagCapacity(Blob b,Rules r) { return (b.Job==Profession.Transporteur?r.TransporterBagSlots:3)+(r.TrumpFactionEnabled && b.FactionId==1?2:0); }
        internal bool WantsWorkResource(Blob b,int kind)
        {
            if(!HasWater(b,Rules)) return kind==1;
            if(b.RaidTarget>=0 || b.IsLeader || b.Food<Rules.SurvivalFood) return false;
            if(b.Job==Profession.Agriculteur) return kind==2;
            if(b.Job==Profession.Mineur) return kind==3 || kind==4;
            if(b.Job==Profession.Artisan) return kind==5;
            return kind==5 || kind==1 && Camps[b.FactionId].Stock[1]<ResourceAmount(20);
        }
        public static bool HasWater(Blob b,Rules r) { return !r.EcosystemEnabled || b.Water>=r.WaterForLife; }
        public static double Strength(Blob b,Rules r)
        { return b.Size*(1+(r.EcosystemEnabled?r.WeaponBonus*b.Weapon:0)); }
        public static double Defense(Blob b,Rules r) { return Strength(b,r)*(r.EcosystemEnabled && r.TrumpFactionEnabled && r.LeaderEnabled && b.IsLeader?2:1); }
        public static double RaidDamage(Blob attacker,Rules rules,double baseDamage) { return baseDamage*Strength(attacker,rules); }
        double MovementMultiplier(Blob b) { return !Rules.EcosystemEnabled?1:1+(Rules.MountainDifficulty-1)*Math.Min(1,TerrainHeight(b.X,b.Y)/20); }
        void AvoidLake(Blob b,double speed,ref double x,ref double y)
        { if(!Rules.EcosystemEnabled || !IsLake(x,y)) return; for(int i=1;i<=8;i++) { double angle=b.Heading+i*Math.PI/4; x=b.X+Math.Cos(angle)*speed; y=b.Y+Math.Sin(angle)*speed; if(!IsLake(x,y)) { b.Heading=angle; return; } } x=b.X; y=b.Y; }
        bool WorkDirection(Blob b,out double heading)
        {
            heading=0; if(!Rules.EcosystemEnabled || b.Returning || b.Mate!=null) return false;
            if(b.GuardLeaderId!=0 && HasWater(b,Rules) && b.Food>=Rules.SurvivalFood)
                foreach(Blob chief in Blobs) if(chief.Id==b.GuardLeaderId && !chief.Dead && !chief.Home && Distance(b.X,b.Y,chief.X,chief.Y)>12 && b.Energy>Cost(b,Rules)*(HomeDistance(b)/Math.Max(.2,b.Speed)+30))
                { heading=Math.Atan2(chief.Y-b.Y,chief.X-b.X); return true; }
            if(RaidDirection(b,out heading)) return true; Depot best=null; double distance=double.MaxValue; int bag=0; foreach(int n in b.Bag) bag+=n;
            if(UseCastles && MagaCastleDirection(b,out heading)) return true;
            if(HasWater(b,Rules) && (b.Food<Rules.SurvivalFood || bag>=BagCapacity(b,Rules) || Tick>=Rules.DayLength*.45)) return false;
            foreach(Depot d in Depots)
            {
                bool wanted=WantsWorkResource(b,d.Kind) && (!HasWater(b,Rules) || bag<BagCapacity(b,Rules) && Tick<Rules.DayLength*.45);
                if(!wanted || d.Quantite<=0) continue; double dist=Distance(b.X,b.Y,d.X,d.Y);
                if(b.Job==Profession.Mineur && HasWater(b,Rules)) { Camp camp=Camps[b.FactionId]; if(d.Kind==4 && camp.Stock[4]>camp.Stock[3]/2) dist*=2; }
                if(Rules.PrioritizeWater && b.Job==Profession.Transporteur && d.Kind==1 && Camps[b.FactionId].Stock[1]<ResourceAmount(12)) dist*=.15;
                if(wanted && d.Quantite>0 && dist<distance) { best=d; distance=dist; }
            }
            if(best!=null) { heading=Math.Atan2(best.Y-b.Y,best.X-b.X); return true; } return false;
        }
        internal void WorkerAction(Blob b)
        {
            if(!Rules.EcosystemEnabled || b.Returning) return;
            if(b.IsLeader && Rules.LeaderEnabled)
            {
                foreach(Blob ally in Blobs) if(ally!=b && ally.FactionId==1 && !ally.Dead && Distance(b.X,b.Y,ally.X,ally.Y)<25)
                { if(b.Food<Rules.SurvivalFood && ally.Food>Rules.SurvivalFood) { ally.Food--; b.Food++; LeaderMeals++; } if(b.Water<Rules.WaterForLife && ally.Water>Rules.WaterForLife) { ally.Water--; b.Water++; } }
            }
            CoalitionAction(b); if(Tick-b.LastAction<15) return;
            int total=0; foreach(int n in b.Bag) total+=n;
            foreach(Depot d in Depots) if(d.Quantite>0 && (d.Kind==1 && b.Water<Rules.WaterForLife || total<BagCapacity(b,Rules) && WantsWorkResource(b,d.Kind)) && Distance(b.X,b.Y,d.X,d.Y)<6)
            { if(d.Kind==1 && b.Water<Rules.WaterForLife) { b.Water++; d.Quantite--; b.LastAction=Tick; VisualAction(b,1,1); break; } if(WantsWorkResource(b,d.Kind) && total<BagCapacity(b,Rules)) { b.Bag[d.Kind]++; d.Quantite--; b.LastAction=Tick; VisualAction(b,1,d.Kind); break; } }
            foreach(RaidPlan plan in RaidPlans) if(plan.Fighters.Contains(b) && ValidPlan(plan)) return;
            if(UseCastles && Rules.EconomicDiplomacy && b.RaidTarget>=0 && !(Rules.TrumpFactionEnabled && b.FactionId==1)) {
                int target=b.RaidTarget;
                if(!Allied(b.FactionId,target) && Relations[b.FactionId,target]<=Rules.RaidHostility && Distance(b.X,b.Y,Camps[target].X,Camps[target].Y)<=CastleRadius+8 && Tick-b.LastAction>=45 && ReadyRaider(b)) {
                    StrikeCastle(target,new List<Blob> { b },15); b.LastAction=Tick; Raids++; CommonThreat(b.FactionId,target,true);
                }
                return;
            }
            if(!Rules.TrumpFactionEnabled || b.FactionId!=1 || b.IsLeader || random.NextDouble()>Rules.RaidChance) return;
            if(UseCastles) {
                int target=b.RaidTarget; if(target>=0 && Distance(b.X,b.Y,Camps[target].X,Camps[target].Y)<=CastleRadius+8 && Tick-b.LastAction>=15 && ReadyRaider(b)) {
                    StrikeCastle(target,new List<Blob> { b },15); b.LastAction=Tick; Raids++; CommonThreat(b.FactionId,target,true);
                }
                return;
            }
            foreach(Blob victim in Blobs) if(victim.FactionId!=1 && !Allied(b.FactionId,victim.FactionId) && !victim.Dead && !victim.Home && Distance(b.X,b.Y,victim.X,victim.Y)<9)
            { if(victim.Food>0) { victim.Food--; b.Food++; } victim.Energy=Math.Max(0,victim.Energy-RaidDamage(b,Rules,15)); if(victim.Energy==0) victim.Dead=true; Raids++; b.LastAction=Tick; CommonThreat(b.FactionId,victim.FactionId); break; }
        }
        void CountDeath(Blob b) { if(Rules.EcosystemEnabled && !HasWater(b,Rules)) ThirstDeaths++; }
        void DepositWorker(Blob b) { if(!Rules.EcosystemEnabled) return;
                Camp c=Camps[b.FactionId];
                for(int kind=0;kind<8;kind++) {
                    int carried=b.Bag[kind]+(kind==0?Math.Max(0,b.Food-Rules.SurvivalFood):kind==1?Math.Max(0,b.Water-Rules.WaterForLife):0);
                    if(carried>0) { c.Add(kind,carried); VisualAction(b,4,kind,carried); }
                    b.Bag[kind]=0;
                }
                b.Food=Math.Min(b.Food,Rules.SurvivalFood); b.Water=Math.Min(b.Water,Rules.WaterForLife);
        }
        void FinishEcosystemDay(List<Blob> survivors)
        {
            if(!Rules.EcosystemEnabled) return;
            foreach(Blob b in survivors) DepositWorker(b);
            for(int f=0;f<4;f++)
            {
                Camp c=Camps[f]; int[] jobs=new int[4]; foreach(Blob b in survivors) if(b.FactionId==f) jobs[(int)b.Job]++;
                if(UseCastles && jobs[0]+jobs[1]+jobs[2]+jobs[3]>0 && c.Fortification<100 && (!Rules.CastleRepairsUseResources || c.Consume(0,0,0,0,0,2,1))) c.Fortification=Math.Min(100,c.Fortification+Rules.CastleRepairPerNight);
                for(int i=0;i<Math.Min(ResourceAmount(4),jobs[2]+jobs[3]);i++) { if(c.Consume(0,0,0,0,1,0,0)) { c.Add(5,4); c.ProducedToday[5]+=4; } if(c.Consume(0,0,0,2,0,2,0)) { c.Add(6,1); c.ProducedToday[6]++; } }
                for(int i=0;i<Math.Min(ResourceAmount(4),jobs[0]);i++) if(c.Consume(0,Rules.AgricultureWater,2,0,0,1,0)) { c.Add(0,3); c.ProducedToday[0]+=3; }
                int unarmed=0; foreach(Blob b in survivors) if(b.FactionId==f && b.Weapon==0) unarmed++;
                for(int i=0;i<Math.Min(Math.Min(ResourceAmount(1),jobs[2]),Math.Max(0,unarmed-c.Stock[7]));i++) if(c.Consume(0,0,0,0,0,2,1)) { c.Add(7,1); c.ProducedToday[7]++; }
                bool crowded=false; foreach(int stock in c.Stock) if(stock>=c.Capacite*.85) crowded=true;
                if(crowded && jobs[2]>0 && c.Reserves<Rules.MaxReserves && c.Consume(0,0,0,0,0,ResourceAmount(2),ResourceAmount(2))) { c.Reserves++; c.Capacite+=ResourceAmount(15); } c.LastProduced=(int[])c.ProducedToday.Clone(); c.LastConsumed=(int[])c.ConsumedToday.Clone();
            }
            List<int> pairs=new List<int>(); for(int a=0;a<4;a++) for(int z=a+1;z<4;z++) pairs.Add(a*4+z);
            if(Rules.BalancedFounders) for(int i=pairs.Count-1;i>0;i--) { int j=random.Next(i+1),v=pairs[i]; pairs[i]=pairs[j]; pairs[j]=v; }
            foreach(int pair in pairs) { int a=pair/4,z=pair%4; if(Relations[a,z]>-20) for(int k=0;k<2;k++) { Camp donor=Camps[a].Stock[k]>Camps[z].Stock[k]?Camps[a]:Camps[z],recipient=donor==Camps[a]?Camps[z]:Camps[a]; if(donor.Stock[k]>ResourceAmount(8) && recipient.Stock[k]<ResourceAmount(4)) { donor.Stock[k]--; recipient.Add(k,1); ChangeTrust(a,z,2); } } }
            EconomicDiplomacyNight(survivors);
        }
        public bool ProtectedLeader(Blob prey,Blob hunter)
        {
            if(!Rules.EcosystemEnabled || !Rules.TrumpFactionEnabled || !Rules.LeaderEnabled || !prey.IsLeader) return false;
            foreach(Blob guard in Blobs) if(guard!=prey && guard.FactionId==prey.FactionId && !guard.Dead && !guard.Home && Distance(guard.X,guard.Y,prey.X,prey.Y)<22)
            { guard.Energy-=8; hunter.Energy-=8; Protections++; return true; } return false;
        }
    }
    public partial class World
    {
        static void Rotate(ref double x,ref double y,int quarters) { for(int k=0;k<quarters;k++) { double previous=x; x=-y; y=previous; } }
        void BalanceFounderSexes()
        {
            int count=(Blobs.Count+3)/4,females=(int)Math.Round(count*Rules.FemaleRatio,MidpointRounding.AwayFromZero);
            Sex[] sexes=new Sex[count]; for(int i=0;i<count;i++) sexes[i]=i<females?Sex.Female:Sex.Male;
            for(int i=count-1;i>0;i--) { int j=random.Next(i+1); Sex sex=sexes[i]; sexes[i]=sexes[j]; sexes[j]=sex; }
            for(int i=0;i<Blobs.Count;i++) Blobs[i].Sex=sexes[i/4];
        }
        void PositionFounders()
        {
            int[] ranks=new int[4],turns={0,2,1,3};
            foreach(Blob b in Blobs)
            {
                int rank=ranks[b.FactionId]++; double depth=rank/25*10,x=-150+depth,y=(rank%25+.5)*12-150;
                while(IsLake(x+150,y+150) && depth>0) { depth-=3; x=-150+depth; }
                Rotate(ref x,ref y,turns[b.StartingSide]); b.X=(x+150)*Width/300; b.Y=(y+150)*Height/300;
            }
        }
        void SpawnFoods()
        {
            Foods.Clear();
            if(Rules.UnequalResources) {
                richSide=random.Next(4);
                for(int i=0;i<FoodToday;i++) {
                    double x,y; bool rich=i<(int)Math.Round(FoodToday*.7);
                    do { x=12+random.NextDouble()*(Width-24); y=12+random.NextDouble()*(Height-24); } while(RichHalf(x,y)!=rich);
                    Foods.Add(new Food(x,y));
                }
                return;
            }
            if(!Rules.SymmetricMap) { for(int i=0;i<FoodToday;i++) Foods.Add(new Food(12+random.NextDouble()*(Width-24),12+random.NextDouble()*(Height-24))); return; }
            for(int i=0;i<FoodToday;i+=4)
            {
                double x=random.NextDouble()*138,y=random.NextDouble()*138;
                for(int k=0;k<4 && i+k<FoodToday;k++) { Foods.Add(new Food((x+150)*Width/300,(y+150)*Height/300)); Rotate(ref x,ref y,1); }
            }
        }
        void AddRotatedDepot(double x,double y,int kind,int quantity)
        {
            x-=150; y-=150;
            for(int k=0;k<4;k++) { if(!IsLake((x+150)*Width/300,(y+150)*Height/300)) Depots.Add(new Depot { X=(x+150)*Width/300,Y=(y+150)*Height/300,Kind=kind,Quantite=ResourceAmount(quantity) }); Rotate(ref x,ref y,1); }
        }
        void SpawnSymmetricDepots()
        {
            for(int i=0;i<8;i++) { double x,y; do { x=18+random.NextDouble()*125; y=18+random.NextDouble()*125; } while(IsLake(x*Width/300,y*Height/300) || TerrainHeight(x*Width/300,y*Height/300)>8); AddRotatedDepot(x,y,2,4); }
            for(int i=0;i<6;i++) AddRotatedDepot(90+random.NextDouble()*30-15,90+random.NextDouble()*30-15,i%3==0?4:3,4);
            for(int i=0;i<Rules.EnergySitesPerQuarter;i++) { double x,y; do { x=18+random.NextDouble()*125; y=18+random.NextDouble()*125; } while(IsLake(x*Width/300,y*Height/300)); AddRotatedDepot(x,y,5,Rules.EnergyPerSite); }
            for(int i=0;i<8;i++) { double a=i*Math.PI/4; AddRotatedDepot(70+Math.Cos(a)*22,150+Math.Sin(a)*22,1,12); }
        }
        const double CellSize=25; int GridColumns { get { return (Rules.MapColumns*10+24)/25; } }
        internal bool UseSpatialIndex=true;
        List<Blob> interactionOrder;
        readonly Dictionary<int,List<Blob>> blobCells=new Dictionary<int,List<Blob>>();
        readonly Dictionary<int,int> cellFactionMasks=new Dictionary<int,int>();
        readonly List<Blob> blobQueryBuffer=new List<Blob>();
        readonly List<Food> foodQueryBuffer=new List<Food>();
        readonly Dictionary<Blob,int> blobRanks=new Dictionary<Blob,int>(),blobPositions=new Dictionary<Blob,int>();
        readonly Dictionary<int,List<Food>> foodCells=new Dictionary<int,List<Food>>(); readonly Dictionary<Food,int> foodRanks=new Dictionary<Food,int>();
        int CellX(double coordinate) { return Math.Max(0,Math.Min(GridColumns-1,(int)(coordinate/CellSize))); } int CellY(double coordinate) { return Math.Max(0,Math.Min((Rules.MapRows*10+24)/25-1,(int)(coordinate/CellSize))); }
        int CellKey(double x,double y) { return CellX(x)+GridColumns*CellY(y); }
        void BuildBlobIndex(List<Blob> shuffled)
        {
            interactionOrder=Rules.BalancedFounders?shuffled:Blobs;
            for(int i=0;i<interactionOrder.Count;i++) interactionOrder[i].SpatialRank=i;
            foreach(List<Blob> cell in blobCells.Values) cell.Clear(); cellFactionMasks.Clear(); blobRanks.Clear(); blobPositions.Clear(); if(Blobs.Count<100) return;
            for(int i=0;i<interactionOrder.Count;i++) { Blob b=interactionOrder[i]; b.SpatialRank=i; if(b.Dead || b.Home) continue; int key=CellKey(b.X,b.Y); if(!blobCells.ContainsKey(key)) blobCells[key]=new List<Blob>(); blobCells[key].Add(b); int mask; cellFactionMasks.TryGetValue(key,out mask); cellFactionMasks[key]=mask|(1<<b.FactionId); blobRanks[b]=i; blobPositions[b]=key; }
        }
        void MoveBlobIndex(Blob b)
        {
            int previous; if(!blobPositions.TryGetValue(b,out previous)) return; int next=CellKey(b.X,b.Y); if(previous==next) return;
            blobCells[previous].Remove(b); if(!blobCells.ContainsKey(next)) blobCells[next]=new List<Blob>(); blobCells[next].Add(b); blobPositions[b]=next;
            int mask; cellFactionMasks.TryGetValue(next,out mask); cellFactionMasks[next]=mask|(1<<b.FactionId);
        }
        IEnumerable<Blob> NearbyBlobs(Blob b,double radius,bool ordered=true,bool hostileOnly=false)
        {
            if(!UseSpatialIndex || Blobs.Count<100) return interactionOrder;
            List<Blob> result=blobQueryBuffer; result.Clear();
            int hostileMask=15; if(hostileOnly) { hostileMask=0; if(Rules.Predation) for(int f=0;f<4;f++) if(!Rules.EcosystemEnabled || f!=b.FactionId && !Allied(b.FactionId,f)) hostileMask|=1<<f; if(hostileMask==0) return result; }
            for(int x=CellX(b.X-radius);x<=CellX(b.X+radius);x++) for(int y=CellY(b.Y-radius);y<=CellY(b.Y+radius);y++) { List<Blob> cell; int key=x+GridColumns*y,mask; if(hostileOnly && (!cellFactionMasks.TryGetValue(key,out mask) || (mask&hostileMask)==0)) continue; if(blobCells.TryGetValue(key,out cell)) {
                if(hostileOnly) { if(Rules.Predation) foreach(Blob other in cell) if(!Rules.EcosystemEnabled || other.FactionId!=b.FactionId && !Allied(b.FactionId,other.FactionId)) result.Add(other); }
                else result.AddRange(cell);
            } }
            if(ordered) result.Sort(delegate(Blob a,Blob other) { return a.SpatialRank.CompareTo(other.SpatialRank); }); return result;
        }
        void BuildFoodIndex()
        {
            foodCells.Clear(); foodRanks.Clear();
            for(int i=0;i<Foods.Count;i++) { Food f=Foods[i]; int key=CellKey(f.X,f.Y); if(!foodCells.ContainsKey(key)) foodCells[key]=new List<Food>(); foodCells[key].Add(f); foodRanks[f]=i; }
        }
        IEnumerable<Food> NearbyFoods(Blob b,double radius)
        {
            if(!UseSpatialIndex || Blobs.Count<100) return Foods;
            List<Food> result=foodQueryBuffer; result.Clear();
            for(int x=CellX(b.X-radius);x<=CellX(b.X+radius);x++) for(int y=CellY(b.Y-radius);y<=CellY(b.Y+radius);y++) { List<Food> cell; if(foodCells.TryGetValue(x+GridColumns*y,out cell)) foreach(Food f in cell) if(!f.Eaten) result.Add(f); }
            result.Sort(delegate(Food a,Food other) { return foodRanks[a].CompareTo(foodRanks[other]); }); return result;
        }
    }
    public partial class Arena
    {
        sealed class ActionEffect { public double X,Y,Height,Start; public Blob Actor,Partner; public int Kind,Resource,Amount; public string Label; }
        readonly Font pickupFont=new Font("Segoe UI",12,FontStyle.Bold);
        static double EffectDuration(int kind) { return kind==3?1.8:kind==0 || kind==1?1.3:.7; }
        readonly List<ActionEffect> actionEffects=new List<ActionEffect>();
        readonly System.Diagnostics.Stopwatch effectClock=System.Diagnostics.Stopwatch.StartNew();
        readonly Timer effectTimer=new Timer { Interval=50 }; int effectEpoch=-1; long lastVisualId;
        readonly SolidBrush[] effectBrushes=new SolidBrush[80];
        Bitmap sceneCache; bool sceneDirty=true,effectsOnlyPaint;
        public bool ShowActionEffects=true,SimulationRunning;
        bool IsCourtingHighlight(Blob blob) {
            if(!ShowActionEffects) return false;
            double now=effectClock.Elapsed.TotalSeconds;
            foreach(ActionEffect effect in actionEffects) if(effect.Kind==3 && now-effect.Start<1.8 && (effect.Actor==blob || effect.Partner==blob)) return true;
            foreach(VisualEvent visual in World.VisualEvents) if(visual.Id>lastVisualId && visual.Kind==3 && (visual.Actor==blob || visual.Partner==blob)) return true;
            return false;
        }
        public void InvalidateScene() { sceneDirty=true; Invalidate(); }
        void InvalidateEffects() { effectsOnlyPaint=true; Invalidate(); }
        Brush EffectBrush(int color,int alpha) {
            int slot=color*8+alpha;
            if(effectBrushes[slot]==null) effectBrushes[slot]=new SolidBrush(Color.FromArgb(30+alpha*27,color<8?ResourceDisplay.ColorFor(color):color==8?Color.FromArgb(255,110,115):Color.HotPink));
            return effectBrushes[slot];
        }
        void DisposeEffectGraphics() { pickupFont.Dispose(); if(sceneCache!=null) sceneCache.Dispose(); foreach(SolidBrush brush in effectBrushes) if(brush!=null) brush.Dispose(); }
        void DrawActionEffects(Graphics g)
        {
            if(effectEpoch!=World.VisualEpoch) { actionEffects.Clear(); lastVisualId=0; effectEpoch=World.VisualEpoch; }
            double now=effectClock.Elapsed.TotalSeconds;
            for(int i=actionEffects.Count-1;i>=0;i--) if(now-actionEffects[i].Start>=EffectDuration(actionEffects[i].Kind)) { if(actionEffects[i].Kind==3) InvalidateScene(); actionEffects.RemoveAt(i); }
            foreach(VisualEvent visual in World.VisualEvents) if(visual.Id>lastVisualId) {
                lastVisualId=visual.Id; if(!ShowActionEffects) continue;
                ActionEffect grouped=null;
                foreach(ActionEffect effect in actionEffects) if(visual.Kind!=3 && effect.Kind==visual.Kind && effect.Resource==visual.Resource && now-effect.Start<.2 && World.Distance(effect.X,effect.Y,visual.X,visual.Y)<10) { grouped=effect; break; }
                if(grouped!=null) { if(visual.Kind==0 || visual.Kind==1 || visual.Kind==4) { grouped.Amount+=visual.Amount; grouped.Label=(visual.Kind==4?"−":"+")+grouped.Amount; } continue; }
                actionEffects.Add(new ActionEffect { X=visual.X,Y=visual.Y,Actor=visual.Actor,Partner=visual.Partner,Height=(World.Rules.EcosystemEnabled?World.TerrainHeight(visual.X,visual.Y):0)+10,Kind=visual.Kind,Resource=visual.Resource,Amount=visual.Amount,Start=now,Label=visual.Kind==2?"×":(visual.Kind==4?"−":"+")+visual.Amount });
                if(actionEffects.Count>8) { int remove=actionEffects.FindIndex(delegate(ActionEffect effect) { return effect.Kind!=3; }); actionEffects.RemoveAt(remove<0?0:remove); }
            }
            if(!ShowActionEffects) actionEffects.Clear();
            if(actionEffects.Count==0) { effectTimer.Stop(); return; }
            if(!effectTimer.Enabled) effectTimer.Start();
            foreach(ActionEffect effect in actionEffects) {
                double progress=(now-effect.Start)/EffectDuration(effect.Kind); PointF point; double depth;
                if(!Project(new Vec3(effect.X-World.Width/2,effect.Height,effect.Y-World.Height/2),out point,out depth)) continue;
                if(point.X<0 || point.X>Width || point.Y<0 || point.Y>Height) continue;
                int color=effect.Kind==3?9:effect.Kind==2?8:effect.Resource>=0?effect.Resource:0;
                Brush brush=EffectBrush(color,Math.Max(0,Math.Min(7,(int)((1-Math.Max(0,(progress-.6)/.4))*7))));
                float y=point.Y+(float)(progress*(effect.Kind==4?12:-16));
                if(effect.Kind==3) {
                    PointF first,second; double firstDepth,secondDepth;
                    if(effect.Actor!=null && effect.Partner!=null && !effect.Actor.Home && !effect.Partner.Home && !effect.Actor.Dead && !effect.Partner.Dead && Project(new Vec3(effect.Actor.X-World.Width/2,(World.Rules.EcosystemEnabled?World.TerrainHeight(effect.Actor.X,effect.Actor.Y):0)+Appearance.BodyRadius(effect.Actor.Size)*2.5,effect.Actor.Y-World.Height/2),out first,out firstDepth) && Project(new Vec3(effect.Partner.X-World.Width/2,(World.Rules.EcosystemEnabled?World.TerrainHeight(effect.Partner.X,effect.Partner.Y):0)+Appearance.BodyRadius(effect.Partner.Size)*2.5,effect.Partner.Y-World.Height/2),out second,out secondDepth)) {
                        point.X=(first.X+second.X)/2; y=Math.Min(first.Y,second.Y)-26;
                        g.DrawLine(Pens.HotPink,point.X,y+11,first.X,first.Y-3); g.DrawLine(Pens.HotPink,point.X,y+11,second.X,second.Y-3);
                        g.DrawString("#"+effect.Actor.Id,Font,Brushes.White,first.X+5,first.Y-10); g.DrawString("#"+effect.Partner.Id,Font,Brushes.White,second.X+5,second.Y-10);
                    } else y-=26;
                    float x=point.X,r=11;
                    g.FillPolygon(brush,new PointF[] { new PointF(x,y+r),new PointF(x-r,y),new PointF(x-r,y-r*.6f),new PointF(x-r*.4f,y-r),new PointF(x,y-r*.4f),new PointF(x+r*.4f,y-r),new PointF(x+r,y-r*.6f),new PointF(x+r,y) });
                }
                else {
                    bool pickup=effect.Kind==0 || effect.Kind==1; Font font=pickup?pickupFont:Font; float textX=point.X+8,textY=y-(pickup?24:8);
                    if(pickup) { g.DrawString(effect.Label,font,Brushes.Black,textX-1,textY); g.DrawString(effect.Label,font,Brushes.Black,textX+1,textY); g.DrawString(effect.Label,font,Brushes.Black,textX,textY-1); g.DrawString(effect.Label,font,Brushes.Black,textX,textY+1); }
                    g.DrawString(effect.Label,font,brush,textX,textY);
                }
            }
        }
        void Cuboid(double x,double z,double halfX,double halfZ,double bottom,double height,Color color)
        {
            Vec3 a=new Vec3(x-halfX,bottom,z-halfZ),b=new Vec3(x+halfX,bottom,z-halfZ),c=new Vec3(x+halfX,bottom,z+halfZ),d=new Vec3(x-halfX,bottom,z+halfZ),up=new Vec3(0,height,0);
            AddFace(new Vec3[] {a+up,d+up,c+up,b+up},color,true); AddFace(new Vec3[] {a,a+up,b+up,b},color,true); AddFace(new Vec3[] {b,b+up,c+up,c},color,true); AddFace(new Vec3[] {c,c+up,d+up,d},color,true); AddFace(new Vec3[] {d,d+up,a+up,a},color,true);
        }
        void FlatTerrain() { Cuboid(0,0,World.Width/2,World.Height/2,0,0,Color.FromArgb(66,105,96)); }
        void CastleMeshes()
        {
            if(!World.UseCastles) return;
            for(int faction=0;faction<4;faction++) {
                Camp camp=World.Camps[faction]; double x=camp.X-World.Width/2,z=camp.Y-World.Height/2;
                Color stone=camp.Fortification>0?Color.FromArgb(162,165,173):Color.FromArgb(104,100,94),banner=Appearance.Skins[faction];
                Cuboid(x,z,10,10,0,9,stone); Cuboid(x,z,5,5,9,10,stone);
                for(int dx=-1;dx<=1;dx+=2) for(int dz=-1;dz<=1;dz+=2) { Cuboid(x+dx*10,z+dz*10,3,3,0,17,stone); Cuboid(x+dx*10,z+dz*10,3.5,3.5,17,2,banner); }
                Cuboid(x,z,1,1,19,12,Color.FromArgb(84,73,57)); Cuboid(x+3,z,3,1,26,5,banner);
            }
        }
        void CastleLabels(Graphics graphics)
        {
            if(!World.UseCastles) return; for(int faction=0;faction<4;faction++) { Camp c=World.Camps[faction]; int inside=0; foreach(Blob b in World.Blobs) if(!b.Dead && b.Home && b.FactionId==faction) inside++; PointF p; double depth; if(Project(new Vec3(c.X-World.Width/2,36,c.Y-World.Height/2),out p,out depth)) graphics.DrawString(c.Nom+" · Défenses : "+c.Fortification.ToString("0")+" %"+(inside>0?"\n"+inside+" dans le château":""),Font,Brushes.White,p.X-30,p.Y-12); }
        }
        void WeatherMeshes()
        {
            if(!World.TornadoActive) return;
            for(int level=0;level<7;level++) {
                double angle=World.Tick*.22+level*.7,radius=2+level*1.3;
                Vec3 center=new Vec3(World.TornadoX-World.Width/2+Math.Cos(angle)*2,World.TerrainHeight(World.TornadoX,World.TornadoY)+3+level*5,World.TornadoY-World.Height/2+Math.Sin(angle)*2);
                Sphere(center,radius,3,radius,level%2==0?Color.FromArgb(121,133,145):Color.FromArgb(163,174,182),8,4);
            }
        }
        void Landscape()
        {
            for(int x=0;x<World.Width;x+=10) for(int y=0;y<World.Height;y+=10)
            {
                bool lake=World.IsLake(x+5,y+5); Color c=lake?Color.FromArgb(45,139,184):World.TerrainHeight(x+5,y+5)>8?Color.FromArgb(131,128,116):Color.FromArgb(72,117,83);
                Vec3 a=new Vec3(x-World.Width/2,lake?.1:World.TerrainHeight(x,y),y-World.Height/2),b=new Vec3(x-World.Width/2,lake?.1:World.TerrainHeight(x,y+10),y+10-World.Height/2),cc=new Vec3(x+10-World.Width/2,lake?.1:World.TerrainHeight(x+10,y+10),y+10-World.Height/2),d=new Vec3(x+10-World.Width/2,lake?.1:World.TerrainHeight(x+10,y),y-World.Height/2);
                AddFace(new Vec3[] { a,b,cc,d },c,false);
            }
            foreach(Depot d in World.Depots) if(d.Quantite>0) Sphere(new Vec3(d.X-World.Width/2,World.TerrainHeight(d.X,d.Y)+2,d.Y-World.Height/2),2,2,2,d.Kind==1?Color.DeepSkyBlue:d.Kind==3?Color.Silver:d.Kind==4?Color.FromArgb(45,42,43):d.Kind==5?Color.Gold:Color.FromArgb(153,100,58),6,4);
        }
        void FactionHat(Blob b,Vec3 center,double r)
        {
            if(!World.Rules.EcosystemEnabled || !World.Rules.TrumpFactionEnabled || b.FactionId!=1) return;
            if(b.HasOrangeCrest(World.Rules))
            {
                for(int i=0;i<5;i++) Sphere(center+new Vec3((i-2)*r*.25,r*(1.1+.16*Math.Sin(i)),0),r*.22,r*.43,r*.45,Color.FromArgb(255,143,31),7,5);
            }
            else
            {
                Vec3 front=new Vec3(Math.Cos(b.Heading),0,Math.Sin(b.Heading));
                Sphere(center+new Vec3(0,r*.99,0),r*.94,r*.36,r*.82,Color.FromArgb(210,25,35),12,5);
                Sphere(center+new Vec3(0,r*.88,0)+front*(r*.75),r*.7,r*.08,r*.65,Color.FromArgb(230,32,39),10,4);
            }
        }
        void HatLabels(Graphics g)
        {
            if(!World.Rules.EcosystemEnabled || !World.Rules.TrumpFactionEnabled) return;
            foreach(Blob b in World.Blobs) if(!b.Dead && b.FactionId==1 && (!World.UseCastles || !b.Home))
            {
                PointF p; double depth;
                if(Project(new Vec3(b.X-World.Width/2,World.TerrainHeight(b.X,b.Y)+b.Size*9,b.Y-World.Height/2),out p,out depth))
                using(Font font=new Font("Segoe UI",b.IsLeader?8:6,FontStyle.Bold))
                    g.DrawString(b.HasOrangeCrest(World.Rules)?"CHEF":"MAGA",font,Brushes.White,p.X-12,p.Y-8);
            }
        }
    }
    public static class EcosystemTests
    {
        static void Assert(bool b,string message) { if(!b) throw new Exception(message); Console.WriteLine("OK : "+message); }
        static double ContactDamage(double size,int weapon,double bonus)
        {
            World w=new World(new Rules(true) { RaidChance=1,WeaponBonus=bonus,LeaderEnabled=false }); w.Blobs.Clear(); w.Depots.Clear();
            Blob attacker=new Blob(1,size,1) { Id=100,FactionId=1,Weapon=weapon,X=150,Y=150,Energy=900 };
            Blob victim=new Blob(1,1,1) { Id=101,FactionId=0,X=150,Y=150,Energy=900 };
            w.Blobs.Add(attacker); w.Blobs.Add(victim); w.WorkerAction(attacker); return 900-victim.Energy;
        }
        public static void Run()
        {
            Assert(Math.Abs(ContactDamage(2,0,.6)-2*ContactDamage(1,0,.6))<.000001,"taille doublée : dégâts du raid individuel doublés");
            Assert(Math.Abs(ContactDamage(1,1,.6)-24)<.000001 && Math.Abs(ContactDamage(1,1,0)-15)<.000001,"bonus des armes réglable appliqué au raid individuel");
            World w=new World(new Rules(true)); Blob chief=null; int chiefs=0;
            foreach(Blob b in w.Blobs) if(b.IsLeader) { chief=b; chiefs++; }
            Assert(chiefs==1 && chief.FactionId==1,"un chef unique dans la faction trumpiste");
            Assert(chief.Food>=w.Rules.SurvivalFood && chief.Water>=w.Rules.WaterForLife && w.LeaderMeals==1,"rations réelles du chef prélevées dans les stocks");
            Blob guard=new Blob(1,1,1) { FactionId=1,X=150,Y=150,Energy=100 }; Blob attacker=new Blob(1,4,1) { X=150,Y=150,Energy=100 };
            chief.X=150; chief.Y=150; chief.Home=false; w.Blobs.Add(guard);
            Assert(w.ProtectedLeader(chief,attacker) && guard.Energy==92 && attacker.Energy==92,"protection locale avec coût pour le garde");
            guard.X=0; foreach(Blob b in w.Blobs) if(b!=chief && b.FactionId==1) b.X=0;
            Assert(!w.ProtectedLeader(chief,attacker),"chef isolé sans protection magique");
            Assert(!World.HasWater(new Blob(1,1,1),w.Rules),"eau obligatoire pour survivre");
            Assert(World.Lake(208,90) && !World.Lake(150,150) && World.HeightAt(92,80)>25,"lacs et montagnes sur la carte");
            Camp camp=new Camp(); camp.Stock[3]=2; camp.Stock[5]=1;
            Assert(!camp.Consume(0,0,0,2,0,2,0) && camp.Stock[3]==2,"recette atomique sans énergie suffisante");
            camp.Stock[5]=2; Assert(camp.Consume(0,0,0,2,0,2,0) && camp.Stock[3]==0,"consommation des ingrédients");
            chief.Dead=true; foreach(Blob b in w.Blobs) if(b!=chief) { b.Home=true; b.Food=2; b.Water=1; b.Energy=100; }
            w.FinishDay(); w.Step(); chiefs=0; foreach(Blob b in w.Blobs) if(b.IsLeader) chiefs++;
            Assert(chiefs==1,"succession à l'aube après la mort du chef");
            Blob successor=null; foreach(Blob b in w.Blobs) if(b.IsLeader) successor=b;
            Assert(successor!=null && successor!=chief && successor.HasOrangeCrest(w.Rules) && !chief.HasOrangeCrest(w.Rules),"le successeur reçoit la crête orange du rôle de chef");
            w=new World(new Rules(true)); for(int i=0;i<8 && w.Blobs.Count>0;i++) w.AdvanceDay();
            bool bounded=true; foreach(Camp c in w.Camps) foreach(int n in c.Stock) if(n<0 || n>c.Capacite) bounded=false;
            Assert(bounded,"stocks bornés sur huit journées simulées");
            w=new World(new Rules(true) { LeaderEnabled=false }); bool disabled=true; foreach(Blob b in w.Blobs) if(b.IsLeader) disabled=false; Assert(disabled,"chef désactivable dans les règles");
            w=new World(new Rules(true)); chief=null; foreach(Blob b in w.Blobs) if(b.IsLeader) chief=b;
            w.Blobs.Clear(); chief.Home=true; chief.Food=0; chief.Water=0; w.Blobs.Add(chief); w.FinishDay();
            Assert(w.Blobs.Count==0,"chef mortel sans nourriture ni eau");
            using(EconomyPanel panel=new EconomyPanel(w)) { panel.Size=new Size(1100,800); using(Bitmap b=new Bitmap(1100,800)) panel.DrawToBitmap(b,panel.ClientRectangle); }
        }
    }
    public static class BalancedWorldTests
    {
        static void Check(bool value,string text) { if(!value) throw new Exception(text); Console.WriteLine("OK : "+text); }
        public static void Run()
        {
            World w=new World(new Rules()); int[] totals=new int[4],females=new int[4]; int[,] jobs=new int[4,4];
            foreach(Blob b in w.Blobs) { totals[b.FactionId]++; if(b.Sex==Sex.Female) females[b.FactionId]++; jobs[b.FactionId,(int)b.Job]++; if(b.IsLeader || b.GuardLeaderId!=0 || b.Food!=1 || b.Water!=1) throw new Exception("Fondateur non neutre ou sans ration"); }
            for(int f=0;f<4;f++) { Check(totals[f]==50 && females[f]==25,"50 habitants et 25 femelles par faction"); for(int j=0;j<4;j++) Check(jobs[f,j]==(j<2?13:12),"métiers équilibrés entre factions"); }
            bool matched=true; for(int i=0;i<200;i+=4) for(int k=1;k<4;k++) { Blob a=w.Blobs[i],b=w.Blobs[i+k]; if(a.Speed!=b.Speed || a.Size!=b.Size || a.Sense!=b.Sense || Object.ReferenceEquals(a.Genes.A,b.Genes.A)) matched=false; }
            Check(matched,"profils génétiques initiaux identiques entre factions avec copies indépendantes");
            Check(w.Foods.Count==80 && w.Camps[1].Nom=="Corail" && !w.Rules.TrumpFactionEnabled,"scénario par défaut sans avantage MAGA et nourriture adaptée");
            for(int f=1;f<4;f++) for(int k=0;k<8;k++) Check(w.Camps[f].Stock[k]==w.Camps[0].Stock[k],"stocks initiaux égaux entre factions");
            bool symmetric=true; for(int x=0;x<=300;x+=13) for(int y=0;y<=300;y+=17) if(w.IsLake(x,y)!=w.IsLake(300-y,x) || Math.Abs(w.TerrainHeight(x,y)-w.TerrainHeight(300-y,x))>.000001) symmetric=false;
            Check(symmetric,"relief et lacs symétriques sous rotation de 90 degrés");
            World large=new World(Rules.Preset(6)); Check(large.Blobs.Count==400 && large.Foods.Count==160 && Math.Abs(large.Camps[0].Capacite-w.Camps[0].Capacite*2)<=1,"grand monde : populations, nourriture et capacité doublées");
            Check(Math.Abs(large.Depots[0].Quantite-w.Depots[0].Quantite*2)<=1,"gisements proportionnels à la population");
            World special=new World(Rules.Preset(5)); int chiefs=0; foreach(Blob b in special.Blobs) if(b.IsLeader) chiefs++;
            Check(chiefs==1 && special.Rules.TrumpFactionEnabled && special.Camps[1].Nom=="Trumpistes","scénario MAGA séparé avec son chef");
            World plain=new World(new Rules()); plain.UseSpatialIndex=false;
            for(int tick=0;tick<35;tick++) { w.Step(); plain.Step(); }
            bool same=true; for(int i=0;i<w.Blobs.Count;i++) { Blob a=w.Blobs[i],b=plain.Blobs[i]; if(a.X!=b.X || a.Y!=b.Y || a.Food!=b.Food || a.Dead!=b.Dead || a.Energy!=b.Energy || (a.Mate==null?0:a.Mate.Id)!=(b.Mate==null?0:b.Mate.Id)) same=false; }
            for(int i=0;i<w.Foods.Count;i++) if(w.Foods[i].Eaten!=plain.Foods[i].Eaten) same=false;
            Check(same,"recherche spatiale conserve les déplacements, rencontres et consommations du calcul complet");
            for(int i=0;i<7;i++) { Rules r=Rules.Preset(i); r.Validate(); Check(r.InitialPopulation%4==0 && (i==5 || !r.TrumpFactionEnabled),"scénario de base équilibré et valide"); }
            foreach(int size in new int[] {1,25,100}) {
                Rules r=new Rules(); r.FactionSize=size; r.Validate(); World resized=new World(r);
                int[] counts=new int[4]; foreach(Blob b in resized.Blobs) counts[b.FactionId]++;
                Check(counts[0]==size && counts[1]==size && counts[2]==size && counts[3]==size && r.FoodPerDay==(int)Math.Round(size*1.6),"réglage de taille et nourriture proportionnelle : "+size);
            }
            Rules high=new Rules { EcosystemEnabled=false }; high.FactionSize=100; World growing=new World(high);
            foreach(Blob b in growing.Blobs) { b.Home=true; b.Food=2; b.Energy=100; }
            Blob mother=null,father=null; foreach(Blob b in growing.Blobs) if(b.FactionId==0) { if(b.Sex==Sex.Female) mother=b; else father=b; }
            mother.Mate=father; father.Mate=mother; growing.FinishDay();
            Check(growing.Blobs.Count==401 && growing.History[0].Births==1,"croissance au-delà de 100 par faction autorisée");
            World storm=new World(new Rules { TornadoesEnabled=true });
            storm.Blobs.Clear(); Blob exposed=new Blob(1,1,1) { X=storm.TornadoX,Y=storm.TornadoY,Energy=10 },sheltered=new Blob(1,1,1) { X=storm.TornadoX,Y=storm.TornadoY,Energy=10,Home=true };
            storm.Blobs.Add(exposed); storm.Blobs.Add(sheltered); Food vulnerable=new Food(storm.TornadoX,storm.TornadoY); storm.Foods.Add(vulnerable);
            storm.StepWeather(); Check(exposed.Dead && !sheltered.Dead && sheltered.Energy==10 && vulnerable.Eaten && storm.TornadoDeaths==1,"tornade : dégâts réels, nourriture détruite et abris protégés");
            Check(!new World(new Rules()).TornadoActive,"tornades désactivées par défaut");
            sheltered.Food=1; sheltered.Water=1; sheltered.X=storm.Camps[sheltered.FactionId].X; sheltered.Y=storm.Camps[sheltered.FactionId].Y;
            Rules withoutStorm=storm.Rules.Copy(); withoutStorm.TornadoesEnabled=false; storm.QueueRules(withoutStorm); storm.FinishDay(); storm.Step();
            Check(!storm.TornadoActive,"désactivation des tornades appliquée à l'aube suivante");
            World unequal=new World(new Rules { UnequalResources=true }); int[] halves=new int[4];
            foreach(Food f in unequal.Foods) { if(f.X<150) halves[0]++; else halves[1]++; if(f.Y<150) halves[2]++; else halves[3]++; }
            Check(Array.IndexOf(halves,56)>=0 && unequal.Foods.Count==80,"répartition inégale : 70 % dans une moitié, quantité totale conservée");
            Rules weatherRules=new Rules { TornadoesEnabled=true,UnequalResources=true }; weatherRules.FactionSize=25;
            World repeatA=new World(weatherRules),repeatB=new World(weatherRules); bool repeat=true;
            for(int tick=0;tick<35;tick++) { repeatA.Step(); repeatB.Step(); }
            for(int i=0;i<repeatA.Blobs.Count;i++) if(repeatA.Blobs[i].X!=repeatB.Blobs[i].X || repeatA.Blobs[i].Energy!=repeatB.Blobs[i].Energy) repeat=false;
            Check(repeat && repeatA.TornadoX==repeatB.TornadoX && repeatA.TornadoY==repeatB.TornadoY,"tornades et ressources inégales reproductibles avec la même graine");
            string optionsFile=System.IO.Path.GetTempFileName(); try {
                Rules options=new Rules { TornadoesEnabled=true,UnequalResources=true }; options.FactionSize=25; options.Save(optionsFile); Rules loaded=Rules.Load(optionsFile);
                Check(loaded.TornadoesEnabled && loaded.UnequalResources && loaded.FactionSize==25 && loaded.FoodPerDay==40,"taille et options conservées en XML");
            } finally { System.IO.File.Delete(optionsFile); }
            using(MainWindow window=new MainWindow()) { window.RestartWithFactionSize(1); Check(window.World.Blobs.Count==4 && window.World.Rules.FoodPerDay==2,"commande rapide de taille applique les règles et redémarre"); }
            CastleWorldTests.Run();
        }
    }
    public static class CastleWorldTests
    {
        static void Check(bool ok,string message) { if(!ok) throw new Exception(message); Console.WriteLine("OK : "+message); }
        public static void Run()
        {
            foreach(int[] dimensions in new int[][] { new int[] {10,80},new int[] {80,10},new int[] {60,60},new int[] {30,30} }) {
                Rules rules=new Rules { MapColumns=dimensions[0],MapRows=dimensions[1],TornadoesEnabled=true }; rules.FactionSize=5; World world=new World(rules); bool bounds=true;
                foreach(Blob b in world.Blobs) if(b.X<0 || b.X>world.Width || b.Y<0 || b.Y>world.Height || world.HomeDistance(b)>0) bounds=false;
                foreach(Food f in world.Foods) if(f.X<0 || f.X>world.Width || f.Y<0 || f.Y>world.Height) bounds=false;
                foreach(Depot d in world.Depots) if(d.X<0 || d.X>world.Width || d.Y<0 || d.Y>world.Height) bounds=false;
                foreach(Camp c in world.Camps) if(world.IsLake(c.X,c.Y) || world.TerrainHeight(c.X,c.Y)!=0) bounds=false;
                Check(bounds && world.Width==dimensions[0]*10 && world.Height==dimensions[1]*10,"terrain et châteaux dans les limites : "+dimensions[0]+" × "+dimensions[1]);
                using(Arena arena=new Arena(world)) { arena.Size=new Size(700,500); using(Bitmap bitmap=new Bitmap(700,500)) arena.DrawToBitmap(bitmap,arena.ClientRectangle); }
            }
            bool rejected=false; try { new Rules { MapColumns=80,MapRows=80 }.Validate(); } catch(ArgumentException) { rejected=true; } Check(rejected,"limite de 3 600 cases validée");
            World night=new World(new Rules()); night.Blobs.Clear(); Blob returning=new Blob(1,1,1) { Id=1,FactionId=0,X=0,Y=0,Food=1,Water=1,Energy=900,Returning=true }; night.Blobs.Add(returning); night.Step();
            Check(!returning.Home && night.HomeDistance(returning)>0,"un bord ne remplace pas le château pour le retour");
            returning.X=night.Camps[0].X; returning.Y=night.Camps[0].Y; night.Step(); Check(returning.Home && night.Blobs.Count==1 && returning.X==night.Camps[0].X && returning.Y==night.Camps[0].Y,"entrée complète au château et survie nocturne");
            World wrong=new World(new Rules()); wrong.Blobs.Clear(); wrong.Blobs.Add(new Blob(1,1,1) { FactionId=0,X=wrong.Camps[1].X,Y=wrong.Camps[1].Y,Food=1,Water=1,Energy=100,Home=true }); wrong.FinishDay(); Check(wrong.Blobs.Count==0,"le château d'une autre faction ne valide pas la nuit");
            World deposit=new World(new Rules()); deposit.Blobs.Clear(); Blob worker=new Blob(1,1,1) { FactionId=0,Job=Profession.Mineur,X=deposit.Camps[0].X,Y=deposit.Camps[0].Y,Home=true,Food=3,Water=1,Energy=100 }; worker.Bag[0]=2; worker.Bag[6]=1; deposit.Blobs.Add(worker);
            int food=deposit.Camps[0].Stock[0],iron=deposit.Camps[0].Stock[6]; deposit.FinishDay(); Check(deposit.Camps[0].Stock[0]==food+4 && deposit.Camps[0].Stock[6]==iron+1,"surplus et ressources pillées déposés dans les stocks du château");
            World raid=new World(new Rules { Predation=false }); raid.Blobs.Clear(); Camp target=raid.Camps[1];
            raid.Blobs.Add(new Blob(1,1,1) { Id=1,FactionId=1,X=target.X,Y=target.Y,Energy=5000,Food=1,Water=1,Home=true });
            Blob attacker=new Blob(1,2,1) { Id=2,FactionId=0,X=target.X+15,Y=target.Y,Energy=5000,Food=1,Water=1 },ally=new Blob(1,1,1) { Id=3,FactionId=2,X=target.X+16,Y=target.Y,Energy=5000,Food=1,Water=1 };
            raid.Blobs.Add(attacker); raid.Blobs.Add(ally); raid.ChangeTrust(0,2,25); raid.ChangeTrust(0,1,-35); raid.ChangeTrust(2,1,-35); raid.PlanCoalitionRaids();
            Check(raid.RaidPlans.Count==1 && raid.RaidPlans[0].Castle && raid.RaidPlans[0].X==target.X,"coalition vise le château même si les habitants sont à l'abri");
            RaidPlan plan=raid.RaidPlans[0]; int before=0; foreach(int n in target.Stock) before+=n;
            Check(raid.TryCoalitionStrike(plan) && Math.Abs(target.Fortification-80)<.000001,"assaut réduit les défenses selon taille, armes et défenseurs");
            for(int strike=1;strike<=4;strike++) { raid.Tick=strike*45; raid.TryCoalitionStrike(plan); }
            int after=0; foreach(int n in target.Stock) after+=n; Check(target.Fortification==0 && after<before && Array.Exists(attacker.Bag,delegate(int amount) { return amount>0; }),"défenses épuisées : pillage réel des stocks, transport dans les sacs");
            World maga=new World(Rules.Preset(5)); maga.Blobs.Clear(); maga.Depots.Clear(); maga.Rules.RaidChance=1;
            Blob solo=new Blob(1,1,1) { Id=1,FactionId=1,RaidTarget=0,Job=Profession.Mineur,X=maga.Camps[0].X,Y=maga.Camps[0].Y,Energy=5000,Food=1,Water=1 }; maga.Blobs.Add(solo); maga.WorkerAction(solo);
            Check(maga.Raids==1 && maga.Camps[0].Fortification==85 && maga.Relations[0,1]==-6,"raid individuel MAGA vise les défenses du château adverse");
            Rules rectangle=new Rules { MapColumns=60,MapRows=20 }; World indexed=new World(rectangle),plain=new World(rectangle); plain.UseSpatialIndex=false;
            for(int tick=0;tick<40;tick++) { indexed.Step(); plain.Step(); } bool same=true;
            for(int i=0;i<indexed.Blobs.Count;i++) if(indexed.Blobs[i].X!=plain.Blobs[i].X || indexed.Blobs[i].Y!=plain.Blobs[i].Y || indexed.Blobs[i].Energy!=plain.Blobs[i].Energy) same=false;
            Check(same,"recherche spatiale correcte sur terrain rectangulaire");
            bool queued=false; try { indexed.QueueRules(new Rules { MapColumns=20,MapRows=20 }); } catch(ArgumentException) { queued=true; } Check(queued,"dimensions modifiées uniquement avec une nouvelle expérience");
            string path=System.IO.Path.GetTempFileName(); try { rectangle.Save(path); Rules loaded=Rules.Load(path); Check(loaded.MapColumns==60 && loaded.MapRows==20 && loaded.CastlesEnabled,"dimensions et châteaux sauvegardés en XML"); } finally { System.IO.File.Delete(path); }
            World factory=new World(new Rules()); factory.Blobs.Clear(); Camp workshop=factory.Camps[0]; workshop.Stock=new int[] {0,0,0,2,0,4,0,0};
            factory.Blobs.Add(new Blob(1,1,1) { FactionId=0,Job=Profession.Artisan,X=workshop.X,Y=workshop.Y,Home=true,Food=1,Water=1,Energy=100 }); factory.FinishDay();
            Check(workshop.Stock[7]==1 && workshop.ProducedToday[6]==1 && workshop.ProducedToday[7]==1 && workshop.Reserves==0,"minerai transformé en arme avant une construction de réserve inutile");
            World repair=new World(new Rules()); repair.Blobs.Clear(); Camp damaged=repair.Camps[0]; damaged.Stock=new int[8]; damaged.Fortification=50;
            repair.Blobs.Add(new Blob(1,1,1) { FactionId=0,Job=Profession.Mineur,X=damaged.X,Y=damaged.Y,Home=true,Food=1,Water=1,Energy=100 }); repair.FinishDay();
            Check(damaged.Fortification==50,"aucune réparation sans fer et énergie");
            repair.DayFinished=false; damaged.Stock[6]=1; damaged.Stock[5]=2; repair.FinishDay();
            Check(damaged.Fortification==53 && damaged.Stock[6]==0 && damaged.Stock[5]==0 && damaged.ConsumedToday[6]==1 && damaged.ConsumedToday[5]==2,"réparation de trois points payée avec un fer et deux énergies");
            World farming=new World(new Rules()); farming.Blobs.Clear(); Camp farm=farming.Camps[0]; farm.Stock=new int[] {0,1,2,0,0,1,0,0};
            farming.Blobs.Add(new Blob(1,1,1) { FactionId=0,Job=Profession.Agriculteur,X=farm.X,Y=farm.Y,Home=true,Food=1,Water=1,Energy=100 }); farming.FinishDay();
            Check(farm.ProducedToday[0]==3 && farm.ConsumedToday[1]==1 && farm.ConsumedToday[2]==2 && farm.ConsumedToday[5]==1,"agriculture : trois nourritures produites avec deux terres, une eau et une énergie");
            World collection=new World(new Rules()); collection.Blobs.Clear(); collection.Depots.Clear(); collection.Foods.Clear();
            Blob miner=new Blob(1,1,1) { FactionId=0,Job=Profession.Mineur,X=150,Y=150,Food=1,Water=1,Energy=900 }; collection.Blobs.Add(miner);
            Depot water=new Depot { X=150,Y=150,Kind=1,Quantite=10 },oreSource=new Depot { X=150,Y=150,Kind=3,Quantite=10 }; collection.Depots.Add(water); collection.Depots.Add(oreSource); collection.WorkerAction(miner);
            Check(miner.Bag[1]==0 && miner.Bag[3]==1 && water.Quantite==10 && oreSource.Quantite==9 && collection.VisualEvents[0].Resource==3,"mineur hydraté : récolte de minerai, pas de remplissage opportuniste avec de l'eau");
            miner.Bag[3]=3; miner.X=collection.Camps[0].X; miner.Y=collection.Camps[0].Y; miner.Returning=true; collection.Step();
            int depositEvents=0; foreach(VisualEvent visual in collection.VisualEvents) if(visual.Kind==4 && visual.Resource==3 && visual.Amount==3) depositEvents++;
            Check(miner.Bag[3]==0 && depositEvents==1,"déchargement réel au château : sac vidé et animation moins trois, sans doublon au bilan");
            World couple=new World(new Rules()); couple.Blobs.Clear(); Blob female=new Blob(1,1,1) { Sex=Sex.Female,X=150,Y=150,Food=3,Water=1,Energy=100 },male=new Blob(1,1,1) { Sex=Sex.Male,X=151,Y=150,Food=3,Water=1,Energy=100 }; couple.Blobs.Add(female); couple.Blobs.Add(male);
            Check(couple.TryMate(female,male) && !couple.TryMate(female,male) && couple.VisualEvents.Count==1 && couple.VisualEvents[0].Kind==3,"un cœur unique à la formation du couple");
            World expedition=new World(Rules.Preset(5)); Blob raider=null; foreach(Blob b in expedition.Blobs) if(b.RaidTarget>=0) raider=b;
            Check(raider!=null,"combattants MAGA affectés dès l'aube à une expédition accessible");
            raider.Food=2; female.X=raider.X; female.Y=raider.Y; female.Sex=raider.Sex==Sex.Female?Sex.Male:Sex.Female; Check(!expedition.TryMate(raider,female),"une mission de raid n'est pas interrompue par la reproduction");
        }
    }
    public partial class HistoryPlot
    {
        public int ResourceKind=-1;
        void DrawResourceHistory(Graphics g)
        {
            int kind=ResourceKind; List<ResourceSample> samples=World.ResourceHistory;
            RectangleF area=new RectangleF(55,32,Math.Max(10,Width-80),Math.Max(10,Height-62)); int current=World.MapResourceQuantities()[kind];
            g.DrawString(ResourceDisplay.Names[kind].ToUpperInvariant()+" DISPONIBLE SUR LA CARTE  ·  "+current+" unités",Font,Brushes.DarkSlateGray,area.Left,6);
            double max=1; foreach(ResourceSample sample in samples) max=Math.Max(max,sample.Quantities[kind]); max=Math.Ceiling(max*1.1);
            using(Pen grid=new Pen(Color.FromArgb(225,232,239))) for(int i=0;i<=2;i++) { float y=area.Bottom-area.Height*i/2; g.DrawLine(grid,area.Left,y,area.Right,y); g.DrawString((max*i/2).ToString("0"),Font,Brushes.SlateGray,3,y-8); }
            if(samples.Count<2) { g.DrawString("La courbe se dessine pendant la simulation.",Font,Brushes.SlateGray,area.Left,area.Top+12); return; }
            long first=samples[0].Step,last=samples[samples.Count-1].Step; double span=Math.Max(1,last-first);
            PointF[] points=new PointF[samples.Count];
            for(int i=0;i<points.Length;i++) points[i]=new PointF(area.Left+(float)((samples[i].Step-first)/span)*area.Width,area.Bottom-(float)(samples[i].Quantities[kind]/max)*area.Height);
            using(Pen pen=new Pen(ResourceDisplay.ColorFor(kind),2)) g.DrawLines(pen,points);
            g.DrawString("Jour "+samples[0].Day+" · pas "+samples[0].Tick,Font,Brushes.SlateGray,area.Left,area.Bottom+4);
            g.DrawString("Jour "+samples[samples.Count-1].Day+" · pas "+samples[samples.Count-1].Tick,Font,Brushes.SlateGray,Math.Max(area.Left,area.Right-165),area.Bottom+4);
        }
    }
    public class EconomyPanel : Control
    {
        readonly World world;
        public EconomyPanel(World w) { world=w; Dock=DockStyle.Fill; DoubleBuffered=true; BackColor=Color.FromArgb(24,34,48); Font=new Font("Segoe UI",10); }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g=e.Graphics; g.Clear(BackColor); int width=Math.Max(210,(Width-40)/4);
            for(int f=0;f<4;f++) { Camp c=world.Camps[f]; int x=16+f*width,y=20; g.DrawString(c.Nom+" — réserves : "+c.Reserves+(world.UseCastles?" · château "+c.Fortification.ToString("0")+" %":""),Font,Brushes.White,x,y); y+=30;
                int count=0,armed=0; Blob leader=null; foreach(Blob b in world.Blobs) if(!b.Dead && b.FactionId==f) { count++; if(b.Weapon>0) armed++; if(b.IsLeader) leader=b; }
                g.DrawString(count+" habitants · "+armed+" armés"+(leader==null?"":" · chef #"+leader.Id),Font,Brushes.White,x,y); y+=30;
                for(int k=0;k<8;k++) { g.DrawString(((Ressource)k)+" : "+c.Stock[k]+" / "+c.Capacite,Font,Brushes.White,x,y); y+=23; g.FillRectangle(Brushes.DimGray,x,y,width-25,6); using(Brush b=new SolidBrush(Appearance.Skins[f])) g.FillRectangle(b,x,y,(width-25)*c.Stock[k]/(float)c.Capacite,6); y+=18; }
                g.DrawString("Dernier soir : fer +"+c.LastProduced[6]+" · armes +"+c.LastProduced[7]+"\nCharbon utilisé : "+c.LastConsumed[4]+"\nAssauts reçus : "+c.Assaults+" · min. "+c.LowestFortification.ToString("0")+" %",Font,Brushes.Gold,new RectangleF(x,y+4,width-18,75));
            }
            int yy=510; string text="CHAÎNES DE PRODUCTION (par camp, chaque soir)\nAgriculteur : 2 terre + "+world.Rules.AgricultureWater+" eau + 1 énergie → 3 nourriture\nArtisan / transporteur : 1 charbon → 4 énergie ; 2 minerai + 2 énergie → 1 fer\nArtisan : "+world.ResourceAmount(2)+" fer + "+world.ResourceAmount(2)+" énergie → réserve (+"+world.ResourceAmount(15)+" capacité) ; 1 fer + 2 énergie → arme\nMontagnes : mouvement ralenti et plus coûteux. Lacs : obstacles ; nourriture engloutie.\n"+(world.Rules.TrumpFactionEnabled?"Mode MAGA : chef orange protégé ; casquettes MAGA, collecte accrue et raids.":"Mode équilibré : mêmes règles pour les quatre factions, aucun chef ni bonus de collecte.")+"\nRaids : "+world.Raids+" · nourriture coulée aujourd'hui : "+world.SunkFood+" · morts de soif : "+world.ThirstDeaths+"\nRepas du chef : "+world.LeaderMeals+" · protections : "+world.Protections+"\nConfiance envers "+world.Camps[1].Nom+" : bleu "+world.Relations[0,1]+", vert "+world.Relations[2,1]+", violet "+world.Relations[3,1]+" (échanges interrompus à −20)";
            g.DrawString(text,Font,Brushes.White,new RectangleF(16,yy,Width-32,320));
        }
    }
}
