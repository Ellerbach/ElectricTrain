using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

namespace ElectricTrain
{
    public enum TrainDirection { Forward = 0, Backward = 1, Stop = 2 }

    class Train
    {
        static IStream connection;
        static RemoteDevice arduino;
        public static bool IsInitialized { get; internal set; }
        public static bool IsConnected { get; internal set; }
        public int NumberTrains { get; internal set; }
        static public int NUMBER_TRAIN_MAX { get { return 4; } }
        //for the PWM 
        private byte[] PWMPins = { 3, 5, 6, 9 };
        //for the left direction
        private byte[] LeftPins = { 2, 4, 7, 8 };
        //for the right direction
        private byte[] RightPins = { 10, 11, 12, 13 };

        public byte[] Speed = new byte[NUMBER_TRAIN_MAX];

        public Train(byte NumTrains)
        {
            NumberTrains = NumTrains;
            if (NumberTrains > NUMBER_TRAIN_MAX)
                throw new Exception($"Too many trains, max is {NUMBER_TRAIN_MAX}");
            if (!IsInitialized)
                InitAll().Wait();
            
        }


        private async Task InitAll()
        {
            IsInitialized = true;
            //connection = new UsbSerial("VID_0403", "PID_6001");
            string aqs = SerialDevice.GetDeviceSelector();
            var dis = await DeviceInformation.FindAllAsync(aqs);
            for (int i = 0; i < dis.Count; i++)
            {
                Debug.WriteLine(string.Format("Serial device found: {0}", dis[i].Id));
                if (dis[i].Id.IndexOf("UART0") == -1)
                {
                    connection = new UsbSerial(dis[i]);
                    arduino = new RemoteDevice(connection);
                    connection.ConnectionEstablished += Connection_ConnectionEstablished; ;
                    connection.begin(115200, SerialConfig.SERIAL_8N1);
                }
            }

        }

        private void Connection_ConnectionEstablished()
        {
            IsConnected = true;
            Debug.Write("Connected to Arduino!");
            //initialize all the Pins
            for(int i = 0; i<PWMPins.Length;i++)
            {
                arduino.pinMode(PWMPins[i], PinMode.PWM);
                arduino.pinMode(LeftPins[i], PinMode.OUTPUT);
                arduino.pinMode(RightPins[i], PinMode.OUTPUT);
            }
        }

        public void SetSpeed(byte train, byte speed)
        {
            if(train>=NUMBER_TRAIN_MAX)
                throw new Exception($"Too many trains, max is {NUMBER_TRAIN_MAX}");
            if (speed > 100)
                speed = 100;
            Speed[train] = speed;
            arduino.analogWrite(PWMPins[train], (ushort)(speed * 255 / 100));
        }

        public byte GetSpeed(byte train)
        {
            if (train >= NUMBER_TRAIN_MAX)
                throw new Exception($"Too many trains, max is {NUMBER_TRAIN_MAX}");
            return Speed[train];
        }

        public void SetDirection(byte train, TrainDirection dir)
        {
            switch (dir)
            {
                case TrainDirection.Forward:
                    arduino.digitalWrite(LeftPins[train], PinState.HIGH);
                    arduino.digitalWrite(RightPins[train], PinState.LOW);
                    break;
                case TrainDirection.Backward:
                    arduino.digitalWrite(LeftPins[train], PinState.LOW);
                    arduino.digitalWrite(RightPins[train], PinState.HIGH);
                    break;
                case TrainDirection.Stop:
                    arduino.digitalWrite(LeftPins[train], PinState.LOW);
                    arduino.digitalWrite(RightPins[train], PinState.LOW);
                    break;
                default:
                    break;
            }
        }
    }
}
