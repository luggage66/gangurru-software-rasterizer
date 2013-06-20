using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharp3D.Math.Core;

namespace Gangurru
{
    public static class Matrix4FUtils
    {
        //From: http://webglfactory.blogspot.com/2011/06/how-to-create-view-matrix.html
        public static Matrix4F CreateLookAt(Vector3F position, Vector3F target, Vector3F upVector)
        {
            Vector3F vz = position - target;
            vz.Normalize();

            Vector3F vx = Vector3F.CrossProduct(upVector, vz);
            vx.Normalize();

            // vy doesn't need to be normalized because it's a cross
            // product of 2 normalized vectorsF
            Vector3F vy = Vector3F.CrossProduct(vz, vx);

            return new Matrix4F(
                new Vector4F(
                vx.X,
                vy.X,
                vz.X,
                0f),
                new Vector4F(vx.Y,
                vy.Y,
                vz.Y,
                0f),
                new Vector4F(vx.Z,
                vy.Z,
                vz.Z,
                0f),
                new Vector4F(-Vector3F.DotProduct(vx, position),
                -Vector3F.DotProduct(vy, position),
                -Vector3F.DotProduct(vz, position),
                1f));
        }

        //from: http://msdn.microsoft.com/en-us/library/windows/desktop/bb147302(v=vs.85).aspx
        public static Matrix4F CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlane, float farPlane)
        {
            float fov, Q;

            fov = (float)(1f / Math.Tan(fieldOfView * 0.5));  // 1/tan(x) == cot(x)
            Q = farPlane / (nearPlane - farPlane);

            return new Matrix4F(
                new Vector4F( fov / aspectRatio, 0, 0, 0),
                new Vector4F(0, fov, 0, 0),
                new Vector4F(0, 0, Q, -1),
                new Vector4F(0, 0, -Q * nearPlane, 0)
                );
        }

    }
}
