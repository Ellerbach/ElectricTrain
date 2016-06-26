using IoTCoreHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;

namespace ElectricTrain
{
    class Switch
    {
        public int NumberSwitches { get; internal set; }
        static public int NUMBER_SWITCH_MAX { get { return 8; } }
        //private variables
        private bool[] mSwitchStatus;        
        private const Int32 SPI_CHIP_SELECT_LINE = 0;       /* Line 0 maps to physical pin number 24 on the Rpi2        */
        private SpiDevice MySwitch;

        public Switch(int numberSwitches)
        {
            NumberSwitches = numberSwitches;
            if ((NumberSwitches <= 0) && (NumberSwitches > NUMBER_SWITCH_MAX))
                new Exception("Not correct number of Switches");
            mSwitchStatus = new bool[NumberSwitches];
            InitSPI();
        }

        private async void InitSPI()
        {
            var settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
            settings.ClockFrequency = 650000;
            settings.SharingMode = SpiSharingMode.Shared;
            settings.Mode = SpiMode.Mode2; // should be either 1 or 2
            string aqs = SpiDevice.GetDeviceSelector();                     /* Get a selector string that will return all SPI controllers on the system */
            var dis = await DeviceInformation.FindAllAsync(aqs);            /* Find the SPI bus controller devices with our selector string             */
            if (dis.Count == 0)
            {
                Debug.WriteLine("SPI does not exist on the current system.");
                return;
            }

            MySwitch = await SpiDevice.FromIdAsync(dis[0].Id, settings);    /* Create an SpiDevice with our bus controller and SPI settings             */
            if (MySwitch == null)
            {
                Debug.WriteLine("No SPI device present on the system");
                return;
            }
            //initialise all signals to "false"
            for (byte i = 0; i < NumberSwitches; i++)
                ChangeSwitch(i, true);

        }

        public void ChangeSwitch(byte NumSwitch, bool value)
        {
            if ((NumSwitch <= 0) && (NumSwitch > NumberSwitches))
                new Exception("Not correct number of Signals");
            //need to convert to select the right Signal
            mSwitchStatus[NumSwitch] = value;
            // fill the buffer to be sent
            ushort[] mySign = new ushort[1] { 0 };
            for (ushort i = 0; i < NumberSwitches; i++)
                if (mSwitchStatus[i])
                    if (mSwitchStatus[i])
                        mySign[0] = (ushort)(mySign[0] | (ushort)(1 << i * 2));
                    else
                        mySign[0] = (ushort)(mySign[0] | (ushort)(1 << (i * 2 + 1)));
            //send the bytes
            if (MySwitch != null)
                MySwitch.Write(Helpers.UshortToByte(mySign));
            //wait 2 seconds and reset all
            Task.Delay(2000).Wait();
            mySign[0] = 0;
            MySwitch.Write(Helpers.UshortToByte(mySign));
        }

        public bool GetSwitch(byte NumSwitch)
        {
            if ((NumSwitch <= 0) && (NumSwitch > NumberSwitches))
                new Exception("Not correct number of Signals");
            // need to convert to BCD
            return mSwitchStatus[NumSwitch];
        }
    }
}
