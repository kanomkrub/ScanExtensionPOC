using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace ScanExtension
{
    /// <summary>
    /// Interaction logic for ScanDialog.xaml
    /// </summary>
    public partial class ScanDialog : Window
    {
        public BitmapSource ScannedImage { get; set; }
        public string serviceURI { get; set; }
        public string ticket { get; set; }
        public string folderId { get; set; }
        public string repositoryId { get; set; }
        public ScanDialog(string serviceUri, string ticket, string folderId, string repositoryId)
        {
            InitializeComponent();
            this.serviceURI = serviceUri + "/browser/";
            this.ticket = ticket;
            this.folderId = folderId;
            this.repositoryId = repositoryId;
        }

        public static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        private void scanButton_Click(object sender, RoutedEventArgs e)
        {
            setPropPanel.IsEnabled = true;
            var Handle = new WindowInteropHelper(this).Handle;

            var gdpicture = new GdPicture11.GdPictureImaging();
            var tempFile = System.IO.Path.GetTempFileName();
            var ids = GDScanService.Scan(Handle);

            //if (ids.Count > 0) stack1.Children.RemoveAt(0);

            for (int i = 0; i < ids.Count; i++)
            {
                //gdpicture.SelectPage(imageId, i);

                byte[] imageBytes = null;
                int lengths = 0;
                gdpicture.SaveAsByteArray(ids[i], ref imageBytes, ref lengths, GdPicture11.DocumentFormat.DocumentFormatJPEG, 60);
                var bitmapSource = (BitmapSource)new ImageSourceConverter().ConvertFrom(imageBytes);
                stack1.Children.Add(new System.Windows.Controls.Image() { Source = bitmapSource });
                stack1.Children.Add(new Separator());
                stack1.Children.Add(new Separator());
                
                gdpicture.ReleaseGdPictureImage(ids[i]);
                
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fileNameTextBox.Text))
            {
                MessageBox.Show("FileName required");
                return;
            }
            var tempFile = System.IO.Path.GetTempPath() + fileNameTextBox.Text.Replace(".pdf","") + ".pdf";
            using (var gdPdf = new GdPicture11.GdPicturePDF())
            {
                gdPdf.NewPDF();
                foreach (UIElement item in stack1.Children)
                {
                    if (item.GetType() != typeof(System.Windows.Controls.Image)) continue;
                    var bSource = (BitmapSource)((System.Windows.Controls.Image)item).Source;
                    using (var bitmap = BitmapFromSource(bSource))
                    {
                        gdPdf.AddImageFromBitmap(bitmap, true, true);
                    }
                }
                gdPdf.SaveToFile(tempFile);
            }

            //tempFile = @"D:\tessj.jpg";
            var docType = "MEA01";
            var properties = new Dictionary<string, string>();
            properties.Add("MEA_metadata01", prop1.Text);
            properties.Add("MEA_metadata02", prop2.Text);
            properties.Add("MEA_metadata03", prop3.Text);
            properties.Add("MEA_metadata04", prop4.Text);
            properties.Add("MEA_metadata05", prop4.Text);
            properties.Add("MEA_metadata06", prop4.Text);
            var mediaType = "application/pdf";
            ExportService.Export(tempFile, ticket, mediaType, docType, properties, serviceURI, repositoryId, folderId);
            try { File.Delete(tempFile); } catch (Exception ex) { }
            this.DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach(UIElement item in stack1.Children)
            {
                try
                {
                    if (item.GetType() != typeof(System.Windows.Controls.Image)) continue;
                    var bSource = (BitmapSource)((System.Windows.Controls.Image)item).Source;
                    using (var bitmap = BitmapFromSource(bSource))
                    {
                        var reader = new QRCodeReader();
                        LuminanceSource source = new BitmapLuminanceSource(bitmap);
                        var binarizer = new HybridBinarizer(source);
                        var binBitmap = new BinaryBitmap(binarizer);
                        BitMatrix bm = binBitmap.BlackMatrix;
                        Detector detector = new Detector(bm);
                        var hintDic = new Dictionary<DecodeHintType, object>();
                        hintDic.Add(DecodeHintType.TRY_HARDER, true);
                        hintDic.Add(DecodeHintType.CHARACTER_SET, "utf-8");
                        DetectorResult dtectResult = detector.detect(hintDic);
                        QRResultStack.Children.Clear();
                        
                        var retStr = "Barcode detected at ";
                        if (dtectResult != null)
                        {
                            foreach (ResultPoint point in dtectResult.Points)
                            {
                                retStr += point.ToString() + ", ";
                            }
                            var result = reader.decode(binBitmap, hintDic);
                            var qrText = "";
                            if (result == null)
                            {
                                qrText = "Failed to decode..., Please re-scan document.";
                                QRResultStack.Children.Insert(0, new TextBox() { Text = retStr, IsReadOnly = true, Foreground = System.Windows.Media.Brushes.DarkOrange, MinLines=2, MaxWidth = 300, TextWrapping = TextWrapping.Wrap });
                                QRResultStack.Children.Add(new TextBox() { Text = qrText, IsReadOnly = true, MinLines = 3, TextWrapping = TextWrapping.Wrap, MaxWidth=300 });

                            }
                            else
                            {
                                qrText = result.Text;
                                QRResultStack.Children.Insert(0, new TextBox() { Text = retStr, IsReadOnly = true, Foreground = System.Windows.Media.Brushes.DarkGreen, MinLines = 2, MaxWidth = 300, TextWrapping = TextWrapping.Wrap });
                                QRResultStack.Children.Add(new TextBox() { Text = qrText, IsReadOnly = true, MinLines = 2, TextWrapping = TextWrapping.Wrap, MaxWidth = 300 });

                                qrExpander.IsExpanded = true;
                                //set metadata


                                this.propertyStack.Children.Clear();

                                var props = qrText.Split('|');
                                prop1.Text = props.Length > 0 ? props[0] : "";
                                prop2.Text = props.Length > 1 ? props[1] : "";
                                prop3.Text = props.Length > 2 ? props[2] : "";
                                prop4.Text = props.Length > 3 ? props[3] : "";
                                
                                break;
                            }
                            
                        }
                        else
                        {
                            retStr = "Barcode not found please re-scan the document.";
                            QRResultStack.Children.Add(new TextBox() { Text = retStr, IsReadOnly = true, Foreground = System.Windows.Media.Brushes.DarkOrange, MinLines = 2 });
                        }
                        qrExpander.IsExpanded = true;
                    }
                }
                catch(Exception ex)
                {

                }
            }
        }
        
    }
}
