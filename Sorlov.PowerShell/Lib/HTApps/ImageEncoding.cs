using System;
using System.Drawing;
using System.IO;
using System.Net;

namespace Sorlov.PowerShell.Lib.HTApps
{
    public static class ImageEncoding
    {

        public static Image ReadImage(string path)
        {
            return System.Drawing.Image.FromFile(path);
        }

        public static Image DownloadImage(string url)
        {
            WebClient wc = new WebClient();
            byte[] bytes = wc.DownloadData(url);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return System.Drawing.Image.FromStream(ms);
            }
        }

        public static string ImageToBase64String(Image image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, image.RawFormat);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static Image ImageFromBase64String(string base64String)
        {
            using (MemoryStream stream = new MemoryStream(
                Convert.FromBase64String(base64String)))
            using (Image sourceImage = Image.FromStream(stream))
            {
                return new Bitmap(sourceImage);
            }
        }

    }
}
