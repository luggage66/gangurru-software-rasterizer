using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharp3D.Math.Core;

namespace Gangurru
{
    public class Effect
    {
        public Matrix4F World;
        public Matrix4F View;
        public Matrix4F Projection;
        public Buffer<Vector4F> Texture;
        public Buffer<Vector4F> NormalBuffer;
        public Vector3F LightDirection;
        public Vector3F CameraPosition;


        //color in, color out is not so useful.
        public virtual void PixelShader(Vertex input, out Vector4F output)
        {
            Vector3F norm = input.Normal;
            norm.Normalize();

            float diffuseColor = clamp(Vector3F.DotProduct(LightDirection, norm), 0.01f, 1) * 0.5f;

            input.CamView.Normalize();
            Vector3F half = LightDirection - input.CamView;
            half.Normalize();

            var preSpecular = clamp(Vector3F.DotProduct(norm, half), 0, 1);
            preSpecular = (float)Math.Pow(preSpecular, 50);



            output = (tex(Texture, input.Texture) * diffuseColor) + preSpecular;// *diffuseColor;
        }

        /*
        PS_OUT PS_AmbientDiffuse(VS_OUT input)
        {
            PS_OUT output = (PS_OUT)0;
            float3 Norm = normalize(input.Normal);
            float3 LightDir = normalize(input.Light);
            // Get ambient light
            AmbientColor *= AmbientIntensity;
            // Get diffuse light
            DiffuseColor = (DiffuseIntensity * DiffuseColor) * saturate(dot(LightDir, Norm));
            float3 Half = normalize(LightDir + normalize(input.CamView));
            float specular = pow(saturate(dot(Norm, Half)), 25);
            output.Color = AmbientColor + DiffuseColor + ((SpecularColor * SpecularIntensity) * specular);
            return output;
        }
        */

        public virtual void VertexShader(Vertex input, out Vertex output)
        {
            output.Position = mul(mul(mul(input.Position, World), View), Projection);
            output.Normal = input.Normal;
            output.Texture = input.Texture;
            Vector4F camView = mul(input.Position, World);
            output.CamView = CameraPosition - new Vector3F(camView.X, camView.Y, camView.Z);

            //diffuse lighting.
            
            var light = clamp(Vector3F.DotProduct(LightDirection, input.Normal), 0.01f, 1);

            output.Color = new Vector4F(light, light, light, 1);
        }

        protected Matrix4F mul(Matrix4F matrix1, Matrix4F matrix2)
        {
            return matrix1 * matrix2;
        }

        protected Vector4F mul(Vector4F vector, Matrix4F transform)
        {
            return Matrix4F.Transform(transform, vector);
            //return Vector4F.Transform(vector, transform);
        }

        protected Vector4F clamp(Vector4F input, float lower, float upper)
        {
            input.X = clamp(input.X, lower, upper);
            input.Y = clamp(input.Y, lower, upper);
            input.Z = clamp(input.Z, lower, upper);
            input.W = clamp(input.W, lower, upper);

            return input;
        }

        protected Vector3F clamp(Vector3F input, float lower, float upper)
        {
            input.X = clamp(input.X, lower, upper);
            input.Y = clamp(input.Y, lower, upper);
            input.Z = clamp(input.Z, lower, upper);

            return input;
        }

        protected float clamp(float a, float lower, float upper)
        {
            if (a > upper)
                return upper;
            else if (a < lower)
                return lower;

            return a;
        }

        protected void writeTex<T>(Buffer<T> buffer, T value, Vector2F texCoords)
        {
            var x = (int)(texCoords.X * (Texture.Sizes[0] - 1));
            var y = (int)(texCoords.Y * (Texture.Sizes[1] - 1));
            buffer[x, y] = value;

        }

        protected T tex<T>(Buffer<T> buffer, Vector2F texCoords)
        {
            var x = (int)(texCoords.X * (Texture.Sizes[0] - 1));
            var y = (int)(texCoords.Y * (Texture.Sizes[1] - 1));
            return buffer[x, y];
        }
    }

    //this should be configurable... somehow.
    public struct Vertex
    {
        public Vector4F Position;
        public Vector3F Normal;
        public Vector4F Color;
        public Vector2F Texture;
        public Vector3F CamView;
    }
}
