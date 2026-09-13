using System.IO;
using System.Reflection;

namespace EBM2x.UI.PresetImage
{
    public class PresetUtil : PresetImagefileService
    {

        public static Stream GetImageStreamFromResource(string imageName)
        {
            var type = typeof(PresetUtil).GetTypeInfo();
            var assembly = type.Assembly;
            return assembly.GetManifestResourceStream($"EBM2x.UI.PresetImage.{imageName}");
        }
        public static string GetImageName(string imageName)
        {
                string filename = GetFileName("PresetImage", imageName);

                return filename;
        }
        public static Stream GetImageStream(string imageName)
        {
            if(IsFileExist("PresetImage", imageName))
            {
                string filename = GetFileName("PresetImage", imageName);
                byte[] buff = System.IO.File.ReadAllBytes(filename);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(buff);

                return stream;
            }
            else
            {
                imageName = "empty.png";
                var type = typeof(PresetUtil).GetTypeInfo();
                var assembly = type.Assembly;
                return assembly.GetManifestResourceStream($"EBM2x.UI.PresetImage.{imageName}");
            }
        }
        public static Stream GetImageSourceStream(string imageName)
        {
            byte[] buff = System.IO.File.ReadAllBytes(imageName);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(buff);

            return stream;
        }
    }
}
