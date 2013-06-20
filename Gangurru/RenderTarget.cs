using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Sharp3D.Math.Core;

namespace Gangurru
{
    public class RenderTarget
    {
        public float[] BackBuffer;
        public float[] ZBuffer;

        public RenderTarget(int width, int height)
        {
            BackBuffer = new float[4 * width * height];
            ZBuffer = new float[width * height];
            Width = width;
            Height = height;
        }

        public void SetPixel(int x, int y, Vector4F color)
        {
            int startIndex = x * 4 + y * Width * 4;

            BackBuffer[startIndex] = color.X;
            BackBuffer[startIndex + 1] = color.Y;
            BackBuffer[startIndex + 2] = color.Z;
            BackBuffer[startIndex + 3] = color.W;
        }

        public Vector4F GetPixel(int x, int y)
        {
            int startIndex = x * 4 + y * Width * 4;

            return new Vector4F(BackBuffer[startIndex], BackBuffer[startIndex + 1], BackBuffer[startIndex + 2], BackBuffer[startIndex + 3]);
        }

        public void ClearZBuffer()
        {
            ZBuffer = new float[Width * Height]; //probably horrible
        }

        public Int32 Width { get; protected set; }
        public Int32 Height { get; protected set; }

    }
}
