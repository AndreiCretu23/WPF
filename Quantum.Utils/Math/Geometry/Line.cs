using Quantum.Utils;
using System;
using System.Windows;

namespace Quantum.Math
{
    public class Line
    {
        public Point DefinitionPoint { get; private set; }
        public double Slope { get; private set; }

        public Line(Point P, double Slope)
        {
            this.DefinitionPoint = P;
            this.Slope = Slope;
        }

        public Line(Point P1, Point P2)
        {
            this.DefinitionPoint = P1;
            this.Slope = (P2.Y - P1.Y) / (P2.X - P1.X);
        }

        public bool IsPointOnLine(Point P)
        {
            if (this.IsHorizontal()) return P.Y == this.DefinitionPoint.Y;
            else if (this.IsVertical()) return P.X == this.DefinitionPoint.X;
            else return (P.Y - DefinitionPoint.Y).IsInCloseProximityOf(Slope * (P.X - DefinitionPoint.X));
        }

        public bool IsParallelWith(Line otherLine)
        {
            return this.Slope.IsInCloseProximityOf(otherLine.Slope);
        }

        public bool IsHorizontal()
        {
            return this.Slope.IsInCloseProximityOf(0d);
        }

        public bool IsVertical()
        {
            return this.Slope.IsCloseToInfinity();
        }

        public Point FindIntersectionPointWith(Line otherLine)
        {
            Point IntersectionPoint = new Point()
            {
                X = Double.NaN,
                Y = Double.NaN
            };

            if (!this.IsParallelWith(otherLine))
            {
                if (this.IsHorizontal())
                {
                    if (otherLine.IsVertical()) IntersectionPoint = SolveIntersectionForVerticalHorizontal(otherLine, this);
                    else IntersectionPoint = SolveIntersectionWithHorizontal(otherLine, this);
                }

                else if (this.IsVertical())
                {
                    if (otherLine.IsHorizontal()) IntersectionPoint = SolveIntersectionForVerticalHorizontal(this, otherLine);
                    else IntersectionPoint = SolveIntersectionWithVertical(otherLine, this);
                }

                else if (otherLine.IsHorizontal())
                {
                    IntersectionPoint = SolveIntersectionWithHorizontal(this, otherLine);
                }

                else if (otherLine.IsVertical())
                {
                    IntersectionPoint = SolveIntersectionWithVertical(this, otherLine);
                }

                else
                {
                    IntersectionPoint = SolveRegularIntersection(this, otherLine);
                }
            }

            return IntersectionPoint;
        }

        private Point SolveIntersectionForVerticalHorizontal(Line Vertical, Line Horizontal)
        {
            return new Point()
            {
                X = Vertical.DefinitionPoint.X,
                Y = Horizontal.DefinitionPoint.Y
            };
        }

        private Point SolveIntersectionWithHorizontal(Line Line, Line Horizontal)
        {
            return new Point()
            {
                X = ((Line.Slope * Line.DefinitionPoint.X) + Horizontal.DefinitionPoint.Y - Line.DefinitionPoint.Y) / Line.Slope,
                Y = Horizontal.DefinitionPoint.Y
            };
        }

        private Point SolveIntersectionWithVertical(Line Line, Line Vertical)
        {
            return new Point()
            {
                X = Vertical.DefinitionPoint.X,
                Y = Line.Slope * (Vertical.DefinitionPoint.X - Line.DefinitionPoint.X) + Line.DefinitionPoint.Y
            };
        }

        private Point SolveRegularIntersection(Line Line, Line OtherLine)
        {
            double XIntersectionPoint = ((Line.Slope * Line.DefinitionPoint.X) - (OtherLine.Slope * OtherLine.DefinitionPoint.X) - Line.DefinitionPoint.Y + OtherLine.DefinitionPoint.Y) /
                                                (this.Slope - OtherLine.Slope);
            double YIntersectionPoint = OtherLine.Slope * (XIntersectionPoint - OtherLine.DefinitionPoint.X) + OtherLine.DefinitionPoint.Y;
            return new Point()
            {
                X = XIntersectionPoint,
                Y = YIntersectionPoint
            };
        }
    }
}