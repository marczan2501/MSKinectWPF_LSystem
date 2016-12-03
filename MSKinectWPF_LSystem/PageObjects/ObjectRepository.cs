using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MSKinectWPF_LSystem.PageObjects
{
    internal class ObjectRepository
    {
        public class MainPage
        {
            public static string str = "L";
            public static Stack<State> states = new Stack<State>();
            public static List<Point> lines = new List<Point>();
            public static Pen pen = new Pen(new SolidColorBrush(Colors.SteelBlue), 0.25);
            public static GeometryGroup geometryGroup = new GeometryGroup();
            public static Dictionary<char, string> tbl = new Dictionary<char, string>();
        }
        public class PagePoroze
        {
            public static string Lstring = "|-S!L!Y";
            public static string Sstring = "[F[FF-YS]F)G]+";
            public static string Ystring = "--[F-)<F-FG]-";
            public static string Gstring = "FGF[Y+>F]+Y";
            public static double initSize = 14.11;
            public static double initAngle = -3963.7485;
            public static double sizeGrowth = -1.359672;
            public static double angleGrowth = -0.138235;
            public static int levels = 12;
        }
        public class PageSpirale
        {
            public static string Lstring = "S";
            public static string Sstring = "F>+[F-Y[S]]F)G";
            public static string Ystring = "--[|F-F--FY]-";
            public static string Gstring = "FGF[+F]+Y";
            public static double initSize = 10;
            public static double initAngle = -3832.29;
            public static double sizeGrowth = 0.01;
            public static double angleGrowth = 0.08145300000000245;
            public static int levels = 30;
        }
        public class PageChmura
        {
            public static string Lstring = "SSS";
            public static string Sstring = "[F>[FF-YS]F)G]+";
            public static string Ystring = "--[F-)F-FG]-";
            public static string Gstring = "FGF[Y+F]+Y";
            public static double initSize = 10.3943;
            public static double initAngle = -3731.69;
            public static double sizeGrowth = 0.010104;
            public static double angleGrowth = 0.05244601300000046;
            public static int levels = 11;
        }
        public class PageWiatrak
        {
            public static string Lstring = "S";
            public static string Sstring = "F+[F>-Y[S]]F)G";
            public static string Ystring = "--[|F-F--FY]-";
            public static string Gstring = "FGF[+F]+Y";
            public static double initSize = 4;
            public static double initAngle = -3869.97;
            public static double sizeGrowth = 0.01;
            public static double angleGrowth = 0.11540900000000301;
            public static int levels = 30;
        }
        public class PageGalaktyki
        {
            public static string Lstring = "S";
            public static string Sstring = "F+>[F-Y[S]]F)G";
            public static string Ystring = "--[|F-F-FY]";
            public static string Gstring = "FGY[+F]+Y";
            public static double initSize = 9;
            public static double initAngle = -3669.39;
            public static double sizeGrowth = 0.0001;
            public static double angleGrowth = -0.05531299999999828;
            public static int levels = 30;
        }
        public class PagePunkty
        {
            public static string Lstring = "S";
            public static string Sstring = "F|+[F-Y[S]]FG";
            public static string Ystring = "--[|F-F+|+FY]+";
            public static string Gstring = "FGF>[+F]Y";
            public static double initSize = 6.03;
            public static double initAngle = -1705.7;
            public static double sizeGrowth = 0.01;
            public static double angleGrowth = 0.05;
            public static int levels = 30;
        }
        public class PageKlosy
        {
            public static string Lstring = "S";
            public static string Sstring = "F+[F-Y[S]]F)G";
            public static string Ystring = "--[|F-F-)-F>Y]-";
            public static string Gstring = "FGF[+F]+<Y";

            public static double initSize = 4;
            public static double initAngle = -3769.7;
            public static double sizeGrowth = 0.01;
            public static double angleGrowth = 0.05;

            public static int levels = 30;
        }
    }
}
