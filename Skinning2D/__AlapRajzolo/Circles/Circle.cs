using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace __AlapRajzolo
{
    public class Circle
    {
        private static Color COLORCIRCLE = Color.Red;
        private static Color COLORDEF = Color.Black;

        public Circle(PointF O, PointF P, Color Color, Color CDef)
        {
            this.O = O;
            this.P = P;
            this.Color = Color;
            this.CDef = CDef;
            this.LineWidth = 2f;
        }
        public Circle(PointF O, PointF P)
            : this(O, P, Circle.COLORCIRCLE, Circle.COLORDEF)
        {
            this.Color = Color;
            this.CDef = CDef;
        }
        public Circle(PointF O, float R, Color Color, Color CDef)
            : this(O, new PointF(O.X + R, O.Y), Color, CDef) //Itt volt az egyik probléma
        { }
        public Circle(PointF O, float R)
            : this(O, R, Circle.COLORCIRCLE, Circle.COLORDEF)
        { }

        public PointF O { get; set; }
        public PointF P { get; set; }
        public Color Color { get; set; } //körvonal
        public Color CDef { get; set; } //definiáló alakzatok színe
        public float LineWidth { get; set; }

        public float R
        {
            get
            {
                return (float)Math.Sqrt((this.O.X - this.P.X) * (this.O.X - this.P.X) +
                                        (this.O.Y - this.P.Y) * (this.O.Y - this.P.Y));
            }
        }

        public float X { get { return this.O.X; } }
        public float Y { get { return this.O.Y; } }

        
    }
}
