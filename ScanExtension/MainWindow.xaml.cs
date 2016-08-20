
using CefSharp;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WIA;
using ZXing;

namespace ScanExtension
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeCEF();
            InitializeComponent();
            SetAddress();
        }
        public void InitializeCEF()
        {
            //Perform dependency check to make sure all relevant resources are in output directory.
            var settings = new CefSettings();
            settings.EnableInternalPdfViewerOffScreen();
            // Disable GPU in WPF and Offscreen examples until #1634 has been resolved
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            var initialized = Cef.Initialize(settings, shutdownOnProcessExit: true, performDependencyCheck: false);
        }

        private void SetAddress()
        {

            var scriptObj = new ObjectForScriptingHelper(this, @"http://172.20.36.59:9012");
            scriptObj.InvokeScanDialog("", 2);
            browser.RegisterJsObject("external", scriptObj, false);

            browser.Address = @"http://localhost";
            browser.Focus();

            ScanDialog diag = new ScanDialog();
            diag.ShowDialog();
        }

    }
    public class ObjectForScriptingHelper
    {
        MainWindow mExternalWPF;
        string serviceURI;

        public ObjectForScriptingHelper(Window w, string serviceURI)
        {
            this.mExternalWPF = (MainWindow)w;
            this.serviceURI = serviceURI;
        }

        public void InvokeScanDialog(string v1, int v2)
        {
            
        }
        
    }
}
