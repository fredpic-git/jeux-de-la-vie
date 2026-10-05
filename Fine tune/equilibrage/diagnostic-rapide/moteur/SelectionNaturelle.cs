using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace SelectionNaturelle
{
    public partial class Rules
    {
        [Category("1. Environnement"), DisplayName("Nourriture par jour (total)"), Description("Total distribué sur le plateau, hors réserves et agriculture. Le modèle standard fournit 80 nourritures pour 200 habitants ; cette valeur reste réglable explicitement.")]
        public int FoodPerDay { get; set; }
        [Category("1. Environnement"), DisplayName("Durée maximale du jour"), Description("En pas de simulation. Les créatures encore hors de leur château à la nuit meurent (hors des bords si les châteaux sont désactivés).")]
        public int DayLength { get; set; }
        [Category("1. Environnement"), DisplayName("Énergie au réveil"), Description("Budget identique pour chaque créature, renouvelé chaque matin. Manger ne recharge pas ce budget.")]
        public double StartingEnergy { get; set; }
        [Category("1. Environnement"), DisplayName("Baisse de nourriture"), Description("Ressources retirées à chaque intervalle. 0 désactive la raréfaction.")]
        public int FoodDecrease { get; set; }
        [Category("1. Environnement"), DisplayName("Intervalle de baisse (jours)")]
        public int DecreaseInterval { get; set; }
        [Category("1. Environnement"), DisplayName("Plancher de nourriture")]
        public int MinimumFood { get; set; }
        [Category("2. Survie et reproduction"), DisplayName("Nourriture pour survivre"), Description("Il faut aussi rentrer vivant dans son château ; sans châteaux, rentrer au bord.")]
        public int SurvivalFood { get; set; }
        [Category("2. Survie et reproduction"), DisplayName("Nourriture par parent"), Description("Chaque parent doit posséder ce nombre de ressources au moment de la rencontre. Un mâle et une femelle donnent un bébé si tous deux rentrent vivants.")]
        public int ReproductionFood { get; set; }
        [Category("2. Survie et reproduction"), DisplayName("Distance de rencontre"), Description("Distance maximale entre un mâle et une femelle sur le terrain. Un seul couple et un bébé par couple et par jour.")]
        public double MatingDistance { get; set; }
        [Category("5. Nouvelle expérience"), DisplayName("Proportion de femelles"), Description("Entre 0 et 1. Proportion initiale, arrondie à l'individu le plus proche ; probabilité d'être femelle pour chaque bébé.")]
        public double FemaleRatio { get; set; }
        [Category("2. Survie et reproduction"), DisplayName("Marge pour le retour"), Description("Multiplicateur du coût estimé pour rentrer. Plus élevé : retour plus prudent après le seuil de survie.")]
        public double ReturnMargin { get; set; }
        [Category("2. Survie et reproduction"), DisplayName("Prédation activée")]
        public bool Predation { get; set; }
        [Category("2. Survie et reproduction"), DisplayName("Rapport de taille prédateur"), Description("1,2 signifie qu'il faut être au moins 20 % plus grand pour manger une autre créature.")]
        public double PredatorRatio { get; set; }
        [Category("3. Coûts et perception"), DisplayName("Coefficient du mouvement")]
        public double MovementCost { get; set; }
        [Category("3. Coûts et perception"), DisplayName("Exposant de la taille")]
        public double SizePower { get; set; }
        [Category("3. Coûts et perception"), DisplayName("Exposant de la vitesse")]
        public double SpeedPower { get; set; }
        [Category("3. Coûts et perception"), DisplayName("Coefficient de perception")]
        public double SenseCost { get; set; }
        [Category("3. Coûts et perception"), DisplayName("Portée de base"), Description("Portée = cette valeur × perception, en unités du terrain (10 unités par case).")]
        public double SenseRadius { get; set; }
        [Category("4. Mutations"), DisplayName("Mutation de la vitesse")]
        public bool MutateSpeed { get; set; }
        [Category("4. Mutations"), DisplayName("Mutation de la taille")]
        public bool MutateSize { get; set; }
        [Category("4. Mutations"), DisplayName("Mutation de la perception")]
        public bool MutateSense { get; set; }
        [Category("4. Mutations"), DisplayName("Probabilité par copie de gène"), Description("Entre 0 et 1. Appliquée à chaque copie héritée d'un gène actif. 0,0025 correspond à environ 5 % de bébés mutés par trait (20 copies).")]
        public double MutationChance { get; set; }
        [Category("4. Mutations"), DisplayName("Amplitude sur un marqueur"), Description("Ajout ou retrait sur une copie de gène, avant le calcul du trait. Les effets des dix gènes sont combinés ; le changement visible est donc généralement plus petit.")]
        public double MutationAmount { get; set; }
        [Category("6. Génétique"), DisplayName("Brassage entre gènes"), Description("Probabilité de changer de chromosome parental entre deux gènes voisins lors de la production d'un gamète. 0 conserve un chromosome entier ; 0,5 donne un brassage fort.")]
        public double RecombinationChance { get; set; }
        [Category("6. Génétique"), DisplayName("Force de dominance"), Description("Entre 0 et 1. Certains gènes favorisent la plus grande ou la plus petite des deux variantes. 0 donne des effets purement additifs.")]
        public double DominanceStrength { get; set; }
        [Category("5. Nouvelle expérience"), DisplayName("Diversité génétique initiale"), Description("Amplitude des différences entre copies de gènes des fondateurs. 0 crée des fondateurs génétiquement identiques pour les trois traits.")]
        public double InitialDiversity { get; set; }
        [Category("5. Nouvelle expérience"), DisplayName("Population initiale totale"), Description("200 signifie 50 habitants par faction ; 400 signifie 100 par faction. Utilisez Habitants par faction pour adapter aussi la nourriture.")]
        public int InitialPopulation { get; set; }
        [XmlIgnore,Category("5. Nouvelle expérience"),DisplayName("Habitants par faction (1–100)"),Description("Change les quatre effectifs et adapte proportionnellement la nourriture quotidienne, son plancher et sa baisse. Une nouvelle expérience est nécessaire.")]
        public int FactionSize {
            get { return Math.Max(1,InitialPopulation/4); }
            set {
                Range(value,1,100,"Habitants par faction"); int old=Math.Max(1,InitialPopulation),total=value*4;
                FoodPerDay=(int)Math.Round(FoodPerDay*(double)total/old);
                MinimumFood=(int)Math.Round(MinimumFood*(double)total/old);
                FoodDecrease=(int)Math.Round(FoodDecrease*(double)total/old);
                InitialPopulation=total;
            }
        }
        [Category("5. Nouvelle expérience"), DisplayName("Vitesse initiale")]
        public double InitialSpeed { get; set; }
        [Category("5. Nouvelle expérience"), DisplayName("Taille initiale")]
        public double InitialSize { get; set; }
        [Category("5. Nouvelle expérience"), DisplayName("Perception initiale")]
        public double InitialSense { get; set; }
        [Category("5. Nouvelle expérience"), DisplayName("Graine aléatoire"), Description("Même graine, mêmes règles et même suite de changements : même expérience, quelle que soit la vitesse d'affichage.")]
        public int Seed { get; set; }

        public Rules()
        {
            FoodPerDay=80; DayLength=600; StartingEnergy=800; MinimumFood=60; DecreaseInterval=2;
            SurvivalFood=1; ReproductionFood=2; MatingDistance=8; FemaleRatio=.5; ReturnMargin=2; Predation=true; PredatorRatio=1.2;
            MovementCost=1; SizePower=3; SpeedPower=2; SenseCost=1; SenseRadius=25;
            MutateSpeed=MutateSize=MutateSense=true; MutationChance=.0025; MutationAmount=.2;
            RecombinationChance=.12; DominanceStrength=.35; InitialDiversity=.4;
            InitialPopulation=200; InitialSpeed=InitialSize=InitialSense=1; Seed=42;
        }
        internal Rules(bool legacy) : this()
        {
            if(legacy) { FoodPerDay=100; MinimumFood=10; InitialPopulation=25; BalancedFounders=false; SymmetricMap=false; ScaleResources=false; TrumpFactionEnabled=true; LeaderEnabled=true; RaidChance=.2; DiplomaticContactsEnabled=false; CastlesEnabled=false; }
        }
        public Rules Copy() { return (Rules)MemberwiseClone(); }
        static void Range(double v, double lo, double hi, string name) { if(double.IsNaN(v) || double.IsInfinity(v) || v<lo || v>hi) throw new ArgumentException(name+" : entre "+lo+" et "+hi+"."); }
        public void Validate()
        {
            Range(FoodPerDay,0,20000,"Nourriture"); Range(DayLength,20,5000,"Durée du jour"); Range(StartingEnergy,10,10000,"Énergie");
            Range(FoodDecrease,0,1000,"Baisse de nourriture"); Range(DecreaseInterval,1,1000,"Intervalle"); Range(MinimumFood,0,20000,"Plancher");
            if(FoodDecrease>0 && MinimumFood>FoodPerDay) throw new ArgumentException("Le plancher doit être inférieur ou égal à la nourriture initiale.");
            Range(SurvivalFood,1,10,"Seuil de survie"); Range(ReproductionFood,2,20,"Seuil de reproduction");
            if(ReproductionFood<SurvivalFood) throw new ArgumentException("Le seuil de reproduction doit être supérieur ou égal au seuil de survie.");
            Range(MatingDistance,1,30,"Distance de rencontre"); Range(FemaleRatio,0,1,"Proportion de femelles");
            Range(ReturnMargin,1,5,"Marge de retour"); Range(PredatorRatio,1.01,3,"Rapport de taille");
            Range(MovementCost,0,10,"Coût de mouvement"); Range(SizePower,0,5,"Exposant de taille"); Range(SpeedPower,0,5,"Exposant de vitesse");
            Range(SenseCost,0,10,"Coût de perception"); Range(SenseRadius,1,150,"Portée"); Range(MutationChance,0,1,"Probabilité"); Range(MutationAmount,0,1,"Amplitude");
            Range(RecombinationChance,0,.5,"Brassage"); Range(DominanceStrength,0,1,"Dominance"); Range(InitialDiversity,0,2,"Diversité initiale");
            Range(InitialPopulation,1,400,"Population initiale (maximum 100 par faction)"); Range(InitialSpeed,.2,4,"Vitesse initiale"); Range(InitialSize,.2,4,"Taille initiale"); Range(InitialSense,.2,4,"Perception initiale");
            ValidateEcosystem();
        }
        public string Formula { get { return string.Format(CultureInfo.CurrentCulture,"Coût / pas = {0} × taille^{1} × vitesse^{2} + {3} × perception",MovementCost,SizePower,SpeedPower,SenseCost); } }
        public static Rules Preset(int index)
        {
            Rules r=new Rules();
            if(index==0) r.MutateSpeed=r.MutateSize=r.MutateSense=false;
            if(index==1) { r.MutateSize=r.MutateSense=false; }
            if(index==3) { r.FoodDecrease=4; r.DecreaseInterval=2; r.MinimumFood=20; }
            if(index==4) { r.FoodPerDay=40; r.MinimumFood=40; }
            if(index==5) { r.TrumpFactionEnabled=true; r.LeaderEnabled=true; r.RaidChance=.2; }
            if(index==6) { r.FactionSize=100; r.PermanentBlocs=true; }
            return r;
        }
        public void Save(string path)
        {
            Validate(); using(XmlWriter w=XmlWriter.Create(path,new XmlWriterSettings { Indent=true, Encoding=new UTF8Encoding(false) })) new XmlSerializer(typeof(Rules)).Serialize(w,this);
        }
        public static Rules Load(string path)
        {
            if(new FileInfo(path).Length>65536) throw new ArgumentException("Le fichier de règles est trop volumineux.");
            using(XmlReader r=XmlReader.Create(path,new XmlReaderSettings { DtdProcessing=DtdProcessing.Prohibit, XmlResolver=null }))
            { Rules rules=(Rules)new XmlSerializer(typeof(Rules)).Deserialize(r); rules.Validate(); return rules; }
        }
    }

    public enum Sex { Male, Female }
    public class Genome
    {
        public const int Traits=3, GenesPerTrait=10, GeneCount=Traits*GenesPerTrait;
        public double[] A=new double[GeneCount],B=new double[GeneCount];
        public int SkinA,SkinB;
        public static double Clamp(double value) { return Math.Max(.2,Math.Min(4,value)); }
        public static Genome Uniform(double speed,double size,double sense,int skin)
        {
            Genome genome=new Genome { SkinA=skin,SkinB=skin }; double[] traits=new double[] { speed,size,sense };
            for(int i=0;i<GeneCount;i++) genome.A[i]=genome.B[i]=traits[i/GenesPerTrait]; return genome;
        }
        public static Genome Founder(Random random,Rules rules,int side)
        {
            Genome genome=Uniform(rules.InitialSpeed,rules.InitialSize,rules.InitialSense,side);
            for(int i=0;i<GeneCount;i++) { genome.A[i]=Clamp(genome.A[i]+(random.NextDouble()*2-1)*rules.InitialDiversity); genome.B[i]=Clamp(genome.B[i]+(random.NextDouble()*2-1)*rules.InitialDiversity); }
            return genome;
        }
        public double Trait(int trait,double dominance)
        {
            double value=0,total=0;
            for(int gene=0;gene<GenesPerTrait;gene++)
            {
                int i=trait*GenesPerTrait+gene; double weight=1+(gene/2)%3;
                double expressed=(A[i]+B[i])/2;
                expressed+=(gene%2==0?1:-1)*dominance*Math.Abs(A[i]-B[i])/2;
                value+=expressed*weight; total+=weight;
            }
            return Clamp(value/total);
        }
        public static Genome Child(Genome mother,Genome father,Random random,Rules rules)
        {
            Genome child=new Genome();
            Gamete(mother,child.A,random,rules,out child.SkinA); Gamete(father,child.B,random,rules,out child.SkinB);
            return child;
        }
        static void Gamete(Genome parent,double[] destination,Random random,Rules rules,out int skin)
        {
            skin=0;
            for(int trait=0;trait<Traits;trait++)
            {
                bool useA=random.Next(2)==0;
                // The skin marker is linked to the start of the first chromosome.
                if(trait==0) skin=useA?parent.SkinA:parent.SkinB;
                bool mutate=trait==0?rules.MutateSpeed:trait==1?rules.MutateSize:rules.MutateSense;
                for(int gene=0;gene<GenesPerTrait;gene++)
                {
                    if(gene>0 && random.NextDouble()<rules.RecombinationChance) useA=!useA;
                    int i=trait*GenesPerTrait+gene; double allele=useA?parent.A[i]:parent.B[i];
                    if(mutate && random.NextDouble()<rules.MutationChance) allele+=(random.Next(2)==0?-1:1)*rules.MutationAmount;
                    destination[i]=Clamp(allele);
                }
            }
        }
        public string Summary(int trait)
        {
            StringBuilder s=new StringBuilder(); for(int gene=0;gene<GenesPerTrait;gene++) { if(gene>0) s.Append("  "); int i=trait*GenesPerTrait+gene; s.AppendFormat(CultureInfo.CurrentCulture,"{0:F2}/{1:F2}",A[i],B[i]); } return s.ToString();
        }
    }
    public static class Appearance
    {
        public static readonly Color[] Skins=new Color[] { Color.FromArgb(94,178,241),Color.FromArgb(244,129,107),Color.FromArgb(153,208,111),Color.FromArgb(197,148,236) };
        public static readonly string[] Names=new string[] { "bleu (ouest)","corail (est)","vert (nord)","violet (sud)" };
        public static double EyeScale(double sense) { return Math.Max(.5,Math.Min(2.5,Math.Pow(sense,.8))); }
        public static double BodyRadius(double size) { return size*4; }
        public static int PrimarySkin(Genome genes) { return Math.Min(genes.SkinA,genes.SkinB); }
        public static int SecondarySkin(Genome genes) { return Math.Max(genes.SkinA,genes.SkinB); }
        public static string SkinName(Genome genes) { return genes.SkinA==genes.SkinB?Names[genes.SkinA]:Names[PrimarySkin(genes)]+" + "+Names[SecondarySkin(genes)]; }
    }
    public partial class Blob
    {
        public int Id, Food;
        public double X,Y,Heading,Energy,Speed,Size,Sense;
        public bool Dead,Home,Returning;
        public Sex Sex;
        public Blob Mate;
        public int MotherId,FatherId;
        public int StartingSide;
        public Genome Genes;
        public Blob(double speed,double size,double sense) { Speed=speed; Size=size; Sense=sense; Genes=Genome.Uniform(speed,size,sense,0); }
        public void Express(Rules rules) { Speed=Genes.Trait(0,rules.DominanceStrength); Size=Genes.Trait(1,rules.DominanceStrength); Sense=Genes.Trait(2,rules.DominanceStrength); }
    }
    public class Food { public double X,Y; public bool Eaten; public Food(double x,double y) { X=x; Y=y; } }
    public class DayRecord
    {
        public int Day,Population,Births,Deaths,Predations,Capped,Food,Males,Females,Matings;
        public double Speed,Size,Sense;
        public int[] SkinCopies=new int[4],SkinCarriers=new int[4],PureSkins=new int[4];
        public int Mixed;
        public Rules Rules;
    }
    public partial class World
    {
        public const double Extent=300;
        public const int PopulationLimit=2000;
        public List<Blob> Blobs=new List<Blob>();
        public List<Food> Foods=new List<Food>();
        public List<DayRecord> History=new List<DayRecord>();
        public Rules Rules { get; private set; }
        public Rules Pending { get; private set; }
        public int Day=1,Tick,FoodToday,Predations,Matings;
        public bool DayFinished;
        Random random;
        int nextId,ruleStartDay=1;
        public World(Rules rules) { Reset(rules); }
        public void Reset(Rules rules)
        {
            rules.Validate(); Rules=rules.Copy(); Pending=null; random=new Random(Rules.Seed); nextId=1; ruleStartDay=1;
            Day=1; Tick=0; Blobs.Clear(); History.Clear(); Foods.Clear();
            ResetEcosystem();
            Genome matched=null; int sideOffset=Rules.BalancedFounders && Rules.InitialPopulation%4!=0?random.Next(4):0;
            for(int i=0;i<Rules.InitialPopulation;i++)
            {
                int side=(i+sideOffset)%4; Blob founder=NewBlob(Rules.InitialSpeed,Rules.InitialSize,Rules.InitialSense);
                founder.StartingSide=side; founder.FactionId=side; founder.Job=(Profession)((i/4)%4); if(!Rules.BalancedFounders || i%4==0) matched=Genome.Founder(random,Rules,side); founder.Genes=new Genome { A=(double[])matched.A.Clone(),B=(double[])matched.B.Clone(),SkinA=side,SkinB=side }; founder.Express(Rules); Blobs.Add(founder);
            }
            if(Rules.BalancedFounders) BalanceFounderSexes(); else {
            int females=(int)Math.Round(Rules.InitialPopulation*Rules.FemaleRatio,MidpointRounding.AwayFromZero);
            for(int i=0;i<Blobs.Count;i++) Blobs[i].Sex=i<females?Sex.Female:Sex.Male;
            for(int i=Blobs.Count-1;i>0;i--) { int j=random.Next(i+1); Sex sex=Blobs[i].Sex; Blobs[i].Sex=Blobs[j].Sex; Blobs[j].Sex=sex; }
            }
            BeginDay();
        }
        public void QueueRules(Rules rules) { rules.Validate(); if(rules.MapColumns!=Rules.MapColumns || rules.MapRows!=Rules.MapRows) throw new ArgumentException("Pour changer les dimensions du terrain, choisissez Nouvelle expérience."); Pending=rules.Copy(); }
        Blob NewBlob(double speed,double size,double sense) { return new Blob(speed,size,sense) { Id=nextId++,Sex=random.NextDouble()<Rules.FemaleRatio?Sex.Female:Sex.Male }; }
        public static bool CanMate(Blob a,Blob b,Rules r)
        { return a.RaidTarget<0 && b.RaidTarget<0 && a!=b && a.Sex!=b.Sex && !a.Dead && !b.Dead && !a.Home && !b.Home && !a.Returning && !b.Returning && a.Mate==null && b.Mate==null && a.Energy>0 && b.Energy>0 && a.Food>=FoodGoal(a,r) && b.Food>=FoodGoal(b,r) && HasWater(a,r) && HasWater(b,r); }
        public bool TryMate(Blob a,Blob b)
        {
            if(a.RaidTarget>=0 || b.RaidTarget>=0) return false;
            if(!CanMate(a,b,Rules) || Distance(a.X,a.Y,b.X,b.Y)>Rules.MatingDistance) return false;
            a.Mate=b; b.Mate=a; a.Returning=b.Returning=true; Matings++; VisualAction(a,3); return true;
        }
        public static double Cost(Blob b, Rules r) { return r.MovementCost*Math.Pow(b.Size,r.SizePower)*Math.Pow(b.Speed,r.SpeedPower)+r.SenseCost*b.Sense; }
        public static bool CanEat(Blob hunter, Blob prey, Rules r) { return r.Predation && hunter!=prey && hunter.Mate!=prey && !CanMate(hunter,prey,r) && !hunter.Dead && !hunter.Home && !prey.Dead && !prey.Home && Strength(hunter,r)>=Defense(prey,r)*r.PredatorRatio; }
        public static double Distance(double x,double y,double xx,double yy) { double dx=xx-x,dy=yy-y; return Math.Sqrt(dx*dx+dy*dy); }
        public double HomeDistance(Blob b) { return UseCastles?Math.Max(0,Distance(b.X,b.Y,Camps[b.FactionId].X,Camps[b.FactionId].Y)-CastleRadius):Math.Min(Math.Min(b.X,Width-b.X),Math.Min(b.Y,Height-b.Y)); }
        void BeginDay()
        {
            if(Pending!=null) { Rules=Pending; Pending=null; ruleStartDay=Day; }
            PositionCastles();
            Tick=0; Predations=Matings=0; DayFinished=false;
            FoodToday=Rules.FoodPerDay;
            if(Rules.FoodDecrease>0) FoodToday=Math.Max(Rules.MinimumFood, Rules.FoodPerDay-((Day-ruleStartDay)/Rules.DecreaseInterval)*Rules.FoodDecrease);
            SpawnFoods();
            StartWeatherDay();
            BeginEcosystemDay();
            foreach(Blob b in Blobs)
            {
                b.Express(Rules);
                b.Dead=b.Home=b.Returning=false; b.Mate=null; b.RaidTarget=-1; b.Food=0; b.Energy=Rules.StartingEnergy;
                WakeWorker(b);
                double t=random.NextDouble()*Extent;
                switch(Day==1?b.StartingSide:random.Next(4))
                {
                    case 0: b.X=0; b.Y=t; b.Heading=0; break;
                    case 1: b.X=Extent; b.Y=t; b.Heading=Math.PI; break;
                    case 2: b.X=t; b.Y=0; b.Heading=Math.PI/2; break;
                    default: b.X=t; b.Y=Extent; b.Heading=-Math.PI/2; break;
                }
            }
            foreach(Blob b in Blobs) { b.X=b.X*Width/300; b.Y=b.Y*Height/300; } if(Day==1 && Rules.BalancedFounders) PositionFounders(); BuildFoodIndex();
            if(UseCastles) PositionCastleResidents();
            PlanCoalitionRaids();
            PlanMagaRaids();
            SampleResources(true);
        }
        double HomeHeading(Blob b)
        {
            if(UseCastles) return Math.Atan2(Camps[b.FactionId].Y-b.Y,Camps[b.FactionId].X-b.X);
            double d=HomeDistance(b);
            if(d==b.X) return Math.PI; if(d==Width-b.X) return 0; if(d==b.Y) return -Math.PI/2; return Math.PI/2;
        }
        public void Step()
        {
            if(Blobs.Count==0) return;
            if(DayFinished) { Day++; BeginDay(); }
            // Random order each step avoids systematic priority for older individuals.
            List<Blob> order=new List<Blob>(Blobs);
            for(int i=order.Count-1;i>0;i--) { int j=random.Next(i+1); Blob tmp=order[i]; order[i]=order[j]; order[j]=tmp; }
            BuildBlobIndex(order);
            foreach(Blob b in order)
            {
                if(b.Dead || b.Home) continue; if(UseCastles && b.RaidTarget>=0 && Tick>=Rules.DayLength*.65) b.Returning=true;
                double cost=Cost(b,Rules)*MovementMultiplier(b); if(b.Energy<cost || b.Energy<=0) { b.Dead=true; continue; }
                if(b.RaidTarget<0 && !b.Returning && b.Mate==null && b.Food>=Rules.ReproductionFood && HasWater(b,Rules)) foreach(Blob other in NearbyBlobs(b,Rules.MatingDistance)) if(TryMate(b,other)) break;
                double dHome=HomeDistance(b), timeHome=dHome/(b.Speed/MovementMultiplier(b));
                if(b.Mate!=null || (b.Food>=Rules.SurvivalFood && HasWater(b,Rules) && (b.Energy<=timeHome*cost*ReturnSafety(b) || Rules.DayLength-Tick<=timeHome*ReturnSafety(b)+1))) b.Returning=true;
                double reach=Rules.SenseRadius*b.Sense;
                Blob threat=null,prey=null,partner=null; Food target=null; double nearestThreat=reach,nearest=reach,nearestPartner=reach;
                foreach(Blob other in NearbyBlobs(b,reach))
                {
                    if(other==b || other.Dead || other.Home) continue;
                    double d=Distance(b.X,b.Y,other.X,other.Y);
                    if(CanHunt(other,b) && d<=nearestThreat) { threat=other; nearestThreat=d; }
                    if(CanMate(b,other,Rules) && d<=nearestPartner) { partner=other; nearestPartner=d; }
                    if(!b.Returning && b.Food<FoodGoal(b,Rules) && CanHunt(b,other) && d<=nearest) { prey=other; target=null; nearest=d; }
                }
                if(!b.Returning && b.Food<FoodGoal(b,Rules)) foreach(Food food in NearbyFoods(b,reach))
                {
                    if(food.Eaten) continue; double d=Distance(b.X,b.Y,food.X,food.Y);
                    if(d<=nearest) { target=food; prey=null; nearest=d; }
                }
                double workHeading; bool work=WorkDirection(b,out workHeading);
                if(threat!=null) b.Heading=Math.Atan2(b.Y-threat.Y,b.X-threat.X);
                else if(b.Returning) b.Heading=HomeHeading(b);
                else if(work) b.Heading=workHeading;
                else if(partner!=null) b.Heading=Math.Atan2(partner.Y-b.Y,partner.X-b.X);
                else if(target!=null) b.Heading=Math.Atan2(target.Y-b.Y,target.X-b.X);
                else if(prey!=null) b.Heading=Math.Atan2(prey.Y-b.Y,prey.X-b.X);
                else b.Heading+=(random.NextDouble()-.5)*.4;
                double walkingSpeed=b.Speed/MovementMultiplier(b);
                double nx=b.X+Math.Cos(b.Heading)*walkingSpeed,ny=b.Y+Math.Sin(b.Heading)*walkingSpeed;
                AvoidLake(b,walkingSpeed,ref nx,ref ny);
                if(nx<0 || nx>Width) b.Heading=Math.PI-b.Heading;
                if(ny<0 || ny>Height) b.Heading=-b.Heading;
                b.X=Math.Max(0,Math.Min(Width,nx)); b.Y=Math.Max(0,Math.Min(Height,ny)); b.Energy=Math.Max(0,b.Energy-cost);
                MoveBlobIndex(b); WorkerAction(b);
                // A single food item can be consumed only once. Energy remains a daily budget.
                if(!b.Returning && b.Food<FoodGoal(b,Rules))
                {
                    foreach(Food food in NearbyFoods(b,3+b.Size)) if(!food.Eaten && Distance(b.X,b.Y,food.X,food.Y)<=3+b.Size)
                    { food.Eaten=true; b.Food++; VisualAction(b,0); if(b.Food>=FoodGoal(b,Rules)) break; }
                    if(b.Food<FoodGoal(b,Rules)) foreach(Blob other in NearbyBlobs(b,2*(b.Size+4)))
                        if(CanHunt(b,other) && Distance(b.X,b.Y,other.X,other.Y)<=2*(b.Size+other.Size))
                        { if(ProtectedLeader(other,b)) continue; other.Dead=true; b.Food+=1+other.Food; Predations++; VisualAction(b,0); if(Rules.EcosystemEnabled && b.FactionId!=other.FactionId) CommonThreat(b.FactionId,other.FactionId); if(b.Food>=FoodGoal(b,Rules)) break; }
                }
                if(b.Energy>0 && b.RaidTarget<0 && !b.Returning && b.Mate==null && b.Food>=Rules.ReproductionFood && HasWater(b,Rules)) foreach(Blob other in NearbyBlobs(b,Rules.MatingDistance)) if(TryMate(b,other)) break;
                if(b.Returning && b.Food>=Rules.SurvivalFood && HasWater(b,Rules) && HomeDistance(b)<=.001 && b.Energy>0) { b.Home=true; if(UseCastles) { b.X=Camps[b.FactionId].X; b.Y=Camps[b.FactionId].Y; MoveBlobIndex(b); } DepositWorker(b); }
                else if(b.Energy<=0) b.Dead=true;
                DiplomacyContacts(b);
            }
            StepWeather(); foreach(Blob blob in Blobs) NotifyDeath(blob); Tick++; resourceStep++; if(Tick%10==0) SampleResources(false);
            bool active=false; foreach(Blob b in Blobs) if(!b.Home && !b.Dead) { active=true; break; }
            if(Tick>=Rules.DayLength || !active) FinishDay();
        }
        public void FinishDay()
        {
            if(DayFinished) return;
            List<Blob> survivors=new List<Blob>(),mothers=new List<Blob>(); int deaths=0;
            foreach(Blob b in Blobs)
            {
                if(!b.Dead && b.Home && (!UseCastles || HomeDistance(b)<=.001) && b.Energy>0 && b.Food>=Rules.SurvivalFood && HasWater(b,Rules)) survivors.Add(b);
                else { CountDeath(b); b.Dead=true; NotifyDeath(b); deaths++; }
            }
            FinishEcosystemDay(survivors);
            foreach(Blob b in survivors) if(b.Sex==Sex.Female && b.Mate!=null && b.Mate.Sex==Sex.Male && b.Mate.Mate==b && survivors.Contains(b.Mate)) mothers.Add(b);
            int births=0,capped=0;
            foreach(Blob b in mothers)
            {
                if(survivors.Count>=PopulationLimit) { capped++; continue; }
                Blob father=b.Mate;
                Blob child=NewBlob(1,1,1); child.Genes=Genome.Child(b.Genes,father.Genes,random,Rules); child.Express(Rules);
                child.FactionId=random.Next(2)==0?b.FactionId:father.FactionId; child.Job=(Profession)random.Next(4); child.Water=Rules.WaterForLife;
                child.MotherId=b.Id; child.FatherId=father.Id;
                child.X=UseCastles?Camps[child.FactionId].X:b.X; child.Y=UseCastles?Camps[child.FactionId].Y:b.Y; child.Home=true; child.Energy=Rules.StartingEnergy;
                survivors.Add(child); births++;
            }
            Blobs=survivors; DayFinished=true; SampleResources(false);
            double speed=0,size=0,sense=0; foreach(Blob b in Blobs) { speed+=b.Speed; size+=b.Size; sense+=b.Sense; }
            int n=Blobs.Count,females=0; foreach(Blob b in Blobs) if(b.Sex==Sex.Female) females++;
            PopulationSnapshot snapshot=PopulationSnapshot.Capture(this,-1);
            History.Add(new DayRecord { Day=Day,Population=n,Births=births,Deaths=deaths,Predations=Predations,Capped=capped,Food=FoodToday,Females=females,Males=n-females,Matings=Matings,Speed=n==0?0:speed/n,Size=n==0?0:size/n,Sense=n==0?0:sense/n,SkinCopies=snapshot.Copies,SkinCarriers=snapshot.Carriers,PureSkins=snapshot.Pure,Mixed=snapshot.Mixed,Rules=Rules.Copy() });
        }
        public void AdvanceDay() { int day=Day; if(DayFinished) { Step(); day=Day; } while(Blobs.Count>0 && Day==day && !DayFinished) Step(); }
        public string Csv()
        {
            StringBuilder s=new StringBuilder("jour,population,naissances,morts,predations,naissances_limitees,nourriture,vitesse_moyenne,taille_moyenne,perception_moyenne,males,femelles,rencontres,copies_bleues,copies_corail,copies_vertes,copies_violettes,porteurs_bleus,porteurs_corail,porteurs_verts,porteurs_violets,unis_bleus,unis_corail,unis_verts,unis_violets,mixtes,regles_xml\r\n");
            foreach(DayRecord d in History)
            {
                StringBuilder xml=new StringBuilder(); using(XmlWriter w=XmlWriter.Create(xml,new XmlWriterSettings { OmitXmlDeclaration=true })) new XmlSerializer(typeof(Rules)).Serialize(w,d.Rules);
                s.AppendFormat(CultureInfo.InvariantCulture,"{0},{1},{2},{3},{4},{5},{6},{7:F5},{8:F5},{9:F5},{10},{11},{12}",d.Day,d.Population,d.Births,d.Deaths,d.Predations,d.Capped,d.Food,d.Speed,d.Size,d.Sense,d.Males,d.Females,d.Matings);
                foreach(int count in d.SkinCopies) s.Append(","+count); foreach(int count in d.SkinCarriers) s.Append(","+count); foreach(int count in d.PureSkins) s.Append(","+count); s.Append(","+d.Mixed+",\""+xml.ToString().Replace("\"","\"\"")+"\"\r\n");
            }
            return s.ToString();
        }
    }

    public struct Vec3
    {
        public double X,Y,Z;
        public Vec3(double x,double y,double z) { X=x; Y=y; Z=z; }
        public static Vec3 operator +(Vec3 a,Vec3 b) { return new Vec3(a.X+b.X,a.Y+b.Y,a.Z+b.Z); }
        public static Vec3 operator -(Vec3 a,Vec3 b) { return new Vec3(a.X-b.X,a.Y-b.Y,a.Z-b.Z); }
        public static Vec3 operator *(Vec3 a,double s) { return new Vec3(a.X*s,a.Y*s,a.Z*s); }
        public static double Dot(Vec3 a,Vec3 b) { return a.X*b.X+a.Y*b.Y+a.Z*b.Z; }
        public static Vec3 Cross(Vec3 a,Vec3 b) { return new Vec3(a.Y*b.Z-a.Z*b.Y,a.Z*b.X-a.X*b.Z,a.X*b.Y-a.Y*b.X); }
        public Vec3 Unit() { double n=Math.Sqrt(Dot(this,this)); return n<.000001?new Vec3(0,1,0):this*(1/n); }
    }
    public class Camera3D
    {
        public double Yaw=-.55,Elevation=.72,Distance=550,TargetX,TargetZ,Zoom=1;
        public Vec3 Right { get { return new Vec3(Math.Cos(Yaw),0,-Math.Sin(Yaw)); } }
        public Vec3 Up { get { return new Vec3(-Math.Sin(Yaw)*Math.Sin(Elevation),Math.Cos(Elevation),-Math.Cos(Yaw)*Math.Sin(Elevation)); } }
        public Vec3 Out { get { return new Vec3(Math.Sin(Yaw)*Math.Cos(Elevation),Math.Sin(Elevation),Math.Cos(Yaw)*Math.Cos(Elevation)); } }
        public Vec3 Position { get { return new Vec3(TargetX,0,TargetZ)+Out*Distance; } }
        public bool Project(Vec3 point,Size viewport,out PointF screen,out double depth)
        {
            Vec3 v=point-Position; depth=-Vec3.Dot(v,Out);
            if(depth<5) { screen=PointF.Empty; return false; }
            double f=Math.Max(1,Math.Min(viewport.Width,viewport.Height))*1.2*Zoom;
            screen=new PointF((float)(viewport.Width*.5+Vec3.Dot(v,Right)*f/depth),(float)(viewport.Height*.51-Vec3.Dot(v,Up)*f/depth)); return true;
        }
        public void Reset() { Yaw=-.55; Elevation=.72; Distance=550; TargetX=TargetZ=0; Zoom=1; }
    }
    public class Face3D
    {
        public PointF[] Points;
        public double Depth;
        public Color Color;
    }
    public partial class Arena : Control
    {
        public World World;
        public bool ShowSenses;
        public int SelectedResourceKind=-1;
        Blob selectedCreature; bool followMoving;
        readonly Timer followTimer=new Timer { Interval=33 };
        readonly System.Diagnostics.Stopwatch followClock=new System.Diagnostics.Stopwatch();
        DateTime manualCameraUntil;
        public Blob Selected {
            get { return selectedCreature; }
            set {
                if(selectedCreature==value) return;
                selectedCreature=value; followMoving=value!=null;
                if(value==null) { followTimer.Stop(); followClock.Reset(); }
                else { followClock.Restart(); followTimer.Start(); }
                InvalidateScene();
            }
        }
        public readonly Camera3D Camera=new Camera3D();
        public event EventHandler SelectionChanged;
        readonly List<Face3D> faces=new List<Face3D>();
        Point down,last;
        MouseButtons dragging;
        bool moved;
        static readonly Vec3 Light=new Vec3(-.4,.85,-.35).Unit();
        public Arena(World world) { World=world; DoubleBuffered=true; BackColor=Color.FromArgb(16,25,39); Dock=DockStyle.Fill; Cursor=Cursors.Hand; SetStyle(ControlStyles.ResizeRedraw,true); ResetCamera(); effectTimer.Tick+=delegate { if(Visible && !SimulationRunning) InvalidateEffects(); else if(!Visible) effectTimer.Stop(); }; followTimer.Tick+=delegate { double elapsed=followClock.Elapsed.TotalSeconds; followClock.Restart(); if(Visible) UpdateFollow(elapsed); }; }
        internal void UpdateFollow(double elapsed)
        {
            Blob creature=Selected;
            if(creature==null || creature.Dead || !World.Blobs.Contains(creature)) { followTimer.Stop(); return; }
            if(dragging!=MouseButtons.None || DateTime.UtcNow<manualCameraUntil || Width<1 || Height<1) return;
            double x=creature.X-World.Width/2,z=creature.Y-World.Height/2;
            double height=(World.Rules.EcosystemEnabled?World.TerrainHeight(creature.X,creature.Y):0)+creature.Size*4.4;
            PointF point; double depth; bool visible=Project(new Vec3(x,height,z),out point,out depth);
            double dx=visible?Math.Abs(point.X-Width*.5)/Width:1,dy=visible?Math.Abs(point.Y-Height*.51)/Height:1;
            if(!followMoving && dx<=.18 && dy<=.18) return;
            followMoving=true;
            if(visible && dx<=.07 && dy<=.07) { followMoving=false; return; }
            x-=Math.Sin(Camera.Yaw)*height/Math.Tan(Camera.Elevation); z-=Math.Cos(Camera.Yaw)*height/Math.Tan(Camera.Elevation);
            double dt=Math.Max(0,Math.Min(.1,elapsed)),alpha=1-Math.Exp(-3*dt);
            double moveX=(x-Camera.TargetX)*alpha,moveZ=(z-Camera.TargetZ)*alpha;
            double distance=Math.Sqrt(moveX*moveX+moveZ*moveZ),limit=Math.Max(20,Camera.Distance*.18)*dt;
            if(distance>limit && distance>0) { moveX*=limit/distance; moveZ*=limit/distance; }
            Camera.TargetX+=moveX; Camera.TargetZ+=moveZ; InvalidateScene();
        }
        protected override void Dispose(bool disposing) { if(disposing) { followTimer.Stop(); followTimer.Dispose(); effectTimer.Stop(); effectTimer.Dispose(); DisposeEffectGraphics(); } base.Dispose(disposing); }
        public void ResetCamera() { Camera.Reset(); Camera.Distance=550*Math.Max(World.Width,World.Height)/300; Camera.Zoom=1; InvalidateScene(); }
        bool Project(Vec3 v,out PointF p,out double d) { return Camera.Project(v,ClientSize,out p,out d); }
        void AddFace(Vec3[] vertices,Color color,bool cull)
        {
            Vec3 normal=Vec3.Cross(vertices[1]-vertices[0],vertices[2]-vertices[0]).Unit();
            Vec3 center=new Vec3(0,0,0); foreach(Vec3 v in vertices) center=center+v; center=center*(1.0/vertices.Length);
            if(cull && Vec3.Dot(normal,Camera.Position-center)<=0) return;
            PointF[] pts=new PointF[vertices.Length]; double depth=0;
            for(int i=0;i<vertices.Length;i++) { double d; if(!Project(vertices[i],out pts[i],out d)) return; depth+=d; }
            double shade=.38+.62*Math.Max(0,Vec3.Dot(normal,Light));
            faces.Add(new Face3D { Points=pts,Depth=depth/vertices.Length,Color=Color.FromArgb(color.A,(int)(color.R*shade),(int)(color.G*shade),(int)(color.B*shade)) });
        }
        Vec3 SphereVertex(Vec3 center,double rx,double ry,double rz,double theta,double phi)
        { return center+new Vec3(rx*Math.Sin(phi)*Math.Cos(theta),ry*Math.Cos(phi),rz*Math.Sin(phi)*Math.Sin(theta)); }
        void Sphere(Vec3 center,double rx,double ry,double rz,Color color,int segments,int rings,Color? patch=null)
        {
            for(int j=0;j<rings;j++) for(int i=0;i<segments;i++)
            {
                double t=i*Math.PI*2/segments,tt=(i+1)*Math.PI*2/segments,p=j*Math.PI/rings,pp=(j+1)*Math.PI/rings;
                Vec3 a=SphereVertex(center,rx,ry,rz,t,p),b=SphereVertex(center,rx,ry,rz,tt,p),c=SphereVertex(center,rx,ry,rz,tt,pp),d=SphereVertex(center,rx,ry,rz,t,pp);
                // Alternating longitudinal bands cover exactly half the body faces.
                Color faceColor=patch.HasValue && i%2==0?patch.Value:color;
                if(j>0) AddFace(new Vec3[] { a,b,c },faceColor,true);
                if(j<rings-1) AddFace(new Vec3[] { a,c,d },faceColor,true);
            }
        }
        void Box(double x,double z,double half,double height,Color color)
        {
            double floor=World.Rules.EcosystemEnabled?World.TerrainHeight(x+World.Width/2,z+World.Height/2):0; Vec3 a=new Vec3(x-half,floor,z-half),b=new Vec3(x+half,floor,z-half),c=new Vec3(x+half,floor,z+half),d=new Vec3(x-half,floor,z+half);
            Vec3 aa=a+new Vec3(0,height,0),bb=b+new Vec3(0,height,0),cc=c+new Vec3(0,height,0),dd=d+new Vec3(0,height,0);
            AddFace(new Vec3[] { aa,dd,cc,bb },color,true); AddFace(new Vec3[] { a,aa,bb,b },color,true); AddFace(new Vec3[] { b,bb,cc,c },color,true); AddFace(new Vec3[] { c,cc,dd,d },color,true); AddFace(new Vec3[] { d,dd,aa,a },color,true);
        }
        void GroundLine(Graphics g,Pen pen,double x,double z,double xx,double zz)
        { PointF a,b; double depth; if(Project(new Vec3(x,.15,z),out a,out depth) && Project(new Vec3(xx,.15,zz),out b,out depth)) g.DrawLine(pen,a,b); }
        void GroundCircle(Graphics g,double x,double z,double radius,Color color,bool fill)
        {
            PointF[] p=new PointF[40];
            for(int i=0;i<p.Length;i++) { double depth,t=i*Math.PI*2/p.Length; if(!Project(new Vec3(x+Math.Cos(t)*radius,.2,z+Math.Sin(t)*radius),out p[i],out depth)) return; }
            if(fill) using(Brush b=new SolidBrush(color)) g.FillPolygon(b,p); else using(Pen pen=new Pen(color,1.4f)) g.DrawPolygon(pen,p);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); if(effectsOnlyPaint && !sceneDirty && sceneCache!=null && sceneCache.Size==ClientSize) { effectsOnlyPaint=false; e.Graphics.DrawImageUnscaled(sceneCache,0,0); DrawActionEffects(e.Graphics); return; } effectsOnlyPaint=false; if(sceneCache==null || sceneCache.Size!=ClientSize) { if(sceneCache!=null) sceneCache.Dispose(); sceneCache=new Bitmap(Math.Max(1,Width),Math.Max(1,Height)); } using(Graphics g=Graphics.FromImage(sceneCache)) { g.SmoothingMode=SmoothingMode.AntiAlias;
            using(LinearGradientBrush sky=new LinearGradientBrush(ClientRectangle,Color.FromArgb(16,25,39),Color.FromArgb(35,55,72),90)) g.FillRectangle(sky,ClientRectangle);
            faces.Clear();
            if(World.Rules.EcosystemEnabled) Landscape(); else FlatTerrain();
            faces.Sort(delegate(Face3D a,Face3D b) { return b.Depth.CompareTo(a.Depth); }); foreach(Face3D f in faces) using(Brush brush=new SolidBrush(f.Color)) g.FillPolygon(brush,f.Points); faces.Clear();
            using(Pen grid=new Pen(Color.FromArgb(75,157,187,164))) { for(int i=0;i<=World.Width;i+=10) GroundLine(g,grid,i-World.Width/2,-World.Height/2,i-World.Width/2,World.Height/2); for(int i=0;i<=World.Height;i+=10) GroundLine(g,grid,-World.Width/2,i-World.Height/2,World.Width/2,i-World.Height/2); }
            foreach(Blob b in World.Blobs)
            {
                if(b.Dead || World.UseCastles && b.Home) continue; double x=b.X-World.Width/2,z=b.Y-World.Height/2,r=b.Size*4;
                GroundCircle(g,x+1,z+1,r*1.25,Color.FromArgb(70,4,13,20),true);
                if(ShowSenses || b==Selected) GroundCircle(g,x,z,World.Rules.SenseRadius*b.Sense,Color.FromArgb(b==Selected?180:45,135,200,245),false);
                if(b.Home || b==Selected) GroundCircle(g,x,z,r*1.5,b==Selected?Color.White:Color.FromArgb(120,233,182),false);
            }
            if(SelectedResourceKind>=0) {
                if(SelectedResourceKind==0) foreach(Food f in World.Foods) if(!f.Eaten) GroundCircle(g,f.X-World.Width/2,f.Y-World.Height/2,5,Color.Gold,false);
                if(World.Rules.EcosystemEnabled) foreach(Depot d in World.Depots) if(d.Kind==SelectedResourceKind && d.Quantite>0) GroundCircle(g,d.X-World.Width/2,d.Y-World.Height/2,5,Color.White,false);
            }
            // Ground first, then all solid meshes sorted from far to near.
            double hw=World.Width/2,hh=World.Height/2; Border(-hw-3,-hh-3,hw+3,-hh); Border(-hw-3,hh,hw+3,hh+3); Border(-hw-3,-hh,-hw,hh); Border(hw,-hh,hw+3,hh); CastleMeshes();
            foreach(Food food in World.Foods) if(!food.Eaten) Box(food.X-World.Width/2,food.Y-World.Height/2,2.1,4.5,Color.FromArgb(255,206,90));
            WeatherMeshes();
            foreach(Blob b in World.Blobs)
            {
                if(b.Dead || World.UseCastles && b.Home) continue; double r=Appearance.BodyRadius(b.Size),x=b.X-World.Width/2,z=b.Y-World.Height/2;
                double bob=b.Home?0:Math.Sin(World.Tick*.3+b.Id)*r*.07;
                Vec3 center=new Vec3(x,r*1.1+bob+(World.Rules.EcosystemEnabled?World.TerrainHeight(b.X,b.Y):0),z);
                Color color=Appearance.Skins[Appearance.PrimarySkin(b.Genes)];
                Color? skinPatch=b.Genes.SkinA==b.Genes.SkinB?(Color?)null:Appearance.Skins[Appearance.SecondarySkin(b.Genes)];
                if(IsCourtingHighlight(b)) {
                    color=Color.FromArgb((int)(color.R*.55),(int)(color.G*.55),(int)(color.B*.55));
                    if(skinPatch.HasValue) { Color patch=skinPatch.Value; skinPatch=Color.FromArgb((int)(patch.R*.55),(int)(patch.G*.55),(int)(patch.B*.55)); }
                }
                Sphere(center,r,r*1.12,r*.85,color,World.Blobs.Count>160?8:12,World.Blobs.Count>160?5:8,skinPatch); FactionHat(b,center,r);
                Vec3 forward=new Vec3(Math.Cos(b.Heading),0,Math.Sin(b.Heading)),side=new Vec3(-Math.Sin(b.Heading),0,Math.Cos(b.Heading));
                Sphere(center+new Vec3(0,r*1.08,0),r*.16,r*.18,r*.16,b.Sex==Sex.Female?Color.FromArgb(255,115,185):Color.FromArgb(73,226,235),6,4);
                double eyeScale=Appearance.EyeScale(b.Sense);
                for(int eye=-1;eye<=1;eye+=2)
                {
                    Vec3 pos=center+forward*(r*.82)+side*(r*(.36+.12*(eyeScale-1))*eye)+new Vec3(0,r*.38,0);
                    Sphere(pos,r*.23*eyeScale,r*.27*eyeScale,r*.23*eyeScale,Color.FromArgb(243,247,250),8,5);
                    Sphere(pos+forward*(r*.18*eyeScale),r*.11*eyeScale,r*.14*eyeScale,r*.11*eyeScale,Color.FromArgb(24,34,51),6,4);
                }
                if(b.Food>0) for(int i=0;i<Math.Min(4,b.Food);i++) Sphere(center+new Vec3((i-(Math.Min(4,b.Food)-1)*.5)*2,r*1.4,0),.8,.8,.8,Color.FromArgb(255,214,100),5,3);
            }
            faces.Sort(delegate(Face3D a,Face3D b) { return b.Depth.CompareTo(a.Depth); });
            foreach(Face3D face in faces) using(Brush brush=new SolidBrush(face.Color)) g.FillPolygon(brush,face.Points);
            using(Brush text=new SolidBrush(Color.FromArgb(211,228,236)))
            {
                HatLabels(g); CastleLabels(g); g.DrawString("TERRAIN 3D",Font,text,12,12);
                g.DrawString("Peaux : ouest bleu · est corail · nord vert · sud violet",Font,text,12,32);
                g.DrawString("Glisser : tourner  ·  Molette : zoom  ·  Clic droit + glisser : déplacer",Font,text,12,Height-28);
                if(World.Blobs.Count==0) g.DrawString("Population éteinte — démarrez une nouvelle expérience",Font,text,12,42);
                if(Selected!=null && !Selected.Dead && (!World.UseCastles || !Selected.Home) && World.Blobs.Contains(Selected)) { PointF p; double d; if(Project(new Vec3(Selected.X-World.Width/2,Selected.Size*13,Selected.Y-World.Height/2),out p,out d)) g.DrawString("#"+Selected.Id,Font,text,p); }
            }
            } sceneDirty=false; e.Graphics.DrawImageUnscaled(sceneCache,0,0); DrawActionEffects(e.Graphics);
        }
        void Border(double x0,double z0,double x1,double z1)
        {
            Color c=Color.FromArgb(108,195,160); Vec3 a=new Vec3(x0,0,z0),b=new Vec3(x1,0,z0),d=new Vec3(x0,0,z1),cc=new Vec3(x1,0,z1),up=new Vec3(0,2.5,0);
            AddFace(new Vec3[] { a+up,d+up,cc+up,b+up },c,true); AddFace(new Vec3[] { a,a+up,b+up,b },c,true); AddFace(new Vec3[] { b,b+up,cc+up,cc },c,true); AddFace(new Vec3[] { cc,cc+up,d+up,d },c,true); AddFace(new Vec3[] { d,d+up,a+up,a },c,true);
        }
        protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); Focus(); down=last=e.Location; moved=false; dragging=e.Button; Capture=true; }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e); if(dragging==MouseButtons.None) return;
            int dx=e.X-last.X,dy=e.Y-last.Y;
            if(Math.Abs(e.X-down.X)+Math.Abs(e.Y-down.Y)>4) moved=true;
            if(moved)
            {
                manualCameraUntil=DateTime.UtcNow.AddSeconds(.8); followMoving=false;
                if(dragging==MouseButtons.Left) { Camera.Yaw-=dx*.009; Camera.Elevation=Math.Max(.25,Math.Min(1.45,Camera.Elevation+dy*.006)); }
                else if(dragging==MouseButtons.Right || dragging==MouseButtons.Middle) { double factor=Camera.Distance/(Math.Max(1,Height)*Camera.Zoom); Vec3 right=Camera.Right; Camera.TargetX=Math.Max(-World.Width,Math.Min(World.Width,Camera.TargetX-dx*factor*right.X-dy*factor*Math.Sin(Camera.Yaw))); Camera.TargetZ=Math.Max(-World.Height,Math.Min(World.Height,Camera.TargetZ-dx*factor*right.Z-dy*factor*Math.Cos(Camera.Yaw))); }
                InvalidateScene();
            }
            last=e.Location;
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if(!moved && e.Button==MouseButtons.Left)
            {
                double nearest=double.MaxValue; Selected=null; SelectedResourceKind=-1;
                foreach(Blob b in World.Blobs)
                {
                    if(b.Dead || World.UseCastles && b.Home) continue; PointF p; double depth;
                    if(!Project(new Vec3(b.X-World.Width/2,(World.Rules.EcosystemEnabled?World.TerrainHeight(b.X,b.Y):0)+b.Size*4.4,b.Y-World.Height/2),out p,out depth)) continue;
                    double radius=Math.Max(7,b.Size*5*Math.Min(Width,Height)*1.2*Camera.Zoom/depth);
                    if(World.Distance(p.X,p.Y,e.X,e.Y)<=radius && depth<nearest) { Selected=b; nearest=depth; }
                }
                foreach(Food f in World.Foods) if(!f.Eaten) PickResource(e.Location,f.X,f.Y,0,2.25,ref nearest);
                if(World.Rules.EcosystemEnabled) foreach(Depot d in World.Depots) if(d.Quantite>0) PickResource(e.Location,d.X,d.Y,d.Kind,2,ref nearest);
                if(Selected==null && SelectedResourceKind<0 && World.Rules.EcosystemEnabled) {
                    double focal=Math.Max(1,Math.Min(Width,Height))*1.2*Camera.Zoom;
                    Vec3 ray=Camera.Out*(-1)+Camera.Right*((e.X-Width*.5)/focal)+Camera.Up*((Height*.51-e.Y)/focal);
                    if(ray.Y<-.00001) { Vec3 ground=Camera.Position+ray*(-Camera.Position.Y/ray.Y); double x=ground.X+World.Width/2,y=ground.Z+World.Height/2;
                        if(x>=0 && x<=World.Width && y>=0 && y<=World.Height && World.IsLake(x,y)) SelectedResourceKind=1;
                    }
                }
                if(SelectionChanged!=null) SelectionChanged(this,EventArgs.Empty);
            }
            dragging=MouseButtons.None; Capture=false; InvalidateScene();
        }
        void PickResource(Point click,double x,double y,int kind,double height,ref double nearest)
        {
            PointF point; double depth; double terrain=World.Rules.EcosystemEnabled?World.TerrainHeight(x,y):0;
            if(!Project(new Vec3(x-World.Width/2,terrain+height,y-World.Height/2),out point,out depth)) return;
            double radius=Math.Max(6,3*Math.Min(Width,Height)*1.2*Camera.Zoom/depth);
            if(World.Distance(point.X,point.Y,click.X,click.Y)<=radius && depth<nearest) { nearest=depth; Selected=null; SelectedResourceKind=kind; }
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e) { base.OnMouseDoubleClick(e); if(e.Button==MouseButtons.Left) ResetCamera(); }
        protected override void OnMouseCaptureChanged(EventArgs e) { base.OnMouseCaptureChanged(e); if(!Capture) dragging=MouseButtons.None; }
        protected override void OnMouseWheel(MouseEventArgs e) { base.OnMouseWheel(e); manualCameraUntil=DateTime.UtcNow.AddSeconds(.8); followMoving=false; Camera.Zoom=Math.Max(.45,Math.Min(16,Camera.Zoom*Math.Pow(1.15,e.Delta/120.0))); InvalidateScene(); }
    }

    public class TraitDistribution
    {
        public double[] Values;
        public double Mean,Minimum,Maximum,Median,Deviation;
        public TraitDistribution(List<double> values)
        {
            values.Sort(); Values=values.ToArray(); if(Values.Length==0) return;
            Minimum=Values[0]; Maximum=Values[Values.Length-1]; Median=Values.Length%2==0?(Values[Values.Length/2-1]+Values[Values.Length/2])/2:Values[Values.Length/2];
            foreach(double value in Values) Mean+=value; Mean/=Values.Length;
            foreach(double value in Values) Deviation+=(value-Mean)*(value-Mean); Deviation=Math.Sqrt(Deviation/Values.Length);
        }
    }
    public class PopulationSnapshot
    {
        public int Count,Males,Females,Mixed;
        public int[] Copies=new int[4],Carriers=new int[4],Pure=new int[4];
        public List<Blob> Individuals=new List<Blob>();
        public TraitDistribution[] Traits=new TraitDistribution[3];
        public static PopulationSnapshot Capture(World world,int filter)
        {
            PopulationSnapshot snapshot=new PopulationSnapshot(); List<double>[] values=new List<double>[] { new List<double>(),new List<double>(),new List<double>() };
            foreach(Blob b in world.Blobs)
            {
                if(b.Dead) continue; snapshot.Count++; if(b.Sex==Sex.Female) snapshot.Females++; else snapshot.Males++;
                int a=b.Genes.SkinA,bb=b.Genes.SkinB; snapshot.Copies[a]++; snapshot.Copies[bb]++; snapshot.Carriers[a]++;
                if(a!=bb) { snapshot.Mixed++; snapshot.Carriers[bb]++; } else snapshot.Pure[a]++;
                if(filter>=0 && filter<4 && a!=filter && bb!=filter || filter==4 && a==bb) continue;
                snapshot.Individuals.Add(b); values[0].Add(b.Speed); values[1].Add(b.Size); values[2].Add(b.Sense);
            }
            for(int trait=0;trait<3;trait++) snapshot.Traits[trait]=new TraitDistribution(values[trait]); return snapshot;
        }
    }
    public class DashboardCanvas : Control
    {
        public World World;
        public int Filter=-1;
        public event Action<int> LineageClicked;
        readonly Font heading=new Font("Segoe UI",11,FontStyle.Bold),number=new Font("Segoe UI",25,FontStyle.Bold),valueFont=new Font("Segoe UI",16,FontStyle.Bold),small=new Font("Segoe UI",9);
        readonly RectangleF[] lineageCards=new RectangleF[4];
        static readonly Color Ink=Color.FromArgb(33,48,65),Muted=Color.FromArgb(101,116,133),BorderColor=Color.FromArgb(224,231,238);
        static readonly string[] Names=new string[] { "BLEUE","CORAIL","VERTE","VIOLETTE" };
        static readonly Color[] TraitColors=new Color[] { Color.FromArgb(53,139,218),Color.FromArgb(226,137,59),Color.FromArgb(151,99,205) };
        public DashboardCanvas(World world) { World=world; DoubleBuffered=true; BackColor=Color.FromArgb(240,244,248); Height=760; MinimumSize=new Size(940,760); SetStyle(ControlStyles.ResizeRedraw,true); }
        void DrawText(Graphics g,string text,Font font,Color color,float x,float y) { using(Brush b=new SolidBrush(color)) g.DrawString(text,font,b,x,y); }
        void Box(Graphics g,RectangleF r,Color fill,Color border) { using(Brush b=new SolidBrush(fill)) g.FillRectangle(b,r); using(Pen p=new Pen(border)) g.DrawRectangle(p,r.X,r.Y,r.Width,r.Height); }
        void Bar(Graphics g,RectangleF r,Color color) { if(r.Width>0 && r.Height>0) using(Brush b=new SolidBrush(color)) g.FillRectangle(b,r); }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); Graphics g=e.Graphics; g.SmoothingMode=SmoothingMode.AntiAlias;
            PopulationSnapshot snapshot=PopulationSnapshot.Capture(World,Filter);
            float gap=14,margin=14,w=ClientSize.Width-margin*2;
            DrawText(g,"TABLEAU DE BORD  /  JOUR "+World.Day+"  /  "+(World.DayFinished?"bilan terminé":"population vivante"),heading,Ink,margin,12);
            DrawText(g,"Les phénotypes sont les caractéristiques exprimées : vitesse, taille, perception et peau.",small,Muted,margin,36);
            float cardW=(w-gap*3)/4;
            Metric(g,new RectangleF(margin,64,cardW,88),"POPULATION",snapshot.Count.ToString(),"individus vivants",Color.FromArgb(49,161,128));
            Metric(g,new RectangleF(margin+(cardW+gap),64,cardW,88),"MÂLES / FEMELLES",snapshot.Males+" / "+snapshot.Females,"répartition actuelle",Color.FromArgb(65,142,203));
            Metric(g,new RectangleF(margin+(cardW+gap)*2,64,cardW,88),"LIGNÉES MIXTES",snapshot.Mixed.ToString(),(snapshot.Count-snapshot.Mixed)+" individus à peau unie",Color.FromArgb(153,101,197));
            DayRecord last=World.History.Count==0?null:World.History[World.History.Count-1];
            Metric(g,new RectangleF(margin+(cardW+gap)*3,64,cardW,88),"DERNIER BILAN",last==null?"—":"+"+last.Births+" / −"+last.Deaths,last==null?"après la première journée":"naissances / morts · jour "+last.Day,Color.FromArgb(213,137,60));
            float leftW=w*.55f,rightX=margin+leftW+gap,rightW=w-leftW-gap;
            DrawLineages(g,new RectangleF(margin,166,leftW,324),snapshot);
            for(int trait=0;trait<3;trait++) DrawHistogram(g,new RectangleF(rightX,166+trait*110,rightW,104),snapshot.Traits[trait],trait,snapshot.Individuals.Count);
            DrawHistory(g,new RectangleF(margin,506,leftW,232));
            DrawScatter(g,new RectangleF(rightX,506,rightW,232),snapshot);
        }
        void Metric(Graphics g,RectangleF r,string title,string value,string caption,Color accent)
        {
            Box(g,r,Color.White,BorderColor); Bar(g,new RectangleF(r.Left,r.Top,4,r.Height),accent);
            DrawText(g,title,small,Muted,r.Left+14,r.Top+8); DrawText(g,value,value.Length>9?valueFont:number,Ink,r.Left+12,r.Top+23); DrawText(g,caption,small,Muted,r.Left+14,r.Bottom-21);
        }
        void DrawLineages(Graphics g,RectangleF r,PopulationSnapshot snapshot)
        {
            Box(g,r,Color.White,BorderColor); DrawText(g,"QUANTITÉS PAR LIGNÉE DE PEAU",heading,Ink,r.Left+14,r.Top+10);
            DrawText(g,"Un individu mixte compte parmi les porteurs de ses deux couleurs.",small,Muted,r.Left+14,r.Top+34);
            float width=(r.Width-42)/2;
            for(int color=0;color<4;color++)
            {
                RectangleF card=new RectangleF(r.Left+14+(color%2)*(width+14),r.Top+61+(color/2)*94,width,84); lineageCards[color]=card;
                Color c=Appearance.Skins[color]; Box(g,card,Filter==color?Color.FromArgb(242,247,253):Color.FromArgb(249,251,253),Filter==color?c:BorderColor);
                Bar(g,new RectangleF(card.Left,card.Top,4,card.Height),c); DrawText(g,Names[color],small,Ink,card.Left+13,card.Top+7);
                DrawText(g,snapshot.Carriers[color]+" porteurs",valueFont,Ink,card.Left+12,card.Top+24);
                DrawText(g,snapshot.Pure[color]+" unis · "+(snapshot.Carriers[color]-snapshot.Pure[color])+" mixtes",small,Muted,card.Left+13,card.Top+53);
                float fraction=snapshot.Count==0?0:(float)snapshot.Copies[color]/(snapshot.Count*2);
                Bar(g,new RectangleF(card.Left+13,card.Bottom-9,(card.Width-26)*fraction,3),c);
                DrawText(g,(fraction*100).ToString("0.0")+" % des copies",small,Muted,card.Right-112,card.Top+8);
            }
            DrawText(g,snapshot.Count==0?"AUCUN MARQUEUR  ·  population éteinte":"PART DES MARQUEURS HÉRITÉS  ·  total = 100 %",small,Ink,r.Left+14,r.Top+257);
            float x=r.Left+14; for(int color=0;color<4;color++) { float widthPart=snapshot.Count==0?0:(r.Width-28)*snapshot.Copies[color]/(snapshot.Count*2f); Bar(g,new RectangleF(x,r.Top+278,widthPart,14),Appearance.Skins[color]); x+=widthPart; }
            DrawText(g,"Cliquer une carte filtre les graphiques de phénotypes.",small,Muted,r.Left+14,r.Top+301);
        }
        void DrawHistogram(Graphics g,RectangleF r,TraitDistribution distribution,int trait,int n)
        {
            Box(g,r,Color.White,BorderColor); string name=new string[] { "VITESSE","TAILLE / VOLUME","PERCEPTION / YEUX" }[trait]; Color color=TraitColors[trait];
            DrawText(g,name,heading,Ink,r.Left+12,r.Top+6); DrawText(g,n==0?"—":"moy. "+distribution.Mean.ToString("0.000"),small,color,r.Right-109,r.Top+10);
            if(n==0) { DrawText(g,"Aucun individu dans ce groupe.",small,Muted,r.Left+12,r.Top+40); return; }
            const int bins=12; int[] counts=new int[bins]; double low=Math.Max(.2,Math.Floor((distribution.Minimum-.05)*10)/10),high=Math.Min(4,Math.Ceiling((distribution.Maximum+.05)*10)/10); if(high-low<.4) { double center=(high+low)/2; low=Math.Max(.2,center-.2); high=Math.Min(4,center+.2); }
            double span=Math.Max(.001,high-low); int peak=1;
            foreach(double value in distribution.Values) { int bin=Math.Max(0,Math.Min(bins-1,(int)((value-low)/span*bins))); counts[bin]++; peak=Math.Max(peak,counts[bin]); }
            RectangleF area=new RectangleF(r.Left+13,r.Top+32,r.Width-26,42); float barW=area.Width/bins;
            using(Pen p=new Pen(BorderColor)) g.DrawLine(p,area.Left,area.Bottom,area.Right,area.Bottom);
            for(int bin=0;bin<bins;bin++) { float height=area.Height*counts[bin]/peak; Bar(g,new RectangleF(area.Left+bin*barW+1,area.Bottom-height,Math.Max(1,barW-2),height),color); }
            float meanX=area.Left+(float)((distribution.Mean-low)/span)*area.Width; using(Pen p=new Pen(Ink,1.4f)) { p.DashStyle=DashStyle.Dash; g.DrawLine(p,meanX,area.Top,meanX,area.Bottom); }
            DrawText(g,"Min "+distribution.Minimum.ToString("0.00")+" · Médiane "+distribution.Median.ToString("0.00")+" · Max "+distribution.Maximum.ToString("0.00"),small,Muted,r.Left+12,r.Top+81);
            DrawText(g,peak.ToString(),small,Muted,area.Left,area.Top-4);
        }
        void DrawHistory(Graphics g,RectangleF r)
        {
            Box(g,r,Color.White,BorderColor); DrawText(g,"ÉVOLUTION DES QUATRE LIGNÉES",heading,Ink,r.Left+14,r.Top+10);
            DrawText(g,"Part des copies de chaque couleur · bilans de fin de jour",small,Muted,r.Left+14,r.Top+34);
            RectangleF area=new RectangleF(r.Left+46,r.Top+62,r.Width-65,r.Height-105);
            for(int tick=0;tick<=2;tick++) { float y=area.Bottom-area.Height*tick/2; using(Pen p=new Pen(BorderColor)) g.DrawLine(p,area.Left,y,area.Right,y); DrawText(g,(tick*50)+" %",small,Muted,r.Left+7,y-7); }
            int count=World.History.Count;
            if(count<2) { DrawText(g,"Les courbes apparaissent après deux jours.",small,Muted,area.Left,area.Top+35); return; }
            for(int color=0;color<4;color++) using(Pen pen=new Pen(Appearance.Skins[color],2.2f))
            {
                PointF[] points=new PointF[count]; for(int i=0;i<count;i++) { DayRecord d=World.History[i]; double fraction=d.Population==0?0:d.SkinCopies[color]/(d.Population*2.0); points[i]=new PointF(area.Left+area.Width*i/(count-1),area.Bottom-(float)fraction*area.Height); } g.DrawLines(pen,points);
            }
            DrawText(g,"Jour "+World.History[0].Day,small,Muted,area.Left,area.Bottom+4); DrawText(g,"Jour "+World.History[count-1].Day,small,Muted,area.Right-58,area.Bottom+4);
            float x=r.Left+15; for(int color=0;color<4;color++) { Bar(g,new RectangleF(x,r.Bottom-21,8,8),Appearance.Skins[color]); DrawText(g,Names[color],small,Muted,x+12,r.Bottom-26); x+=(r.Width-30)/4; }
        }
        void DrawScatter(Graphics g,RectangleF r,PopulationSnapshot snapshot)
        {
            Box(g,r,Color.White,BorderColor); DrawText(g,"PHÉNOTYPES  ·  TAILLE × PERCEPTION",heading,Ink,r.Left+14,r.Top+10);
            string filter=Filter==-1?"Toute la population":Filter==4?"Individus mixtes":"Porteurs de la lignée "+Names[Filter].ToLower(); DrawText(g,filter+" · "+snapshot.Individuals.Count+" individus",small,Muted,r.Left+14,r.Top+34);
            RectangleF area=new RectangleF(r.Left+49,r.Top+62,r.Width-69,r.Height-111);
            if(snapshot.Individuals.Count==0) { DrawText(g,"Aucun individu dans ce groupe.",small,Muted,area.Left,area.Top+25); return; }
            double minSize=Math.Max(.2,snapshot.Traits[1].Minimum-.1),maxSize=snapshot.Traits[1].Maximum+.1,minSense=Math.Max(.2,snapshot.Traits[2].Minimum-.1),maxSense=snapshot.Traits[2].Maximum+.1;
            for(int i=0;i<=2;i++) { float y=area.Bottom-area.Height*i/2; using(Pen p=new Pen(BorderColor)) g.DrawLine(p,area.Left,y,area.Right,y); DrawText(g,(minSense+(maxSense-minSense)*i/2).ToString("0.0"),small,Muted,r.Left+15,y-7); }
            foreach(Blob b in snapshot.Individuals)
            {
                float x=area.Left+(float)((b.Size-minSize)/(maxSize-minSize))*area.Width,y=area.Bottom-(float)((b.Sense-minSense)/(maxSense-minSense))*area.Height;
                using(Brush brush=new SolidBrush(Appearance.Skins[b.Genes.SkinA])) g.FillPie(brush,x-4,y-4,8,8,90,180);
                using(Brush brush=new SolidBrush(Appearance.Skins[b.Genes.SkinB])) g.FillPie(brush,x-4,y-4,8,8,270,180);
                using(Pen p=new Pen(Color.FromArgb(160,70,83,96),.5f)) g.DrawEllipse(p,x-4,y-4,8,8);
            }
            DrawText(g,minSize.ToString("0.00"),small,Muted,area.Left,area.Bottom+3); DrawText(g,maxSize.ToString("0.00"),small,Muted,area.Right-32,area.Bottom+3);
            DrawText(g,"Taille →    ·    Perception ↑    ·    un point = un individu",small,Muted,r.Left+14,r.Bottom-23);
        }
        protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); if(e.Button!=MouseButtons.Left) return; for(int i=0;i<4;i++) if(lineageCards[i].Contains(e.Location) && LineageClicked!=null) LineageClicked(Filter==i?-1:i); }
        protected override void Dispose(bool disposing) { if(disposing) { heading.Dispose(); number.Dispose(); valueFont.Dispose(); small.Dispose(); } base.Dispose(disposing); }
    }
    public partial class HistoryPlot : Control
    {
        public World World;
        public HistoryPlot(World world) { World=world; DoubleBuffered=true; Dock=DockStyle.Fill; BackColor=Color.White; SetStyle(ControlStyles.ResizeRedraw,true); }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); Graphics g=e.Graphics; g.SmoothingMode=SmoothingMode.AntiAlias;
            if(ResourceKind>=0) { DrawResourceHistory(g); return; }
            RectangleF left=new RectangleF(44,30,Math.Max(10,Width/2f-70),Height-56),right=new RectangleF(Width/2f+44,30,Math.Max(10,Width/2f-70),Height-56);
            Draw(g,left,true); Draw(g,right,false);
        }
        void Draw(Graphics g,RectangleF area,bool pop)
        {
            double max=pop?10:1.5; int count=World.History.Count;
            foreach(DayRecord d in World.History) max=Math.Max(max,pop?d.Population:Math.Max(d.Speed,Math.Max(d.Size,d.Sense)));
            max=Math.Ceiling(max*1.1);
            using(Brush text=new SolidBrush(Color.FromArgb(55,70,85)))
            using(Pen grid=new Pen(Color.FromArgb(229,234,239)))
            {
                g.DrawString(pop?"POPULATION À LA FIN DU JOUR":"TRAITS MOYENS  ·  vitesse / taille / perception",Font,text,area.Left,4);
                for(int i=0;i<=2;i++) { float y=area.Bottom-area.Height*i/2; g.DrawLine(grid,area.Left,y,area.Right,y); g.DrawString((max*i/2).ToString("0.#"),Font,text,area.Left-35,y-7); }
                g.DrawString(count<2?"Les courbes apparaissent après deux jours.":"Jour "+World.History[0].Day+" → "+World.History[count-1].Day,Font,text,area.Left,area.Bottom+3);
            }
            if(count<2) return;
            Color[] colors=pop?new Color[] { Color.FromArgb(37,153,117) }:new Color[] { Color.FromArgb(60,136,220),Color.FromArgb(231,128,60),Color.FromArgb(137,89,199) };
            for(int series=0;series<colors.Length;series++) using(Pen pen=new Pen(colors[series],2))
            {
                PointF[] points=new PointF[count]; for(int i=0;i<count;i++) { DayRecord d=World.History[i]; double v=pop?d.Population:(series==0?d.Speed:(series==1?d.Size:d.Sense)); points[i]=new PointF(area.Left+area.Width*i/(count-1),area.Bottom-(float)(v/max)*area.Height); } g.DrawLines(pen,points);
            }
        }
    }

    public class RulesDialog : Form
    {
        public Rules Draft;
        public bool Restart;
        PropertyGrid grid; int originalColumns,originalRows;
        Label formula;
        public RulesDialog(Rules rules)
        {
            originalColumns=rules.MapColumns; originalRows=rules.MapRows; Draft=rules.Copy(); Text="Éditer les règles"; StartPosition=FormStartPosition.CenterParent; Size=new Size(650,790); MinimumSize=new Size(580,600); Font=new Font("Segoe UI",10);
            TableLayoutPanel layout=new TableLayoutPanel { Dock=DockStyle.Fill,Padding=new Padding(12),ColumnCount=1,RowCount=5 };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute,66)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute,42)); layout.RowStyles.Add(new RowStyle(SizeType.Percent,100)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute,68)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute,80)); Controls.Add(layout);
            layout.Controls.Add(new Label { Text="Modifiez une valeur puis validez avec Entrée.\n« Prochain jour » conserve les individus. « Nouvelle expérience »\nutilise aussi la population initiale et la graine aléatoire.",Dock=DockStyle.Fill },0,0);
            FlowLayoutPanel presets=new FlowLayoutPanel { Dock=DockStyle.Fill,WrapContents=false }; ComboBox combo=new ComboBox { DropDownStyle=ComboBoxStyle.DropDownList,Width=400 };
            combo.Items.AddRange(new object[] { "Équilibré · 4 × 50 · sans mutations", "Équilibré · 4 × 50 · mutation vitesse", "Équilibré · 4 × 50 · trois traits", "Raréfaction · 80 → 20 nourritures", "Pénurie · 4 × 50 · 40 nourritures", "Conflit MAGA · 4 × 50", "Grand monde · 4 × 100" }); combo.SelectedIndex=2;
            presets.Controls.Add(combo); presets.Controls.Add(Make("Charger le modèle",delegate { Draft=Rules.Preset(combo.SelectedIndex); UpdateGrid(); })); layout.Controls.Add(presets,0,1);
            grid=new PropertyGrid { Dock=DockStyle.Fill,SelectedObject=Draft,ToolbarVisible=false,PropertySort=PropertySort.Categorized,HelpVisible=true }; grid.PropertyValueChanged+=delegate { formula.Text=Draft.Formula+"\nPortée = "+Draft.SenseRadius+" × perception"; }; layout.Controls.Add(grid,0,2);
            formula=new Label { Dock=DockStyle.Fill,Padding=new Padding(0,8,0,0),Text=Draft.Formula+"\nPortée = "+Draft.SenseRadius+" × perception" }; layout.Controls.Add(formula,0,3);
            FlowLayoutPanel actions=new FlowLayoutPanel { Dock=DockStyle.Fill,WrapContents=true };
            actions.Controls.Add(Make("Importer XML",Import)); actions.Controls.Add(Make("Exporter XML",Export)); actions.Controls.Add(Make("Annuler",delegate { DialogResult=DialogResult.Cancel; }));
            actions.Controls.Add(Make("Appliquer au prochain jour",delegate { Accept(false); })); actions.Controls.Add(Make("Nouvelle expérience",delegate { Accept(true); })); layout.Controls.Add(actions,0,4);
        }
        static Button Make(string text,Action action) { Button b=new Button { Text=text,AutoSize=true,Height=32 }; b.Click+=delegate { action(); }; return b; }
        void UpdateGrid() { grid.SelectedObject=Draft; formula.Text=Draft.Formula+"\nPortée = "+Draft.SenseRadius+" × perception"; }
        void Accept(bool restart) { try { grid.Focus(); Draft.Validate(); if(!restart && (Draft.MapColumns!=originalColumns || Draft.MapRows!=originalRows)) throw new ArgumentException("Pour changer les lignes ou colonnes, choisissez Nouvelle expérience."); Restart=restart; DialogResult=DialogResult.OK; } catch(Exception ex) { MessageBox.Show(this,ex.Message,"Règles invalides",MessageBoxButtons.OK,MessageBoxIcon.Warning); } }
        void Import()
        {
            using(OpenFileDialog d=new OpenFileDialog { Filter="Règles (*.xml)|*.xml" }) if(d.ShowDialog(this)==DialogResult.OK)
                try { Draft=Rules.Load(d.FileName); UpdateGrid(); } catch(Exception ex) { MessageBox.Show(this,ex.Message,"Import impossible"); }
        }
        void Export()
        {
            try { Draft.Validate(); using(SaveFileDialog d=new SaveFileDialog { Filter="Règles (*.xml)|*.xml",FileName="mes-regles.xml",DefaultExt="xml",AddExtension=true }) if(d.ShowDialog(this)==DialogResult.OK) Draft.Save(d.FileName); }
            catch(Exception ex) { MessageBox.Show(this,ex.Message,"Export impossible"); }
        }
    }

    public class InspectorLabel : Label
    {
        string displayed=""; string[] lines=new string[0]; int[] tops=new int[0],heights=new int[0];
        public InspectorLabel() { SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true); Width=235; }
        public override string Text {
            get { return displayed; }
            set {
                string next=value??""; if(next==displayed) return;
                string[] updated=next.Split('\n'); int[] newTops=new int[updated.Length],newHeights=new int[updated.Length]; int y=0;
                for(int i=0;i<updated.Length;i++) {
                    newTops[i]=y; newHeights[i]=TextRenderer.MeasureText(updated[i].Length==0?" ":updated[i],Font,new Size(Width,int.MaxValue),TextFormatFlags.WordBreak|TextFormatFlags.NoPrefix).Height; y+=newHeights[i];
                }
                bool layoutChanged=updated.Length!=lines.Length;
                if(!layoutChanged) for(int i=0;i<updated.Length;i++) if(newHeights[i]!=heights[i]) { layoutChanged=true; break; }
                if(layoutChanged) { Height=y; Invalidate(); }
                else for(int i=0;i<updated.Length;i++) if(updated[i]!=lines[i]) Invalidate(new Rectangle(0,newTops[i],Width,newHeights[i]));
                displayed=next; lines=updated; tops=newTops; heights=newHeights;
            }
        }
        protected override void OnPaint(PaintEventArgs e) {
            for(int i=0;i<lines.Length;i++) {
                Rectangle row=new Rectangle(0,tops[i],Width,heights[i]);
                if(e.ClipRectangle.IntersectsWith(row)) TextRenderer.DrawText(e.Graphics,lines[i],Font,row,ForeColor,TextFormatFlags.WordBreak|TextFormatFlags.NoPrefix);
            }
        }
    }

    public class MainWindow : Form
    {
        public World World;
        readonly Timer timer=new Timer();
        DiplomacyPanel diplomacy; EconomyPanel economy; Arena arena; HistoryPlot plot; DashboardCanvas dashboard; Label status,details,pending,selected; TextBox geneticDetails; Button play; int stepsPerFrame=4;
        NumericUpDown factionSize; Rules displayedRules;
        public MainWindow()
        {
            World=new World(new Rules()); Text="Sélection naturelle 3D — laboratoire d'évolution"; ClientSize=new Size(1220,850); MinimumSize=new Size(1000,700); StartPosition=FormStartPosition.CenterScreen;
            Font=new Font("Segoe UI",10); BackColor=Color.FromArgb(240,244,247); KeyPreview=true;
            TableLayoutPanel layout=new TableLayoutPanel { Dock=DockStyle.Fill,Padding=new Padding(16),ColumnCount=1,RowCount=4 };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute,58)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute,86)); layout.RowStyles.Add(new RowStyle(SizeType.Percent,100)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute,32)); Controls.Add(layout);
            layout.Controls.Add(new Label { Dock=DockStyle.Fill,Text="SÉLECTION NATURELLE 3D\nUn laboratoire inspiré de Primer : chercher, rentrer, transmettre.",Font=new Font("Segoe UI",12,FontStyle.Bold) },0,0);
            FlowLayoutPanel toolbar=new FlowLayoutPanel { Dock=DockStyle.Fill,WrapContents=true,AutoScroll=true };
            play=Make("Lancer",Toggle); toolbar.Controls.Add(play); toolbar.Controls.Add(Make("Un pas",delegate { Pause(); World.Step(); RefreshAll(); }));
            toolbar.Controls.Add(Make("Fin du jour",delegate { Pause(); Cursor=Cursors.WaitCursor; try { World.AdvanceDay(); } finally { Cursor=Cursors.Default; RefreshAll(); } }));
            toolbar.Controls.Add(Make("Recommencer",delegate { Pause(); World.Reset(World.Pending??World.Rules); arena.Selected=null; arena.SelectedResourceKind=-1; RefreshAll(); }));
            Button randomStart=Make("Nouvelle graine + relancer",RerollAndRestart); randomStart.Name="RandomSeedRestart"; toolbar.Controls.Add(randomStart);
            toolbar.Controls.Add(Make("Éditer les règles",Edit)); toolbar.Controls.Add(Make("Exporter CSV",Export)); toolbar.Controls.Add(Make("Aide",Help));
            toolbar.Controls.Add(new Label { Text="Habitants / faction",AutoSize=true,Margin=new Padding(8,8,0,0) });
            factionSize=new NumericUpDown { Name="FactionSize",Minimum=1,Maximum=100,Value=World.Rules.FactionSize,Width=55 };
            toolbar.Controls.Add(factionSize);
            toolbar.Controls.Add(Make("Appliquer et recommencer",delegate { RestartWithFactionSize((int)factionSize.Value); }));
            toolbar.Controls.Add(new Label { Text="Vitesse",AutoSize=true,Margin=new Padding(8,8,0,0) });
            ComboBox speed=new ComboBox { DropDownStyle=ComboBoxStyle.DropDownList,Width=65 }; speed.Items.AddRange(new object[] { "×1", "×4", "×12", "×30" }); speed.SelectedIndex=1; speed.SelectedIndexChanged+=delegate { stepsPerFrame=new int[] { 1,4,12,30 }[speed.SelectedIndex]; }; toolbar.Controls.Add(speed); layout.Controls.Add(toolbar,0,1);
            TableLayoutPanel content=new TableLayoutPanel { Dock=DockStyle.Fill,ColumnCount=2,RowCount=1 }; content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100)); content.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,285));
            TabControl sidebar=new TabControl { Dock=DockStyle.Fill,Margin=new Padding(12,0,0,0) }; TabPage balanceTab=new TabPage("Bilan"),creatureTab=new TabPage("Créature"),genomeTab=new TabPage("Génome"),cameraTab=new TabPage("Caméra"); sidebar.TabPages.AddRange(new TabPage[] { balanceTab,creatureTab,genomeTab,cameraTab });
            arena=new Arena(World); arena.SelectionChanged+=delegate { RefreshSelected(); if(arena.Selected!=null || arena.SelectedResourceKind>=0) sidebar.SelectedTab=creatureTab; plot.Invalidate(); }; content.Controls.Add(arena,0,0);
            FlowLayoutPanel balance=new FlowLayoutPanel { Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,AutoScroll=true,Padding=new Padding(8) }; balanceTab.Controls.Add(balance);
            details=new Label { Width=230,Height=305 }; balance.Controls.Add(details); pending=new Label { Width=230,Height=100,ForeColor=Color.FromArgb(151,87,29) }; balance.Controls.Add(pending);
            CheckBox senses=new CheckBox { Text="Afficher les portées\nde perception",Width=230,Height=45 }; senses.CheckedChanged+=delegate { arena.ShowSenses=senses.Checked; arena.InvalidateScene(); }; balance.Controls.Add(senses); CheckBox animations=new CheckBox { Text="Animations des actions",Checked=true,Width=230,Height=28 }; animations.CheckedChanged+=delegate { arena.ShowActionEffects=animations.Checked; arena.InvalidateScene(); }; balance.Controls.Add(animations);
            balance.Controls.Add(new Label { Width=230,Height=145,Text="Petite crête turquoise : mâle\nPetite crête rose : femelle\nPeau : lignée du bord initial\nDeux couleurs : deux lignées\nVolume : taille\nGrands yeux : forte perception\nPoints dorés : nourriture collectée" });
            Panel inspector=new Panel { Dock=DockStyle.Fill,AutoScroll=true,Padding=new Padding(10) }; selected=new InspectorLabel { Location=new Point(10,10) }; inspector.Controls.Add(selected); creatureTab.Controls.Add(inspector);
            geneticDetails=new TextBox { Dock=DockStyle.Fill,Multiline=true,ReadOnly=true,ScrollBars=ScrollBars.Vertical,Font=new Font("Consolas",9),BackColor=Color.White }; genomeTab.Controls.Add(geneticDetails);
            FlowLayoutPanel cameraPanel=new FlowLayoutPanel { Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,Padding=new Padding(10) }; cameraTab.Controls.Add(cameraPanel);
            cameraPanel.Controls.Add(new Label { Width=230,Height=190,Text="CAMÉRA 3D\n\nClic gauche + glisser : tourner\nMolette : zoomer\nClic droit + glisser : déplacer\nDouble-clic gauche : recentrer\n\nClic sur une créature : suivi doux\net caractéristiques.\nClic sur le terrain : arrêter le suivi." }); cameraPanel.Controls.Add(Make("Recentrer la caméra",delegate { arena.ResetCamera(); }));
            content.Controls.Add(sidebar,1,0);
            TabControl views=new TabControl { Dock=DockStyle.Fill }; TabPage terrainTab=new TabPage("Terrain 3D"),dashboardTab=new TabPage("Tableau de bord"); views.TabPages.AddRange(new TabPage[] { terrainTab,dashboardTab }); TabPage economyTab=new TabPage("Ressources et factions"); economy=new EconomyPanel(World); Panel economyScroll=new Panel { Dock=DockStyle.Fill,AutoScroll=true }; economy.MinimumSize=new Size(940,880); economyScroll.Controls.Add(economy); economyTab.Controls.Add(economyScroll); views.TabPages.Add(economyTab); TabPage diplomacyTab=new TabPage("Diplomatie et raids"); diplomacy=new DiplomacyPanel(World); Panel diplomacyScroll=new Panel { Dock=DockStyle.Fill,AutoScroll=true }; diplomacyScroll.Controls.Add(diplomacy); diplomacyTab.Controls.Add(diplomacyScroll); views.TabPages.Add(diplomacyTab); layout.Controls.Add(views,0,2);
            TableLayoutPanel terrainLayout=new TableLayoutPanel { Dock=DockStyle.Fill,ColumnCount=1,RowCount=2 }; terrainLayout.RowStyles.Add(new RowStyle(SizeType.Percent,100)); terrainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute,155)); terrainTab.Controls.Add(terrainLayout); terrainLayout.Controls.Add(content,0,0);
            plot=new HistoryPlot(World); terrainLayout.Controls.Add(plot,0,1);
            TableLayoutPanel dashboardLayout=new TableLayoutPanel { Dock=DockStyle.Fill,ColumnCount=1,RowCount=2 }; dashboardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute,44)); dashboardLayout.RowStyles.Add(new RowStyle(SizeType.Percent,100)); dashboardTab.Controls.Add(dashboardLayout);
            FlowLayoutPanel filters=new FlowLayoutPanel { Dock=DockStyle.Fill,Padding=new Padding(10,5,0,0),WrapContents=false }; filters.Controls.Add(new Label { Text="Phénotypes :",AutoSize=true,Margin=new Padding(0,5,5,0) });
            ComboBox filter=new ComboBox { DropDownStyle=ComboBoxStyle.DropDownList,Width=245 }; filter.Items.AddRange(new object[] { "Toute la population", "Porteurs de bleu", "Porteurs de corail", "Porteurs de vert", "Porteurs de violet", "Individus mixtes uniquement" }); filter.SelectedIndex=0; filters.Controls.Add(filter); filters.Controls.Add(new Label { Text="Histogrammes et points filtrés ; effectifs globaux conservés.",AutoSize=true,Margin=new Padding(12,5,0,0) }); dashboardLayout.Controls.Add(filters,0,0);
            Panel dashboardScroll=new Panel { Dock=DockStyle.Fill,AutoScroll=true,AutoScrollMinSize=new Size(940,760) }; dashboard=new DashboardCanvas(World) { Dock=DockStyle.Top }; dashboardScroll.Controls.Add(dashboard); dashboardLayout.Controls.Add(dashboardScroll,0,1);
            filter.SelectedIndexChanged+=delegate { dashboard.Filter=filter.SelectedIndex-1; dashboard.Invalidate(); }; dashboard.LineageClicked+=delegate(int lineage) { filter.SelectedIndex=lineage+1; };
            status=new Label { Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleLeft }; layout.Controls.Add(status,0,3);
            timer.Interval=33; timer.Tick+=delegate { for(int i=0;i<stepsPerFrame;i++) { World.Step(); if(World.Blobs.Count==0) { Pause(); break; } } RefreshAll(); };
            KeyDown+=delegate(object sender,KeyEventArgs e) { if(e.KeyCode==Keys.Space) { Toggle(); e.SuppressKeyPress=true; } };
            RefreshAll();
        }
        static Button Make(string text,Action action) { Button b=new Button { Text=text,AutoSize=true,Height=32,Padding=new Padding(3,0,3,0) }; b.Click+=delegate { action(); }; return b; }
        void Pause() { timer.Stop(); play.Text="Lancer"; }
        void Toggle() { if(timer.Enabled) Pause(); else if(World.Blobs.Count>0) { timer.Start(); play.Text="Pause"; } RefreshAll(); }
        internal void RerollAndRestart()
        {
            Pause(); Rules rules=(World.Pending??World.Rules).Copy();
            long activeSeed=Math.Abs((long)World.Rules.Seed),draftSeed=Math.Abs((long)rules.Seed);
            byte[] bytes=new byte[4]; int seed;
            using(System.Security.Cryptography.RandomNumberGenerator generator=System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                do { generator.GetBytes(bytes); seed=BitConverter.ToInt32(bytes,0)&int.MaxValue; }
                while(seed==int.MaxValue || seed==activeSeed || seed==draftSeed);
            }
            rules.Seed=seed; World.Reset(rules); arena.Selected=null; arena.SelectedResourceKind=-1; Toggle();
        }
        internal void RestartWithFactionSize(int size)
        {
            Pause(); Rules rules=(World.Pending??World.Rules).Copy(); rules.FactionSize=size;
            World.Reset(rules); arena.Selected=null; arena.SelectedResourceKind=-1; RefreshAll();
        }
        void RefreshSelected()
        {
            plot.ResourceKind=arena.SelectedResourceKind;
            if(arena.SelectedResourceKind>=0) {
                int kind=arena.SelectedResourceKind,quantity=World.MapResourceQuantities()[kind],stock=0,sites=0;
                foreach(Camp c in World.Camps) stock+=c.Stock[kind];
                if(kind==0) foreach(Food f in World.Foods) { if(!f.Eaten) sites++; }
                if(World.Rules.EcosystemEnabled) foreach(Depot d in World.Depots) if(d.Kind==kind && d.Quantite>0) sites++;
                selected.Text=ResourceDisplay.Names[kind].ToUpperInvariant()+"\n\nDisponible sur la carte : "+quantity+" unités\nSites encore disponibles : "+sites+"\nÀ l'aube : "+World.ResourceDailyStart[kind]+" unités\nVariation du jour : "+(quantity-World.ResourceDailyStart[kind]).ToString("+0;-0;0")+"\n\nDans les châteaux : "+stock+" unités\n\nLa courbe sous le terrain suit tous les dépôts de ce type, même si le dépôt cliqué est épuisé.\n\nLes stocks et les sacs des habitants sont exclus de la quantité sur la carte. Les ressources naturelles se renouvellent à l'aube.\n\nCliquez sur le terrain vide ou sur un habitant pour quitter ce suivi.";
                geneticDetails.Text="Une ressource est sélectionnée. Cliquez sur un habitant pour consulter son génome."; return;
            }
            Blob b=arena.Selected;
            if(b==null || !World.Blobs.Contains(b)) { arena.Selected=null; selected.Text="INSPECTER UNE CRÉATURE\n\nCliquez sur un individu pour voir ses traits, son énergie et son comportement."; geneticDetails.Text="Sélectionnez une créature, puis ouvrez cet onglet pour inspecter ses deux copies de chaque gène."; return; }
            string creatureText="CRÉATURE #"+b.Id+"\nSexe : "+(b.Sex==Sex.Female?"femelle":"mâle");
            creatureText+=string.Format("\n\nÉnergie : {0:F0}\nNourriture : {1}",b.Energy,b.Food);
            if(World.Rules.EcosystemEnabled) creatureText+="\nEau : "+b.Water+" / "+World.Rules.WaterForLife;
            creatureText+="\nÉtat : "+(b.Dead?"morte":b.Home?"à l'abri":b.Returning?"retour":b.Food>=World.Rules.ReproductionFood?"cherche partenaire":"cherche nourriture");
            creatureText+="\n\nFaction : "+World.Camps[b.FactionId].Nom;
            if(b.Genes.SkinA!=b.FactionId || b.Genes.SkinB!=b.FactionId || b.FactionId==1 && World.Rules.TrumpFactionEnabled) {
                string skin=Appearance.Names[b.Genes.SkinA].Split('(')[0].Trim();
                if(b.Genes.SkinA!=b.Genes.SkinB) skin+=" + "+Appearance.Names[b.Genes.SkinB].Split('(')[0].Trim();
                creatureText+="\nPeau : "+skin;
            }
            if(World.Rules.EcosystemEnabled) {
                creatureText+="\nMétier : "+b.Job+"\nArme : "+(b.Weapon>0?"oui":"non");
                if(b.RaidTarget>=0 || b.IsLeader || b.GuardLeaderId!=0) creatureText+="\nRôle : "+(b.RaidTarget>=0?"raid vers "+World.Camps[b.RaidTarget].Nom:b.IsLeader?"chef":"garde du chef #"+b.GuardLeaderId);
            }
            creatureText+=string.Format("\n\nPHYSIQUE\nVitesse : {0:F2}\nTaille : {1:F2}\nPerception : {2:F2}",b.Speed,b.Size,b.Sense);
            if(World.Rules.EcosystemEnabled) {
                int bagUsed=0; foreach(int amount in b.Bag) bagUsed+=amount;
                int bagCapacity=World.Rules.TrumpFactionEnabled && b.FactionId==1?5:3;
                creatureText+="\n\nSAC ("+bagUsed+"/"+bagCapacity+")";
                for(int kind=0;kind<b.Bag.Length;kind++) creatureText+="\n"+ResourceDisplay.Names[kind]+" : "+b.Bag[kind];
            }
            selected.Text=creatureText;
            StringBuilder genes=new StringBuilder("GÉNOME #"+b.Id+"\r\n\r\nPeau A : "+Appearance.Names[b.Genes.SkinA]+"\r\nPeau B : "+Appearance.Names[b.Genes.SkinB]+"\r\n");
            string[] traitNames=new string[] { "VITESSE","TAILLE","PERCEPTION" };
            for(int trait=0;trait<Genome.Traits;trait++) { genes.Append("\r\n"+traitNames[trait]+"\r\n  Gène    copie A / copie B\r\n"); for(int gene=0;gene<Genome.GenesPerTrait;gene++) { int i=trait*Genome.GenesPerTrait+gene; genes.AppendFormat("  {0,2}      {1:F3} / {2:F3}\r\n",gene+1,b.Genes.A[i],b.Genes.B[i]); } }
            if(geneticDetails.Text!=genes.ToString()) geneticDetails.Text=genes.ToString();
        }
        void RefreshAll()
        {
            if(displayedRules!=World.Rules) { factionSize.Value=Math.Min(100,World.Rules.FactionSize); displayedRules=World.Rules; }
            int living=0,home=0,food=0,females=0; double speed=0,size=0,sense=0;
            foreach(Blob b in World.Blobs) if(!b.Dead) { living++; if(b.Sex==Sex.Female) females++; if(b.Home) home++; speed+=b.Speed; size+=b.Size; sense+=b.Sense; } foreach(Food f in World.Foods) if(!f.Eaten) food++;
            string summary=string.Format("JOUR {0}  ·  {1}\n\nPopulation vivante : {2}\nMâles : {9}   Femelles : {10}\nCouples formés : {11}\nÀ l'abri : {3}\nNourriture restante : {4} / {5}\n\nTraits moyens actuels\nVitesse : {6:F2}   Taille : {7:F2}\nPerception : {8:F2}",World.Day,World.DayFinished?"terminé":World.Tick+" / "+World.Rules.DayLength,living,home,food,World.FoodToday,living==0?0:speed/living,living==0?0:size/living,living==0?0:sense/living,living-females,females,World.Matings);
            if(World.History.Count>0) { DayRecord d=World.History[World.History.Count-1]; summary+=string.Format("\n\nDernier bilan : +{0} naissances, {1} morts\nDont {2} prédations",d.Births,d.Deaths,d.Predations); if(d.Capped>0) summary+="\nPlafond : "+d.Capped+" naissances bloquées"; }
            details.Text=World.UseCastles?summary.Replace("À l'abri :", "Au château :"):summary;
            pending.Text=World.Pending==null?"Règles actives\n"+World.Rules.Formula:"Règles modifiées en attente.\nApplication à l'aube du jour "+(World.Day+1)+".\nRecommencer les applique immédiatement.";
            status.Text="Graine : "+World.Rules.Seed+"  ·  "+(World.Blobs.Count==0?"Population éteinte. Recommencez ou modifiez les règles.":(timer.Enabled?"En cours":"En pause")+"  ·  Espace : lancer / pause  ·  Plafond de sécurité : 2 000 individus");
            status.Text+="  ·  Tornades : "+(World.TornadoActive?"oui (morts : "+World.TornadoDeaths+", nourriture détruite : "+World.WeatherFoodDestroyed+")":"non")+"  ·  Ressources : "+(World.Rules.UnequalResources?"inégales":"répartition standard"); arena.SimulationRunning=timer.Enabled; RefreshSelected(); diplomacy.Invalidate(); economy.Invalidate(); arena.InvalidateScene(); plot.Invalidate(); dashboard.Invalidate();
        }
        void Edit()
        {
            Pause(); RefreshAll(); using(RulesDialog d=new RulesDialog(World.Pending??World.Rules)) if(d.ShowDialog(this)==DialogResult.OK)
            { if(d.Restart) { World.Reset(d.Draft); arena.ResetCamera(); arena.Selected=null; arena.SelectedResourceKind=-1; } else World.QueueRules(d.Draft); RefreshAll(); }
        }
        void Export()
        {
            Pause(); RefreshAll(); using(SaveFileDialog d=new SaveFileDialog { Filter="Résultats (*.csv)|*.csv",FileName="experience.csv",DefaultExt="csv",AddExtension=true }) if(d.ShowDialog(this)==DialogResult.OK)
                try { File.WriteAllText(d.FileName,World.Csv(),new UTF8Encoding(true)); } catch(Exception ex) { MessageBox.Show(this,ex.Message,"Export impossible"); }
        }
        void Help()
        {
            Pause(); RefreshAll(); MessageBox.Show(this,"Chaque matin, les individus partent de leur château avec un budget d'énergie neuf. Sans châteaux, ils partent des bords.\nIls explorent, poursuivent la nourriture ou une proie visible, et fuient les prédateurs visibles.\n\nSurvie : rentrer vivant avec assez de nourriture (1 par défaut).\nReproduction : un mâle et une femelle se rencontrent sur le terrain avec 2 ressources chacun ; les deux doivent rentrer vivants pour permettre une naissance à la nuit.\nLe bébé hérite de copies de gènes issues des deux parents, avec brassage puis mutations. Dix gènes participent à chaque trait. Un couple donne au maximum un bébé par jour. Les petites crêtes turquoise et rose indiquent les mâles et les femelles. La peau suit la lignée du bord initial ; de grands yeux indiquent une forte perception. Onglet Génome : marqueurs hérités. Onglet Tableau de bord : quantités par lignée, histogrammes des traits, fréquences des marqueurs et filtres par couleur.\n\nLe coût par pas est taille³ × vitesse² + perception par défaut.\nManger ne recharge pas l'énergie. Les habitants rentrés au château sont protégés des prédateurs. Les raids attaquent les défenses et les stocks du château.\n\nCaméra : glisser pour tourner ; molette pour zoomer ; clic droit et glisser pour déplacer ; double-clic pour recentrer.\n\nÉditer les règles : modifiez les valeurs, puis choisissez le prochain jour ou une nouvelle expérience. Import/export XML pour conserver vos modèles.\nFin du jour : terminer la journée ; si elle est déjà terminée, simuler la suivante.\nRecommencer : même graine et mêmes règles (ou règles en attente).\nNouvelle graine + relancer : choisit une graine différente, applique les règles en attente et démarre au jour 1. La graine active est affichée en bas de la fenêtre.\nExporter CSV : bilans des jours terminés et règles correspondantes.\n\nPetit monde : eau et nourriture nécessaires ; consultez Ressources et factions pour les chaînes de production et la protection du chef. Désactivez le petit monde dans les règles pour retrouver la simulation précédente. Adaptation 3D, avec des choix de déplacement propres au projet.\nAnalyse et sources : ANALYSE-PRIMER.md dans le dossier.","Guide du laboratoire",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
        protected override void Dispose(bool disposing) { if(disposing) timer.Dispose(); base.Dispose(disposing); }
    }

    public static class Tests
    {
        sealed class FollowArena : Arena {
            public FollowArena(World world) : base(world) { }
            public void ClickAt(Point point) { OnMouseUp(new MouseEventArgs(MouseButtons.Left,1,point.X,point.Y,0)); }
        }
        static void CheckCameraFollow()
        {
            World world=new World(new Rules(true) { EcosystemEnabled=false }); world.Blobs.Clear();
            Blob first=new Blob(1,1,1) { X=280,Y=150 },second=new Blob(1,1,1) { X=30,Y=150 }; world.Blobs.Add(first); world.Blobs.Add(second);
            using(FollowArena arena=new FollowArena(world)) {
                arena.Size=new Size(900,650); arena.Camera.Zoom=3; arena.Selected=first; arena.UpdateFollow(.033);
                Check(arena.Camera.TargetX>0 && arena.Camera.TargetX<=arena.Camera.Distance*.18*.033,"suivi : déplacement progressif limité, sans saut de caméra");
                for(int frame=0;frame<300;frame++) arena.UpdateFollow(.033);
                double settled=arena.Camera.TargetX; arena.UpdateFollow(.033); first.X+=1; arena.UpdateFollow(.033);
                Check(arena.Camera.TargetX==settled && arena.Selected==first,"zone centrale : caméra immobile pour de petits déplacements, sélection conservée");
                double yaw=arena.Camera.Yaw,zoom=arena.Camera.Zoom; arena.Selected=second; arena.UpdateFollow(.033);
                Check(arena.Camera.TargetX<settled && arena.Camera.Yaw==yaw && arena.Camera.Zoom==zoom,"changement de créature : nouveau suivi, angle et zoom conservés");
                for(int frame=0;frame<300;frame++) arena.UpdateFollow(.033);
                PointF location; double depth; arena.Camera.Project(new Vec3(second.X-world.Width/2,second.Size*4.4,second.Y-world.Height/2),arena.Size,out location,out depth);
                arena.ClickAt(Point.Round(location)); Check(arena.Selected==second,"clic sur une créature active sa sélection persistante");
                arena.ClickAt(new Point(1,1)); double x=arena.Camera.TargetX; first.X=100; arena.UpdateFollow(.1);
                Check(arena.Selected==null && arena.Camera.TargetX==x,"clic sur le terrain annule le suivi et laisse la caméra en place");
            }
        }
        static void CheckResourceSelection()
        {
            World world=new World(new Rules()); world.Foods.Clear(); world.Depots.Clear(); world.Blobs.Clear();
            Food food=new Food(150,150),eaten=new Food(151,150) { Eaten=true }; world.Foods.Add(food); world.Foods.Add(eaten);
            Depot ore=new Depot { X=210,Y=150,Kind=3,Quantite=5 }; world.Depots.Add(ore); world.Depots.Add(new Depot { X=70,Y=150,Kind=1,Quantite=12 });
            Blob worker=new Blob(1,1,1) { X=world.Camps[0].X,Y=world.Camps[0].Y,FactionId=0,Food=1,Water=1,Energy=100,Home=true }; worker.Bag[3]=9; world.Blobs.Add(worker);
            int[] quantities=world.MapResourceQuantities(); Check(quantities[0]==1 && quantities[1]==12 && quantities[3]==5,"quantités sur la carte : nourriture restante et gisements, sans stocks ni sacs");
            using(FollowArena arena=new FollowArena(world)) {
                arena.Size=new Size(1000,650); arena.Camera.Zoom=2; arena.Selected=worker;
                PointF point; double depth; arena.Camera.Project(new Vec3(food.X-world.Width/2,2.25,food.Y-world.Height/2),arena.Size,out point,out depth); arena.ClickAt(Point.Round(point));
                Check(arena.Selected==null && arena.SelectedResourceKind==0,"clic nourriture : sélection du type et arrêt du suivi de créature");
                arena.Camera.Project(new Vec3(ore.X-world.Width/2,world.TerrainHeight(ore.X,ore.Y)+2,ore.Y-world.Height/2),arena.Size,out point,out depth); arena.ClickAt(Point.Round(point));
                Check(arena.SelectedResourceKind==3,"clic minerai : sélection de tous les gisements de minerai");
                ore.Quantite=0; food.Eaten=true; world.FinishDay(); Check(arena.SelectedResourceKind==3 && world.ResourceHistory[world.ResourceHistory.Count-1].Quantities[3]==0,"épuisement du dépôt : sélection conservée et zéro enregistré");
                using(HistoryPlot plot=new HistoryPlot(world) { ResourceKind=3,Size=new Size(1000,155) }) using(Bitmap bitmap=new Bitmap(1000,155)) plot.DrawToBitmap(bitmap,plot.ClientRectangle);
                world.Step(); Check(world.ResourceHistory[world.ResourceHistory.Count-1].Day==2 && world.ResourceDailyStart[3]>0,"renouvellement à l'aube visible dans l'historique des ressources");
                arena.ClickAt(new Point(1,1)); Check(arena.SelectedResourceKind<0,"clic vide annule la sélection de ressource");
            }
            world.Reset(new Rules()); Check(world.ResourceHistory.Count==1 && world.ResourceHistory[0].Day==1,"nouvelle expérience réinitialise l'historique des ressources");
        }
        static void CheckActionEffects()
        {
            World world=new World(new Rules { Predation=false }); world.Blobs.Clear(); world.Depots.Clear(); world.Foods.Clear();
            Blob worker=new Blob(1,1,1) { Id=1,Job=Profession.Mineur,X=150,Y=150,Food=1,Water=1,Energy=900 }; world.Blobs.Add(worker); world.Depots.Add(new Depot { X=150,Y=150,Kind=3,Quantite=3 });
            world.WorkerAction(worker); Check(world.VisualEvents.Count==1 && world.VisualEvents[0].Kind==1,"récolte déclenche un effet visuel");
            world.Foods.Add(new Food(150,150)); world.Step(); bool meal=false; foreach(VisualEvent visual in world.VisualEvents) if(visual.Kind==0) meal=true; Check(meal,"repas déclenche un effet visuel");
            worker.Dead=true; world.Step(); int deaths=0; foreach(VisualEvent visual in world.VisualEvents) if(visual.Kind==2) deaths++; world.Step(); int repeated=0; foreach(VisualEvent visual in world.VisualEvents) if(visual.Kind==2) repeated++;
            Check(deaths==1 && repeated==1,"une seule animation par mort");
            for(int i=0;i<100;i++) world.VisualAction(worker,i%3); Check(world.VisualEvents.Count==64,"file d'événements visuels limitée en mémoire");
            world.VisualAction(worker,3); for(int i=0;i<100;i++) world.VisualAction(worker,1,3); bool retained=false; foreach(VisualEvent visual in world.VisualEvents) if(visual.Kind==3) retained=true;
            Check(retained && world.VisualEvents.Count==64,"cœur conservé malgré une rafale de récoltes, sans augmenter la mémoire");
            using(Arena arena=new Arena(world)) { arena.Size=new Size(800,550); using(Bitmap bitmap=new Bitmap(800,550)) { arena.DrawToBitmap(bitmap,arena.ClientRectangle); arena.ShowActionEffects=false; arena.DrawToBitmap(bitmap,arena.ClientRectangle); } }
            world.Reset(new Rules()); Check(world.VisualEvents.Count==0,"nouvelle expérience efface les anciennes animations");
        }
        static void Check(bool condition,string name) { if(!condition) throw new Exception("Échec : "+name); Console.WriteLine("OK : "+name); }
        static Blob Pair(World world)
        {
            Blob mother=new Blob(1,1,1) { Id=1000+world.Blobs.Count,Sex=Sex.Female,Food=world.Rules.ReproductionFood,Energy=100,X=150,Y=150 };
            Blob father=new Blob(1,1,1) { Id=mother.Id+1,Sex=Sex.Male,Food=world.Rules.ReproductionFood,Energy=100,X=150,Y=150 };
            world.Blobs.Add(mother); world.Blobs.Add(father);
            if(!world.TryMate(mother,father)) throw new Exception("Couple de test invalide.");
            mother.Home=father.Home=true; return mother;
        }
        static void Genetics()
        {
            Rules rules=Rules.Preset(0); rules.EcosystemEnabled=false; rules.DominanceStrength=0; rules.RecombinationChance=0;
            Genome heterozygote=Genome.Uniform(1,1,1,0); for(int i=0;i<Genome.GeneCount;i++) { heterozygote.A[i]=.6; heterozygote.B[i]=1.4; } heterozygote.SkinB=1;
            Check(Math.Abs(heterozygote.Trait(0,0)-1)<.000001,"deux variantes peuvent être cachées derrière un même trait visible");
            Random random=new Random(2026); bool different=false,pure=true,linked=true;
            for(int sample=0;sample<80;sample++)
            {
                Genome child=Genome.Child(heterozygote,heterozygote,random,rules);
                if(Math.Abs(child.Trait(0,0)-1)>.01) different=true;
                for(int trait=0;trait<Genome.Traits;trait++) for(int gene=1;gene<Genome.GenesPerTrait;gene++) if(child.A[trait*10+gene]!=child.A[trait*10] || child.B[trait*10+gene]!=child.B[trait*10]) pure=false;
                if(child.SkinA!=(child.A[0]==.6?0:1) || child.SkinB!=(child.B[0]==.6?0:1)) linked=false;
            }
            Check(different,"enfants différents des moyennes parentales même sans mutation"); Check(pure,"sans brassage, transmission de chromosomes entiers"); Check(linked,"marqueur de peau lié au premier chromosome");
            rules.RecombinationChance=.5; bool mixed=false; for(int sample=0;sample<20;sample++) { Genome child=Genome.Child(heterozygote,heterozygote,random,rules); for(int gene=1;gene<10;gene++) if(child.A[gene]!=child.A[0]) mixed=true; } Check(mixed,"brassage de segments entre chromosomes");
            Genome singleMarker=Genome.Uniform(1,1,1,0); singleMarker.A[0]=.6; singleMarker.B[0]=1.4;
            Check(Math.Abs(singleMarker.Trait(0,1)-singleMarker.Trait(0,0))>.001,"dominance modifie les effets des variantes, sans changer les gènes");
            Check(Math.Abs(heterozygote.Trait(0,1)-1)<.000001,"dominance équilibrée : autant de poids vers le haut que vers le bas");
            rules.MutateSize=true; rules.MutationChance=1; rules.MutationAmount=.1; Genome uniform=Genome.Uniform(1,1,1,0),mutated=Genome.Child(uniform,uniform,random,rules); bool isolated=true; for(int i=0;i<30;i++) { if(i<10 || i>=20) { if(mutated.A[i]!=1 || mutated.B[i]!=1) isolated=false; } else if(Math.Abs(mutated.A[i]-1)<.01 || Math.Abs(mutated.B[i]-1)<.01) isolated=false; } Check(isolated,"mutation ciblée des gènes de taille seulement");
            World world=new World(new Rules(true) { EcosystemEnabled=false, InitialDiversity=0 }); bool origins=true; int[] regions=new int[4]; foreach(Blob b in world.Blobs) { regions[b.StartingSide]++; if(b.Genes.SkinA!=b.StartingSide || b.Genes.SkinB!=b.StartingSide) origins=false; if(b.StartingSide==0 && b.X!=0 || b.StartingSide==1 && b.X!=World.Extent || b.StartingSide==2 && b.Y!=0 || b.StartingSide==3 && b.Y!=World.Extent) origins=false; } Check(origins && regions[0]>0 && regions[1]>0 && regions[2]>0 && regions[3]>0,"quatre peaux fondatrices correspondant aux quatre bords de départ");
            Blob first=world.Blobs[0]; int original=first.Genes.SkinA; foreach(Blob b in world.Blobs) { b.Home=true; b.Food=1; } world.FinishDay(); world.Step(); Check(first.Genes.SkinA==original,"peau conservée après changement de jour et déplacement");
            world=new World(new Rules(true) { EcosystemEnabled=false }); bool diversity=false; foreach(Blob b in world.Blobs) for(int i=0;i<30;i++) if(b.Genes.A[i]!=b.Genes.B[i]) diversity=true; Check(diversity,"diversité génétique chez les fondateurs");
            Check(Appearance.EyeScale(2)>Appearance.EyeScale(1) && Appearance.EyeScale(1)>Appearance.EyeScale(.5),"taille des yeux liée à la perception"); Check(Appearance.BodyRadius(2)==2*Appearance.BodyRadius(1),"encombrement du corps proportionnel à la taille");
            Genome skins=Genome.Uniform(1,1,1,3); skins.SkinB=0; Check(Appearance.PrimarySkin(skins)==0 && Appearance.SecondarySkin(skins)==3 && Appearance.Skins.Length==4,"deux couleurs héritées exprimées avec la palette de quatre couleurs");
        }
        static void Dashboard()
        {
            World world=new World(new Rules(true) { EcosystemEnabled=false }); world.Blobs.Clear();
            int[] a=new int[] { 0,1,0,2 },b=new int[] { 0,1,2,3 };
            for(int i=0;i<4;i++) { Blob individual=new Blob(.5+i*.5,1,1) { Home=true,Energy=10,Food=1 }; individual.Genes.SkinA=a[i]; individual.Genes.SkinB=b[i]; world.Blobs.Add(individual); }
            world.Blobs.Add(new Blob(4,4,4) { Dead=true });
            PopulationSnapshot snapshot=PopulationSnapshot.Capture(world,-1);
            Check(snapshot.Count==4 && snapshot.Mixed==2 && snapshot.Pure[0]==1 && snapshot.Pure[1]==1,"effectifs uniques : vivants, peaux unies et mixtes");
            Check(snapshot.Copies[0]==3 && snapshot.Copies[1]==2 && snapshot.Copies[2]==2 && snapshot.Copies[3]==1,"quantités réelles des marqueurs de couleur");
            Check(snapshot.Carriers[0]==2 && snapshot.Carriers[1]==1 && snapshot.Carriers[2]==2 && snapshot.Carriers[3]==1,"porteurs par lignée, sans compter deux fois un individu uni");
            Check(Math.Abs(snapshot.Traits[0].Mean-1.25)<.000001 && snapshot.Traits[0].Median==1.25 && snapshot.Traits[0].Minimum==.5 && snapshot.Traits[0].Maximum==2,"statistiques des phénotypes exprimés");
            snapshot=PopulationSnapshot.Capture(world,0); Check(snapshot.Count==4 && snapshot.Individuals.Count==2 && snapshot.Traits[0].Mean==1,"filtre de lignée sur les traits, avec compteurs globaux");
            snapshot=PopulationSnapshot.Capture(world,4); Check(snapshot.Individuals.Count==2,"filtre des individus mixtes");
            world.FinishDay(); Check(world.History[0].SkinCopies[0]==3 && world.History[0].Mixed==2 && world.Csv().Contains("copies_corail"),"historique et export des lignées");
            world.Blobs.Clear(); snapshot=PopulationSnapshot.Capture(world,-1); Check(snapshot.Count==0 && snapshot.Traits[0].Values.Length==0,"tableau de bord après extinction");
            using(DashboardCanvas canvas=new DashboardCanvas(world)) { canvas.Size=new Size(1180,760); using(Bitmap bitmap=new Bitmap(canvas.Width,canvas.Height)) { canvas.DrawToBitmap(bitmap,canvas.ClientRectangle); Check(bitmap.GetPixel(20,70).ToArgb()!=bitmap.GetPixel(0,0).ToArgb(),"rendu du tableau de bord même sans population"); } }
        }
        public static void Run()
        {
            Rules r=new Rules(true) { EcosystemEnabled=false }; r.Validate(); Blob a=new Blob(2,3,4); Check(World.Cost(a,r)==112,"coût taille³ × vitesse² + perception");
            Blob hunter=new Blob(1,1.2,1),prey=new Blob(1,1,1); Check(World.CanEat(hunter,prey,r),"prédation à +20 %"); prey.Home=true; Check(!World.CanEat(hunter,prey,r),"protection des abris"); prey.Home=false; r.Predation=false; Check(!World.CanEat(hunter,prey,r),"prédation désactivée");
            World w=new World(new Rules(true) { EcosystemEnabled=false }); w.Blobs.Clear(); Blob noFood=new Blob(1,1,1) { Home=true,Energy=10 },one=new Blob(1,1,1) { Home=true,Food=1,Energy=10 },two=new Blob(1,1,1) { Home=true,Food=2,Energy=10 },outside=new Blob(1,1,1) { Food=2,Energy=10 };
            w.Blobs.AddRange(new Blob[] { noFood,one,two,outside }); w.FinishDay(); Check(w.Blobs.Count==2 && w.History[0].Births==0 && w.History[0].Deaths==2,"survie avec retour, aucune naissance sans rencontre");
            Check(two.Speed==1 && two.Size==1 && two.Sense==1,"les parents ne mutent pas");
            Rules mutation=new Rules(true) { EcosystemEnabled=false, MutationChance=1,MutationAmount=.1 }; w=new World(mutation); w.Blobs.Clear(); Blob parent=Pair(w); w.FinishDay(); Blob child=w.Blobs[2]; bool allMutated=true; for(int i=0;i<Genome.GeneCount;i++) if(Math.Abs(Math.Abs(child.Genes.A[i]-1)-.1)>.00001 || Math.Abs(Math.Abs(child.Genes.B[i]-1)-.1)>.00001) allMutated=false; Check(allMutated,"mutations sur chaque copie héritée avec une probabilité de 1"); Check(parent.Speed==1 && parent.Mate.Speed==1 && parent.Genes.A[0]==1,"traits et génomes des parents inchangés");
            Rules frozen=Rules.Preset(0); frozen.EcosystemEnabled=false; w=new World(frozen); w.Blobs.Clear(); parent=Pair(w); parent.Genes=Genome.Uniform(.8,.6,1,0); parent.Mate.Genes=Genome.Uniform(1.2,1.4,1,1); w.FinishDay(); Check(w.Blobs[2].Genes.A[0]==.8 && w.Blobs[2].Genes.B[0]==1.2 && w.Blobs[2].Genes.A[10]==.6 && w.Blobs[2].Genes.B[10]==1.4,"copies de gènes transmises sans mutation"); Check(w.Blobs[2].MotherId==parent.Id && w.Blobs[2].FatherId==parent.Mate.Id,"identité des parents du bébé"); Check(w.Blobs[2].Genes.SkinA==0 && w.Blobs[2].Genes.SkinB==1,"deux marqueurs de peau hérités des deux parents");
            Rules custom=new Rules(true) { EcosystemEnabled=false, SurvivalFood=2,ReproductionFood=3 }; w=new World(custom); w.Blobs.Clear(); w.Blobs.Add(new Blob(1,1,1) { Food=1,Home=true,Energy=10 }); Pair(w); w.FinishDay(); Check(w.Blobs.Count==3 && w.History[0].Deaths==1 && w.History[0].Births==1,"seuils personnalisés appliqués aux deux parents");
            w=new World(new Rules(true) { EcosystemEnabled=false }); Rules queued=new Rules(true) { EcosystemEnabled=false, FoodPerDay=7 }; w.QueueRules(queued); queued.FoodPerDay=9; Check(w.Rules.FoodPerDay==100 && w.Pending.FoodPerDay==7,"règles copiées et différées"); foreach(Blob b in w.Blobs) { b.Home=true; b.Food=1; } w.FinishDay(); w.Step(); Check(w.Day==2 && w.FoodToday==7 && w.Pending==null,"application des règles à l'aube");
            World dry=new World(new Rules(true) { EcosystemEnabled=false, FoodPerDay=0,DayLength=20 }); dry.AdvanceDay(); Check(dry.Blobs.Count==0,"extinction sans ressources");
            w=new World(new Rules(true) { EcosystemEnabled=false, FoodPerDay=100,FoodDecrease=1,DecreaseInterval=2 }); for(int i=0;i<2;i++) { foreach(Blob b in w.Blobs) { b.Home=true; b.Food=1; } w.FinishDay(); w.Step(); } Check(w.Day==3 && w.FoodToday==99,"raréfaction d'une ressource tous les deux jours");
            w=new World(new Rules(true) { EcosystemEnabled=false }); w.Blobs.Clear(); for(int i=0;i<World.PopulationLimit/2;i++) Pair(w); w.FinishDay(); Check(w.Blobs.Count==World.PopulationLimit && w.History[0].Capped==World.PopulationLimit/2,"plafond explicite de population");
            World first=new World(new Rules(true) { EcosystemEnabled=false }),second=new World(new Rules(true) { EcosystemEnabled=false }); for(int i=0;i<3;i++) { first.AdvanceDay(); second.AdvanceDay(); } Check(first.Csv()==second.Csv(),"reproductibilité avec la même graine"); Check(first.History.Count>=1,"bilans et export CSV");
            string file=Path.GetTempFileName(); try { custom.MatingDistance=12; custom.FemaleRatio=.4; custom.Save(file); Rules loaded=Rules.Load(file); Check(loaded.SurvivalFood==2 && loaded.ReproductionFood==3 && loaded.MatingDistance==12 && loaded.FemaleRatio==.4,"export/import XML et règles de rencontre"); File.WriteAllText(file,"<Rules><FoodPerDay>100</FoodPerDay></Rules>"); loaded=Rules.Load(file); Check(loaded.MatingDistance==8 && loaded.FemaleRatio==.5,"anciens XML : nouveaux paramètres par défaut"); File.WriteAllText(file,"<!DOCTYPE Rules [<!ENTITY ext SYSTEM 'file:///missing'>]><Rules>&ext;</Rules>"); bool rejected=false; try { Rules.Load(file); } catch(Exception) { rejected=true; } Check(rejected,"rejet des déclarations XML externes"); } finally { File.Delete(file); }
            bool invalid=false; try { new Rules(true) { EcosystemEnabled=false, MutationChance=double.NaN }.Validate(); } catch(ArgumentException) { invalid=true; } Check(invalid,"validation des valeurs non finies");
            w=new World(new Rules(true) { EcosystemEnabled=false, Predation=false }); w.Blobs.Clear(); w.Foods.Clear(); Blob firstEater=new Blob(.2,1,1) { X=150,Y=150,Energy=100 },secondEater=new Blob(.2,1,1) { X=150,Y=150,Energy=100 }; w.Blobs.Add(firstEater); w.Blobs.Add(secondEater); w.Foods.Add(new Food(150,150)); w.Step(); Check(firstEater.Food+secondEater.Food==1 && w.Foods[0].Eaten,"une ressource ne peut pas être mangée deux fois"); Check(firstEater.Energy<100 && secondEater.Energy<100,"manger ne recharge pas le budget énergétique");
            w=new World(new Rules(true) { EcosystemEnabled=false }); w.Blobs.Clear(); w.Foods.Clear(); parent=Pair(w); parent.Home=parent.Mate.Home=false; parent.X=parent.Mate.X=.5; w.Step(); Check(parent.Home && parent.Mate.Home && w.History.Count==1 && w.History[0].Births==1,"rencontre puis retour des deux parents et naissance unique");
            w=new World(new Rules(true) { EcosystemEnabled=false, Predation=false }); w.Blobs.Clear(); w.Foods.Clear(); Blob male=new Blob(1,1,1) { Sex=Sex.Male,Food=2,Energy=800,X=150,Y=150 },female=new Blob(1,1,1) { Sex=Sex.Female,Food=2,Energy=800,X=160,Y=150 }; w.Blobs.Add(male); w.Blobs.Add(female);
            Check(!w.TryMate(male,female),"aucun accouplement à distance"); female.X=158; female.Food=1; Check(!w.TryMate(male,female),"deux nourritures nécessaires pour chaque parent"); female.Food=2; female.Sex=Sex.Male; Check(!w.TryMate(male,female),"deux mâles ne peuvent pas se reproduire"); male.Sex=Sex.Female; female.Sex=Sex.Female; Check(!w.TryMate(male,female),"deux femelles ne peuvent pas se reproduire"); male.Sex=Sex.Male; female.Home=true; Check(!w.TryMate(male,female),"rencontre impossible dans les abris"); female.Home=false;
            female.X=170; w.Step(); Check(World.Distance(male.X,male.Y,female.X,female.Y)<20 && male.Mate==null,"recherche active d'un partenaire visible avant le retour"); female.X=male.X+8; female.Y=male.Y; Check(w.TryMate(male,female) && male.Mate==female && female.Mate==male,"rencontre mâle-femelle au rayon autorisé"); Check(!w.TryMate(male,female) && w.Matings==1,"un seul accouplement par individu et par jour");
            male.Home=female.Home=true; female.Dead=true; w.FinishDay(); Check(w.History[0].Births==0,"aucune naissance si un parent meurt après la rencontre");
            w=new World(new Rules(true) { EcosystemEnabled=false }); w.Blobs.Clear(); parent=Pair(w); w.FinishDay(); int births=w.History[0].Births; w.FinishDay(); Check(w.History.Count==1 && births==1,"un seul bébé par couple et bilan non dupliqué"); w.Step(); bool linksReset=true; foreach(Blob b in w.Blobs) if(b.Mate!=null) linksReset=false; Check(linksReset && w.Day==2,"couples réinitialisés chaque matin");
            w=new World(new Rules(true) { EcosystemEnabled=false, InitialPopulation=20 }); int initialFemales=0; foreach(Blob b in w.Blobs) if(b.Sex==Sex.Female) initialFemales++; Check(initialFemales==10,"population initiale équilibrée");
            w=new World(new Rules(true) { EcosystemEnabled=false, FemaleRatio=1 }); bool allFemale=true; foreach(Blob b in w.Blobs) if(b.Sex!=Sex.Female) allFemale=false; Check(allFemale,"population exclusivement femelle selon le réglage");
            Genetics();
            Dashboard();
            Camera3D camera=new Camera3D(); PointF center,above,near; double centerDepth,aboveDepth,nearDepth; Size viewport=new Size(700,600); Check(camera.Project(new Vec3(0,0,0),viewport,out center,out centerDepth),"projection perspective du centre"); camera.Project(new Vec3(0,10,0),viewport,out above,out aboveDepth); camera.Project(camera.Out*30,viewport,out near,out nearDepth); Check(above.Y<center.Y && nearDepth<centerDepth,"hauteur et profondeur réelles dans la caméra 3D");
            using(Arena scene=new Arena(new World(new Rules(true) { EcosystemEnabled=false }))) { scene.Size=viewport; using(Bitmap bitmap=new Bitmap(700,600)) { scene.DrawToBitmap(bitmap,new Rectangle(0,0,700,600)); Check(bitmap.GetPixel(350,300).ToArgb()!=bitmap.GetPixel(0,0).ToArgb(),"rendu du terrain et des maillages 3D"); } }
            CheckCameraFollow(); CheckResourceSelection(); CheckActionEffects(); using(MainWindow window=new MainWindow())
            {
                Check(window.Controls.Count==1,"construction de la fenêtre principale");
                window.World.Step(); window.World.QueueRules(new Rules(true) { Seed=-42,FoodPerDay=10,InitialPopulation=24 });
                window.RerollAndRestart(); int seed=window.World.Rules.Seed;
                Check(seed>=0 && seed!=42 && window.World.Day==1 && window.World.Tick==0 && window.World.History.Count==0,"nouvelle graine différente et simulation remise au premier jour");
                Check(window.World.Rules.FoodPerDay==10 && window.World.Rules.InitialPopulation==24 && window.World.Pending==null,"relance aléatoire conserve et applique les règles en attente");
                Check(window.Controls.Find("RandomSeedRestart",true).Length==1,"bouton de relance aléatoire présent dans la fenêtre");
            }
            using(RulesDialog dialog=new RulesDialog(new Rules(true) { EcosystemEnabled=false })) Check(dialog.Draft.FoodPerDay==100,"construction de l'éditeur de règles");
            EcosystemTests.Run(); DiplomacyTests.Run(); BalancedWorldTests.Run(); Console.WriteLine("Toutes les vérifications ont réussi.");
        }
        public static void Render(string path)
        {
            World world=new World(new Rules()); for(int i=0;i<100;i++) world.Step();
            using(Arena arena=new Arena(world)) { arena.Size=new Size(900,700); using(Bitmap bitmap=new Bitmap(900,700)) { arena.DrawToBitmap(bitmap,new Rectangle(0,0,900,700)); bitmap.Save(path); } }
        }
        public static void RenderTraits(string path)
        {
            World world=new World(new Rules()); world.Blobs.Clear(); world.Foods.Clear();
            double[] sizes=new double[] { .6,1,1.4,1.8,1.1 },senses=new double[] { .4,1,2,3,1.5 };
            for(int i=0;i<5;i++) { Blob b=new Blob(1,sizes[i],senses[i]) { Id=i+1,X=90+i*30,Y=150,Heading=Math.PI/2,Sex=i%2==0?Sex.Female:Sex.Male }; b.Genes=Genome.Uniform(1,sizes[i],senses[i],i%4); if(i==4) b.Genes.SkinB=3; b.Express(world.Rules); world.Blobs.Add(b); }
            using(Arena arena=new Arena(world))
            {
                arena.Size=new Size(1000,600); arena.Camera.Yaw=0; arena.Camera.Elevation=.4; arena.Camera.Zoom=3.5;
                using(Bitmap bitmap=new Bitmap(1000,600))
                {
                    arena.DrawToBitmap(bitmap,new Rectangle(0,0,1000,600));
                    using(Graphics graphics=Graphics.FromImage(bitmap)) using(Brush white=new SolidBrush(Color.White))
                    {
                        graphics.DrawString("DÉMONSTRATION DES TRAITS — quatre peaux fondatrices et une lignée mixte",arena.Font,white,12,65);
                        foreach(Blob b in world.Blobs) { PointF p; double depth; arena.Camera.Project(new Vec3(b.X-150,-8,b.Y-150),arena.Size,out p,out depth); graphics.DrawString("Taille "+b.Size.ToString("0.0")+"\nPerception "+b.Sense.ToString("0.0"),arena.Font,white,p.X-40,p.Y); }
                    }
                    bitmap.Save(path);
                }
            }
        }
        public static void RenderDashboard(string path)
        {
            World world=new World(new Rules()); for(int day=0;day<20 && world.Blobs.Count>0;day++) world.AdvanceDay();
            using(DashboardCanvas canvas=new DashboardCanvas(world)) { canvas.Size=new Size(1180,760); using(Bitmap bitmap=new Bitmap(canvas.Width,canvas.Height)) { canvas.DrawToBitmap(bitmap,canvas.ClientRectangle); bitmap.Save(path); } }
        }
    }
}
