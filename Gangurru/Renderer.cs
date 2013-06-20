using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharp3D.Math.Core;

namespace Gangurru
{
    public class Renderer
    {
        Buffer<Vector4F> target;
        Rasterizer rasterizer;

        public Renderer(Buffer<Vector4F> target)
        {
            this.rasterizer = new Rasterizer(target, 2);
            this.target = target;
        }

        public Rasterizer Rasterizer { get { return rasterizer; } }

        public void DrawTriangle(Effect effect, Vertex[] verts, int startIndex)
        {
            Vertex[] buffer = new Vertex[3];
            for (int i = 0; i < 3; i++)
            {
                effect.VertexShader(verts[i + startIndex], out buffer[i]);


                buffer[i].Position /= buffer[i].Position.W; //divide the components by the homegeneus factor W ( http://stackoverflow.com/questions/10475735/projecting-3d-vector-to-2d-screen-coordinates )
            }

            rasterizer.DrawTriangle(effect, buffer, 0);
        }

        public void DrawPrimitives(Effect effect, Vertex[] verts, PrimitiveType primitiveType, int startIndex, int count)
        {
            if (primitiveType == PrimitiveType.TriangleList)
            {
                for (int i = startIndex; i < startIndex + count * 3; i += 3)
                {
                    DrawTriangle(effect, verts, i);
                }
            }
        }
    }
}
