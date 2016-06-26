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
    class Signal
    {
        public int NumberSignals { get; internal set; }
        static public int NUMBER_SIGNAL_MAX { get { return 8; } }
        //private variables
        private bool[] mSignalStatus;
        private const Int32 SPI_CHIP_SELECT_LINE = 1;       /* Line 1 maps to physical pin number 26 on the Rpi2        */
        private SpiDevice MySignal;

        public Signal(int numberSignals)
        {
            NumberSignals = numberSignals;
            if ((NumberSignals <= 0) && (NumberSignals > NUMBER_SIGNAL_MAX))
                new Exception("Not correct number of Switches");
            mSignalStatus = new bool[NumberSignals];
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

            MySignal = await SpiDevice.FromIdAsync(dis[0].Id, settings);    /* Create an SpiDevice with our bus controller and SPI settings             */
            if (MySignal == null)
            {
                Debug.WriteLine("No SPI device present on the system");
                return;
            }
            //initialise all signals to "false"
            for (byte i = 0; i < NumberSignals; i++)
                ChangeSignal(i, true);

        }

        public void ChangeSignal(byte NumSignal, bool value)
        {
            if ((NumSignal <= 0) && (NumSignal > NumberSignals))
                new Exception("Not correct number of Signals");
            //need to convert to select the right Signal
            mSignalStatus[NumSignal] = value;
            // fill the buffer to be sent
            ushort[] mySign = new ushort[1] { 0 };
            for (ushort i = 0; i < NumberSignals; i++)
                if (mSignalStatus[i])
                    mySign[0] = (ushort)(mySign[0] | (ushort)(1 << i*2));
            else
                    mySign[0] = (ushort)(mySign[0] | (ushort)(1 << (i * 2 +1)));
            //send the bytes
            if (MySignal != null)
                MySignal.Write(Helpers.UshortToByte(mySign));
        }

        public bool GetSignal(byte NumSignal)
        {
            if ((NumSignal <= 0) && (NumSignal > NumberSignals))
                new Exception("Not correct number of Signals");
            // need to convert to BCD
            return mSignalStatus[NumSignal];
        }

    }
}
