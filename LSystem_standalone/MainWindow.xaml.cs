using System.Windows;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace LSystem_standalone
{
    class DrawingVisualElement : FrameworkElement
    {
        public DrawingVisual visual;
        public DrawingVisualElement() { visual = new DrawingVisual(); }
        protected override int VisualChildrenCount { get { return 1; } }
        protected override Visual GetVisualChild(int index) { return visual; }
    }
    class State
    {
        public double size;
        public double angle;
        public double x;
        public double y;
        public double dir;

        public State Clone() { return (State)this.MemberwiseClone(); }
    }
    public partial class MainWindow : Window
    {
        static string Rewrite(Dictionary<char, string> tbl, string str)
        {
            var sb = new StringBuilder();

            foreach (var elt in str)
            {
                if (tbl.ContainsKey(elt)) sb.Append(tbl[elt]);
                else sb.Append(elt);
            }
            return sb.ToString();
        }
        public MainWindow()
        {
            InitializeComponent();
            var mapabitowa = BitmapFactory.New(800, 800);
            lsystem.Source = mapabitowa;
            var states = new Stack<State>();

            var str = "L";
            int levels = 4;
            string Lstring = "S";
            string Sstring = "F+S";
            //string Ystring = "--[|F-F-FY]";
            // Gstring = "FGY[+F]+Y";

            var tbl = new Dictionary<char, string>();

            tbl.Add('L', Lstring);
            tbl.Add('S', Sstring);
           // tbl.Add('Y', Ystring);
           // tbl.Add('G', Gstring);

            for (var i = 0; i < levels; i++) str = Rewrite(tbl, str);

            State state;

            var lines = new List<Point>();
            var pen = new Pen(new SolidColorBrush(Colors.Black), 0.25);
            var geometryGroup = new GeometryGroup();
            var initAngle = -3669.39;
            var initSize = 9.0;
            var sizeGrowth = 0.0001;
            var angleGrowth = -0.055313;

            Action buildLines = () =>
            {
                lines.Clear();
                state = new State()
                {
                    x = 400,
                    y = 400,
                    dir = 0,
                    size = initSize,
                    angle = initAngle
                };

                foreach (var elt in str)
                {
                    if (elt == 'F')
                    {
                        var new_x = state.x + state.size * Math.Cos(state.dir * Math.PI / 180.0);
                        var new_y = state.y + state.size * Math.Sin(state.dir * Math.PI / 180.0);

                        lines.Add(new Point(state.x, state.y));
                        lines.Add(new Point(new_x, new_y));

                        state.x = new_x;
                        state.y = new_y;
                    }
                    else if (elt == '+') state.dir += state.angle;
                    else if (elt == '-') state.dir -= state.angle;
                    else if (elt == '>') state.size *= (1.0 - sizeGrowth);
                    else if (elt == '<') state.size *= (1.0 + sizeGrowth);
                    else if (elt == ')') state.angle *= (1 + angleGrowth);
                    else if (elt == '(') state.angle *= (1 - angleGrowth);
                    else if (elt == '[') states.Push(state.Clone());
                    else if (elt == ']') state = states.Pop();
                    else if (elt == '!') state.angle *= -1.0;
                    else if (elt == '|') state.dir += 180.0;
                }
            };
            
            Action updateBitmap = () =>
            {
                using (mapabitowa.GetBitmapContext())
                {
                    mapabitowa.Clear();

                    for (var i = 0; i < lines.Count; i += 2)
                    {
                        var a = lines[i];
                        var b = lines[i + 1];

                        mapabitowa.DrawLine((int)a.X, (int)a.Y, (int)b.X, (int)b.Y, Colors.Blue);
                    }
                }
            };
            KeyDown += (s, e) =>
            {
                if (Keyboard.IsKeyDown(Key.Q))
                {
                    angleGrowth += 0.0001;
                    //kat.Content = ("Angle: " + angleGrowth);
                    buildLines();
                    updateBitmap();
                }
                else if (Keyboard.IsKeyDown(Key.A))
                {
                    angleGrowth -= 0.0001;
                    //kat.Content = ("Angle: " + angleGrowth);
                    buildLines();
                    updateBitmap();
                }
                else if (Keyboard.IsKeyDown(Key.W))
                {
                    initAngle += 0.2;
                    //roz.Content = ("Size: " + initAngle);
                    buildLines();
                    updateBitmap();
                }
                else if (Keyboard.IsKeyDown(Key.S))
                {
                    initAngle -= 0.2;
                    //roz.Content = ("Size: " + initAngle);
                    buildLines();
                    updateBitmap();
                }
            };
            buildLines();
            updateBitmap();
        }
    }
}
