using SkiaSharp;
using System.IO;
using System.Reflection;
using ZXing;
using ZXing.QrCode;

namespace EBM2x.UI.Resource
{
    public class ResourceUtil
    {
        public static string PINWHEEL_RED = "#e52828";
        public static string PINWHEEL_DARKRED = "#b70808";
        public static string PINWHEEL_ORENGE = "#ffb303";
        public static string PINWHEEL_DARKORENGE = "#d06c12";
        public static string PINWHEEL_BLUE = "#4bcfe3";
        public static string PINWHEEL_DARKBLUE = "#037eaf";
        public static string PINWHEEL_GREEN = "#b0eb36";
        public static string PINWHEEL_DARKGREEN = "#629914";

        public static Stream GetImageStream(string imageName)
        {
            var type = typeof(ResourceUtil).GetTypeInfo();
            var assembly = type.Assembly;
            return assembly.GetManifestResourceStream($"EBM2x.UI.Resource.{imageName}");
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

    }
}
