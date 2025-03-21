using ControlzEx.Standard;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Media.Media3D;
using System.Reflection;
using System.Windows.Interop;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Reg;


namespace DemuraEditor
{
    /// <summary>
    /// HDMIView.xaml 的互動邏輯
    /// </summary>
    public partial class HDMIView : Window
    {
        public HDMIView()
        {
            if (System.Windows.Forms.Screen.AllScreens.Count() >= 2)
            {
                foreach (Screen curScreen in Screen.AllScreens)
                {
                    if (!curScreen.Primary)
                    {
                        WindowStartupLocation = WindowStartupLocation.Manual;
                        Left = curScreen.WorkingArea.Left;
                        Top = curScreen.WorkingArea.Top;
                        Width = curScreen.Bounds.Width;
                        Height = curScreen.Bounds.Height;
                        Topmost = true;
                        WindowState = WindowState.Maximized;
                        ResizeMode = ResizeMode.NoResize;
                        WindowStyle = WindowStyle.None;
                        WindowState = WindowState.Normal;
                        ShowInTaskbar = false;

                    }
                }
            }
            InitializeComponent();
        }

        private int width = 960;
        private int height = 540;
        WriteableBitmap mbitmap = new WriteableBitmap(960, 540, 96, 96, PixelFormats.Bgra32, null);
        int stride;
        byte[] pixels;
        public void ImageChange()
        {
            // 建立 960x540 的 WriteableBitmap
            //WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            stride = width * 4; // 每行像素的 byte 數 (BGRA 每像素 4 bytes)
            pixels = new byte[stride * height];

            // 填滿整張圖片為白色 (BGRA: 255,255,255,255)
            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i] = 240;     // B
                pixels[i + 1] = 240; // G
                pixels[i + 2] = 240; // R
                pixels[i + 3] = 255; // A (透明度)
            }

            // 將像素數據寫入 WriteableBitmap
            mbitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            // 設定到 Image 控制項
            ViewImage.Source = mbitmap;

        }

        public void Adjust(string HrV, int start, int End, int Pt,int value)
        {
            decimal changevalue = (1 + (decimal)value / 100);
            // 讓最左側 (x=0) 的像素提高 60% 亮度（HSL 方法）
            AdjustLeftmostHSL(HrV, start, End, Pt, pixels, stride, (float)changevalue);

            // 將像素數據寫入 WriteableBitmap
            mbitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            // 設定到 Image 控制項
            ViewImage.Source = mbitmap;
        }

        public void Recover()
        {
            // 填滿整張圖片為白色 (BGRA: 255,255,255,255)
            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i] = 240;     // B
                pixels[i + 1] = 240; // G
                pixels[i + 2] = 240; // R
                pixels[i + 3] = 255; // A (透明度)
            }

            // 將像素數據寫入 WriteableBitmap
            mbitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            // 設定到 Image 控制項
            ViewImage.Source = mbitmap;
        }

        private void AdjustLeftmostHSL(string HrV, int start, int End, int Pt, byte[] pixels, int stride, float brightnessFactor)
        {
            switch (HrV)
            {
                case "H":
                    for (int y = start * 4; y < End * 4; y++) // 遍歷每一行
                    {
                        int index = (y + (960 * 4) * (Pt)); // x=0 的像素索引（每行的第一個像素）// 960*4 是每一行的byte數 如果要跳行就是要+上 n*960*4

                        // 取得 RGB 值
                        byte r = pixels[index + 2];
                        byte g = pixels[index + 1];
                        byte b = pixels[index];

                        // RGB 轉換為 HSL
                        (float h, float s, float l) = RgbToHsl(r, g, b);

                        // 提高亮度 60%
                        l = Math.Min(l * brightnessFactor, 1.0f); // 確保亮度不超過 1.0（HSL 亮度範圍是 0~1）

                        // HSL 轉回 RGB
                        (r, g, b) = HslToRgb(h, s, l);

                        // 寫回像素數據
                        pixels[index] = b;
                        pixels[index + 1] = g;
                        pixels[index + 2] = r;
                    }
                    break;
                case "V":
                    for (int y = start; y < End; y++) // 遍歷每一行
                    {
                        int index = (y * stride) + (Pt * 4); // x=0 的像素索引（每行的第一個像素）

                        // 取得 RGB 值
                        byte r = pixels[index + 2];
                        byte g = pixels[index + 1];
                        byte b = pixels[index];

                        // RGB 轉換為 HSL
                        (float h, float s, float l) = RgbToHsl(r, g, b);

                        // 提高亮度 60%
                        l = Math.Min(l * brightnessFactor, 1.0f); // 確保亮度不超過 1.0（HSL 亮度範圍是 0~1）

                        // HSL 轉回 RGB
                        (r, g, b) = HslToRgb(h, s, l);

                        // 寫回像素數據
                        pixels[index] = b;
                        pixels[index + 1] = g;
                        pixels[index + 2] = r;
                    }
                    break;
            }
        }

        // 轉換 RGB -> HSL
        private (float h, float s, float l) RgbToHsl(byte r, byte g, byte b)
        {
            float rf = r / 255f, gf = g / 255f, bf = b / 255f;
            float max = Math.Max(rf, Math.Max(gf, bf));
            float min = Math.Min(rf, Math.Min(gf, bf));
            float h = 0, s, l = (max + min) / 2;

            if (max == min)
            {
                h = s = 0; // 無色
            }
            else
            {
                float d = max - min;
                s = (l > 0.5) ? d / (2 - max - min) : d / (max + min);
                if (max == rf) h = (gf - bf) / d + (gf < bf ? 6 : 0);
                else if (max == gf) h = (bf - rf) / d + 2;
                else h = (rf - gf) / d + 4;
                h /= 6;
            }

            return (h, s, l);
        }

        // 轉換 HSL -> RGB
        private (byte r, byte g, byte b) HslToRgb(float h, float s, float l)
        {
            if (s == 0)
            {
                byte gray = (byte)(l * 255);
                return (gray, gray, gray);
            }

            float q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            float p = 2 * l - q;
            float[] rgb = { h + 1f / 3f, h, h - 1f / 3f };

            for (int i = 0; i < 3; i++)
            {
                if (rgb[i] < 0) rgb[i] += 1;
                if (rgb[i] > 1) rgb[i] -= 1;

                if (rgb[i] < 1f / 6f) rgb[i] = p + (q - p) * 6 * rgb[i];
                else if (rgb[i] < 1f / 2f) rgb[i] = q;
                else if (rgb[i] < 2f / 3f) rgb[i] = p + (q - p) * (2f / 3f - rgb[i]) * 6;
                else rgb[i] = p;
            }

            return ((byte)(rgb[0] * 255), (byte)(rgb[1] * 255), (byte)(rgb[2] * 255));
        }

        public void ImageChange2()
        {
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Rgba128Float, null);
            int stride = width * 16; // 每行像素的 byte 數 (RGBA128 每像素 16 bytes)
            float[] pixels = new float[width * height * 4]; // 4 通道 (RGBA)

            // 填滿一般白色區域 (1.0, 1.0, 1.0, 1.0) 代表標準 255
            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i] = 1.0f;   // R
                pixels[i + 1] = 1.0f; // G
                pixels[i + 2] = 1.0f; // B
                pixels[i + 3] = 1.0f; // A
            }

            // 讓 x=0 和 x=1 的亮度提高 60%（HDR）
            AdjustLeftmostHDR(pixels, width, height, 1.6f);

            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            // 設定到 Image 控制項
            ViewImage.Source = bitmap;
        }

        private void AdjustLeftmostHDR(float[] pixels, int width, int height, float brightnessFactor)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < 2; x++) // x=0 和 x=1
                {
                    int index = ((y * width + x) * 4) + 960; // 計算像素索引 (RGBA128 每像素 4 通道)

                    // **🔹 Debug: 確保這些值真的改變**
                    Console.WriteLine($"Before: R={pixels[index]}, G={pixels[index + 1]}, B={pixels[index + 2]}");

                    // **提高亮度 60%**
                    pixels[index] *= brightnessFactor;       // R
                    pixels[index + 1] *= brightnessFactor; // G
                    pixels[index + 2] *= brightnessFactor; // B
                                                           // Alpha 通道 (pixels[index + 3]) 保持不變

                    // **🔹 Debug: 再次檢查變更後的數值**
                    Console.WriteLine($"After: R={pixels[index]}, G={pixels[index + 1]}, B={pixels[index + 2]}");

                }
            }
        }

        // SDR Tone Mapping (模擬 HDR 亮度)
        private void ApplyToneMapping(ref float r, ref float g, ref float b)
        {
            r = r / (r + 1);
            g = g / (g + 1);
            b = b / (b + 1);
        }

        public void ImageChange3()
        {
            // **1. 產生白色圖片**
            Mat image = new Mat(height, width, DepthType.Cv8U, 3);
            image.SetTo(new MCvScalar(255, 255, 255)); // 填充白色

            // **2. 提高最左側 (x=0, x=1) 的亮度**
            AdjustLeftmostHDR(image, 100);
            CvInvoke.Imshow("44", image);
            // **3. 轉換 OpenCV Mat → WPF BitmapSource**
            ViewImage.Source = ConvertMatToBitmapSource(image);
        }

        private void AdjustLeftmostHDR(Mat image, int brightnessIncrease)
        {
            // **定義增亮區域**
            System.Drawing.Rectangle roi = new System.Drawing.Rectangle(0, 0, 30, height); // 只修改 x=0 和 x=1

            using (Mat subMat = new Mat(image, roi)) // 擷取區域
            {
                // **使用 ConvertScaleAbs 增加亮度**
                CvInvoke.ConvertScaleAbs(subMat, subMat, 2.0, brightnessIncrease);
            }
        }

        private BitmapSource ConvertMatToBitmapSource(Mat mat)
        {
            Bitmap bmp = mat.ToBitmap();
            IntPtr hBitmap = bmp.GetHbitmap();

            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap, IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return bitmapSource;
        }

    }
}
