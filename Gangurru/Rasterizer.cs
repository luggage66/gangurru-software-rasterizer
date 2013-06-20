using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharp3D.Math.Core;

namespace Gangurru
{
    public class Rasterizer
    {
        Buffer<Vector4F> rasterTarget;
        Buffer<Vector4F> target;
        Buffer<float> zBuffer;
        //RenderTarget finalTarget;
        //RenderTarget target;
        int sampleMultiplier;
        
        int finalWidth;
        int finalHeight;

        int sampleWidth;
        int sampleHeight;

        public Rasterizer(Buffer<Vector4F> rasterTarget, int sampleMultiplier)
        {
            if (rasterTarget.Dimensions != 2)
                throw new ApplicationException("Raster targets must be 2 dimensional");

            this.finalWidth = rasterTarget.Sizes[0];
            this.finalHeight = rasterTarget.Sizes[1];
            this.sampleWidth = finalWidth * sampleMultiplier;
            this.sampleHeight = finalHeight * sampleMultiplier;

            this.rasterTarget = rasterTarget;
            this.sampleMultiplier = sampleMultiplier;
            this.target = new Buffer<Vector4F>(2, sampleWidth, sampleHeight);
            this.zBuffer = new Buffer<float>(2, sampleWidth, sampleHeight);
        }

        public void ClearZBuffer()
        {
            zBuffer.SetAll(0.0f);
        }

        public void Downsample()
        {
            for (int y = 0; y < finalHeight; y++)
            {
                for (int x = 0; x < finalWidth; x++)
                {
                    var finalTargetOffset = x * 4 + y * finalWidth * 4;

                    Vector4F[] samples = new Vector4F[sampleMultiplier * sampleMultiplier];

                    for (int i = 0; i < sampleMultiplier; i++)
                    {
                        for (int j = 0; j < sampleMultiplier; j++)
                        {
                            samples[j * sampleMultiplier + i] = target[x * sampleMultiplier + i, y * sampleMultiplier + j];
                        }
                    }

                    Vector4F sum = Vector4F.Zero;
                    for (int i = 0; i < sampleMultiplier * sampleMultiplier; i++)
                        sum += samples[i];

                    rasterTarget[x, y] = sum / samples.Length;
                }
            }
        }

        public void DrawTriangle(Effect effect, Vertex[] buffer, int startIndex)
        {
            //from: http://joshbeam.com/articles/triangle_rasterization/

            Edge[] edges = new Edge[3];

            edges[0] = new Edge(this, buffer, startIndex + 0, startIndex + 1);
            edges[1] = new Edge(this, buffer, startIndex + 1, startIndex + 2);
            edges[2] = new Edge(this, buffer, startIndex + 2, startIndex + 0);

            float maxLength = 0;
            int longEdge = 0;

            // find edge with the greatest length in the y axis
            for (int i = 0; i < 3; i++)
            {
                float length = edges[i].Y2 - edges[i].Y1;
                if (length > maxLength)
                {
                    maxLength = length;
                    longEdge = i;
                }
            }

            int shortEdge1 = (longEdge + 1) % 3;
            int shortEdge2 = (longEdge + 2) % 3;

            // draw spans between edges; the long edge can be drawn
            // with the shorter edges to draw the full triangle
            DrawSpansBetweenEdges(effect, edges[longEdge], edges[shortEdge1]);
            DrawSpansBetweenEdges(effect, edges[longEdge], edges[shortEdge2]);
        }

        public struct Point2D
        {
            public int X;
            public int Y;
        }

        private Point2D GetBackBufferCoordinates(Vector4F position)
        {
            return new Point2D()
            {
                X = (int)((position.X + 1f) / 2 * sampleWidth),
                Y = (int)((-position.Y + 1f) / 2 * sampleHeight)
            };
        }

        private void DrawSpan(Effect effect, Span span, int y)
        {
            int xdiff = span.X2 - span.X1;
            if (xdiff == 0)
                return;

            //Next, we initialize a factor for interpolating between the beginning and end of the span, and calculate a step value for incrementing the factor each time the loop runs:
            float factor = 0.0f;
            float factorStep = 1.0f / (float)xdiff;
            //Finally, we loop through each x position in the span and set pixels using the y value passed to this function and a calculated color value:

            // draw each pixel in the span
            for (int x = span.X1; x < span.X2; x++)
            {
                Vertex lerpedVertex = Lerp(span.Vertex1, span.Vertex2, factor);

                Vector4F color;

                //Z Buffer!
                var oldZ = zBuffer[x, y];

                // 0 is when nothing has been rendered in that pixel, yet. It seems redundant with < z,
                // but it's not for certain types or projection transforms.
                if (oldZ == 0 || oldZ < lerpedVertex.Position.Z)
                {
                    zBuffer[x, y] = lerpedVertex.Position.Z;
                    effect.PixelShader(lerpedVertex, out color);
                    target[x, y] = color;
                }
                factor += factorStep;
            }
        }

        private void DrawSpansBetweenEdges(Effect effect, Edge longEdge, Edge shortEdge)
        {
            
            // calculate difference between the y coordinates
            // of the first edge and return if 0
            float e1ydiff = (float)(longEdge.Y2 - longEdge.Y1);
            if(e1ydiff == 0.0f)
                    return;

            // calculate difference between the y coordinates
            // of the second edge and return if 0
            float e2ydiff = (float)(shortEdge.Y2 - shortEdge.Y1);
            if(e2ydiff == 0.0f)
                    return;

            // calculate differences between the x coordinates
            // and colors of the points of the edges
            float e1xdiff = (float)(longEdge.X2 -  longEdge.X1);
            float e2xdiff = (float)(shortEdge.X2 - shortEdge.X1);
            Vector4F e1colordiff = (longEdge.Color2 - longEdge.Color1);
            Vector4F e2colordiff = (shortEdge.Color2 - shortEdge.Color1);
            float e1Zdiff = (longEdge.Z2 - longEdge.Z1);
            float e2Zdiff = (shortEdge.Z2 - shortEdge.Z1);

            // calculate factors to use for interpolation
            // with the edges and the step values to increase
            // them by after drawing each span
            float factor1 = (float)(shortEdge.Y1 - longEdge.Y1) / e1ydiff;
            float factorStep1 = 1.0f / e1ydiff;
            float factor2 = 0.0f;
            float factorStep2 = 1.0f / e2ydiff;

            //Lerp(e1.buffer[e1.index1].Color, e1.buffer[e1.index2].Color, s);

            // loop through the lines between the edges and draw spans
            for(int y = shortEdge.Y1; y < shortEdge.Y2; y++) {
                var longS = (y - longEdge.Y1) / ((float)longEdge.Y2 - longEdge.Y1);
                var shortS = (y - shortEdge.Y1) / ((float)shortEdge.Y2 - shortEdge.Y1);
                    // create and draw span
                Span span = new Span(
                    longEdge.X1 + (int)(e1xdiff * factor1),
                    shortEdge.X1 + (int)(e2xdiff * factor2),
                    Lerp(longEdge.Vertex1, longEdge.Vertex2, longS),
                    Lerp(shortEdge.Vertex1, shortEdge.Vertex2, shortS));
                    DrawSpan(effect, span, y);

                    // increase factors
                    factor1 += factorStep1;
                    factor2 += factorStep2;
            }
        }

        public class Edge
        {
            public Vertex[] buffer;
            public int index1;
            public int index2;

            public int X1;
            public int Y1;
            public int X2;
            public int Y2;

            public Edge(Rasterizer rasterizer, Vertex[] buffer, int index1, int index2)
            {
                this.buffer = buffer;

                var coords1 = rasterizer.GetBackBufferCoordinates(buffer[index1].Position);
                var coords2 = rasterizer.GetBackBufferCoordinates(buffer[index2].Position);

                if (coords1.Y < coords2.Y)
                {
                    this.index1 = index1;
                    this.index2 = index2;
                    X1 = coords1.X;
                    Y1 = coords1.Y;
                    X2 = coords2.X;
                    Y2 = coords2.Y;
                }
                else
                {
                    this.index1 = index2;
                    this.index2 = index1;
                    X2 = coords1.X;
                    Y2 = coords1.Y;
                    X1 = coords2.X;
                    Y1 = coords2.Y;
                }
            }
            public Vertex Vertex1 { get { return buffer[index1]; } }
            public Vertex Vertex2 { get { return buffer[index2]; } }
            public float Z1 { get { return buffer[index1].Position.Z; } }
            public float Z2 { get { return buffer[index2].Position.Z; } }
            public Vector4F Color1 { get { return buffer[index1].Color; } }
            public Vector4F Color2 { get { return buffer[index2].Color; } }
        }

        public class Span
        {
            public Vertex Vertex1;
            public Vertex Vertex2;

            public int X1;
            public int X2;

            public Span(int x1, int x2, Vertex vertex1, Vertex vertex2)
            {
                if (x1 < x2)
                {
                    Vertex1 = vertex1;
                    Vertex2 = vertex2;

                    X1 = x1;
                    X2 = x2;
                }
                else
                {
                    Vertex1 = vertex2;
                    Vertex2 = vertex1;

                    X1 = x2;
                    X2 = x1;
                }
            }
        }

        private Vector4F Lerp(Vector4F x, Vector4F y, Vector4F s)
        {
            return x * (1 - s) + y * s;
        }

        private Vector4F Lerp(Vector4F x, Vector4F y, float s)
        {
            return x * (1 - s) + y * s;
        }

        private Vector2F Lerp(Vector2F x, Vector2F y, float s)
        {
            return x * (1 - s) + y * s;
        }

        private Vector3F Lerp(Vector3F x, Vector3F y, float s)
        {
            return x * (1 - s) + y * s;
        }


        private Vertex Lerp(Vertex a, Vertex b, float s)
        {
            return new Vertex()
            {
                Color = Lerp(a.Color, b.Color, s),
                Position = Lerp(a.Position, b.Position, s),
                Texture = Lerp(a.Texture, b.Texture, s),
                Normal = Lerp(a.Normal, b.Normal, s),
                CamView = Lerp(a.CamView, b.CamView, s)
            };
        }
    }
}
