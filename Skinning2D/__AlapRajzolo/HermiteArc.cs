using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace __AlapRajzolo
{
    public class HermiteArc
    {
        private static Color COLORDEF = Color.Black;
        private static float LINEWIDTHDEF = 3.0f;
        private static int points = 500;
        public PointF P0 { get; set; }
        public PointF P1 { get; set; }
        public PointF T0 { get; set; }
        public PointF T1 { get; set; }
        public Color Color { get; set; }
        public float LineWidth { get; set; }
        public HermiteArc(Color color, float width, PointF p0, PointF p1, PointF t0, PointF t1)
        {
            this.Color = color;
            this.LineWidth = width;
            this.P0 = p0;
            this.P1 = p1;
            this.T0 = t0;
            this.T1 = t1;
        }
        public HermiteArc(Color color, PointF p0, PointF p1, PointF t0, PointF t1)
            : this(color, LINEWIDTHDEF, p0, p1, t0, t1)
        { }
        public HermiteArc(float width, PointF p0, PointF p1, PointF t0, PointF t1)
         : this(COLORDEF, width, p0, p1, t0, t1)
        { }
        public HermiteArc(PointF p0, PointF p1, PointF t0, PointF t1) 
            : this(COLORDEF, LINEWIDTHDEF, p0, p1, t0, t1)
        { }

    }
}
