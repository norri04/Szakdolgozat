using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace __AlapRajzolo
{
    public static class ExtensionMethods
    {
        public static int DISTANCE = 5;

        public static bool CloseTo(this PointF p, PointF other)
        {
            return Math.Abs(p.X - other.X) <= DISTANCE && Math.Abs(p.Y - other.Y) <= DISTANCE;
        }
        public static PointF Rotate(this PointF p, float alpha)
        {
            return new PointF((float)(p.X * Math.Cos(alpha) + p.Y * Math.Sin(alpha)),
                              (float)(-p.X * Math.Sin(alpha) + p.Y * Math.Cos(alpha)));
        }
        public static PointF Translate(this PointF p, PointF v)
        {
            return new PointF(p.X + v.X, p.Y + v.Y);
        }
        public static PointF Translate(this PointF p, float x, float y)
        {
            return new PointF(p.X + x, p.Y + y);
        }

        public static void DrawLine(this Graphics g, Color c, PointF p0, PointF p1)
        {
            g.DrawLine(new Pen(c), p0, p1);
        }
        public static void DrawCircle(this Graphics g, Circle circle)
        {
            float t = 0.0f;
            float b = 2 * (float)Math.PI;
            float h = b / 200.0f;
            Pen p = new Pen(circle.Color, circle.LineWidth);
            PointF p1;
            PointF p0 = new PointF(circle.R * (float)Math.Cos(t) + circle.X,
                                   circle.R * (float)Math.Sin(t) + circle.Y);
            while (t <= b)
            {
                t += h;
                p1 = new PointF(circle.R * (float)Math.Cos(t) + circle.X,
                                circle.R * (float)Math.Sin(t) + circle.Y);
                g.DrawLine(p, p0, p1);
                p0 = p1;
            }
        }

        public static double H0(double t) { return 2 * t * t * t - 3 * t * t + 1; }
        public static double H1(double t) { return -2 * t * t * t + 3 * t * t; }
        public static double H2(double t) { return t * t * t - 2 * t * t + t; }
        public static double H3(double t) { return t * t * t - t * t; }
        public static void DrawHermiteArc(this Graphics g, HermiteArc arc)
        {
            double a = 0;
            double t = a;
            double h = 1.0 / 500.0;
            PointF d0, d1;
            d0 = new PointF((float)(H0(t) * arc.P0.X + H1(t) * arc.P1.X + H2(t) * arc.T0.X + H3(t) * arc.T1.X),
                            (float)(H0(t) * arc.P0.Y + H1(t) * arc.P1.Y + H2(t) * arc.T0.Y + H3(t) * arc.T1.Y));
            while (t < 1)
            {
                t += h;
                d1 = new PointF((float)(H0(t) * arc.P0.X + H1(t) * arc.P1.X + H2(t) * arc.T0.X + H3(t) * arc.T1.X),
                                (float)(H0(t) * arc.P0.Y + H1(t) * arc.P1.Y + H2(t) * arc.T0.Y + H3(t) * arc.T1.Y));
                g.DrawLine(new Pen(arc.Color, arc.LineWidth), d0, d1);
                d0 = d1;
            }
        }
    }
}
