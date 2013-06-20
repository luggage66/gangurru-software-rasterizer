using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Sharp3D.Math.Core;

namespace Gangurru
{
    public class Model
    {
        IList<Vertex> Vertexes;
        IList<int> Indexes;

        public Model()
        {
            Vertexes = new List<Vertex>();
            Indexes = new List<int>();
        }

        public Vertex[] CreateVertexes()
        {
            var retval = new Vertex[Indexes.Count];

            for (var i = 0; i < Indexes.Count; i++)
            {
                retval[i] = Vertexes[Indexes[i] - 1];
            }

            return retval;
        }

        public static Model FromFile(string path)
        {
            Model model = new Model();

            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (line.StartsWith("v")) //vertex
                    {
                        var parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        float x = float.Parse(parts[1]);
                        float y = float.Parse(parts[2]);
                        float z = float.Parse(parts[3]);

                        model.Vertexes.Add(new Vertex() {
                            Position = new Vector4F(x, y, z, 1)
                        });
                    }
                    else if (line.StartsWith("f")) //face
                    {
                        var parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        model.Indexes.Add(int.Parse(parts[1]));
                        model.Indexes.Add(int.Parse(parts[3]));
                        model.Indexes.Add(int.Parse(parts[2]));
                    }
                }
            }

            return model;
        }
    }
}
