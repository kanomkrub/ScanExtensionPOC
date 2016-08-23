﻿using GdPicture11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ScanExtension
{
    public static class GDScanService
    {
        public static List<int> Scan(IntPtr handle)
        {
            new LicenseManager().RegisterKEY("0446235468463336153471348");
            var gdPicture = new GdPictureImaging();

            //gdPicture.TwainSetDSMPath("TWAIN_32.dll"); //force use twain dsm 1.9 (default 2.2)

            if (!gdPicture.TwainIsAvailable()) { MessageBox.Show("Twain not available on this machine. please check your twain driver."); }
            //var Handle = new WindowInteropHelper(this).Handle;

            //var Handle = IntPtr.Zero;
            var count = gdPicture.TwainGetSourceCount(handle);
            var sourceName = gdPicture.TwainGetSourceName(handle, 1);
            //var opened = gdPicture.TwainOpenDefaultSource(handle);
            //var major = 0;
            //var minor = 0;
            //var language = TwainLanguage.TWLG_ENGLISH;
            //var country = TwainCountry.TWCY_CHINA;
            //string info = null;
            //gdPicture.TwainGetSourceVersionInfo(handle, 1, ref major, ref minor, ref language, ref country, ref info);
            //var text = string.Format("{0}", gdPicture.TwainGetSourceName(handle, 1), major, minor);
            //gdPicture.TwainSetAutomaticColor(true);
            
            var x = gdPicture.TwainGetState();
            var o = gdPicture.TwainHasFeeder();
            gdPicture.TwainOpenSource(0, sourceName);
            //var s = gdPicture.TwainOpenDefaultSource(handle);
            //var opened = gdPicture.TwainOpenSource(handle, sourceName);
            //gdPicture.twain
            var online = gdPicture.TwainIsDeviceOnline();
            gdPicture.TwainSetAutomaticColor(false);
            //gdPicture.TwainSelectFeeder(true);
            //gdPicture.TwainSetAutoFeed(true);
            var maxRes = gdPicture.TwainGetAvailableXResolutionCount();
            gdPicture.TwainSetResolution(300);
            gdPicture.TwainSetPaperSize(TwainPaperSize.A4);
            //gdPicture.TwainSetMultiTransfer(true);
            //gdPicture.twainset
            gdPicture.TwainSetAutoFeed(false);
            gdPicture.TwainSetHideUI(true);
            //var imageId = gdPicture.TwainAcquireToGdPictureImage(handle);
            var loaded = gdPicture.TwainIsFeederLoaded();
            //var tempFile = System.IO.Path.GetTempFileName();
            //gdPicture.TwainAcquireToFile(tempFile, handle);
            List<int> imageIds = new List<int>();
            while (gdPicture.TwainIsFeederLoaded())
            {
                var imageId = gdPicture.TwainAcquireToGdPictureImage(handle);
                imageIds.Add(imageId);
            } 
            gdPicture.TwainCloseSource();
            //gdPicture.ReleaseGdPictureImage(imageId);
            return imageIds;
        }
    }
}
