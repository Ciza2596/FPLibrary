using System;
using UnityEngine;

namespace FPLibrary
{
    /// <summary>
    /// A simple fixed point rect structure.
    /// </summary>
    [Serializable]
    public struct FPOBB
    {
        /// <summary>
        /// Half lengths of the box along each axis.
        /// </summary>
        public FPVector Extents;

        /// <summary>
        /// The matrix which aligns and scales the box, and its translation vector represents the center of the box.
        /// </summary>
        public FPMatrix Transformation;

        /// <summary>
        /// Creates an <see cref="OrientedBoundingBox"/> from a BoundingBox.
        /// </summary>
        /// <param name="bb">The BoundingBox to create from.</param>
        /// <remarks>
        /// Initially, the OBB is axis-aligned box, but it can be rotated and transformed later.
        /// </remarks>
        public FPOBB(FPBB bb)
        {
            FPVector Center = bb.Min + (bb.Max - bb.Max) / 2;
            Extents = bb.Max - Center;
            Transformation = FPMatrix.Translate(Center);
        }

        /// <summary>
        /// Creates an <see cref="OrientedBoundingBox"/> which contained between two minimum and maximum points.
        /// </summary>
        /// <param name="minimum">The minimum vertex of the bounding box.</param>
        /// <param name="maximum">The maximum vertex of the bounding box.</param>
        /// <remarks>
        /// Initially, the OrientedBoundingBox is axis-aligned box, but it can be rotated and transformed later.
        /// </remarks>
        public FPOBB(FPVector minimum, FPVector maximum)
        {
            var Center = minimum + (maximum - minimum) / 2f;
            Extents = maximum - Center;
            Transformation = FPMatrix.Translate(Center);
        }

        /// <summary>
        /// Creates an <see cref="OrientedBoundingBox"/> that fully contains the given points.
        /// </summary>
        /// <param name="points">The points that will be contained by the box.</param>
        /// <remarks>
        /// This method is not for computing the best tight-fitting OrientedBoundingBox.
        /// And initially, the OrientedBoundingBox is axis-aligned box, but it can be rotated and transformed later.
        /// </remarks>
        public FPOBB(FPVector[] points)
        {
            if (points == null || points.Length == 0)
                throw new ArgumentNullException("points");

            FPVector minimum = new FPVector(Fix64.MaxValue);
            FPVector maximum = new FPVector(Fix64.MinValue);

            for (int i = 0; i < points.Length; ++i)
            {
                FPVector.Min(ref minimum, ref points[i], out minimum);
                FPVector.Max(ref maximum, ref points[i], out maximum);
            }

            var Center = minimum + (maximum - minimum) / 2f;
            Extents = maximum - Center;
            Transformation = FPMatrix.Translate(Center);
        }

        /// <summary>
        /// Retrieves the eight corners of the bounding box.
        /// </summary>
        /// <returns>An array of points representing the eight corners of the bounding box.</returns>
        public FPVector[] GetCorners()
        {
            var xv = new FPVector(Extents.x, 0, 0);
            var yv = new FPVector(0, Extents.y, 0);
            var zv = new FPVector(0, 0, Extents.z);
            FPVector.TransformNormal(ref xv, ref Transformation, out xv);
            FPVector.TransformNormal(ref yv, ref Transformation, out yv);
            FPVector.TransformNormal(ref zv, ref Transformation, out zv);

            FPVector center = Transformation.TranslationVector;

            var corners = new FPVector[8];
            corners[0] = center + xv + yv + zv;
            corners[1] = center + xv + yv - zv;
            corners[2] = center - xv + yv - zv;
            corners[3] = center - xv + yv + zv;
            corners[4] = center + xv - yv + zv;
            corners[5] = center + xv - yv - zv;
            corners[6] = center - xv - yv - zv;
            corners[7] = center - xv - yv + zv;

            return corners;
        }

        /// <summary>
        /// Transforms this box using a transformation matrix.
        /// </summary>
        /// <param name="mat">The transformation matrix.</param>
        /// <remarks>
        /// While any kind of transformation can be applied, it is recommended to apply scaling using scale method instead, which
        /// scales the Extents and keeps the Transformation matrix for rotation only, and that preserves collision detection accuracy.
        /// </remarks>
        public void Transform(ref FPMatrix mat)
        {
            Transformation *= mat;
        }

        /// <summary>
        /// Transforms this box using a transformation matrix.
        /// </summary>
        /// <param name="mat">The transformation matrix.</param>
        /// <remarks>
        /// While any kind of transformation can be applied, it is recommended to apply scaling using scale method instead, which
        /// scales the Extents and keeps the Transformation matrix for rotation only, and that preserves collision detection accuracy.
        /// </remarks>
        public void Transform(FPMatrix mat)
        {
            Transformation *= mat;
        }

        /// <summary>
        /// Scales the <see cref="OrientedBoundingBox"/> by scaling its Extents without affecting the Transformation matrix,
        /// By keeping Transformation matrix scaling-free, the collision detection methods will be more accurate.
        /// </summary>
        /// <param name="scaling"></param>
        public void Scale(ref FPVector scaling)
        {
            Extents.x *= scaling.x;
            Extents.y *= scaling.y;
            Extents.z *= scaling.z;
        }

        /// <summary>
        /// Scales the <see cref="OrientedBoundingBox"/> by scaling its Extents without affecting the Transformation matrix,
        /// By keeping Transformation matrix scaling-free, the collision detection methods will be more accurate.
        /// </summary>
        /// <param name="scaling"></param>
        public void Scale(FPVector scaling)
        {
            Extents.x *= scaling.x;
            Extents.y *= scaling.y;
            Extents.z *= scaling.z;
        }

        /// <summary>
        /// Scales the <see cref="OrientedBoundingBox"/> by scaling its Extents without affecting the Transformation matrix,
        /// By keeping Transformation matrix scaling-free, the collision detection methods will be more accurate.
        /// </summary>
        /// <param name="scaling"></param>
        public void Scale(Fix64 scaling)
        {
            Extents *= scaling;
        }

        /// <summary>
        /// Translates the <see cref="OrientedBoundingBox"/> to a new position using a translation vector;
        /// </summary>
        /// <param name="translation">the translation vector.</param>
        public void Translate(ref FPVector translation)
        {
            Transformation.TranslationVector += translation;
        }

        /// <summary>
        /// Translates the <see cref="OrientedBoundingBox"/> to a new position using a translation vector;
        /// </summary>
        /// <param name="translation">the translation vector.</param>
        public void Translate(FPVector translation)
        {
            Transformation.TranslationVector += translation;
        }

        /// <summary>
        /// The size of the <see cref="OrientedBoundingBox"/> if no scaling is applied to the transformation matrix.
        /// </summary>
        /// <remarks>
        /// The property will return the actual size even if the scaling is applied using Scale method, 
        /// but if the scaling is applied to transformation matrix, use GetSize Function instead.
        /// </remarks>
        public FPVector Size
        {
            get
            {
                return Extents * 2;
            }
        }

        /// <summary>
        /// Returns the size of the <see cref="OrientedBoundingBox"/> taking into consideration the scaling applied to the transformation matrix.
        /// </summary>
        /// <returns>The size of the consideration</returns>
        /// <remarks>
        /// This method is computationally expensive, so if no scale is applied to the transformation matrix
        /// use <see cref="OrientedBoundingBox.Size"/> property instead.
        /// </remarks>
        public FPVector GetSize()
        {
            var xv = new FPVector(Extents.x * 2, 0, 0);
            var yv = new FPVector(0, Extents.y * 2, 0);
            var zv = new FPVector(0, 0, Extents.z * 2);
            FPVector.TransformNormal(ref xv, ref Transformation, out xv);
            FPVector.TransformNormal(ref yv, ref Transformation, out yv);
            FPVector.TransformNormal(ref zv, ref Transformation, out zv);

            return new FPVector(xv.magnitude, yv.magnitude, zv.magnitude);
        }

        /// <summary>
        /// Returns the square size of the <see cref="OrientedBoundingBox"/> taking into consideration the scaling applied to the transformation matrix.
        /// </summary>
        /// <returns>The size of the consideration</returns>
        public FPVector GetSizeSquared()
        {
            var xv = new FPVector(Extents.x * 2, 0, 0);
            var yv = new FPVector(0, Extents.y * 2, 0);
            var zv = new FPVector(0, 0, Extents.z * 2);
            FPVector.TransformNormal(ref xv, ref Transformation, out xv);
            FPVector.TransformNormal(ref yv, ref Transformation, out yv);
            FPVector.TransformNormal(ref zv, ref Transformation, out zv);

            return new FPVector(xv.sqrMagnitude, yv.sqrMagnitude, zv.sqrMagnitude);
        }

        /// <summary>
        /// Returns the center of the <see cref="OrientedBoundingBox"/>.
        /// </summary>
        public FPVector Center
        {
            get
            {
                return Transformation.TranslationVector;
            }
        }

        /// <summary>
        /// Determines whether a <see cref="OrientedBoundingBox"/> contains a point. 
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>The type of containment the two objects have.</returns>
        public bool Contains(ref FPVector point)
        {
            // Transform the point into the obb coordinates
            FPMatrix invTrans;
            FPMatrix.Invert(ref Transformation, out invTrans);

            FPVector locPoint;
            FPVector.TransformCoordinate(ref point, ref invTrans, out locPoint);

            locPoint.x = FPMath.Abs(locPoint.x);
            locPoint.y = FPMath.Abs(locPoint.y);
            locPoint.z = FPMath.Abs(locPoint.z);

            //Simple axes-aligned BB check
            if (locPoint.x <= Extents.x && locPoint.y <= Extents.y && locPoint.z <= Extents.z)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Determines whether a <see cref="OrientedBoundingBox"/> contains a point. 
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>The type of containment the two objects have.</returns>
        public bool Contains(FPVector point)
        {
            return Contains(ref point);
        }
    }
}