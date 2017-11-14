using System;
using System.Drawing;
using System.IO;
using ZXing;

namespace Module4TopShelfService
{
    public static class BarCode
    {
        public static BarCodeResult IsBarCode(string file, string textBarcode)
        {
            Result result = null;
            try
            {
                var reader = new BarcodeReader() { AutoRotate = true };
                using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    Bitmap bmp = new Bitmap(stream);
                    result = reader.Decode(bmp);
                }
            }
            catch (Exception)
            {
                return BarCodeResult.BrkokenFormat;
            }
            if ((result != null) && result.Text.Equals(textBarcode))
            {
                return BarCodeResult.Equals;
            }
            return BarCodeResult.NotEqual;
        }
    }
}
