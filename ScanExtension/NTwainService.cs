using NTwain;
using NTwain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScanExtension
{
    public static class NTwainService
    {
        public static void scan()
        {
            var appId = TWIdentity.CreateFromAssembly(DataGroups.Image, Assembly.GetExecutingAssembly());

            // new it up and handle events
            var session = new TwainSession(appId);

            session.TransferReady += ((t, r) => {

            });
            session.DataTransferred += ((t, r) => {

            });

            // finally open it
            session.Open();

            DataSource myDS = session.FirstOrDefault();
            myDS.Open();


            // All low-level triplet operations are defined through these properties.
            // If the operation you want is not available, that most likely means 
            // it's not for consumer use or it's been abstracted away with an equivalent API in this lib.
            //myDS.DGControl...;
            //myDS.DGImage...;

            // This example sets pixel type of scanned image to BW and
            // IPixelType is the wrapper property on the data source.
            // The name of the wrapper property is the same as the CapabilityId enum.
            PixelType myValue = PixelType.BlackWhite;

            if (myDS.Capabilities.ICapPixelType.CanSet &&
                myDS.Capabilities.ICapPixelType.GetValues().Contains(myValue))
            {
                myDS.Capabilities.ICapPixelType.SetValue(myValue);
            }
            myDS.Enable(SourceEnableMode.ShowUI, true, IntPtr.Zero);

            myDS.Close();
            session.Close();
        }
    }
}
