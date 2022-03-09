using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace __AlapRajzolo
{
    class ApolloniusProblem
    {
        private static Color COLORCIRCLE = Color.Green;
        private static Color COLORDEF = Color.DarkGreen;

        private Circle a, b, c;

        private float deltaU, deltaV, alpha;

        public ApolloniusProblem(Circle a, Circle b, Circle c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public Circle[] Transform(Circle a, Circle b, Circle c)
        {
            float deltaX = b.X - a.X;
            float deltaY = b.Y - a.Y;

            alpha = (float)Math.Atan2(deltaY, deltaX);

            PointF centerA = a.O.Rotate(alpha);
            PointF centerB = b.O.Rotate(alpha);
            PointF centerC = c.O.Rotate(alpha);

            deltaU = centerC.X;
            deltaV = centerB.Y;

            PointF centerAA = centerA.Translate(-deltaU, -deltaV);
            PointF centerBB = centerB.Translate(-deltaU, -deltaV);
            PointF centerCC = centerC.Translate(-deltaU, -deltaV);
            return new Circle[] { new Circle(centerAA, a.R, Color.Blue, Color.Black),
                                  new Circle(centerBB, b.R, Color.Blue, Color.Black),
                                  new Circle(centerCC, c.R, Color.Blue, Color.Black) };
        }
        public Circle TransformBack(Circle circle)
        {
            PointF center = circle.O.Translate(deltaU, deltaV);
            center = center.Rotate(-alpha);
            return new Circle(center, circle.R, ApolloniusProblem.COLORCIRCLE, ApolloniusProblem.COLORDEF);
        }

        public Circle GetTangentCircle(int s1, int s2, int s3)
        {
            Circle[] circles = Transform(this.a, this.b, this.c);

            float x1 = circles[0].O.X, y1 = circles[0].O.Y, r1 = circles[0].R;
            float x2 = circles[1].O.X, y2 = circles[1].O.Y, r2 = circles[1].R;
            float x3 = circles[2].O.X, y3 = circles[2].O.Y, r3 = circles[2].R;

            //https://rosettacode.org/wiki/Problem_of_Apollonius

            float v11 = 2 * x2 - 2 * x1;
            float v12 = 2 * y2 - 2 * y1;
            float v13 = x1 * x1 - x2 * x2 + y1 * y1 - y2 * y2 - r1 * r1 + r2 * r2;
            float v14 = 2 * s2 * r2 - 2 * s1 * r1;

            float v21 = 2 * x3 - 2 * x2;
            float v22 = 2 * y3 - 2 * y2;
            float v23 = x2 * x2 - x3 * x3 + y2 * y2 - y3 * y3 - r2 * r2 + r3 * r3;
            float v24 = 2 * s3 * r3 - 2 * s2 * r2;

            float w12 = v12 / v11;
            float w13 = v13 / v11;
            float w14 = v14 / v11;

            float w22 = v22 / v21 - w12;
            float w23 = v23 / v21 - w13;
            float w24 = v24 / v21 - w14;

            float P = -w23 / w22;
            float Q = w24 / w22;
            float M = -w12 * P - w13;
            float N = w14 - w12 * Q;

            float a = N * N + Q * Q - 1;
            float b = 2 * M * N - 2 * N * x1 + 2 * P * Q - 2 * Q * y1 + 2 * s1 * r1;
            float c = x1 * x1 + M * M - 2 * M * x1 + P * P + y1 * y1 - 2 * P * y1 - r1 * r1;

            float D = b * b - 4 * a * c;

            float rs = (-b - float.Parse(Math.Sqrt(D).ToString())) / (2 * float.Parse(a.ToString()));
            float xs = M + N * rs;
            float ys = P + Q * rs;

            return TransformBack(new Circle(new PointF(xs, ys), rs, ApolloniusProblem.COLORCIRCLE, ApolloniusProblem.COLORDEF));
        }
        public PointF GetTangentPoint(int i, Circle tangentwith)
        {
            Circle apollonius = this.GetTangentCircle(i, i, i);
            PointF p;
            Vector2 a = new Vector2(apollonius.X, apollonius.Y);
            Vector2 tan = new Vector2(tangentwith.X, tangentwith.Y);
            Vector2 v = tan - a;

            Vector2 point = a + (Vector2.Normalize(v) * apollonius.R);
            
            return new PointF(point.X, point.Y);
        }

       
    }
}
