using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gangurru
{
    //A buffer. Usually for textures or rasterizer output. Variable dimensions. T is the type of each pixel (or element).
    public class Buffer<T>
    {
        T[] buffer;
        int[] sizes;

        public Buffer(int dimensions, params int[] sizes)
        {
            if (dimensions != sizes.Length)
                throw new IndexOutOfRangeException();

            this.sizes = sizes;

            int size = sizes[0];
            
            for (int i = 1; i < sizes.Length; i++) //starting at 1 in intentional
                size *= sizes[i];

            buffer = new T[size];
        }

        public Buffer(T[] array, int dimensions, params int[] sizes)
        {
            int size = sizes[0];
            
            for (int i = 1; i < sizes.Length; i++) //starting at 1 in intentional
                size *= sizes[i];

            if (array.Length != size)
                throw new ApplicationException();

            this.buffer = array;
            this.sizes = sizes;
        }

        public int Dimensions
        {
            get { return sizes.Length; }
        }

        public int Length { get { return buffer.Length; } }

        public int[] Sizes
        {
            get
            {
                return sizes;
            }
        }

        public T[] Array //used by the PNG saving.. hopefully i can remove this in the future
        {
            get { return buffer; }
        }

        public void SetAll(T value)
        {
            //HACK: better/faster way? I hope so.
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = value;
        }

        //accepts 1 index OR the same number of indexes and dimensions
        private int GetIndex(params int[] indexes)
        {
            if (indexes.Length != 1 && indexes.Length != sizes.Length)
                throw new IndexOutOfRangeException("Wrong number of indexes.");

            int index = indexes[0];
            for (int i = 1; i < indexes.Length; i++) //starting at 1 in intentional
                index += indexes[i] * sizes[i - 1]; //each dimimension's coordinate is multiple by the previous dimension's size.

            return index;
        }

        public T this[params int[] indexes]
        {
            get
            {
                return buffer[GetIndex(indexes)];
            }
            set
            {
                buffer[GetIndex(indexes)] = value;
            }
        }
    }
}
