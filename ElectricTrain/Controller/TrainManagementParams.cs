using ElectricTrain.Models;
using IoTCoreHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ElectricTrain.Controller
{
    partial class TrainManagement
    {
        async static Task<string> GetFilePath(string filename)
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                //var files = await localFolder.GetFilesAsync();
                //StorageFile file = files.FirstOrDefault(x => x.Name == filename);
                var file = await localFolder.GetFileAsync(filename);
                if (file != null)
                    return file.Path;

            }
            catch (Exception)
            {
            }
            return "";
        }

        static private async Task<ParamRail> LoadParamRail()
        {
            FileStream fileToRead = null;
            ParamRail myParamRail = new ParamRail();
            try
            {
                //fileToRead = new FileStream(strDefaultDir + "\\" + strFileProgram, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fileToRead = new FileStream(await GetFilePath(strFileProgram), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                long fileLength = fileToRead.Length;

                byte[] buf = new byte[fileLength];
                //string mySetupString = "";

                // Reads the data.
                fileToRead.Read(buf, 0, (int)fileLength);
                //await str.ReadAsync(buf,  )
                // convert the read into a string

                List<Param> Params = Param.decryptParam(new String(Encoding.UTF8.GetChars(buf)));
                byte mSignal = byte.MaxValue;
                byte mSwitch = byte.MaxValue;
                byte mTrains = byte.MaxValue;
                if (Params == null)
                    return myParamRail;
                if (Params.Count == 0)
                    return myParamRail;

                mSignal = Param.CheckConvertByte(Params, paramNumberSignal);
                if ((mSignal <= 0) || (mSignal > ElectricTrain.Signal.NUMBER_SIGNAL_MAX))
                    mSignal = (byte)ElectricTrain.Signal.NUMBER_SIGNAL_MAX;
                mSwitch = Param.CheckConvertByte(Params, paramNumberSwitch);
                if ((mSwitch <= 0) || (mSwitch > ElectricTrain.Switch.NUMBER_SWITCH_MAX))
                    mSwitch = (byte)ElectricTrain.Switch.NUMBER_SWITCH_MAX;
                mTrains = Param.CheckConvertByte(Params, paramNumberTrain);
                if ((mTrains <= 0) || (mTrains > ParamTrain.NUMBER_TRAIN_MAX))
                    mTrains = (byte)ParamTrain.NUMBER_TRAIN_MAX;

                myParamRail.SecurityKey = Param.CheckConvertString(Params, paramSecurity);

                //now load the params for the trains
                if (mTrains != 255)
                {
                    myParamRail.NumberOfTrains = mTrains;
                    myParamRail.Trains = new ParamTrain[mTrains];
                    for (byte a = 1; a <= mTrains; a++)
                    {
                        myParamRail.Trains[a - 1] = new ParamTrain();
                        byte mSpeed = Param.CheckConvertByte(Params, paramTrainSpeed + a.ToString());
                        if (mSpeed > 7)
                            mSpeed = 0;
                        myParamRail.Trains[a - 1].TrainName = Param.CheckConvertString(Params, paramTrainName + a.ToString());
                        myParamRail.Trains[a - 1].Speed = mSpeed;
                    }
                }

                if (mSignal != 255)
                    myParamRail.NumberOfSignals = mSignal;
                if (mSwitch != 255)
                {
                    myParamRail.NumberOfSwitchs = mSwitch;
                    myParamRail.Switchs = new ParamSwitchs[mSwitch];
                    for (byte a = 1; a <= mSwitch; a++)
                    {
                        myParamRail.Switchs[a - 1] = new ParamSwitchs();
                        string mName = Param.CheckConvertString(Params, paramNameSwitch + a.ToString());
                        if (mName == "")
                            mName = "Switch " + a.ToString();
                        myParamRail.Switchs[a - 1].Name = mName;
                        myParamRail.Switchs[a - 1].Left = Param.CheckConvertInt32(Params, paramleft + a.ToString());
                        myParamRail.Switchs[a - 1].Top = Param.CheckConvertInt32(Params, paramtop + a.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                if (fileToRead != null)
                {
                    fileToRead.Dispose();
                }
            }
            return myParamRail;
        }
    }
}
