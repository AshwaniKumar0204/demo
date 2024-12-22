using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace SchoolERP.Models
{
    public class BarCodeGenerator
    {
        public static byte[] GetBarCode(String value)
        {
            Zen.Barcode.Code128BarcodeDraw barCode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
            Image img = barCode.Draw(value, 50);
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                data = ms.ToArray();
            }
            return data;
        }
    }
}