using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace __AlapRajzolo
{
    public partial class Form1 : Form
    {
        Graphics g;

        List<Circle> C = new List<Circle>();
        int seged;
        int seged2;


        int foundCenter = -1;
        int foundPoint = -1;
        float s = 5;
        public static List<PointF> innertanpoints;
        public static List<PointF> outertanpoints;
        public static List<PointF> radicallinepoints;
        public Form1()
        {
            InitializeComponent();
            innertanpoints = new List<PointF>();
            outertanpoints = new List<PointF>();
            radicallinepoints = new List<PointF>();
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            seged2 = 0;
            PointF p;
            //közép és sugár pontok
            for (int i = 0; i < C.Count; i++)
            {
                g.FillRectangle(new SolidBrush(C[i].CDef), C[i].O.X - s, C[i].O.Y - s, 2 * s, 2 * s);
                g.FillRectangle(new SolidBrush(C[i].CDef), C[i].P.X - s, C[i].P.Y - s, 2 * s, 2 * s);
            }

            //érintő pontok
            for (int i = 0; i < outertanpoints.Count; i++)
            {
                //g.FillRectangle(new SolidBrush(Color.Red), outertanpoints[i].X - s, outertanpoints[i].Y - s, 2 * s, 2 * s);
                g.FillRectangle(new SolidBrush(Color.Red), innertanpoints[i].X - s, innertanpoints[i].Y - s, 2 * s, 2 * s);
            }

            //körök
            for (int i = 0; i < C.Count; i++)
            {
                g.DrawLine(new Pen(C[i].CDef), C[i].O, C[i].P);
                g.DrawCircle(C[i]);

            }

            //radical line
            for (int i = 0; i < C.Count - 1; i++)
            {

                p = GetRadicalLinePoint(C[i], C[i + 1]);


                g.DrawLine(Color.DarkOrange, radicallinepoints[i], p); //radical line rajzolása

            }

            //Hermite ívek
            for (int i = 0; i < C.Count - 2; i++)
            {
                seged = i;

                if (i > 1)
                {
                    seged = i * 3;
                }

                PointF p1 = GetTangentLineEndPoint(innertanpoints[seged2], C[i], C[i + 1]);
                PointF p2 = GetTangentLineEndPoint(innertanpoints[seged + 1], C[i + 1], C[i + 2]);
                g.DrawLine(Color.Gray, innertanpoints[seged2], p1);
                g.DrawLine(Color.Gray, innertanpoints[seged + 1], p2);

                PointF p3 = GetTangentLineEndPoint(outertanpoints[seged2], C[i], C[i + 1]);
                PointF p4 = GetTangentLineEndPoint(outertanpoints[seged + 1], C[i + 1], C[i + 2]);
                g.DrawLine(Color.Gray, outertanpoints[seged2], p3);
                g.DrawLine(Color.Gray, outertanpoints[seged + 1], p4);
                HermiteArc innerarc;
                HermiteArc outerarc;

                innerarc = new HermiteArc(Color.Blue, innertanpoints[seged2], innertanpoints[seged + 1],
                    Mult(Subs(innertanpoints[seged2], p1), 2), //első pont érintője
                    Mult(Subs(innertanpoints[seged + 1], p2), 2)); //második pont érintője
                g.DrawHermiteArc(innerarc);
                g.FillRectangle(new SolidBrush(Color.Blue), p1.X - s, p1.Y - s, 2 * s, 2 * s);
                g.FillRectangle(new SolidBrush(Color.Blue), p2.X - s, p2.Y - s, 2 * s, 2 * s);
                
                outerarc = new HermiteArc(Color.Purple, outertanpoints[seged2], outertanpoints[seged + 1],
                    Mult(Subs(outertanpoints[seged2], p3), 2), //első pont érintője
                    Mult(Subs(outertanpoints[seged + 1], p4), 2)); //második pont érintője
                g.DrawHermiteArc(outerarc);
                g.FillRectangle(new SolidBrush(Color.Blue), p3.X - s, p3.Y - s, 2 * s, 2 * s);
                g.FillRectangle(new SolidBrush(Color.Blue), p4.X - s, p4.Y - s, 2 * s, 2 * s);

                seged2 = seged + 1;

            }


            ////Apollóniusz körök
            //for (int i = 0; i < C.Count - 2; i++)
            //{
            //    if (C[i].R > 0 && C[i + 1].R > 0 && C[i + 2].R > 0)
            //    {
            //        ApolloniusProblem apollonius = new ApolloniusProblem(C[i], C[i + 1], C[i + 2]);

            //        g.DrawCircle(apollonius.GetTangentCircle(1, 1, 1)); //külső
            //        //g.DrawCircle(apollonius.GetTangentCircle(-1,  1,  1));
            //        //g.DrawCircle(apollonius.GetTangentCircle( 1, -1,  1));
            //        //g.DrawCircle(apollonius.GetTangentCircle( 1,  1, -1));
            //        //g.DrawCircle(apollonius.GetTangentCircle(-1, -1,  1));
            //        //g.DrawCircle(apollonius.GetTangentCircle( 1, -1, -1));
            //        //g.DrawCircle(apollonius.GetTangentCircle(-1,  1, -1));
            //        g.DrawCircle(apollonius.GetTangentCircle(-1, -1, -1)); //belső

            //    }
            //}

        }

        #region Mouse handling
        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < C.Count; i++)
            {
                if (C[i].O.CloseTo(e.Location))
                    foundCenter = i;
                else if (C[i].P.CloseTo(e.Location))
                    foundPoint = i;
            }

            if (foundCenter == -1 && foundPoint == -1)
            {
                C.Add(new Circle(e.Location, 0));
                if (C.Count > 1)
                {
                    for (int i = 0; i < C.Count - 1; i++)
                    {

                        radicallinepoints.Add(e.Location);
                        radicallinepoints.Add(e.Location);

                    }

                    if (C.Count > 2)
                    {
                        innertanpoints.Add(e.Location);
                        innertanpoints.Add(e.Location);
                        innertanpoints.Add(e.Location);

                        outertanpoints.Add(e.Location);
                        outertanpoints.Add(e.Location);
                        outertanpoints.Add(e.Location);

                    }
                }

                foundPoint = C.Count - 1;

                canvas.Invalidate();
            }
        }
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (foundCenter != -1 || foundPoint != -1)
            {
                if (foundCenter != -1)
                {
                    float dX = C[foundCenter].P.X - C[foundCenter].O.X;
                    float dY = C[foundCenter].P.Y - C[foundCenter].O.Y;
                    C[foundCenter].O = e.Location;
                    C[foundCenter].P = new PointF(e.X + dX, e.Y + dY);
                    RefreshTanPoints();
                    RefreshRadicalLine();
                }
                else
                {
                    C[foundPoint].P = e.Location;
                    RefreshTanPoints();
                    RefreshRadicalLine();
                }
                canvas.Invalidate();
            }
        }
        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            foundCenter = -1;
            foundPoint = -1;
        }
        #endregion

        private void RefreshTanPoints()
        {
            if (C.Count > 2)
            {
                for (int i = 0; i < C.Count - 2; i++)
                {
                    if (C[i].R > 0 && C[i + 1].R > 0 && C[i + 2].R > 0)
                    {

                        ApolloniusProblem apollonius = new ApolloniusProblem(C[i], C[i + 1], C[i + 2]);
                        seged = 0;
                        if (i > 0)
                        {
                            seged = i * 3;

                        }
                        if (seged - i >= 0) seged -= i;

                        outertanpoints[i + seged] = apollonius.GetTangentPoint(1, C[i]);
                        outertanpoints[i + 1 + seged] = apollonius.GetTangentPoint(1, C[i + 1]);
                        outertanpoints[i + 2 + seged] = apollonius.GetTangentPoint(1, C[i + 2]);

                        innertanpoints[i + seged] = apollonius.GetTangentPoint(-1, C[i]);
                        innertanpoints[i + 1 + seged] = apollonius.GetTangentPoint(-1, C[i + 1]);
                        innertanpoints[i + 2 + seged] = apollonius.GetTangentPoint(-1, C[i + 2]);

                    }
                }
            }
        }

        private void RefreshRadicalLine()
        {
            if (C.Count > 1)
            {
                for (int i = 0; i < C.Count - 1; i++)
                {
                    radicallinepoints[i] = GetRadicalLine(C[i], C[i + 1]);
                }
            }
        }

        #region Hermite
        private PointF Add(PointF a, PointF b)
        {
            return new PointF(b.X + a.X, b.Y + a.Y);
        }
        private PointF Subs(PointF a, PointF b)
        {
            return new PointF(b.X - a.X, b.Y - a.Y);
        }
        private PointF Mult(PointF a, float l)
        {
            return new PointF(a.X * l, a.Y * l);
        }

        #endregion

        public PointF GetRadicalLinePoint(Circle a, Circle b)
        {
            PointF p, J, K;
            float x1, x2, d;

            Vector2 a_v, b_v, d_v, k_v, perpendicular_v;
            a_v = new Vector2(a.X, a.Y);
            b_v = new Vector2(b.X, b.Y);
            d_v = b_v - a_v;
            d = d_v.Length();

            x1 = (d * d + a.R * a.R - b.R * b.R) / (2 * d);
            x2 = (d * d + b.R * b.R - a.R * a.R) / (-2 * d);

            k_v = a_v + (Vector2.Normalize(d_v) * x1);
            K = new PointF(k_v.X, k_v.Y);

            perpendicular_v = new Vector2(d_v.Y, d_v.X * -1);
            perpendicular_v = k_v + (Vector2.Normalize(perpendicular_v) * 200);
            J = new PointF(perpendicular_v.X, perpendicular_v.Y);


            return J;
        }
        public PointF GetRadicalLine(Circle a, Circle b)
        {
            PointF p, K, A, B;
            float x1, x2, d;

            Vector2 a_v, b_v, d_v, k_v, perpendicular_v;
            a_v = new Vector2(a.X, a.Y);
            b_v = new Vector2(b.X, b.Y);
            d_v = b_v - a_v;
            d = d_v.Length();

            x1 = (d * d + a.R * a.R - b.R * b.R) / (2 * d);

            //?? nincs felhasználva, innen lehetne tudni az egyenest.
            x2 = (d * d + b.R * b.R - a.R * a.R) / (-2 * d);

            k_v = a_v + (Vector2.Normalize(d_v) * x1);

            perpendicular_v = new Vector2(d_v.Y, d_v.X * -1);
            perpendicular_v = k_v + (Vector2.Normalize(perpendicular_v) * -200);
            A = new PointF(perpendicular_v.X, perpendicular_v.Y);

            return A;
        }
        public double GetLengthofTanPtoRadLine(PointF tanpoint, Circle a, Circle b)
        {
            double d;
            Vector2 tanpoint_v = new Vector2(tanpoint.X, tanpoint.Y);
            PointF radlinepoint1 = GetRadicalLine(a, b); //az apollonius kör közepétől elfele
            PointF radlinepoint2 = GetRadicalLinePoint(a, b); //az apollonius kör közepe fele
            Vector2 radline = new Vector2(radlinepoint2.X - radlinepoint1.X, radlinepoint2.Y - radlinepoint1.Y);
            Vector2 perpendicular = tanpoint_v + new Vector2(radline.Y * -1, radline.X);

            //g.DrawLine(Color.Black, tanpoint, new PointF(perpendicular.X, perpendicular.Y));

            //https://regi.tankonyvtar.hu/hu/tartalom/tamop425/0046_komputergrafika_matematikai_alapok/ch04s03.html
            d = Math.Abs((radlinepoint2.X - radlinepoint1.X) * (radlinepoint1.Y - tanpoint.Y) -
                         (radlinepoint1.X - tanpoint.X) * (radlinepoint2.Y - radlinepoint1.Y)) /
                Math.Sqrt((radlinepoint2.X - radlinepoint1.X) * (radlinepoint2.X - radlinepoint1.X) +
                          (radlinepoint1.Y - radlinepoint2.Y) * (radlinepoint1.Y - radlinepoint2.Y)
                       );

            return d;

        }

        public PointF GetTangentLineEndPoint(PointF p, Circle a, Circle b)
        {
            Vector2 p_v = new Vector2(p.X, p.Y);
            Vector2 o_v = new Vector2(a.X, a.Y);
            Vector2 next_v = new Vector2(b.X, b.Y);
            Vector2 temp = p_v - o_v;
            double distance = GetLengthofTanPtoRadLine(p, a, b);


            //TODO:  fixelni: ha jobról balra,lefele rajzolok köröket bugos, 
            //ha balról felfele rajzolok akkor is bugos
           
                temp = new Vector2(temp.Y*-1, temp.X);


            distance *= 2;
            Vector2 res_v = p_v + (Vector2.Normalize(temp) * Convert.ToInt32(distance));
            PointF res = new PointF(res_v.X, res_v.Y);
            Vector2 dir1_v = res_v - next_v;
            Vector2 dir2_v = new Vector2(p.X, p.Y) - next_v;
            if (dir1_v.Length() >= dir2_v.Length())
            {
                temp = p_v - o_v;
                temp = new Vector2(temp.Y, temp.X*-1);
                res_v = p_v + Vector2.Normalize(temp) * Convert.ToInt32(distance);
                res = new PointF(res_v.X, res_v.Y);
            }

            return res;
        }

        public Vector2 GetDirection(Circle a, Circle b, PointF tanp, PointF tangentlineendpoint, Vector2 temp)
        {
            

            Vector2 tangentlineendpoint_v = new Vector2(tangentlineendpoint.X, tangentlineendpoint.Y);
            Vector2 a_v = new Vector2(a.X, a.Y);
            Vector2 b_v = new Vector2(b.X, b.Y);
            Vector2 dir1_v = tangentlineendpoint_v - b_v;
            Vector2 dir2_v = new Vector2(tanp.X, tanp.Y) - b_v;

            if (dir1_v.Length() > dir2_v.Length())
            {
                temp = tangentlineendpoint_v + new Vector2(temp.X, temp.Y * -1);
            }
            return temp;
        }
    }
}
