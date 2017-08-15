namespace BroEngine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using OpenTK;

    public class Transform : Component, IEnumerable<Transform>
    {
        private readonly List<Transform> m_Children = new List<Transform>();
        private Matrix4 m_Matrix = Matrix4.Identity;

        public int childCount => m_Children.Count;

        public Vector3 localPosition
        {
            get { return m_Matrix.ExtractTranslation(); }
            set
            {
                m_Matrix = m_Matrix.ClearTranslation();
                m_Matrix[3, 0] = value.X;
                m_Matrix[3, 1] = value.Y;
                m_Matrix[3, 2] = value.Z;
            }
        }
        public Vector3 position
        {
            get { return worldSpaceMatrix.ExtractTranslation(); }
            set
            {
                if (parent == null)
                {
                    localPosition = value;
                    return;
                }

                var newTranslationMatrix = Matrix4.CreateTranslation(value);
                newTranslationMatrix *= Matrix4.Invert(parent.worldSpaceMatrix);

                localPosition = newTranslationMatrix.ExtractTranslation();
            }
        }

        public Vector3 localEulerAngles
        {
            get { return m_Matrix.ExtractEulerAngle(); }
            set
            {
                m_Matrix = m_Matrix.ClearRotation();
                Rotate(value);
            }
        }
        public Vector3 eulerAngles
        {
            get { return worldSpaceMatrix.ExtractEulerAngle(); }
            set
            {
                if (parent == null)
                {
                    localEulerAngles = value;
                    return;
                }

                value = value.ToRadians();
                var newRotationMatrix = MathExtensions.CreateEulerRotation(value);

                newRotationMatrix *= Matrix4.Invert(parent.worldSpaceMatrix);
                localEulerAngles = newRotationMatrix.ExtractEulerAngle();
            }
        }

        public Vector3 localScale
        {
            get { return m_Matrix.ExtractScale(); }
            set
            {
                m_Matrix = m_Matrix.ClearScale();
                Scale(value);
            }
        }
        public Vector3 scale => worldSpaceMatrix.ExtractScale();

        public Vector3 forward => -back;
        public Vector3 back => new Vector3(m_Matrix.Column2).Normalized();

        public Vector3 right => new Vector3(m_Matrix.Column0).Normalized();
        public Vector3 left => right;

        public Vector3 up => -down;
        public Vector3 down => new Vector3(m_Matrix.Column1).Normalized();

        public Transform parent { get; private set; }

        public Matrix4 worldSpaceMatrix
        {
            get
            {
                if (parent != null)
                    return m_Matrix * parent.worldSpaceMatrix;

                return m_Matrix;
            }
        }

        IEnumerator<Transform> IEnumerable<Transform>.GetEnumerator()
        {
            return m_Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Children.GetEnumerator();
        }

        public void Translate(Vector3 translation)
        {
            m_Matrix = Matrix4.CreateTranslation(translation) * m_Matrix;
        }

        public void Rotate(Vector3 rotation)
        {
            rotation = rotation.ToRadians();
            m_Matrix = MathExtensions.CreateEulerRotation(rotation) * m_Matrix;
        }

        public void RotateX(float angle)
        {
            m_Matrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(angle)) * m_Matrix;
        }
        public void RotateY(float angle)
        {
            m_Matrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(angle)) * m_Matrix;
        }
        public void RotateZ(float angle)
        {
            m_Matrix = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(angle)) * m_Matrix;
        }

        public void Scale(Vector3 scale)
        {
            m_Matrix = Matrix4.CreateScale(scale) * m_Matrix;
        }

        public void SetLookAt(Vector3 from, Vector3 to, Vector3 up)
        {
            var zAxis = (from - to).Normalized();
            var xAxis = Vector3.Cross(up, zAxis).Normalized();
            var yAxis = Vector3.Cross(zAxis, xAxis);

            var orientation =
                new Matrix4(
                    xAxis.X, yAxis.X, zAxis.X, 0,
                    xAxis.Y, yAxis.Y, zAxis.Y, 0,
                    xAxis.Z, yAxis.Z, zAxis.Z, 0,
                    0, 0, 0, 1);

            var translation =
                new Matrix4(
                    1, 0, 0, 0,
                    0, 1, 0, 0,
                    0, 0, 1, 0,
                    -from.X, -from.Y, -from.Z, 1);

            m_Matrix = (translation * orientation).Inverted();
        }

        public void SetParent(Transform newParent, bool keepWorldTransformation = true)
        {
            if (newParent == parent || newParent == this)
                return;

            if (parent != null)
                parent.m_Children.Remove(this);

            var parentToChild = false;
            if (m_Children.Count > 0)
            {
                var transforms = new Stack<Transform>();
                transforms.Push(this);

                var currentTransform = transforms.Peek();
                while (transforms.Count > 0)
                {
                    foreach (var child in currentTransform.m_Children)
                    {
                        if (child == newParent)
                        {
                            parentToChild = true;
                            break;
                        }

                        transforms.Push(child);
                    }
                    if (parentToChild)
                        break;

                    currentTransform = transforms.Pop();
                }
            }

            if (parentToChild)
            {
                var tempList = m_Children.ToList();
                foreach (var child in tempList)
                    child.SetParent(parent);

                m_Children.Clear();
            }

            Vector3 oldPosition = Vector3.Zero;
            Vector3 oldEulerAngle = Vector3.Zero;
            Vector3 oldScale = Vector3.One;
            if (keepWorldTransformation)
            {
                oldPosition = position;
                oldEulerAngle = eulerAngles;
                oldScale = localScale;
            }

            parent = newParent;

            if (newParent != null)
                newParent.m_Children.Add(this);

            if (keepWorldTransformation)
            {
                position = oldPosition;
                eulerAngles = oldEulerAngle;
                localScale = oldScale;
            }
        }
    }

    public static class MathExtensions
    {
        public static Vector3 ToDegrees(this Vector3 vector3)
        {
            return
                new Vector3(
                    MathHelper.RadiansToDegrees(vector3.X),
                    MathHelper.RadiansToDegrees(vector3.Y),
                    MathHelper.RadiansToDegrees(vector3.Z));
        }
        public static Vector3 ToRadians(this Vector3 vector3)
        {
            return
                new Vector3(
                    MathHelper.DegreesToRadians(vector3.X),
                    MathHelper.DegreesToRadians(vector3.Y),
                    MathHelper.DegreesToRadians(vector3.Z));
        }

        public static Vector3 ExtractEulerAngle(this Matrix4 matrix)
        {
            var sy = (float)Math.Sqrt(matrix[0, 0] * matrix[0, 0] + matrix[0, 1] * matrix[0, 1]);

            var singular = sy < 1e-6;

            float x, y, z;
            if (!singular)
            {
                x = (float)Math.Atan2(matrix[1, 2], matrix[2, 2]);
                y = (float)Math.Atan2(-matrix[0, 2], sy);
                z = (float)Math.Atan2(matrix[0, 1], matrix[0, 0]);
            }
            else
            {
                x = (float)Math.Atan2(-matrix[2, 1], matrix[1, 1]);
                y = (float)Math.Atan2(-matrix[0, 2], sy);
                z = 0;
            }

            var eulerAngle = new Vector3(x, y, z).ToDegrees();
            eulerAngle = ClampAngle(eulerAngle);

            return eulerAngle;
        }

        public static Matrix4 CreateEulerRotation(Vector3 newEulerAngle)
        {
            // Create 3 identity matrices to hold each rotation on an axis
            var rotationX = Matrix4.Identity;
            var rotationY = Matrix4.Identity;
            var rotationZ = Matrix4.Identity;

            // X Axis Rotation
            rotationX[1, 1] = (float)Math.Cos(newEulerAngle.X);
            rotationX[1, 2] = (float)Math.Sin(newEulerAngle.X);

            rotationX[2, 1] = -(float)Math.Sin(newEulerAngle.X);
            rotationX[2, 2] = (float)Math.Cos(newEulerAngle.X);

            // Y Axis Rotation
            rotationY[0, 0] = (float)Math.Cos(newEulerAngle.Y);
            rotationY[0, 2] = -(float)Math.Sin(newEulerAngle.Y);

            rotationY[2, 0] = (float)Math.Sin(newEulerAngle.Y);
            rotationY[2, 2] = (float)Math.Cos(newEulerAngle.Y);

            // Z Axis Rotation
            rotationZ[0, 0] = (float)Math.Cos(newEulerAngle.Z);
            rotationZ[0, 1] = (float)Math.Sin(newEulerAngle.Z);

            rotationZ[1, 0] = -(float)Math.Sin(newEulerAngle.Z);
            rotationZ[1, 1] = (float)Math.Cos(newEulerAngle.Z);

            // Apply
            return rotationX * rotationY * rotationZ;
        }

        public static int Modulus(int a, int b)
        {
            b = Math.Abs(b);

            var returnValue = a % b;
            if (returnValue < 0)
                returnValue += b;

            return returnValue;
        }
        public static float Modulus(float a, float b)
        {
            var decimalValue = a - (int)a;

            return Modulus((int)a, (int)b) + decimalValue;
        }

        public static float ClampAngle(float angle)
        {
            return Modulus(angle, 360f);
        }
        public static Vector3 ClampAngle(Vector3 eulerAngle)
        {
            eulerAngle.X = ClampAngle(eulerAngle.X);
            eulerAngle.Y = ClampAngle(eulerAngle.Y);
            eulerAngle.Z = ClampAngle(eulerAngle.Z);

            return eulerAngle;
        }
    }
}