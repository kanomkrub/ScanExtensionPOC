using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WIA;
using ZXing;

namespace ScanExtension
{
    /// <summary>
    /// Interaction logic for ScanDialog.xaml
    /// </summary>
    public partial class ScanDialog : Window
    {
        public BitmapSource ScannedImage { get; set; }
        public ScanDialog()
        {
            InitializeComponent();
        }
        public void Scan()
        {
            var scanner = new ScannerService();
            try
            {
                ImageFile file = scanner.Scan();
                string QrCodeString;
                if (file != null)
                {
                    var converter = new ScannerImageConverter();

                    ScannedImage = converter.ConvertScannedImage(file);

                    IBarcodeReader reader = new BarcodeReader();
                    var barcodeBitmap = (Bitmap)GetBitmap(ScannedImage);

                    var result = reader.Decode(barcodeBitmap);
                    if (result != null)
                    {
                        QrCodeString = result.Text;
                    }
                }
                else
                {
                    ScannedImage = null;
                }

            }
            catch (ScannerException ex)
            {
                // yeah, I know. Showing UI from the VM. Shoot me now.
                MessageBox.Show(ex.Message, "Unable to Scan Image");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        Bitmap GetBitmap(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(source.PixelWidth, source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        private void scanButton_Click(object sender, RoutedEventArgs e)
        {
            var scanService = new ScannerService();
            try
            {
                ImageFile imageFile = scanService.Scan();

                if (imageFile != null)
                {
                    var converter = new ScannerImageConverter();
                    
                    ScannedImage = converter.ConvertScannedImage(imageFile);
                    IBarcodeReader reader = new BarcodeReader();
                    var barcodeBitmap = GetBitmap(ScannedImage);


                    var img = new System.Windows.Controls.Image(); 
                    img.Source = ScannedImage;

                    stack1.Children.RemoveAt(0);
                    stack1.Children.Add(img);

                }
            }
            catch (Exception) { }
        }
    }
}
