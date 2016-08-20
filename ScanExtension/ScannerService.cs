using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WIA;

namespace ScanExtension
{
    public class ScannerException : ApplicationException
    {
        public ScannerException()
            : base()
        { }

        public ScannerException(string message)
            : base(message)
        { }

        public ScannerException(string message, Exception innerException)
            : base(message, innerException)
        { }

    }

    public class ScannerNotFoundException : ScannerException
    {
        public ScannerNotFoundException()
            : base("Error retrieving a list of scanners. Is your scanner or multi-function printer turned on?")
        {
        }
    }

    public class ScannerService
    {
        public ImageFile Scan()
        {
            ImageFile image;

            try
            {
                const string wiaFormatJPEG = "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}";
                //CommonDialog dialog = new CommonDialog();
                WIA.CommonDialog dialog = new WIA.CommonDialog();
                image = dialog.ShowAcquireImage(
                        WiaDeviceType.ScannerDeviceType,
                        WiaImageIntent.ColorIntent,
                        WiaImageBias.MaximizeQuality,
                        wiaFormatJPEG,
                        false,
                        true,
                        false);

                return image;
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode == -2145320939)
                {
                    throw new ScannerNotFoundException();
                }
                else
                {
                    throw new ScannerException("COM Exception", ex);
                }
            }
        }

    }

    public class ScannerImageConverter
    {
        // this could be in the ScannerService, but then that service
        // takes a dependency on WPF, which I didn't want. Better to have
        // the dependencies wrapped into this service instead. Requires
        // FileIOPermission
        public BitmapSource ConvertScannedImage(ImageFile imageFile)
        {
            if (imageFile == null)
                return null;

            // save the image out to a temp file
            string fileName = Path.GetTempFileName();

            // this is pretty hokey, but since SaveFile won't overwrite, we 
            // need to do something to both guarantee a unique name and
            // also allow SaveFile to write the file
            File.Delete(fileName);

            // now save using the same filename
            imageFile.SaveFile(fileName);

            BitmapFrame img;

            // load the file back in to a WPF type, this is just 
            // to get around size issues with large scans
            using (FileStream stream = File.OpenRead(fileName))
            {
                img = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                stream.Close();
            }

            // clean up
            File.Delete(fileName);

            return img;
        }


        // this will choke on large images (like 1200dpi scans)
        // for that reason, you may want to do the conversion by
        // saving the ImageFile to a temp file and then loading it
        // in to convert it, as we do in the revised method above
        public BitmapSource InMemoryConvertScannedImage(ImageFile imageFile)
        {
            if (imageFile == null)
                return null;

            Vector vector = imageFile.FileData;

            if (vector != null)
            {
                byte[] bytes = vector.get_BinaryData() as byte[];

                if (bytes != null)
                {
                    var ms = new MemoryStream(bytes);
                    return BitmapFrame.Create(ms);
                }
            }

            return null;
        }

    }
}
