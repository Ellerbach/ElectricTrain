using Devkoes.Restup.WebServer.Attributes;
using Devkoes.Restup.WebServer.Models.Schemas;
using ElectricTrain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricTrain.Controller
{
    [RestController(InstanceCreationType.Singleton)]
    partial class TrainManagement
    {
        static private Signal mySignal;
        static private Switch mySwitch;
        static private ParamRail myParamRail;
        static private Train myTrain;

        public static async Task InitTrain()
        {
            myParamRail = await LoadParamRail();
            MySecurityKey = myParamRail.SecurityKey;
            securityKey = paramSecurityKey + ParamEqual + MySecurityKey;
            mySignal = new Signal(myParamRail.NumberOfSignals);
            mySwitch = new Switch(myParamRail.NumberOfSwitchs);
            myTrain = new Train((byte)myParamRail.NumberOfTrains);

        }
        private bool SecCheck(string strFilePath)
        {
            if (strFilePath.IndexOf(securityKey) == -1)
                return false;
            return true;

        }

        private string ErrorAuth()
        {
            string strResp = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">";
            strResp += "<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>Gestion train</title>";
            strResp += "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/></head>";
            strResp += "<meta http-equiv=\"Cache-control\" content=\"no-cache\"/>";
            strResp += "<meta http-equiv=\"EXPIRES\" content=\"0\" />";
            strResp += "<BODY><h1>RaspberryPi2 Lego Train running Windows 10</h1><p>";
            strResp += "Invalid security key</body></html>";
            return strResp;
        }

        [UriFormat("/switch.aspx{param}")]
        public GetResponse Switch(string param)
        {
            if (!SecCheck(param))
                return new GetResponse(GetResponse.ResponseStatus.OK, ErrorAuth());
            return ProcessSwitch(param);
        }

        [UriFormat("/default.aspx{param}")]
        public GetResponse Default(string param)
        {
            if (!SecCheck(param))
                return new GetResponse(GetResponse.ResponseStatus.OK, ErrorAuth());
            return ProcessDisplayDefault(param);
        }
        
        [UriFormat("/train.aspx{param}")]
        public GetResponse Train(string param)
        {
            if (!SecCheck(param))
                return new GetResponse(GetResponse.ResponseStatus.OK, ErrorAuth());
            return ProcessTrain(param);
        }

        [UriFormat("/signal.aspx{param}")]
        public GetResponse Signal(string param)
        {
            if (!SecCheck(param))
                return new GetResponse(GetResponse.ResponseStatus.OK, ErrorAuth());
            return ProcessSignal(param);
        }

        [UriFormat("/circ.aspx{param}")]
        public GetResponse Circuit(string param)
        {
            if (!SecCheck(param))
                return new GetResponse(GetResponse.ResponseStatus.OK, ErrorAuth());
            return ProcessCircuit(param);
        }

    }
}
