using System;
using UnityEngine;

namespace FPLibrary
{
    /// <summary>
    /// A simple fixed point rect structure.
    /// </summary>
    [Serializable]
    public struct FPBB
    {
        /// <summary>
        /// The minimum point of the box.
        /// </summary>
        public FPVector Min;


        /// <summary>
        /// The maximum point of the box.
        /// </summary>
        public FPVector Max;


        /// <param name="min">The minimum vertex of the bounding box.</param>
        /// <param name="max">The maximum vertex of the bounding box.</param>
        public FPBB(FPVector min, FPVector max)
        {
            this.Min = min;
            this.Max = max;
        }


        /// <summary>
        /// Returns the width of the bounding box
        /// </summary>
        public Fix64 Width
        {
            get { return this.Max.x - this.Min.x; }
        }

        /// <summary>
        /// Returns the height of the bounding box
        /// </summary>
        public Fix64 Height
        {
            get { return this.Max.y - this.Min.y; }
        }

        /// <summary>
        /// Returns the height of the bounding box
        /// </summary>
        public Fix64 Depth
        {
            get { return this.Max.z - this.Min.z; }
        }

        /// <summary>
        /// Returns the size of the bounding box
        /// </summary>
        public FPVector Size
        {
            get { return this.Max - this.Min; }
        }

        /// <summary>
        /// Returns the size of the bounding box
        /// </summary>
        public FPVector Center
        {
            get { return (this.Max + this.Min) * 0.5; }
        }
    }
}