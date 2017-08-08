﻿using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IBHoMGeometry GetTransformed(this IBHoMGeometry geometry, TransformMatrix transform)
        {
            return _GetTransformed(geometry as dynamic, transform);
        }


        /***************************************************/
        /**** Private Methods - Vectors                 ****/
        /***************************************************/

        public static Point _GetTransformed(this Point pt, TransformMatrix transform)
        {
            double[,] matrix = transform.Matrix;

            return new Point(
                matrix[0, 0] * pt.X + matrix[0, 1] * pt.Y + matrix[0, 0] * pt.Z + matrix[0, 0],
                matrix[1, 0] * pt.X + matrix[1, 1] * pt.Y + matrix[1, 0] * pt.Z + matrix[1, 0],
                matrix[2, 0] * pt.X + matrix[2, 1] * pt.Y + matrix[2, 0] * pt.Z + matrix[2, 0]
            );
        }

        /***************************************************/

        public static Vector _GetTransformed(this Vector vector, TransformMatrix transform)
        {
            double[,] matrix = transform.Matrix;

            return new Vector(
                matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 0] * vector.Z + matrix[0, 0],
                matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 0] * vector.Z + matrix[1, 0],
                matrix[2, 0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 0] * vector.Z + matrix[2, 0]
            );
        }

        /***************************************************/

        public static Plane _GetTransformed(this Plane plane, TransformMatrix transform)
        {
            return new Plane(plane.Origin._GetTransformed(transform), plane.Normal._GetTransformed(transform));
        }


        /***************************************************/
        /**** Private Methods - Curves                  ****/
        /***************************************************/

        public static Arc _GetTransformed(this Arc arc, TransformMatrix transform)
        {
            return new Arc(arc.Start._GetTransformed(transform), arc.End._GetTransformed(transform), arc.Middle._GetTransformed(transform));
        }

        /***************************************************/

        public static Circle _GetTransformed(this Circle circle, TransformMatrix transform)
        {
            return new Circle(circle.Centre._GetTransformed(transform), circle.Normal._GetTransformed(transform), circle.Radius);
        }

        /***************************************************/

        public static Line _GetTransformed(this Line line, TransformMatrix transform)
        {
            return new Line(line.Start._GetTransformed(transform), line.End._GetTransformed(transform));
        }

        /***************************************************/

        public static NurbCurve _GetTransformed(this NurbCurve curve, TransformMatrix transform)
        {
            return new NurbCurve(curve.ControlPoints.Select(x => x._GetTransformed(transform)), curve.Weights, curve.Knots);
        }


        /***************************************************/

        public static PolyCurve _GetTransformed(this PolyCurve curve, TransformMatrix transform)
        {
            return new PolyCurve(curve.Curves.Select(x => x.GetTransformed(transform) as ICurve));
        }

        /***************************************************/

        public static Polyline _GetTransformed(this Polyline curve, TransformMatrix transform)
        {
            return new Polyline(curve.ControlPoints.Select(x => x._GetTransformed(transform)));
        }


        /***************************************************/
        /**** Private Methods - Surfaces                ****/
        /***************************************************/

        public static Extrusion _GetTransformed(this Extrusion surface, TransformMatrix transform)
        {
            return new Extrusion(surface.Curve.GetTransformed(transform) as ICurve, surface.Direction._GetTransformed(transform), surface.Capped);
        }

        /***************************************************/

        public static Loft _GetTransformed(this Loft surface, TransformMatrix transform)
        {
            return new Loft(surface.Curves.Select(x => x.GetTransformed(transform) as ICurve));
        }

        /***************************************************/

        public static NurbSurface _GetTransformed(this NurbSurface surface, TransformMatrix transform)
        {
            return new NurbSurface(surface.ControlPoints.Select(x => x._GetTransformed(transform)), surface.Weights, surface.UKnots, surface.VKnots);
        }

        /***************************************************/

        public static Pipe _GetTransformed(this Pipe surface, TransformMatrix transform)
        {
            return new Pipe(surface.Centreline.GetTransformed(transform) as ICurve, surface.Radius, surface.Capped);
        }

        /***************************************************/

        public static PolySurface _GetTransformed(this PolySurface surface, TransformMatrix transform)
        {
            return new PolySurface(surface.Surfaces.Select(x => x.GetTransformed(transform) as ISurface));
        }


        /***************************************************/
        /**** Private Methods - Others                  ****/
        /***************************************************/

        public static Mesh _GetTransformed(this Mesh mesh, TransformMatrix transform)
        {
            return new Mesh(mesh.Vertices.Select(x => x._GetTransformed(transform)), mesh.Faces.Select(x => x.GetClone() as Face));
        }

        /***************************************************/

        public static GeometryGroup _GetTransformed(this GeometryGroup group, TransformMatrix transform)
        {
            return new GeometryGroup(group.Elements.Select(x => x.GetTransformed(transform)));
        }
    }
}
