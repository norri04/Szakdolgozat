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
        int seged3;


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
                g.FillRectangle(new SolidBrush(Color.Red), outertanpoints[i].X - s, outertanpoints[i].Y - s, 2 * s, 2 * s);
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

                p = GetRadicalLinePoint(C[C.Count - 1], C[0]);
                g.DrawLine(Color.DarkOrange, radicallinepoints[C.Count - 1], p);
            }

            //Hermite ívek
            for (int i = 0; i < C.Count - 2; i++)
            {
                HermiteArc innerarc;
                HermiteArc outerarc;
                seged = i;
                seged3 = 2;
                if (i >= 1)
                {
                    seged = i * 3;

                }

                #region Belső

                PointF p_inner1 = GetTangentLineEndPoint(innertanpoints[seged2],
                    C[i], C[i + 1], innertanpoints[seged2 + 1]);
                g.DrawLine(Color.Red, innertanpoints[seged2], p_inner1);

                PointF p_inner2 = GetTangentLineEndPoint(innertanpoints[seged + 1],
                    C[i + 1], C[i + 2], innertanpoints[seged + 2]);
                g.DrawLine(Color.Red, innertanpoints[seged + 1], p_inner2);

                PointF p_inner_one_before_last = GetTangentLineEndPoint(innertanpoints[innertanpoints.Count - 2],
                    C[C.Count - 2], C[C.Count - 1], innertanpoints[innertanpoints.Count - 1]);
                g.DrawLine(Color.Red, innertanpoints[innertanpoints.Count - 2], p_inner_one_before_last);
                PointF p_inner_last = GetTangentLineEndPoint(innertanpoints[innertanpoints.Count - 1], C[C.Count - 1], C[0], innertanpoints[0]);
                g.DrawLine(Color.Red, innertanpoints[innertanpoints.Count - 1], p_inner_last);

                innerarc = new HermiteArc(Color.Blue, innertanpoints[seged2], innertanpoints[seged + 1],
                    Mult(Subs(innertanpoints[seged2], p_inner1), 2), //első pont érintője
                    Mult(Subs(innertanpoints[seged + 1], p_inner2), 2)); //második pont érintője
                g.DrawHermiteArc(innerarc);

                
                if (i == C.Count - 3)
                {
                    //utolsó előtti ív
                    innerarc = new HermiteArc(Color.Blue, innertanpoints[innertanpoints.Count - 2], innertanpoints[innertanpoints.Count - 1],
                    Mult(Subs(innertanpoints[innertanpoints.Count - 2], p_inner_one_before_last), 2), //utolsó előtti pont érintője
                    Mult(Subs(innertanpoints[innertanpoints.Count - 1], p_inner_last), 2)); //utolsó pont érintője
                    g.DrawHermiteArc(innerarc);

                    g.FillRectangle(new SolidBrush(Color.Blue), p_inner_one_before_last.X - s, p_inner_one_before_last.Y - s, 2 * s, 2 * s);
                    //utolsó ív
                    innerarc = new HermiteArc(Color.Blue, innertanpoints[innertanpoints.Count - 1], innertanpoints[0],
                        Mult(Subs(innertanpoints[innertanpoints.Count - 1], p_inner_last), 2), //utolsó pont érintője
                        Mult(Subs(innertanpoints[0], p_inner1), 2)); //első pont érintője
                    //g.DrawHermiteArc(innerarc);

                    g.FillRectangle(new SolidBrush(Color.Blue), p_inner_last.X - s, p_inner_last.Y - s, 2 * s, 2 * s);
                }

                g.FillRectangle(new SolidBrush(Color.Blue), p_inner1.X - s, p_inner1.Y - s, 2 * s, 2 * s);
                g.FillRectangle(new SolidBrush(Color.Blue), p_inner2.X - s, p_inner2.Y - s, 2 * s, 2 * s);

                #endregion

                #region Külső
                PointF p_outer1 = GetTangentLineEndPoint(outertanpoints[seged2],
                    C[i], C[i + 1], outertanpoints[seged2 + 1]);
                g.DrawLine(Color.Red, outertanpoints[seged2], p_outer1);

                PointF p_outer2 = GetTangentLineEndPoint(outertanpoints[seged + 1],
                    C[i + 1], C[i + 2], outertanpoints[seged + 2]);
                g.DrawLine(Color.Red, outertanpoints[seged + 1], p_outer2);

                PointF p_outer_one_before_last = GetTangentLineEndPoint(outertanpoints[outertanpoints.Count - 2],
                    C[C.Count - 2], C[C.Count - 1], outertanpoints[outertanpoints.Count - 1]);
                g.DrawLine(Color.Red, outertanpoints[outertanpoints.Count - 2], p_outer_one_before_last);
                PointF p_outer_last = GetTangentLineEndPoint(outertanpoints[outertanpoints.Count - 1],
                    C[C.Count - 1], C[0], outertanpoints[0]);
                g.DrawLine(Color.Red, outertanpoints[outertanpoints.Count - 1], p_outer_last);

                outerarc = new HermiteArc(Color.Blue, outertanpoints[seged2], outertanpoints[seged + 1],
                    Mult(Subs(outertanpoints[seged2], p_outer1), 2), //első pont érintője
                    Mult(Subs(outertanpoints[seged + 1], p_outer2), 2)); //második pont érintője
                g.DrawHermiteArc(outerarc);

                if (i == C.Count - 3)
                {
                    //utolsó előtti ív
                    outerarc = new HermiteArc(Color.Blue, outertanpoints[outertanpoints.Count - 2], outertanpoints[outertanpoints.Count - 1],
                    Mult(Subs(outertanpoints[outertanpoints.Count - 2], p_outer_one_before_last), 2), //utolsó előtti pont érintője
                    Mult(Subs(outertanpoints[outertanpoints.Count - 1], p_outer_last), 2)); //utolsó pont érintője
                    g.DrawHermiteArc(outerarc);
                    g.FillRectangle(new SolidBrush(Color.Blue), p_outer_one_before_last.X - s, p_outer_one_before_last.Y - s, 2 * s, 2 * s);
                    //utolsó ív
                    outerarc = new HermiteArc(Color.Blue, outertanpoints[outertanpoints.Count - 1], outertanpoints[0],
                        Mult(Subs(outertanpoints[outertanpoints.Count - 1], p_outer_last), 2), //utolsó pont érintője
                        Mult(Subs(outertanpoints[0], p_outer1), 2)); //első pont érintője
                    //g.DrawHermiteArc(outerarc);

                    g.FillRectangle(new SolidBrush(Color.Blue), p_outer_last.X - s, p_outer_last.Y - s, 2 * s, 2 * s);
                }

                g.FillRectangle(new SolidBrush(Color.Blue), p_outer1.X - s, p_outer1.Y - s, 2 * s, 2 * s);
                g.FillRectangle(new SolidBrush(Color.Blue), p_outer2.X - s, p_outer2.Y - s, 2 * s, 2 * s);
                #endregion


                seged2 = seged + 1;

            }

            #region apollóniusz kör rajzoló
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
            #endregion

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
                    radicallinepoints[C.Count - 1] = GetRadicalLine(C[C.Count - 1], C[0]);
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
            //x2 = (d * d + b.R * b.R - a.R * a.R) / (-2 * d);

            k_v = a_v + (Vector2.Normalize(d_v) * x1);
            //K = new PointF(k_v.X, k_v.Y);

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

        public PointF GetTangentLineEndPoint(PointF tan_p, Circle a, Circle b, PointF next_tan_p)
        {
            Vector2 tan_p_v = new Vector2(tan_p.X, tan_p.Y);
            Vector2 o_v = new Vector2(a.X, a.Y);
            Vector2 next_v = new Vector2(next_tan_p.X, next_tan_p.Y);
            Vector2 temp = tan_p_v - o_v; //R
            Vector2 res_v;
            PointF res;
            double distance = GetLengthofTanPtoRadLine(tan_p, a, b);


            //TODO:  fixelni: ha jobról balra,lefele rajzolok köröket bugos, 
            //ha balról felfele rajzolok akkor is bugos

            temp = new Vector2(temp.Y * -1, temp.X); //R-re merőleges



            distance *= 2;
            Vector2 t1_v = tan_p_v + (Vector2.Normalize(temp) * Convert.ToInt32(distance));
            Vector2 dir1_v = t1_v - next_v;

            Vector2 t2_v = tan_p_v - (Vector2.Normalize(temp) * Convert.ToInt32(distance));
            Vector2 dir2_v = t2_v - next_v;
            if (dir1_v.Length() <= dir2_v.Length())
            {
                res_v = t1_v;
                res = new PointF(res_v.X, res_v.Y);
            }else
            {
                res_v = t2_v;
                res = new PointF(res_v.X, res_v.Y);
            }

            return res;
        }
    }
}
