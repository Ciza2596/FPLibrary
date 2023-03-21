using System;
using UnityEngine;

namespace FPLibrary
{
    /// <summary>
    /// A fixed point rect structure.
    /// </summary>
    [Serializable]
    public struct FPRect
    {
        public Fix64 _x;
        public Fix64 _y;

        public FPVector topRight;
        public FPVector topLeft;
        public FPVector bottomRight;
        public FPVector bottomLeft;
        public Fix64 width;
        public Fix64 height;
        public Fix64 xMax;
        public Fix64 yMax;

        public Fix64 x { get { return _x; } set { _x = value; RefreshPoints(); } }
        public Fix64 y { get { return _y; } set { _y = value; RefreshPoints(); } }


        public FPRect(Rect rect)
        {
            this.topLeft = new FPVector(rect.x, rect.y, 0);
            this.topRight = new FPVector(rect.xMax, rect.y, 0);
            this.bottomLeft = new FPVector(rect.x, rect.yMax, 0);
            this.bottomRight = new FPVector(rect.xMax, rect.yMax, 0);
            this._x = rect.x;
            this._y = rect.y;
            this.width = rect.width;
            this.height = rect.height;
            this.xMax = rect.xMax;
            this.yMax = rect.yMax;
        }

        public FPRect(Fix64 x, Fix64 y, Fix64 width, Fix64 height)
        {
            this.topLeft = new FPVector(x, y, 0);
            this.topRight = new FPVector(x + width, y, 0);
            this.bottomLeft = new FPVector(x, y + height, 0);
            this.bottomRight = new FPVector(x + width, y + height, 0);
            this._x = x;
            this._y = y;
            this.width = width;
            this.height = height;
            this.xMax = x + width;
            this.yMax = y + height;
        }

        public Rect ToRect()
        {
            return new Rect((float)this._x, (float)this._y, (float)this.width, (float)this.height);
        }

        public void MoveTo(FPVector fpVector)
        {
            this._x = fpVector.x;
            this._y = fpVector.y;
            RefreshPoints();
        }

        private void RefreshPoints()
        {
            this.xMax = this._x + this.width;
            this.yMax = this._y + this.height;

            this.topLeft.x = this._x;
            this.topLeft.y = this._y;

            this.topRight.x = this.xMax;
            this.topRight.y = this._y;

            this.bottomLeft.x = this._x;
            this.bottomLeft.y = this.yMax;

            this.bottomRight.x = this.xMax;
            this.bottomRight.y = this.yMax;
        }
        
        public bool Intersects(FPRect rect)
        {
            return rect.topLeft.x < this.topRight.x &&
                   this.topLeft.x < rect.topRight.x &&
                   rect.topLeft.y < this.bottomLeft.y &&
                   this.topLeft.y < rect.bottomLeft.y;
        }

        public Fix64 IntersectArea(FPRect rect)
        {
            if (Intersects(rect))
            {
                Fix64 left = FPMath.Max(this.x, rect.x);
                Fix64 right = FPMath.Min(this.xMax, rect.xMax);
                Fix64 bottom = FPMath.Max(this.y, rect.y);
                Fix64 top = FPMath.Min(this.yMax, rect.yMax);

                return (right - left) * (top - bottom);
            }

            return 0;
        }

        public Fix64 DistanceToPoint(FPVector point)
        {
            Fix64 xMax = this.topRight.x;
            Fix64 xMin = this.topLeft.x;
            Fix64 yMax = this.bottomRight.y;
            Fix64 yMin = this.topRight.y;

                if (point.x < xMin)
                { // Region I, VIII, or VII
                    if (point.y < yMin)
                    { // I
                    FPVector diff = point - new FPVector(xMin, yMin, 0);
                        return diff.magnitude;
                    }
                    else if (point.y > this.bottomRight.y)
                    { // VII
                    FPVector diff = point - new FPVector(xMin, yMax, 0);
                        return diff.magnitude;
                    }
                    else
                    { // VIII
                        return xMin - point.x;
                    }
                }
                else if (point.x > xMax)
                { // Region III, IV, or V
                    if (point.y < yMin)
                    { // III
                    FPVector diff = point - new FPVector(xMax, yMin, 0);
                        return diff.magnitude;
                    }
                    else if (point.y > yMax)
                    { // V
                    FPVector diff = point - new FPVector(xMax, yMax, 0);
                        return diff.magnitude;
                    }
                    else
                    { // IV
                        return point.x - xMax;
                    }
                }
                else
                { // Region II, IX, or VI
                    if (point.y < yMin)
                    { // II
                        return yMin - point.y;
                    }
                    else if (point.y > yMax)
                    { // VI
                        return point.y - yMax;
                    }
                    else
                    { // IX
                        return 0;
                    }
                }
            }
    }
}