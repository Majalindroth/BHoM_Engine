﻿/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;
using System;
using BH.Engine.Reflection;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/
        [Deprecated("2.4", "Deprecated to expose tolerance as optional parameter for greater control", null, "Centroid(this Polyline curve, double tolerance = Tolerance.Distance)")]
        public static Point Centroid(this Polyline curve)
        {
            return curve.Centroid(Tolerance.Distance);
        }

        [Deprecated("2.4", "Deprecated to expose tolerance as optional parameter for greater control", null, "Centroid(this PolyCurve curve, double tolerance = Tolerance.Distance)")]
        public static Point Centroid(this PolyCurve curve)
        {
            return curve.Centroid(Tolerance.Distance);
        }

        [Deprecated("2.4", "Deprecated to expose tolerance as optional parameter for greater control", null, "Centroid(this Ellipse ellipse, double tolerance = Tolerance.Distance)")]
        public static Point Centroid(this Ellipse ellipse)
        {
            return ellipse.Centroid(Tolerance.Distance);
        }

        [Deprecated("2.4", "Deprecated to expose tolerance as optional parameter for greater control", null, "Centroid(this Circle circle, double tolerance = Tolerance.Distance)")]
        public static Point Centroid(this Circle circle)
        {
            return circle.Centroid(Tolerance.Distance);
        }

        [Deprecated("2.4", "Deprecated to expose tolerance as optional parameter for greater control", null, "Centroid(this Line line, double tolerance = Tolerance.Distance)")]
        public static Point Centroid(this Line line)
        {
            return line.Centroid(Tolerance.Distance);
        }

        [NotImplemented]
        [Deprecated("2.4", "Deprecated to expose tolerance as optional parameter for greater control", null, "Centroid(this Arc arc, double tolerance = Tolerance.Distance)")]
        public static Point Centroid(this Arc arc)
        {
            return arc.Centroid(Tolerance.Distance);
        }

        [NotImplemented]
        [Deprecated("2.4", "Deprecated to expose tolerance as optional parameter for greater control", null, "Centroid(this NurbsCurve nurbsCurve, double tolerance = Tolerance.Distance)")]
        public static Point Centroid(this NurbsCurve nurbsCurve)
        {
            return nurbsCurve.Centroid(Tolerance.Distance);
        }

        /***************************************************/

        public static Point Centroid(this Polyline curve, double tolerance = Tolerance.Distance)
        {
            if (!curve.IsPlanar(tolerance))
            {
                Reflection.Compute.RecordError("Input must be planar.");
                return null;
            }
            else if (!curve.IsClosed(tolerance))
            {
                Reflection.Compute.RecordError("Curve is not closed. Input must be a polygon");
                return null;
            }
            else if (curve.IsSelfIntersecting(tolerance))
            {
                Reflection.Compute.RecordWarning("Curve is self intersecting");
                return null;
            }


            double xc, yc, zc;
            double xc0 = 0, yc0 = 0, zc0 = 0;

            Point pA = curve.ControlPoints[0];
            Point pB0 = curve.ControlPoints[1];
            Point pC0 = curve.ControlPoints[2];

            Vector firstNormal;

            firstNormal = CrossProduct(pB0 - pA, pC0 - pA);

            Vector normal = Normal(curve, tolerance);

            Boolean dir = DotProduct(normal, firstNormal) > 0;

            for (int i = 1; i < curve.ControlPoints.Count - 2; i++)
            {

                Point pB = curve.ControlPoints[i];
                Point pC = curve.ControlPoints[i + 1];

                double triangleArea = Area(pB - pA, pC - pA);

                if (DotProduct(CrossProduct(pB - pA, pC - pA), firstNormal) > 0)
                {
                    xc0 += ((pA.X + pB.X + pC.X) / 3) * triangleArea;
                    yc0 += ((pA.Y + pB.Y + pC.Y) / 3) * triangleArea;
                    zc0 += ((pA.Z + pB.Z + pC.Z) / 3) * triangleArea;
                }
                else
                {
                    xc0 -= ((pA.X + pB.X + pC.X) / 3) * triangleArea;
                    yc0 -= ((pA.Y + pB.Y + pC.Y) / 3) * triangleArea;
                    zc0 -= ((pA.Z + pB.Z + pC.Z) / 3) * triangleArea;
                }
            }


            if (!dir)
            {
                xc0 = -xc0;
                yc0 = -yc0;
                zc0 = -zc0;
            }

            double curveArea = curve.Area();

            xc = xc0 / curveArea;
            yc = yc0 / curveArea;
            zc = zc0 / curveArea;

            return new Point { X = xc, Y = yc, Z = zc };

        }

        /***************************************************/

        public static Point Centroid(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            if (!curve.IsPlanar(tolerance))
            {
                Reflection.Compute.RecordError("Input must be planar.");
                return null;
            }
            else if (!curve.IsClosed(tolerance))
            {
                Reflection.Compute.RecordError("Curve is not closed.");
                return null;
            }
            else if (curve.IsSelfIntersecting(tolerance))
            {
                Reflection.Compute.RecordWarning("Curve is self intersecting");
                return null;
            }

            List<ICurve> curveSubParts = curve.SubParts();

            if (curveSubParts.Count == 1 && curveSubParts[0] is Circle)
                return (curveSubParts[0] as Circle).Centre;

            List<Point> pts = new List<Point> { curveSubParts[0].IStartPoint() };
            foreach (ICurve crv in curveSubParts)
            {
                if (crv is Line)
                    pts.Add((crv as Line).End);
                else if (crv is Arc)
                    pts.Add(crv.IEndPoint());
                else
                    throw new NotImplementedException();
            }

            double xc, yc, zc;
            double xc0 = 0, yc0 = 0, zc0 = 0;

            Vector normal = Normal(curve, tolerance);

            Point pA = pts[0];
            Point pB0 = pts[1];
            Point pC0 = pts[2];

            Vector firstNormal;

            firstNormal = CrossProduct(pB0 - pA, pC0 - pA);

            Boolean dir = DotProduct(normal, firstNormal) > 0;

            for (int i = 1; i < pts.Count - 2; i++)
            {

                Point pB = pts[i];
                Point pC = pts[i + 1];

                double triangleArea = Area(pB - pA, pC - pA);

                if (DotProduct(CrossProduct(pB - pA, pC - pA), firstNormal) > 0)
                {
                    xc0 += ((pA.X + pB.X + pC.X) / 3) * triangleArea;
                    yc0 += ((pA.Y + pB.Y + pC.Y) / 3) * triangleArea;
                    zc0 += ((pA.Z + pB.Z + pC.Z) / 3) * triangleArea;
                }
                else
                {
                    xc0 -= ((pA.X + pB.X + pC.X) / 3) * triangleArea;
                    yc0 -= ((pA.Y + pB.Y + pC.Y) / 3) * triangleArea;
                    zc0 -= ((pA.Z + pB.Z + pC.Z) / 3) * triangleArea;
                }
            }

            foreach (ICurve crv in curveSubParts)
            {
                if (crv is Arc)
                {
                    double alpha = (crv as Arc).EndAngle - (crv as Arc).StartAngle;
                    double area = (Math.Pow((crv as Arc).Radius, 2) / 2) * (alpha - Math.Sin(alpha));

                    Point p1 = crv.IStartPoint();
                    Point p2 = PointAtParameter(crv as Arc, 0.5);
                    Point p3 = crv.IEndPoint();

                    Point arcCentr = CircularSegmentCentroid(crv as Arc);

                    if (DotProduct(CrossProduct(p2 - p1, p3 - p1), firstNormal) > 0)
                    {
                        xc0 += arcCentr.X * area;
                        yc0 += arcCentr.Y * area;
                        zc0 += arcCentr.Z * area;
                    }
                    else
                    {
                        xc0 -= arcCentr.X * area;
                        yc0 -= arcCentr.Y * area;
                        zc0 -= arcCentr.Z * area;
                    }
                }
            }


            if (!dir)
            {
                xc0 = -xc0;
                yc0 = -yc0;
                zc0 = -zc0;
            }

            double curveArea = curve.Area();

            xc = xc0 / curveArea;
            yc = yc0 / curveArea;
            zc = zc0 / curveArea;

            return new Point { X = xc, Y = yc, Z = zc };

        }
        
        /***************************************************/

        public static Point Centroid(this Ellipse ellipse, double tolerance = Tolerance.Distance)
        {
            return ellipse.Centre;
        }
                
        /***************************************************/

        public static Point Centroid(this Circle circle, double tolerance = Tolerance.Distance)
        {
            return circle.Centre;
        }
        
        /***************************************************/

        public static Point Centroid(this Line line, double tolerance = Tolerance.Distance)
        {
            return line.PointAtParameter(0.5);
        }
        
        /***************************************************/

        [NotImplemented]
        public static Point Centroid(this Arc arc, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }
        
        /***************************************************/

        [NotImplemented]
        public static Point Centroid(this NurbsCurve nurbsCurve, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }
        
        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private static Point CircularSegmentCentroid(this Arc arc)
        {
            Point o = arc.CoordinateSystem.Origin;
            double alpha = arc.EndAngle - arc.StartAngle;
            Point mid = PointAtParameter(arc, 0.5);

            Vector v = mid - o;

            Double length = (4 * arc.Radius * Math.Pow(Math.Sin(alpha / 2), 3)) / (3 * (alpha - Math.Sin(alpha)));

            return o + ((v / arc.Radius) * length);
        }
                       
        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Point ICentroid(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            return Centroid(curve as dynamic, tolerance);
        }

        /***************************************************/
    }
}
