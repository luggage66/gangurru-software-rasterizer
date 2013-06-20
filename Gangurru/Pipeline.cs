using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gangurru
{
    public class Pipeline<TVertexShaderIn, TPixelShaderIn>
    {
        public Func<TVertexShaderIn, TPixelShaderIn> VertexShader;

    }
}
