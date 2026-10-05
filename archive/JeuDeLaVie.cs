using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace JeuDeLaVie
{
    public class LifeBoard
    {
        public const int Width = 100, Height = 65;
        public bool[,] Cells = new bool[Width, Height];
        public long Generation;
        public bool Wrap;
        public int Population { get { int n = 0; foreach (bool c in Cells) if (c) n++; return n; } }
        public void Clear() { Cells = new bool[Width, Height]; Generation = 0; }
        public void Step()
        {
            bool[,] next = new bool[Width, Height];
            for (int y = 0; y < Height; y++) for (int x = 0; x < Width; x++)
            {
                int n = 0;
                for (int dy = -1; dy <= 1; dy++) for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = x + dx, ny = y + dy;
                    if (Wrap) { nx = (nx + Width) % Width; ny = (ny + Height) % Height; }
                    if (nx >= 0 && nx < Width && ny >= 0 && ny < Height && Cells[nx, ny]) n++;
                }
                next[x, y] = n == 3 || (Cells[x, y] && n == 2);
            }
            Cells = next; Generation++;
        }
        public void Pattern(string name)
        {
            Clear();
            string[] rows;
            switch (name)
            {
                case "Bloc stable": rows = new string[] { "OO", "OO" }; break;
                case "Clignotant": rows = new string[] { "OOO" }; break;
                case "Pulsar": rows = new string[] { "..OOO...OOO..", ".............", "O....O.O....O", "O....O.O....O", "O....O.O....O", "..OOO...OOO..", ".............", "..OOO...OOO..", "O....O.O....O", "O....O.O....O", "O....O.O....O", ".............", "..OOO...OOO.." }; break;
                case "Canon de Gosper": rows = new string[] { "........................O...........", "......................O.O...........", "............OO......OO............OO", "...........O...O....OO............OO", "OO........O.....O...OO..............", "OO........O...O.OO....O.O...........", "..........O.....O.......O...........", "...........O...O....................", "............OO......................" }; break;
                default: rows = new string[] { ".O.", "..O", "OOO" }; break;
            }
            int ox = (Width - rows[0].Length) / 2, oy = (Height - rows.Length) / 2;
            for (int y = 0; y < rows.Length; y++) for (int x = 0; x < rows[y].Length; x++) if (rows[y][x] == 'O') Cells[ox + x, oy + y] = true;
        }
        public string Serialize()
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("JEU-DE-LA-VIE-1"); s.AppendLine(Wrap ? "wrap" : "fixed");
            for (int y = 0; y < Height; y++) { for (int x = 0; x < Width; x++) s.Append(Cells[x,y] ? 'O' : '.'); s.AppendLine(); }
            return s.ToString();
        }
        public void Deserialize(string content)
        {
            string[] lines = content.Replace("\r", "").TrimEnd('\n').Split('\n');
            if (lines.Length != Height + 2 || lines[0] != "JEU-DE-LA-VIE-1" || (lines[1] != "wrap" && lines[1] != "fixed")) throw new FormatException("Ce fichier n'est pas une grille valide du jeu.");
            bool[,] next = new bool[Width, Height];
            for (int y = 0; y < Height; y++)
            {
                if (lines[y + 2].Length != Width) throw new FormatException("Dimensions de grille incorrectes.");
                for (int x = 0; x < Width; x++) { char c = lines[y + 2][x]; if (c != 'O' && c != '.') throw new FormatException("Cellule incorrecte."); next[x,y] = c == 'O'; }
            }
            Cells = next; Wrap = lines[1] == "wrap"; Generation = 0;
        }
    }

    public class BoardView : Control
    {
        public LifeBoard Board;
        public bool Grid = true;
        public event EventHandler Edited;
        bool painting, value;
        Point previous = new Point(-1, -1);
        float SizeCell { get { return Math.Min((float)ClientSize.Width / LifeBoard.Width, (float)ClientSize.Height / LifeBoard.Height); } }
        float OffsetX { get { return (ClientSize.Width - SizeCell * LifeBoard.Width) / 2; } }
        float OffsetY { get { return (ClientSize.Height - SizeCell * LifeBoard.Height) / 2; } }
        public BoardView(LifeBoard b)
        {
            Board = b; DoubleBuffered = true; BackColor = Color.FromArgb(13, 20, 30); Dock = DockStyle.Fill;
            Cursor = Cursors.Cross; SetStyle(ControlStyles.ResizeRedraw, true);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); float s = SizeCell; if (s <= 0) return;
            float ox = OffsetX, oy = OffsetY;
            using (Brush live = new SolidBrush(Color.FromArgb(77, 225, 174)))
            using (Brush dead = new SolidBrush(Color.FromArgb(20, 31, 43)))
            using (Pen pen = new Pen(Color.FromArgb(36, 49, 63)))
            {
                e.Graphics.FillRectangle(dead, ox, oy, s * LifeBoard.Width, s * LifeBoard.Height);
                for (int y = 0; y < LifeBoard.Height; y++) for (int x = 0; x < LifeBoard.Width; x++)
                    if (Board.Cells[x,y]) e.Graphics.FillRectangle(live, ox + x*s, oy + y*s, s, s);
                if (Grid && s >= 5)
                {
                    for (int x = 0; x <= LifeBoard.Width; x++) e.Graphics.DrawLine(pen, ox + x*s, oy, ox + x*s, oy + LifeBoard.Height*s);
                    for (int y = 0; y <= LifeBoard.Height; y++) e.Graphics.DrawLine(pen, ox, oy + y*s, ox + LifeBoard.Width*s, oy + y*s);
                }
            }
        }
        Point Cell(Point p) { float s = SizeCell; return s > 0 ? new Point((int)Math.Floor((p.X-OffsetX)/s), (int)Math.Floor((p.Y-OffsetY)/s)) : new Point(-1,-1); }
        bool Inside(Point p) { return p.X >= 0 && p.X < LifeBoard.Width && p.Y >= 0 && p.Y < LifeBoard.Height; }
        void Put(Point p) { if (Inside(p)) Board.Cells[p.X,p.Y] = value; }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e); Point p = Cell(e.Location);
            if (!Inside(p) || (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)) return;
            painting = true; Capture = true; value = e.Button == MouseButtons.Left && !Board.Cells[p.X,p.Y]; previous = p;
            if (Edited != null) Edited(this, EventArgs.Empty); Put(p); Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e); if (!painting) return;
            Point p = Cell(e.Location); if (!Inside(p)) { previous = new Point(-1,-1); return; }
            if (Inside(previous))
            {
                int dx = Math.Abs(p.X-previous.X), dy = Math.Abs(p.Y-previous.Y), n = Math.Max(dx,dy);
                for (int i = 0; i <= n; i++) Put(n == 0 ? p : new Point(previous.X+(int)Math.Round((p.X-previous.X)*(double)i/n), previous.Y+(int)Math.Round((p.Y-previous.Y)*(double)i/n)));
            }
            else Put(p);
            previous = p; Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); painting = false; Capture = false; if (Edited != null) Edited(this, EventArgs.Empty); }
        protected override void OnMouseCaptureChanged(EventArgs e) { base.OnMouseCaptureChanged(e); if (!Capture) painting = false; }
    }

    public class LifeWindow : Form
    {
        readonly LifeBoard board = new LifeBoard();
        readonly Timer timer = new Timer();
        readonly Random random = new Random();
        BoardView view; Button play; Label stats, speedLabel; CheckBox wrap;
        public LifeWindow()
        {
            Text = "Jeu de la vie - Conway"; ClientSize = new Size(1180, 820); MinimumSize = new Size(850, 640);
            MinimumSize = new Size(1050, 640);
            StartPosition = FormStartPosition.CenterScreen; Font = new Font("Segoe UI", 10); BackColor = Color.FromArgb(242,245,248); KeyPreview = true;
            TableLayoutPanel layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 5, ColumnCount = 1, Padding = new Padding(16) };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            Controls.Add(layout);
            Label title = new Label { Text = "LE JEU DE LA VIE   /   Conway\nDessinez une population, puis observez son évolution.", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 12, FontStyle.Bold) }; layout.Controls.Add(title,0,0);
            FlowLayoutPanel toolbar = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            play = Button("Lancer", delegate { Toggle(); }); toolbar.Controls.Add(play);
            toolbar.Controls.Add(Button("Un pas", delegate { Pause(); Step(); }));
            toolbar.Controls.Add(Button("Aléatoire", delegate { Pause(); board.Clear(); for(int y=0;y<LifeBoard.Height;y++) for(int x=0;x<LifeBoard.Width;x++) board.Cells[x,y] = random.NextDouble() < .25; RefreshBoard(); }));
            toolbar.Controls.Add(Button("Effacer", delegate { Pause(); board.Clear(); RefreshBoard(); }));
            toolbar.Controls.Add(Button("Sauvegarder", delegate { Save(); })); toolbar.Controls.Add(Button("Ouvrir", delegate { LoadBoard(); }));
            toolbar.Controls.Add(Button("Règles", delegate { MessageBox.Show(this, "Chaque cellule possède huit voisines (diagonales comprises).\n\n• Une cellule morte naît avec exactement 3 voisines vivantes.\n• Une cellule vivante survit avec 2 ou 3 voisines vivantes.\n• Dans tous les autres cas, elle meurt ou reste morte.\n\nToutes les cellules évoluent simultanément.\n\nLa grille contient 100 × 65 cellules. Par défaut, les cellules hors de la grille sont mortes. Avec « Bords reliés », les côtés opposés communiquent.\n\nEspace : lancer / pause. Flèche droite : un pas.\nClic gauche et glisser : dessiner ou effacer.\nClic droit et glisser : effacer. Dessiner met en pause.", "Règles et commandes", MessageBoxButtons.OK, MessageBoxIcon.Information); }));
            layout.Controls.Add(toolbar,0,1);
            FlowLayoutPanel options = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            options.Controls.Add(new Label { Text="Motif :", AutoSize=true, Margin=new Padding(0,6,4,0) });
            ComboBox motifs = new ComboBox { DropDownStyle=ComboBoxStyle.DropDownList, Width=150 };
            motifs.Items.AddRange(new object[] { "Planeur", "Bloc stable", "Clignotant", "Pulsar", "Canon de Gosper" }); motifs.SelectedIndex=0; options.Controls.Add(motifs);
            options.Controls.Add(Button("Placer", delegate { Pause(); board.Pattern((string)motifs.SelectedItem); RefreshBoard(); }));
            speedLabel = new Label { Text="Vitesse : 10 / s", AutoSize=true, Margin=new Padding(8,6,0,0) }; options.Controls.Add(speedLabel);
            TrackBar speed = new TrackBar { Minimum=1, Maximum=30, Value=10, Width=125, Height=30, TickStyle=TickStyle.None };
            speed.ValueChanged += delegate { timer.Interval=1000/speed.Value; speedLabel.Text="Vitesse : "+speed.Value+" / s"; }; options.Controls.Add(speed);
            wrap = new CheckBox { Text="Bords reliés", AutoSize=true, Margin=new Padding(8,6,0,0) }; wrap.CheckedChanged += delegate { board.Wrap=wrap.Checked; }; options.Controls.Add(wrap);
            CheckBox grid = new CheckBox { Text="Quadrillage", Checked=true, AutoSize=true, Margin=new Padding(8,6,0,0) }; grid.CheckedChanged += delegate { view.Grid=grid.Checked; view.Invalidate(); }; options.Controls.Add(grid);
            layout.Controls.Add(options,0,2);
            view = new BoardView(board); view.Edited += delegate { Pause(); RefreshBoard(); }; layout.Controls.Add(view,0,3);
            stats = new Label { Dock=DockStyle.Fill, TextAlign=ContentAlignment.MiddleLeft }; layout.Controls.Add(stats,0,4);
            timer.Interval=100; timer.Tick += delegate { Step(); };
            KeyDown += delegate(object sender, KeyEventArgs e) { if(e.KeyCode==Keys.Space) { Toggle(); e.Handled=true; e.SuppressKeyPress=true; } else if(e.KeyCode==Keys.Right) { Pause(); Step(); e.Handled=true; e.SuppressKeyPress=true; } };
            board.Pattern("Canon de Gosper"); RefreshBoard();
        }
        Button Button(string text, Action action) { Button b = new Button { Text=text, AutoSize=true, Height=32, Padding=new Padding(5,0,5,0) }; b.Click += delegate { action(); }; return b; }
        void Toggle() { if(timer.Enabled) Pause(); else { timer.Start(); play.Text="Pause"; RefreshBoard(); } }
        void Pause() { timer.Stop(); play.Text="Lancer"; }
        void Step() { board.Step(); if(board.Population==0) Pause(); RefreshBoard(); }
        void RefreshBoard() { view.Invalidate(); stats.Text=String.Format("{0}   •   Génération {1:N0}   •   {2:N0} cellules vivantes   •   Clic / glisser pour dessiner", timer.Enabled ? "En cours" : "En pause", board.Generation, board.Population); }
        void Save()
        {
            Pause(); RefreshBoard();
            using(SaveFileDialog d = new SaveFileDialog { Filter="Grille du jeu de la vie (*.life)|*.life", FileName="ma-grille.life", DefaultExt="life", AddExtension=true })
                if(d.ShowDialog(this)==DialogResult.OK) try { File.WriteAllText(d.FileName, board.Serialize(), Encoding.UTF8); } catch(Exception ex) { MessageBox.Show(this,ex.Message,"Sauvegarde impossible"); }
        }
        void LoadBoard()
        {
            Pause(); RefreshBoard();
            using(OpenFileDialog d = new OpenFileDialog { Filter="Grille du jeu de la vie (*.life)|*.life" })
                if(d.ShowDialog(this)==DialogResult.OK) try { board.Deserialize(File.ReadAllText(d.FileName,Encoding.UTF8)); wrap.Checked=board.Wrap; RefreshBoard(); } catch(Exception ex) { MessageBox.Show(this,ex.Message,"Ouverture impossible"); }
        }
        protected override void Dispose(bool disposing) { if(disposing) timer.Dispose(); base.Dispose(disposing); }
    }

    public static class Verification
    {
        static void Check(bool ok, string name) { if(!ok) throw new Exception("Échec : "+name); Console.WriteLine("OK : "+name); }
        public static void Run()
        {
            LifeBoard b = new LifeBoard(); b.Cells[10,10]=true; b.Step(); Check(b.Population==0,"mort par isolement");
            b.Clear(); b.Cells[10,10]=b.Cells[9,10]=b.Cells[11,10]=b.Cells[10,9]=b.Cells[10,11]=true; b.Step(); Check(!b.Cells[10,10],"mort par surpopulation");
            b.Pattern("Bloc stable"); string before=b.Serialize(); b.Step(); Check(b.Serialize()==before,"bloc stable");
            b.Clear(); b.Cells[10,10]=b.Cells[11,10]=b.Cells[12,10]=true; b.Step(); Check(b.Population==3 && b.Cells[11,9] && b.Cells[11,10] && b.Cells[11,11],"clignotant : naissance et évolution simultanée");
            b.Step(); Check(b.Cells[10,10] && b.Cells[11,10] && b.Cells[12,10],"clignotant période 2");
            b.Clear(); b.Cells[0,0]=b.Cells[99,0]=b.Cells[0,64]=true; b.Step(); Check(b.Population==0,"bords fixes");
            b.Clear(); b.Wrap=true; b.Cells[0,0]=b.Cells[99,0]=b.Cells[0,64]=true; b.Step(); Check(b.Population==4 && b.Cells[99,64],"bords reliés");
            b.Pattern("Planeur"); bool[,] old=(bool[,])b.Cells.Clone(); for(int i=0;i<4;i++) b.Step(); bool translated=true;
            for(int y=0;y<LifeBoard.Height-1;y++) for(int x=0;x<LifeBoard.Width-1;x++) if(old[x,y]!=b.Cells[x+1,y+1]) translated=false;
            Check(translated && b.Population==5,"déplacement du planeur après 4 générations");
            string data=b.Serialize(); LifeBoard other=new LifeBoard(); other.Deserialize(data); Check(other.Serialize()==data,"sauvegarde et restauration");
            bool rejected=false; try { other.Deserialize("incorrect"); } catch(FormatException) { rejected=true; } Check(rejected && other.Serialize()==data,"rejet de fichier invalide sans modifier la grille");
            using(LifeWindow window=new LifeWindow()) { window.CreateControl(); using(Bitmap bitmap=new Bitmap(window.Width,window.Height)) window.DrawToBitmap(bitmap,window.ClientRectangle); Check(window.Controls.Count>0,"construction et rendu de la fenêtre"); }
            Console.WriteLine("Toutes les vérifications ont réussi.");
        }
    }
}
