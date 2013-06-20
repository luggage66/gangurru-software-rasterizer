using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gangurru;
using Sharp3D.Math.Core;
using System.Diagnostics;

namespace PngOutputTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.TraceInformation("Start");

            //render the final scene to this buffer
            Buffer<Vector4F> outputBuffer = new Buffer<Vector4F>(2, 800, 480);

            //load up some textures
            var testTexture = Gangurru.Output.PngOutput.LoadTexture2D("..\\..\\stone.png");
            var woodTexture = Gangurru.Output.PngOutput.LoadTexture2D("..\\..\\wood.png");
            Buffer<Vector4F> whiteTexture = new Buffer<Vector4F>(2, 1, 1);
            whiteTexture[0, 0] = new Vector4F(1, 1, 1, 1);

            Renderer renderer = new Renderer(outputBuffer);

            Camera camera = new Camera();
            camera.Position = new Vector3F(2, 6, 12.0f);
            camera.Direction = new Vector3F(0, 1, 0) - camera.Position;

            Model teapot = Model.FromFile("..\\..\\teapot.obj");

            //create our shaders and sett he non-chaning values.
            Effect effect = new Effect();
            effect.View = camera.GetView();
            effect.Projection = Matrix4FUtils.CreatePerspectiveFieldOfView(3.14159f / 4, (float)outputBuffer.Sizes[0] / outputBuffer.Sizes[1], 1.0f, 200.0f);
            effect.LightDirection = new Vector3F(1, -1, -3);
            effect.CameraPosition = camera.Position;

            //needs to be nromalized for calculations
            effect.LightDirection.Normalize();

            //models
            Vertex[] teapotVerts = teapot.CreateVertexes();
            Vertex[] pyramidVerts = CreatePyramid();
            Vertex[] woodVerts = CreateTable();

            //compute normals
            ComputeSmoothNormals(teapotVerts);
            ComputeFlatNormals(pyramidVerts);
            ComputeFlatNormals(woodVerts);

            //draw teapot
            effect.World = Sharp3D.Math.Geometry3D.TransformationF.Translation(0, 0, -2);
            effect.Texture = whiteTexture;
            renderer.DrawPrimitives(effect, teapotVerts, PrimitiveType.TriangleList, 0, teapotVerts.Length / 3);

            //raw pyramid
            effect.World = Sharp3D.Math.Geometry3D.TransformationF.Translation(0, 0, 2);
            effect.Texture = testTexture;
            renderer.DrawPrimitives(effect, pyramidVerts, PrimitiveType.TriangleList, 0, pyramidVerts.Length / 3);

            //table
            effect.World = Matrix4F.Identity;
            effect.Texture = woodTexture;
            renderer.DrawPrimitives(effect, woodVerts, PrimitiveType.TriangleList, 0, woodVerts.Length / 3);

            //pipeline.DrawTriangle(effect, verts, 0);
            //pipeline.DrawTriangle(effect, verts, 3);
            //pipeline.DrawTriangle(effect, verts, 6);
            //pipeline.DrawTriangle(effect, verts, 9);

            renderer.Rasterizer.Downsample(); //this call shouldn't be needed. I need to figuyre out how this should really be done.

            Trace.TraceInformation("Render Complete");

            Gangurru.Output.PngOutput.Save(outputBuffer, "out.png");

            Trace.TraceInformation("PNG Saved");

            Console.ReadLine();
        }

        private static Vertex[] CreateTable()
        {
            return new Vertex[]
            {
                new Vertex()
                {
                    Position = new Vector4F(-5, 0, 5, 1),
                    Texture = new Vector2F(0, 1)
                },
                new Vertex()
                {
                    Position = new Vector4F(-5, 0, -5, 1),
                    Texture = new Vector2F(0, 0)
                },
                new Vertex()
                {
                    Position = new Vector4F(5, 0, 5, 1),
                    Texture = new Vector2F(1, 1)
                },
                new Vertex()
                {
                    Position = new Vector4F(5, 0, 5, 1),
                    Texture = new Vector2F(1, 1)
                },
                new Vertex()
                {
                    Position = new Vector4F(-5, 0, -5, 1),
                    Texture = new Vector2F(0, 0)
                },
                new Vertex()
                {
                    Position = new Vector4F(5, 0, -5, 1),
                    Texture = new Vector2F(1, 0)
                }
            };
        }

        private static Vertex[] CreatePyramid()
        {
            return new Vertex[]
            {
                new Vertex()
                {
                    Position = new Vector4F(-1, 0, 1, 1), Color = new Vector4F(1, 0, 0, 1)
                    , Texture  = new Vector2F(0, 1)
                },
                new Vertex()
                {
                    Position = new Vector4F(0, 1, 0, 1), Color = new Vector4F(0, 1, 0, 1)
                    , Texture  = new Vector2F(0.5f, 0)
                },
                new Vertex()
                {
                    Position = new Vector4F(1, 0, 1, 1), Color = new Vector4F(1, 0, 0, 1)
                    , Texture  = new Vector2F(1, 1)
                },

                new Vertex()
                {
                    Position = new Vector4F(1, 0, 1, 1), Color = new Vector4F(1, 0, 0, 1)
                    , Texture  = new Vector2F(0, 1)
                },new Vertex()
                {
                    Position = new Vector4F(0, 1, 0, 1), Color = new Vector4F(1, 0, 0, 1)
                    , Texture  = new Vector2F(0.5f, 0)
                }
                ,
                new Vertex()
                {
                    Position = new Vector4F(1, 0, -1, 1), Color = new Vector4F(1, 0, 0, 1)
                    , Texture  = new Vector2F(1, 1)
                }
                ,

                new Vertex()
                {
                    Position = new Vector4F(1, 0, -1, 1), Color = new Vector4F(1, 0, 0, 1)
                    , Texture  = new Vector2F(0, 1)
                }
                ,
                new Vertex()
                {
                    Position = new Vector4F(0, 1, 0, 1), Color = new Vector4F(1, 0, 0, 1)
                    , Texture  = new Vector2F(0.5f, 0)
                }
                ,
                new Vertex()
                {
                    Position = new Vector4F(-1, 0, -1, 1), Color = new Vector4F(0, 0, 1, 1)
                    , Texture  = new Vector2F(1, 1)
                }
                ,

                new Vertex()
                {
                    Position = new Vector4F(-1, 0, -1, 1), Color = new Vector4F(1, 0, 0, 1)
                    , Texture  = new Vector2F(0, 1)
                }
                ,
                new Vertex()
                {
                    Position = new Vector4F(0, 1, 0, 1), Color = new Vector4F(1, 0, 0, 1)
                    , Texture  = new Vector2F(0.5f, 0)
                }
                ,
                new Vertex()
                {
                    Position = new Vector4F(-1, 0, 1, 1), Color = new Vector4F(1, 0, 0, 1)
                    , Texture  = new Vector2F(1, 1)
                }

            };
        }

        private static void ComputeFlatNormals(Vertex[] verts)
        {
            for (int i = 0; i < verts.Length; i += 3)
            {
                Vector4F vector1 = verts[i + 1].Position - verts[i].Position;
                Vector4F vector2 = verts[i + 2].Position - verts[i].Position;

                Vector3F normal = Vector3F.CrossProduct(new Vector3F(vector1.X, vector1.Y, vector1.Z), new Vector3F(vector2.X, vector2.Y, vector2.Z));
                normal.Normalize();

                verts[i].Normal = verts[i + 1].Normal = verts[i + 2].Normal = normal;
            }
        }

        private static void ComputeSmoothNormals(Vertex[] verts)
        {
            ComputeFlatNormals(verts);

            var uniquePositions = from v in verts
                                 group v by v.Position into p
                                 select new { Position = p.Key, Vertexes = p };


            foreach (var position in uniquePositions)
            {
                Vector3F normal = new Vector3F(0, 0, 0);

                foreach (var vertex in position.Vertexes)
                {
                    normal = normal + vertex.Normal;
                }

                normal = normal / position.Vertexes.Count();

                for (int i = 0; i < verts.Length; i++)
                {
                    if (verts[i].Position == position.Position)
                        verts[i].Normal = normal;
                }
            }

        }
    }
}
