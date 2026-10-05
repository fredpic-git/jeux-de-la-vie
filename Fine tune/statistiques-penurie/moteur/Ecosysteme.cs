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
        public string Nom; public int[] Stock=new int[8]; public int Capacite=40, Reserves;
        public bool Consume(int food,int water,int soil,int ore,int coal,int energy,int iron)
        {
            int[] needs={food,water,soil,ore,coal,energy,iron,0};
            for(int i=0;i<8;i++) if(Stock[i]<needs[i]) return false;
            for(int i=0;i<8;i++) Stock[i]-=needs[i]; return true;
        }
        public void Add(int kind,int n) { Stock[kind]=Math.Min(Capacite,Stock[kind]+n); }
    }
    public class Depot { public double X,Y; public int Kind,Quantite; }
    public partial class Rules
    {
        bool ecosystemEnabled=true,leaderEnabled=true; int waterForLife=1; double mountainDifficulty=1.7,weaponBonus=.6,raidChance=.2;
        [Category("Monde et ressources"),DisplayName("Activer le petit monde")] public bool EcosystemEnabled { get { return ecosystemEnabled; } set { ecosystemEnabled=value; } }
        [Category("Monde et ressources"),DisplayName("Eau nécessaire par jour")] public int WaterForLife { get { return waterForLife; } set { waterForLife=value; } }
        [Category("Monde et ressources"),DisplayName("Difficulté des montagnes")] public double MountainDifficulty { get { return mountainDifficulty; } set { mountainDifficulty=value; } }
        [Category("Factions"),DisplayName("Chef trumpiste et protection")] public bool LeaderEnabled { get { return leaderEnabled; } set { leaderEnabled=value; } }
        [Category("Factions"),DisplayName("Probabilité de raid au contact")] public double RaidChance { get { return raidChance; } set { raidChance=value; } }
        [Category("Factions"),DisplayName("Bonus de force par arme")] public double WeaponBonus { get { return weaponBonus; } set { weaponBonus=value; } }
        void ValidateEcosystem() { ValidateDiplomacy(); if(WaterForLife<0 || WaterForLife>5 || double.IsNaN(MountainDifficulty) || MountainDifficulty<1 || MountainDifficulty>5 || double.IsNaN(RaidChance) || RaidChance<0 || RaidChance>1 || double.IsNaN(WeaponBonus) || WeaponBonus<0 || WeaponBonus>3) throw new ArgumentException("Règles du monde invalides : eau 0–5, montagne 1–5, raid 0–1, arme 0–3."); }
    }
    public partial class Blob
    {
        public int FactionId,Water,Weapon,GuardLeaderId; public Profession Job; public bool IsLeader;
        public int[] Bag=new int[8]; public int LastAction=-50;
        public bool HasOrangeCrest(Rules rules) { return rules.EcosystemEnabled && rules.LeaderEnabled && FactionId==1 && IsLeader && !Dead; }
    }
    public partial class World
    {
        public Camp[] Camps; public List<Depot> Depots=new List<Depot>(); public int[,] Relations=new int[4,4];
        public int SunkFood,Raids,ThirstDeaths,LeaderMeals,Protections; public List<string> Journal=new List<string>();
        public static bool Lake(double x,double y) { return Distance(x,y,208,90)<26 || Distance(x,y,82,212)<25; }
        public static double HeightAt(double x,double y) { return Math.Max(0,32*(1-Distance(x,y,92,80)/48))+Math.Max(0,38*(1-Distance(x,y,214,215)/45)); }
        void ResetEcosystem()
        {
            Camps=new Camp[4]; string[] names={"Bleus","Trumpistes","Verts","Violets"};
            for(int i=0;i<4;i++) { Camps[i]=new Camp { Nom=names[i] }; Camps[i].Stock=new int[] {12,20,6,4,3,8,0,0}; }
            ResetDiplomacy(); Depots.Clear(); Journal.Clear(); Relations=new int[4,4]; Raids=ThirstDeaths=LeaderMeals=Protections=SunkFood=0;
        }
        void Log(string s) { Journal.Insert(0,"J"+Day+" : "+s); if(Journal.Count>12) Journal.RemoveAt(12); }
        void BeginEcosystemDay()
        {
            if(!Rules.EcosystemEnabled) return;
            SunkFood=0; foreach(Food f in Foods) if(Lake(f.X,f.Y)) { f.Eaten=true; SunkFood++; }
            Depots.Clear();
            for(int i=0;i<32;i++) { double x=20+random.NextDouble()*260,y=20+random.NextDouble()*260; if(!Lake(x,y)) Depots.Add(new Depot { X=x,Y=y,Kind=2,Quantite=4 }); }
            for(int i=0;i<18;i++) { double cx=i%2==0?92:214,cy=i%2==0?80:215; Depots.Add(new Depot { X=cx+random.NextDouble()*50-25,Y=cy+random.NextDouble()*50-25,Kind=i%3==0?4:3,Quantite=4 }); }
            for(int i=0;i<10;i++) Depots.Add(new Depot { X=25+random.NextDouble()*250,Y=25+random.NextDouble()*250,Kind=5,Quantite=3 });
            for(int i=0;i<32;i++) { double a=i*Math.PI/16; double cx=i<16?208:82,cy=i<16?90:212; Depots.Add(new Depot { X=cx+Math.Cos(a)*29,Y=cy+Math.Sin(a)*29,Kind=1,Quantite=12 }); }
            Blob leader=null; foreach(Blob b in Blobs) { if(b.FactionId==1 && !b.Dead && b.IsLeader) leader=b; if(!Rules.LeaderEnabled) b.IsLeader=false; }
            if(Rules.LeaderEnabled && leader==null) foreach(Blob b in Blobs) if(b.FactionId==1 && !b.Dead) { b.IsLeader=true; Log("Nouveau chef trumpiste #"+b.Id); break; }
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
        public static int FoodGoal(Blob b,Rules r) { return r.ReproductionFood; }
        public static bool HasWater(Blob b,Rules r) { return !r.EcosystemEnabled || b.Water>=r.WaterForLife; }
        public static double Strength(Blob b,Rules r)
        { return b.Size*(1+(r.EcosystemEnabled?r.WeaponBonus*b.Weapon:0)); }
        public static double Defense(Blob b,Rules r) { return Strength(b,r)*(r.EcosystemEnabled && r.LeaderEnabled && b.IsLeader?2:1); }
        double MovementMultiplier(Blob b) { return !Rules.EcosystemEnabled?1:1+(Rules.MountainDifficulty-1)*Math.Min(1,HeightAt(b.X,b.Y)/20); }
        void AvoidLake(Blob b,double speed,ref double x,ref double y)
        { if(!Rules.EcosystemEnabled || !Lake(x,y)) return; for(int i=1;i<=8;i++) { double angle=b.Heading+i*Math.PI/4; x=b.X+Math.Cos(angle)*speed; y=b.Y+Math.Sin(angle)*speed; if(!Lake(x,y)) { b.Heading=angle; return; } } x=b.X; y=b.Y; }
        bool WorkDirection(Blob b,out double heading)
        {
            heading=0; if(!Rules.EcosystemEnabled || b.Returning || b.Mate!=null) return false;
            if(b.GuardLeaderId!=0 && HasWater(b,Rules) && b.Food>=Rules.SurvivalFood)
                foreach(Blob chief in Blobs) if(chief.Id==b.GuardLeaderId && !chief.Dead && !chief.Home && Distance(b.X,b.Y,chief.X,chief.Y)>12 && b.Energy>Cost(b,Rules)*(HomeDistance(b)/Math.Max(.2,b.Speed)+30))
                { heading=Math.Atan2(chief.Y-b.Y,chief.X-b.X); return true; }
            if(RaidDirection(b,out heading)) return true; Depot best=null; double distance=double.MaxValue; int bag=0; foreach(int n in b.Bag) bag+=n;
            foreach(Depot d in Depots)
            {
                bool wanted=b.Water<Rules.WaterForLife?d.Kind==1:!b.IsLeader && b.Food>=Rules.SurvivalFood && bag<(b.FactionId==1?5:3) && Tick<Rules.DayLength*.45 && (d.Kind==1 || (int)b.Job==0 && d.Kind==2 || (int)b.Job==1 && (d.Kind==3 || d.Kind==4) || (int)b.Job>=2 && d.Kind==5);
                double dist=Distance(b.X,b.Y,d.X,d.Y); if(wanted && d.Quantite>0 && dist<distance) { best=d; distance=dist; }
            }
            if(best!=null) { heading=Math.Atan2(best.Y-b.Y,best.X-b.X); return true; } return false;
        }
        void WorkerAction(Blob b)
        {
            if(!Rules.EcosystemEnabled || b.Returning) return;
            if(b.IsLeader && Rules.LeaderEnabled)
            {
                foreach(Blob ally in Blobs) if(ally!=b && ally.FactionId==1 && !ally.Dead && Distance(b.X,b.Y,ally.X,ally.Y)<25)
                { if(b.Food<Rules.SurvivalFood && ally.Food>Rules.SurvivalFood) { ally.Food--; b.Food++; LeaderMeals++; } if(b.Water<Rules.WaterForLife && ally.Water>Rules.WaterForLife) { ally.Water--; b.Water++; } }
            }
            CoalitionAction(b); if(Tick-b.LastAction<15) return;
            int total=0; foreach(int n in b.Bag) total+=n;
            foreach(Depot d in Depots) if(d.Quantite>0 && Distance(b.X,b.Y,d.X,d.Y)<6)
            { if(d.Kind==1 && b.Water<Rules.WaterForLife) { b.Water++; d.Quantite--; b.LastAction=Tick; break; } if(!b.IsLeader && total<(b.FactionId==1?5:3)) { b.Bag[d.Kind]++; d.Quantite--; b.LastAction=Tick; break; } }
            foreach(RaidPlan plan in RaidPlans) if(plan.Fighters.Contains(b) && ValidPlan(plan)) return;
            if(b.FactionId!=1 || b.IsLeader || random.NextDouble()>Rules.RaidChance) return;
            foreach(Blob victim in Blobs) if(victim.FactionId!=1 && !Allied(b.FactionId,victim.FactionId) && !victim.Dead && !victim.Home && Distance(b.X,b.Y,victim.X,victim.Y)<9)
            { if(victim.Food>0) { victim.Food--; b.Food++; } victim.Energy=Math.Max(0,victim.Energy-15*(1+b.Weapon)); if(victim.Energy==0) victim.Dead=true; Raids++; b.LastAction=Tick; CommonThreat(b.FactionId,victim.FactionId); break; }
        }
        void CountDeath(Blob b) { if(Rules.EcosystemEnabled && !HasWater(b,Rules)) ThirstDeaths++; }
        void FinishEcosystemDay(List<Blob> survivors)
        {
            if(!Rules.EcosystemEnabled) return;
            foreach(Blob b in survivors) { Camp c=Camps[b.FactionId]; c.Add(0,Math.Max(0,b.Food-Rules.SurvivalFood)); c.Add(1,Math.Max(0,b.Water-Rules.WaterForLife)); for(int i=1;i<8;i++) c.Add(i,b.Bag[i]); }
            for(int f=0;f<4;f++)
            {
                Camp c=Camps[f]; int[] jobs=new int[4]; foreach(Blob b in survivors) if(b.FactionId==f) jobs[(int)b.Job]++;
                for(int i=0;i<Math.Min(4,jobs[2]+jobs[3]);i++) { if(c.Consume(0,0,0,0,1,0,0)) c.Add(5,4); if(c.Consume(0,0,0,2,0,2,0)) c.Add(6,1); }
                for(int i=0;i<Math.Min(4,jobs[0]);i++) if(c.Consume(0,2,2,0,0,1,0)) c.Add(0,3);
                if(jobs[2]>0) { if(c.Reserves<8 && c.Consume(0,0,0,0,0,2,2)) { c.Reserves++; c.Capacite+=15; } if(c.Consume(0,0,0,0,0,2,1)) c.Add(7,1); }
            }
            for(int a=0;a<4;a++) for(int z=a+1;z<4;z++) if(Relations[a,z]>-20) for(int k=0;k<2;k++) { Camp donor=Camps[a].Stock[k]>Camps[z].Stock[k]?Camps[a]:Camps[z],recipient=donor==Camps[a]?Camps[z]:Camps[a]; if(donor.Stock[k]>8 && recipient.Stock[k]<4) { donor.Stock[k]--; recipient.Add(k,1); ChangeTrust(a,z,2); } }
        }
        public bool ProtectedLeader(Blob prey,Blob hunter)
        {
            if(!Rules.EcosystemEnabled || !Rules.LeaderEnabled || !prey.IsLeader) return false;
            foreach(Blob guard in Blobs) if(guard!=prey && guard.FactionId==prey.FactionId && !guard.Dead && !guard.Home && Distance(guard.X,guard.Y,prey.X,prey.Y)<22)
            { guard.Energy-=8; hunter.Energy-=8; Protections++; return true; } return false;
        }
    }
    public partial class Arena
    {
        void Landscape()
        {
            for(int x=0;x<300;x+=10) for(int y=0;y<300;y+=10)
            {
                bool lake=World.Lake(x+5,y+5); Color c=lake?Color.FromArgb(45,139,184):World.HeightAt(x+5,y+5)>8?Color.FromArgb(131,128,116):Color.FromArgb(72,117,83);
                Vec3 a=new Vec3(x-150,lake?.1:World.HeightAt(x,y),y-150),b=new Vec3(x-150,lake?.1:World.HeightAt(x,y+10),y-140),cc=new Vec3(x-140,lake?.1:World.HeightAt(x+10,y+10),y-140),d=new Vec3(x-140,lake?.1:World.HeightAt(x+10,y),y-150);
                AddFace(new Vec3[] { a,b,cc,d },c,false);
            }
            foreach(Depot d in World.Depots) if(d.Quantite>0 && d.Kind!=1) Sphere(new Vec3(d.X-150,World.HeightAt(d.X,d.Y)+2,d.Y-150),2,2,2,d.Kind==3?Color.Silver:d.Kind==4?Color.FromArgb(45,42,43):d.Kind==5?Color.Gold:Color.FromArgb(153,100,58),6,4);
        }
        void FactionHat(Blob b,Vec3 center,double r)
        {
            if(!World.Rules.EcosystemEnabled || b.FactionId!=1) return;
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
            if(!World.Rules.EcosystemEnabled) return;
            foreach(Blob b in World.Blobs) if(!b.Dead && b.FactionId==1)
            {
                PointF p; double depth;
                if(Project(new Vec3(b.X-150,World.HeightAt(b.X,b.Y)+b.Size*9,b.Y-150),out p,out depth))
                using(Font font=new Font("Segoe UI",b.IsLeader?8:6,FontStyle.Bold))
                    g.DrawString(b.HasOrangeCrest(World.Rules)?"CHEF":"MAGA",font,Brushes.White,p.X-12,p.Y-8);
            }
        }
    }
    public static class EcosystemTests
    {
        static void Assert(bool b,string message) { if(!b) throw new Exception(message); Console.WriteLine("OK : "+message); }
        public static void Run()
        {
            World w=new World(new Rules()); Blob chief=null; int chiefs=0;
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
            w=new World(new Rules()); for(int i=0;i<8 && w.Blobs.Count>0;i++) w.AdvanceDay();
            bool bounded=true; foreach(Camp c in w.Camps) foreach(int n in c.Stock) if(n<0 || n>c.Capacite) bounded=false;
            Assert(bounded,"stocks bornés sur huit journées simulées");
            w=new World(new Rules { LeaderEnabled=false }); bool disabled=true; foreach(Blob b in w.Blobs) if(b.IsLeader) disabled=false; Assert(disabled,"chef désactivable dans les règles");
            w=new World(new Rules()); chief=null; foreach(Blob b in w.Blobs) if(b.IsLeader) chief=b;
            w.Blobs.Clear(); chief.Home=true; chief.Food=0; chief.Water=0; w.Blobs.Add(chief); w.FinishDay();
            Assert(w.Blobs.Count==0,"chef mortel sans nourriture ni eau");
            using(EconomyPanel panel=new EconomyPanel(w)) { panel.Size=new Size(1100,800); using(Bitmap b=new Bitmap(1100,800)) panel.DrawToBitmap(b,panel.ClientRectangle); }
        }
    }
    public class EconomyPanel : Control
    {
        readonly World world;
        public EconomyPanel(World w) { world=w; Dock=DockStyle.Fill; DoubleBuffered=true; BackColor=Color.FromArgb(24,34,48); Font=new Font("Segoe UI",10); }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g=e.Graphics; g.Clear(BackColor); int width=Math.Max(210,(Width-40)/4);
            for(int f=0;f<4;f++) { Camp c=world.Camps[f]; int x=16+f*width,y=20; g.DrawString(c.Nom+" — réserves : "+c.Reserves,Font,Brushes.White,x,y); y+=30;
                int count=0; Blob leader=null; foreach(Blob b in world.Blobs) if(!b.Dead && b.FactionId==f) { count++; if(b.IsLeader) leader=b; }
                g.DrawString(count+" habitants"+(leader==null?"":" · chef #"+leader.Id),Font,Brushes.White,x,y); y+=30;
                for(int k=0;k<8;k++) { g.DrawString(((Ressource)k)+" : "+c.Stock[k]+" / "+c.Capacite,Font,Brushes.White,x,y); y+=23; g.FillRectangle(Brushes.DimGray,x,y,width-25,6); using(Brush b=new SolidBrush(Appearance.Skins[f])) g.FillRectangle(b,x,y,(width-25)*c.Stock[k]/(float)c.Capacite,6); y+=18; }
            }
            int yy=430; string text="CHAÎNES DE PRODUCTION (par camp, chaque soir)\nAgriculteur : 2 terre + 2 eau + 1 énergie → 3 nourriture\nArtisan / transporteur : 1 charbon → 4 énergie ; 2 minerai + 2 énergie → 1 fer\nArtisan : 2 fer + 2 énergie → réserve (+15 capacité) ; 1 fer + 2 énergie → arme\nMontagnes : mouvement ralenti et plus coûteux. Lacs : obstacles ; nourriture engloutie.\nChef orange : priorité aux rations, force défensive doublée ; gardes à moins de 22 unités.\nCasquette rouge MAGA : membre trumpiste. Raids : vol, dégâts et perte de confiance.\nRaids : "+world.Raids+" · nourriture coulée aujourd'hui : "+world.SunkFood+" · morts de soif : "+world.ThirstDeaths+"\nRepas du chef : "+world.LeaderMeals+" · protections : "+world.Protections+"\nConfiance envers les Trumpistes : bleu "+world.Relations[0,1]+", vert "+world.Relations[2,1]+", violet "+world.Relations[3,1]+" (échanges interrompus à −20)";
            g.DrawString(text,Font,Brushes.White,new RectangleF(16,yy,Width-32,320));
        }
    }
}
