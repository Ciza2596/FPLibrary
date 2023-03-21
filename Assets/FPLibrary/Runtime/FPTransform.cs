using UnityEngine;

namespace FPLibrary
{
    public class FPTransform : MonoBehaviour
    {
        private FPVector _position;
        public FPVector position {
            get {
				return _position;
            }
            set {
                _position = value;
            }
        }
        
        private FPQuaternion _rotation;
        public FPQuaternion rotation {
            get {
                return _rotation;
            }
            set {
                _rotation = value;
            }
        }

        private FPVector _scale;
        public FPVector scale {
            get {
                return _scale;
            }
            set {
                _scale = value;
            }
        }
        
        public void LookAt(FPTransform other) {
            LookAt(other.position);
        }
        
        public void LookAt(FPVector target) {
            this.rotation = FPQuaternion.CreateFromMatrix(FPMatrix.CreateFromLookAt(position, target));
        }
        
        public void Translate(Fix64 x, Fix64 y, Fix64 z) {
            Translate(x, y, z, Space.World);
        }
        
        public void Translate(Fix64 x, Fix64 y, Fix64 z, Space relativeTo) {
            Translate(new FPVector(x, y, z), relativeTo);
        }
        
        public void Translate(Fix64 x, Fix64 y, Fix64 z, FPTransform relativeTo) {
            Translate(new FPVector(x, y, z), relativeTo);
        }
        
        public void Translate(FPVector translation) {
            Translate(translation, Space.Self);
        }
        
        public void Translate(FPVector translation, Space relativeTo) {
            if (relativeTo == Space.Self) {
                Translate(translation, this);
            } else {
                this.position += translation;
            }
        }
        
        public void Translate(FPVector translation, FPTransform relativeTo) {
            this.position += FPVector.Transform(translation, FPMatrix.CreateFromQuaternion(relativeTo.rotation));
        }
        
        public void RotateAround(FPVector point, FPVector axis, Fix64 angle) {
            FPVector vector = this.position;
            FPVector vector2 = vector - point;
            vector2 = FPVector.Transform(vector2, FPMatrix.AngleAxis(angle * Fix64.Deg2Rad, axis));
            vector = point + vector2;
            this.position = vector;

            Rotate(axis, angle);
        }
        
        public void RotateAround(FPVector axis, Fix64 angle) {
            Rotate(axis, angle);
        }
        
        public void Rotate(Fix64 xAngle, Fix64 yAngle, Fix64 zAngle) {
            Rotate(new FPVector(xAngle, yAngle, zAngle), Space.Self);
        }
        
        public void Rotate(Fix64 xAngle, Fix64 yAngle, Fix64 zAngle, Space relativeTo) {
            Rotate(new FPVector(xAngle, yAngle, zAngle), relativeTo);
        }
        
        public void Rotate(FPVector eulerAngles) {
            Rotate(eulerAngles, Space.Self);
        }
        
        public void Rotate(FPVector axis, Fix64 angle) {
            Rotate(axis, angle, Space.Self);
        }
        
        public void Rotate(FPVector axis, Fix64 angle, Space relativeTo) {
            FPQuaternion result = FPQuaternion.identity;

            if (relativeTo == Space.Self) {
                result = this.rotation * FPQuaternion.AngleAxis(angle, axis);
            } else {
                result = FPQuaternion.AngleAxis(angle, axis) * this.rotation;
            }

            result.Normalize();
            this.rotation = result;
        }
        
        public void Rotate(FPVector eulerAngles, Space relativeTo) {
            FPQuaternion result = FPQuaternion.identity;

            if (relativeTo == Space.Self) {
                result = this.rotation * FPQuaternion.Euler(eulerAngles);
            } else {
                result = FPQuaternion.Euler(eulerAngles) * this.rotation;
            }

            result.Normalize();
            this.rotation = result;
        }
        
        public FPVector forward {
            get {
                return FPVector.Transform(FPVector.forward, FPMatrix.CreateFromQuaternion(rotation));
            }
        }
        
        public FPVector right {
            get {
                return FPVector.Transform(FPVector.right, FPMatrix.CreateFromQuaternion(rotation));
            }
        }
        
        public FPVector up {
            get {
                return FPVector.Transform(FPVector.up, FPMatrix.CreateFromQuaternion(rotation));
            }
        }
        
        public FPVector eulerAngles {
            get {
                return rotation.eulerAngles;
            }
        }

        public void UpdateTransform(Transform transform) {
            transform.position = new Vector3((float)position.x, (float)position.y, (float)position.z);
            //transform.rotation = new Quaternion((float)rotation.x, (float)rotation.y, (float)rotation.z, (float)rotation.w);
            //transform.localScale = new Vector3((float)scale.x, (float)scale.y, (float)scale.z);
        }

    }

}