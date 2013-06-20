using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharp3D.Math.Core;
using Gangurru;

namespace PngOutputTestClient
{
    public class Camera
    {
        public Vector3F Position { get; set; }
        public Vector3F Direction { get; set; }

        public Matrix4F GetView()
        {
            return Matrix4FUtils.CreateLookAt(Position, Position + Direction, new Vector3F(0, 1, 0));
        }
    }
}
