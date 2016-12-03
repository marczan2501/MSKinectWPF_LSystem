using System.Windows;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;
using LSystem_standalone.PageObjects;

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
        static void buildLines(Stack<State> states, List<Point> lines)
        {
            lines.Clear();
            State state = new State()
            {
                x = 400,
                y = 400,
                dir = 0,
                size = ObjectRepository.PagePoroze.initSize,
                angle = ObjectRepository.PagePoroze.initAngle
            };

            foreach (var elt in ObjectRepository.MainPage.str)
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
                else if (elt == '>') state.size *= (1.0 - ObjectRepository.PagePoroze.sizeGrowth);
                else if (elt == '<') state.size *= (1.0 + ObjectRepository.PagePoroze.sizeGrowth);
                else if (elt == ')') state.angle *= (1 + ObjectRepository.PagePoroze.angleGrowth);
                else if (elt == '(') state.angle *= (1 - ObjectRepository.PagePoroze.angleGrowth);
                else if (elt == '[') states.Push(state.Clone());
                else if (elt == ']') state = states.Pop();
                else if (elt == '!') state.angle *= -1.0;
                else if (elt == '|') state.dir += 180.0;
            }
        }
        static void updateBitmap(WriteableBitmap mapabitowa, List<Point> lines)
        {
            using (mapabitowa.GetBitmapContext())
            {
                mapabitowa.Clear();

                for (var i = 0; i < lines.Count; i += 2)
                {
                    var a = lines[i];
                    var b = lines[i + 1];

                    mapabitowa.DrawLine((int)a.X, (int)a.Y, (int)b.X, (int)b.Y, Colors.SteelBlue);
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
         
            var mapabitowa = BitmapFactory.New(800, 800);
            lsystem.Source = mapabitowa;



            ObjectRepository.MainPage.tbl.Add('L', ObjectRepository.PagePoroze.Lstring);
            ObjectRepository.MainPage.tbl.Add('S', ObjectRepository.PagePoroze.Sstring);
            ObjectRepository.MainPage.tbl.Add('Y', ObjectRepository.PagePoroze.Ystring);
            ObjectRepository.MainPage.tbl.Add('G', ObjectRepository.PagePoroze.Gstring);

            for (var i = 0; i < ObjectRepository.PagePoroze.levels; i++) ObjectRepository.MainPage.str = Rewrite(ObjectRepository.MainPage.tbl, ObjectRepository.MainPage.str);

            buildLines(ObjectRepository.MainPage.states, ObjectRepository.MainPage.lines);
            updateBitmap(mapabitowa, ObjectRepository.MainPage.lines);
            KeyDown += (sender, e) =>
            {
                if (Keyboard.IsKeyDown(Key.Q))
                {
                    ObjectRepository.PagePoroze.angleGrowth += 0.0001;
                    buildLines(ObjectRepository.MainPage.states, ObjectRepository.MainPage.lines);
                    updateBitmap(mapabitowa, ObjectRepository.MainPage.lines);
                }
                else if (Keyboard.IsKeyDown(Key.A))
                {
                    ObjectRepository.PagePoroze.angleGrowth -= 0.0001;
                    buildLines(ObjectRepository.MainPage.states, ObjectRepository.MainPage.lines);
                    updateBitmap(mapabitowa, ObjectRepository.MainPage.lines);
                }
                else if (Keyboard.IsKeyDown(Key.W))
                {
                    ObjectRepository.PagePoroze.initAngle += 0.2;
                    buildLines(ObjectRepository.MainPage.states, ObjectRepository.MainPage.lines);
                    updateBitmap(mapabitowa, ObjectRepository.MainPage.lines);
                }
                else if (Keyboard.IsKeyDown(Key.S))
                {
                    ObjectRepository.PagePoroze.initAngle -= 0.2;
                    buildLines(ObjectRepository.MainPage.states, ObjectRepository.MainPage.lines);
                    updateBitmap(mapabitowa, ObjectRepository.MainPage.lines);
                }
            };
        }
    }
}
