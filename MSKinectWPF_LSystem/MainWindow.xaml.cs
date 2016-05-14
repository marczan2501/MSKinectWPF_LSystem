using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Collections.Generic;
using System.Text;

namespace MSKinectWPF_LSystem
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
                if (tbl.ContainsKey(elt))
                {
                    sb.Append(tbl[elt]);
                }

                else
                {
                    sb.Append(elt);
                }

            }
            return sb.ToString();
        }
        KinectSensor sensor;
        byte[] colorBytes;
        Skeleton[] skeletons;
        SolidColorBrush inactiveBrush = new SolidColorBrush(Colors.Red);
        public int nachylenie = -9;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);

            var mapabitowa = BitmapFactory.New(800, 800);
            lsystem.Source = mapabitowa;
            var states = new Stack<State>();

            var str = "L";
            int levels = 30;
            string Lstring = "S";
            string Sstring = "F+>[F-Y[S]]F)G";
            string Ystring = "--[|F-F-FY]";
            string Gstring = "FGY[+F]+Y";

            var tbl = new Dictionary<char, string>();

            tbl.Add('L', Lstring);
            tbl.Add('S', Sstring);
            tbl.Add('Y', Ystring);
            tbl.Add('G', Gstring);

            for (var i = 0; i < levels; i++) str = Rewrite(tbl, str);


            var sizeGrowth = 0.0001;
            var angleGrowth = -0.055313;

            State state;

            var lines = new List<Point>();

            var pen = new Pen(new SolidColorBrush(Colors.Black), 0.25);

            var geometryGroup = new GeometryGroup();

            var initAngle = -3669.39;
            var initSize = 9.0;

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
            kat.Content = ("Angle: " + angleGrowth);
            roz.Content = ("Size: " + initAngle);
            lText.Content = ("L: " + Lstring);
            sText.Content = ("S: " + Sstring);
            yText.Content = ("Y: " + Ystring);
            gText.Content = ("G: " + Gstring);
            levelsText.Content = ("levels: " + levels);
            Action updateBitmap = () =>
            {
                using (mapabitowa.GetBitmapContext())
                {
                    mapabitowa.Clear();

                    for (var i = 0; i < lines.Count; i += 2)
                    {
                        var a = lines[i];
                        var b = lines[i + 1];

                        mapabitowa.DrawLine(
                            (int)a.X, (int)a.Y, (int)b.X, (int)b.Y,
                            Colors.Blue);
                    }
                }
            };
            MouseDown += (s, e) =>
            {
                angleGrowth += 0.0001;
                buildLines();
                updateBitmap();
            };
            KeyDown += (s, e) =>
            {
                if (Keyboard.IsKeyDown(Key.Q))
                {
                    angleGrowth += 0.0001;
                    kat.Content = ("Angle: " + angleGrowth);
                    buildLines();
                    updateBitmap();
                }
                else if (Keyboard.IsKeyDown(Key.A))
                {
                    angleGrowth -= 0.0001;
                    kat.Content = ("Angle: " + angleGrowth);
                    buildLines();
                    updateBitmap();
                }
                else if (Keyboard.IsKeyDown(Key.W))
                {
                    initAngle += 0.2;
                    roz.Content = ("Size: " + initAngle);
                    buildLines();
                    updateBitmap();
                }
                else if (Keyboard.IsKeyDown(Key.S))
                {
                    initAngle -= 0.2;
                    roz.Content = ("Size: " + initAngle);
                    buildLines();
                    updateBitmap();
                }

                else if (Keyboard.IsKeyDown(Key.D1))
                {
                    //tree horse
                    Lstring = "|-S!L!Y";
                    Sstring = "[F[FF-YS]F)G]+";
                    Ystring = "--[F-)<F-FG]-";
                    Gstring = "FGF[Y+>F]+Y";
                    levels = 12;
                    sizeGrowth = -1.359672;
                    angleGrowth = -0.138235;
                    initSize = 14.11;
                    initAngle = -3963.7485;
                    kat.Content = ("Angle: " + angleGrowth);
                    roz.Content = ("Size: " + initAngle);
                    lText.Content = ("L: " + Lstring);
                    sText.Content = ("S: " + Sstring);
                    yText.Content = ("Y: " + Ystring);
                    gText.Content = ("G: " + Gstring);
                    levelsText.Content = ("levels: " + levels);
                    buildLines();
                    updateBitmap();
                }
                else if (Keyboard.IsKeyDown(Key.D2))
                {
                    //Pollenate
                    Lstring = "S";
                    Sstring = "F+>[F-Y[S]]F)G";
                    Ystring = "--[|F-F-FY]";
                    Gstring = "FGY[+F]+Y";
                    levels = 30;
                    sizeGrowth = 0.0001;
                    angleGrowth = -0.05531299999999828;
                    initSize = 9.0;
                    initAngle = -3669.39;
                    kat.Content = ("Angle: " + angleGrowth);
                    roz.Content = ("Size: " + initAngle);
                    lText.Content = ("L: " + Lstring);
                    sText.Content = ("S: " + Sstring);
                    yText.Content = ("Y: " + Ystring);
                    gText.Content = ("G: " + Gstring);
                    levelsText.Content = ("levels: " + levels);
                    buildLines();
                    updateBitmap();
                }
            };

            buildLines();
            updateBitmap();
        }
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.KinectSensors.FirstOrDefault();

            if (sensor == null)
            {
                MessageBox.Show("Aplikacja wymaga Kinecta.");
                this.Close();
            }

            sensor.Start();

            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);

            sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);

            sensor.SkeletonStream.Enable();
            sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);

            sensor.ElevationAngle = nachylenie;
            ShowCircles();
            Application.Current.Exit += new ExitEventHandler(Current_Exit);
        }
        private void bSetTilt_Click(object sender, RoutedEventArgs e)
        {
            if (sensor.IsRunning && sensor != null)
            {
                sensor.ElevationAngle = (int)sSetTilt.Value;
                lTiltValue.Content = sensor.ElevationAngle.ToString();
            }
        }
        void Current_Exit(object sender, ExitEventArgs e)
        {
            if (sensor != null)
            {
                sensor.AudioSource.Stop();
                sensor.Stop();
                sensor.Dispose();
                sensor = null;
            }
        }
        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (var image = e.OpenColorImageFrame())
            {
                if (image == null)
                    return;

                if (colorBytes == null ||
                    colorBytes.Length != image.PixelDataLength)
                {
                    colorBytes = new byte[image.PixelDataLength];
                }

                image.CopyPixelDataTo(colorBytes);

                int length = colorBytes.Length;
                for (int i = 0; i < length; i += 4)
                {
                    colorBytes[i + 3] = 255;
                }

                BitmapSource source = BitmapSource.Create(image.Width,
                    image.Height,
                    96,
                    96,
                    PixelFormats.Bgra32,
                    null,
                    colorBytes,
                    image.Width * image.BytesPerPixel);
                videoImage.Source = source;
            }
        }
        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                    return;

                if (skeletons == null ||
                    skeletons.Length != skeletonFrame.SkeletonArrayLength)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                }

                skeletonFrame.CopySkeletonDataTo(skeletons);
            }

            Skeleton closestSkeleton = skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked)
                                                .OrderBy(s => s.Position.Z * Math.Abs(s.Position.X))
                                                .FirstOrDefault();

            if (closestSkeleton == null)
                return;

            var head = closestSkeleton.Joints[JointType.Head];
            var rightHand = closestSkeleton.Joints[JointType.HandRight];
            var leftHand = closestSkeleton.Joints[JointType.HandLeft];
            var leftFoot = closestSkeleton.Joints[JointType.FootLeft];
            var rightFoot = closestSkeleton.Joints[JointType.FootRight];

            if (head.TrackingState == JointTrackingState.NotTracked ||
                rightHand.TrackingState == JointTrackingState.NotTracked ||
                leftHand.TrackingState == JointTrackingState.NotTracked ||
                rightFoot.TrackingState == JointTrackingState.NotTracked ||
                leftFoot.TrackingState == JointTrackingState.NotTracked
                )
            {
                return;
            }

            SetEllipsePosition(ellipseHead, head);
            SetEllipsePosition(ellipseLeftHand, leftHand);
            SetEllipsePosition(ellipseRightHand, rightHand);
            SetEllipsePosition(ellipseLeftFoot, leftFoot);
            SetEllipsePosition(ellipseRightFoot, rightFoot);

            ProcessFractalAngle(head, rightHand, leftHand, leftFoot, rightFoot);
        }
        private void SetEllipsePosition(Ellipse ellipse, Joint joint)
        {
            ellipse.Width = 20;
            ellipse.Height = 20;
            ellipse.Fill = inactiveBrush;

            CoordinateMapper mapper = sensor.CoordinateMapper;

            var point = mapper.MapSkeletonPointToColorPoint(joint.Position, sensor.ColorStream.Format);

            Canvas.SetLeft(ellipse, point.X - ellipse.ActualWidth / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.ActualHeight / 2);
        }
        private void ProcessFractalAngle(Joint head, Joint rightHand, Joint leftHand, Joint leftFoot, Joint rightFoot)
        {
            bool aktywne = false;

            if (rightHand.Position.X > head.Position.X + 0.45)
            {
                if (!aktywne)
                {
                    aktywne = true;
                    System.Windows.Forms.SendKeys.SendWait("Q");
                }
            }
            else aktywne = false;

            if (leftHand.Position.X < head.Position.X - 0.45)
            {
                if (!aktywne)
                {
                    aktywne = true;
                    System.Windows.Forms.SendKeys.SendWait("A");
                }
            }
            else aktywne = false;

            if (rightFoot.Position.X > head.Position.X + 0.45)
            {
                if (!aktywne)
                {
                    aktywne = true;
                    System.Windows.Forms.SendKeys.SendWait("W");
                }
            }
            else aktywne = false;

            if (leftFoot.Position.X < head.Position.X - 0.45)
            {
                if (!aktywne)
                {
                    aktywne = true;
                    System.Windows.Forms.SendKeys.SendWait("S");
                }
            }
            else aktywne = false;
        }


        void ShowCircles()
        {
            ellipseHead.Visibility = Visibility.Visible;
            ellipseLeftHand.Visibility = Visibility.Visible;
            ellipseRightHand.Visibility = Visibility.Visible;
            ellipseRightFoot.Visibility = Visibility.Visible;
            ellipseLeftFoot.Visibility = Visibility.Visible;
        }
    }
}

