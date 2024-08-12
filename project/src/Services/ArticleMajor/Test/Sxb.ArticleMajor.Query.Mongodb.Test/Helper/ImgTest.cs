using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Query.Mongodb.Test.Helper
{
    public class ImgTest
    {

        public static string[] Covers = new string[]
        {
            "https://cos.sxkid.com//images/article/75da17e1-4ed7-4848-b2b5-07e349175792/9d21349a-c426-4595-a296-67be93020c91.jpg",
            "https://cdn.sxkid.com/images/article/75da17e1-4ed7-4848-b2b5-07e349175792/c34036d8-f8b0-439c-abb4-1630d58e85f8.jpg",
            "https://cos.sxkid.com/images/article/77a67552-8fea-4cbc-9f84-233273274916/d08b0a2f-501f-4f9e-a212-b30c2aefae67.jpg",
            "https://cos.sxkid.com/images/article/91cdf811-0ad0-4f2c-a67f-ea6d5532e0e9/873eadc4-d5e6-42bb-beaf-fda8aaa83b1a.jpg",
            "https://cos.sxkid.com/images/article/405a6433-fa6c-46ed-9e66-793d192b5a0b/e4d5e937-762e-437f-8a0d-7a6da9832e98.jpg",
            "https://cos.sxkid.com/images/article/452c4219-4ca0-4653-bd93-e4bf7860efd7/be1d3bf3-2aaa-4a7a-9d2d-a26e3b6a50fd.jpg",
            "https://cos.sxkid.com/images/article/888f2aa8-4b66-4820-80db-f27bc2d610fb/fc5b618d-fc2c-4793-97d5-76ec0aae7c40.jpg"
        };


        public static string GetTestRandImg()
        {
            var rand = new Random();
            if (rand.Next(2) == 0) {
                return Covers[rand.Next(Covers.Length)];
            }
            else
            {
                return PurColor();
            }
        }

        public static string PurColor(int width = 40, int height = 30)
        {
            using Bitmap bitmap = new Bitmap(width, height);
            using Graphics graphics = Graphics.FromImage(bitmap);

            var rand = new Random();
            var r = rand.Next(0, 256);
            var g = rand.Next(0, 256);
            var b = rand.Next(0, 256);
            Brush brush = new SolidBrush(Color.FromArgb(r, g, b));
            graphics.FillRectangle(brush, 0, 0, 40, 30);
            //bitmap.Save("D:\\test\\test.png");

            return ImgToBase64String(bitmap);
        }

        //图片转为base64编码的字符串
        public static string ImgToBase64String(Bitmap bmp)
        {
            try
            {
                using MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return "data:image/png;base64," + Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
