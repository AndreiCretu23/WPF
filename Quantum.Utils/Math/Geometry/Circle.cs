using Quantum.Utils;
using System.Windows;

namespace Quantum.Math
{
    public class Circle
    {
        public Point Center { get; private set; }
        public double Radius { get; private set; }


        public Circle(Point Center, double Radius)
        {
            this.Center = Center;
            this.Radius = Radius;
        }

        public double GetDiameter()
        {
            return (2d * Radius);
        }

        public double GetCircumference()
        {
            return (System.Math.PI * GetDiameter());
        }

        public Point GetPointAtAngle(double Angle)
        {
            return new Point()
            {
                X = Center.X + Radius * System.Math.Cos(Angle),
                Y = Center.Y + Radius * System.Math.Sin(Angle)
            };
        }

        public bool IsPointOnCircle(Point P)
        {
            if ((System.Math.Pow((P.X - Center.X), 2) + System.Math.Pow((P.Y - Center.Y), 2)).IsInCloseProximityOf(System.Math.Pow(Radius, 2))) return true;
            return false;
        }

        public Line GetTangent(Point P)
        {
            if (IsPointOnCircle(P))
            {
                return new Line(P, (Center.X - P.X) / (P.Y - Center.Y));
            }
            return null;
        }

        public double GetMiddleAngle(double angle1, double angle2)
        {
            if (angle1 == 0d) return GetMiddleOfAngle(angle2);
            else if (angle2 == 0d) return GetMiddleOfAngle(angle1);
            else if (angle1 < (1d / 2d) * System.Math.PI && angle2 > (3d / 2d) * System.Math.PI) return GetMiddleOfDifferentCycleAngles(angle1, angle2);
            else if (angle2 < (1d / 2d) * System.Math.PI && angle1 > (3d / 2d) * System.Math.PI) return GetMiddleOfDifferentCycleAngles(angle2, angle1);
            else return (angle1 + angle2) / 2d;
        }

        private double GetMiddleOfAngle(double angle)
        {
            if (angle < System.Math.PI) return angle / 2d;
            return (angle + 2 * System.Math.PI) / 2d;
        }

        private double GetMiddleOfDifferentCycleAngles(double CycleStart, double CycleEnd)
        {
            double CycleEndValue = 2 * System.Math.PI - CycleEnd;

            if (CycleStart < CycleEndValue) return (CycleEnd + ((CycleStart + CycleEndValue) / 2d));
            else return (CycleStart - ((CycleStart + CycleEndValue) / 2d));
        }

    }
}
