#region Usings
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
using MSKinectWPF_LSystem.PageObjects;
#endregion
namespace MSKinectWPF_LSystem
{
    #region L-System
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
    #endregion
    public partial class MainWindow : Window
    {
        #region L-System
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
        #endregion
        #region Kinect
        KinectSensor sensor;
        byte[] colorBytes;
        Skeleton[] skeletons;
        SolidColorBrush inactiveBrush = new SolidColorBrush(Colors.Red);
        public int nachylenie = -9;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);

            #region L-system
            var mapabitowa = BitmapFactory.New(800, 800);
            lsystem.Source = mapabitowa;


            ObjectRepository.MainPage.tbl.Add('L', ObjectRepository.PagePoroze.Lstring);
            ObjectRepository.MainPage.tbl.Add('S', ObjectRepository.PagePoroze.Sstring);
            ObjectRepository.MainPage.tbl.Add('Y', ObjectRepository.PagePoroze.Ystring);
            ObjectRepository.MainPage.tbl.Add('G', ObjectRepository.PagePoroze.Gstring);

            for (var i = 0; i < ObjectRepository.PagePoroze.levels; i++) ObjectRepository.MainPage.str = Rewrite(ObjectRepository.MainPage.tbl, ObjectRepository.MainPage.str);

            buildLines(ObjectRepository.MainPage.states, ObjectRepository.MainPage.lines);
            updateBitmap(mapabitowa, ObjectRepository.MainPage.lines);

            KeyDown += (s, e) =>
            {
                if (Keyboard.IsKeyDown(Key.W) || Keyboard.IsKeyDown(Key.Up))
                {
                    ObjectRepository.PagePoroze.angleGrowth += 0.0001;
                    buildLines(ObjectRepository.MainPage.states, ObjectRepository.MainPage.lines);
                    updateBitmap(mapabitowa, ObjectRepository.MainPage.lines);
                }
                else if (Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.Down))
                {
                    ObjectRepository.PagePoroze.angleGrowth -= 0.0001;
                    buildLines(ObjectRepository.MainPage.states, ObjectRepository.MainPage.lines);
                    updateBitmap(mapabitowa, ObjectRepository.MainPage.lines);
                }
                else if (Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.Left))
                {
                    ObjectRepository.PagePoroze.initAngle += 0.2;
                    buildLines(ObjectRepository.MainPage.states, ObjectRepository.MainPage.lines);
                    updateBitmap(mapabitowa, ObjectRepository.MainPage.lines);
                }
                else if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right))
                {
                    ObjectRepository.PagePoroze.initAngle -= 0.2;
                    buildLines(ObjectRepository.MainPage.states, ObjectRepository.MainPage.lines);
                    updateBitmap(mapabitowa, ObjectRepository.MainPage.lines);
                }
            };

            #endregion
        }
        #region Kinect
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.KinectSensors.FirstOrDefault();

            if (sensor == null)
            {
                MessageBox.Show("Aplikacja wymaga Kinect'a.");
                Close();
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
            else
            {
                MessageBox.Show("Aplikacja wymaga Kinecta.");
                Close();
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
            else
            {
                MessageBox.Show("Aplikacja wymaga Kinecta.");
                Close();
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
            if (rightHand.Position.X > head.Position.X + 0.45)
            {
                System.Windows.Forms.SendKeys.SendWait("W");
            }

            if (leftHand.Position.X < head.Position.X - 0.45)
            {
                System.Windows.Forms.SendKeys.SendWait("S");
            }

            if (rightFoot.Position.X > head.Position.X + 0.45)
            {
                System.Windows.Forms.SendKeys.SendWait("A");
            }

            if (leftFoot.Position.X < head.Position.X - 0.45)
            {
                System.Windows.Forms.SendKeys.SendWait("D");
            }
        }
        void ShowCircles()
        {
            ellipseHead.Visibility = Visibility.Visible;
            ellipseLeftHand.Visibility = Visibility.Visible;
            ellipseRightHand.Visibility = Visibility.Visible;
            ellipseRightFoot.Visibility = Visibility.Visible;
            ellipseLeftFoot.Visibility = Visibility.Visible;
        }
        #endregion
    }
}