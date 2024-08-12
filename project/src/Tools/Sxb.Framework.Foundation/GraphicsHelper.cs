using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Framework.Foundation
{
    public class GraphicsHelper
    {
        public static MemoryStream ComposeImages(Image baseImage, Image qrcodeImage, int x, int y, int width, int height)
        {
            if (baseImage == null || qrcodeImage == null)
            {
                throw new ArgumentNullException();
            }

            //指定位置画二维码
            using (Graphics graphics = Graphics.FromImage(baseImage))
            {
                //二维码图片
                graphics.DrawImage(qrcodeImage, x, y, width, height);
            }

            var ms = new MemoryStream();
            baseImage.Save(ms, ImageFormat.Png);

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}
