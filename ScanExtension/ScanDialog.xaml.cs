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
using System.Windows.Interop;
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
            setPropPanel.IsEnabled = true;
            var Handle = new WindowInteropHelper(this).Handle;

            var imageId = GDScanService.Scan(Handle);

            //var scanService = new ScannerService();
            //try
            //{
            //    ImageFile imageFile = scanService.Scan();

            //    if (imageFile != null)
            //    {
            //        var converter = new ScannerImageConverter();

            //        ScannedImage = converter.ConvertScannedImage(imageFile);
            //        IBarcodeReader reader = new BarcodeReader();
            //        var barcodeBitmap = GetBitmap(ScannedImage);


            //var img = new System.Windows.Controls.Image();
            var gdpicture = new GdPicture11.GdPictureImaging();
            //gdpicture.SaveAsTIFF(imageId, System.IO.Path.GetTempFileName(), GdPicture11.TiffCompression.TiffCompressionJPEG, 60);

            //gdpicture.SaveAsByteArray(imageId, )

            stack1.Children.RemoveAt(0);
            stack1.Children.Add(img);

            //    }
            //}
            //catch (Exception ex) { }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            var filePath = @"D:\tessj.jpg";
            var docType = "MEAPOC";
            var serviceURI = "http://172.20.36.59:9013/browser/";
            var repositoryID = "628a5308-0e85-4800-ab1d-244525c905a8";
            var folderId = "7293cef9-0e73-48d1-8120-1a8e4efeb9cf";
            var ticket = "85782237-c9f8-4982-86f6-24f7a042f2b4";
            var properties = new Dictionary<string, string>();
            properties.Add("QR1", "value1");
            properties.Add("QR2", "value2");
            properties.Add("QR3", "value3");
            properties.Add("QR4", "value4");
            var mediaType = "application/pdf";
            ExportService.Export(filePath, ticket, mediaType, docType, properties, serviceURI, repositoryID, folderId);

            this.DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
