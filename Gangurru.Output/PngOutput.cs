using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;
using Sharp3D.Math.Core;
using System.Windows.Media;
using System.Runtime.InteropServices;

namespace Gangurru.Output
{
    public class PngOutput
    {
        public static void Save(Buffer<Vector4F> buffer, String filename)
        {
            BitmapSource b = null;
            GCHandle handle = GCHandle.Alloc(buffer.Array, System.Runtime.InteropServices.GCHandleType.Pinned);

            //b = BitmapSource.Create(buffer.Sizes[0], buffer.Sizes[1], 96, 96, PixelFormats.Prgba128Float, null, handle.AddrOfPinnedObject(), 16 * buffer.Length, 16 * buffer.Sizes[1]);
            b = BitmapSource.Create(buffer.Sizes[0], buffer.Sizes[1], 96, 96, PixelFormats.Rgb128Float, null, handle.AddrOfPinnedObject(), 16 * buffer.Length, 16 * buffer.Sizes[0]);
                handle.Free();

            //System.Windows.Media.Imaging.WriteableBitmap b = new System.Windows.Media.Imaging.WriteableBitmap(buffer.Sizes[0], buffer.Sizes[1], 1, 1, System.Windows.Media.PixelFormats.Prgba128Float, null);
            //b.Lock();
            //b.CopyPixels(new System.Windows.Int32Rect(0, 0, buffer.Sizes[0], buffer.Sizes[1]), buffer.Buffer, 12 * buffer.Sizes[1], 0);
            //System.Runtime.InteropServices.Marshal.Copy(buffer.Buffer, 0, b.BackBuffer, buffer.Length);
            //b.Unlock();

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(b));
            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                encoder.Save(stream);
                stream.Flush();
                stream.Close();
            }
        }

        public static Buffer<Vector4F> LoadTexture2D(string filename)
        {
            using (FileStream file = new FileStream(filename, FileMode.Open))
            {
                PngBitmapDecoder decoder = new PngBitmapDecoder(file, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                var image = decoder.Frames[0];
                var buffer = new Buffer<Vector4F>(2, image.PixelWidth, image.PixelHeight);
                var convertedImage = new FormatConvertedBitmap(image, PixelFormats.Rgb128Float, null, 1.0);

                GCHandle handle = GCHandle.Alloc(buffer.Array, System.Runtime.InteropServices.GCHandleType.Pinned);
                convertedImage.CopyPixels(new System.Windows.Int32Rect(0, 0, convertedImage.PixelWidth, convertedImage.PixelHeight), handle.AddrOfPinnedObject(), 16 * buffer.Length, 16 * convertedImage.PixelWidth);
                handle.Free();
                return buffer;
            }
        }
    }
}
