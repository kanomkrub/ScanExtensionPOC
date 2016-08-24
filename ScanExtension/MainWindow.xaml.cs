
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

            var scriptObj = new ObjectForScriptingHelper(this);
            browser.RegisterJsObject("external", scriptObj, false);

            //browser.Address = @"http://localhost";
            browser.Address = @"http://192.168.1.71";
            browser.Focus();



            //var serviceURI = "http://172.20.36.59:9013/browser/";
            //var repositoryID = "628a5308-0e85-4800-ab1d-244525c905a8";
            //var folderId = "7293cef9-0e73-48d1-8120-1a8e4efeb9cf";
            //var ticket = "85782237-c9f8-4982-86f6-24f7a042f2b4";
            //ScanDialog diag = new ScanDialog(serviceURI, ticket, folderId, repositoryID);
            //diag.ShowDialog();
        }

    }
    public class ObjectForScriptingHelper
    {
        MainWindow mExternalWPF;
        //string serviceURI;

        public ObjectForScriptingHelper(Window w)
        {
            this.mExternalWPF = (MainWindow)w;
            //this.serviceURI = serviceURI;
        }

        public bool InvokeScanDialog(string ticket, string repositoryId, string folderId, string serviceUri)
        {
            bool? diagResult = false;
            mExternalWPF.Dispatcher.Invoke(() =>
            {
                var diag = new ScanDialog(serviceUri, ticket, folderId, repositoryId);
                diagResult = diag.ShowDialog();
            });
            return diagResult.GetValueOrDefault();
        }
        
    }
}
