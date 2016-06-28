using Devkoes.Restup.WebServer.Models.Schemas;
using ElectricTrain.Models;
using IoTCoreHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricTrain.Controller
{
    partial class TrainManagement
    {
        private GetResponse ProcessTrain(string rawURL)
        {
            List<Param> Params = Param.decryptParam(rawURL);
            string strResp = "";
            int mSpeed = Param.CheckConvertInt32(Params, paramSinglePWM);
            byte mTrain = Param.CheckConvertByte(Params, paramTrain);

            if(mTrain>=myTrain.NumberTrains)
            {
                strResp = strProblem;
                return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
            }
            if (mSpeed == 255)
                myTrain.SetSpeed(mTrain, 0);
            else
            {
                int speed = myTrain.Speed[mTrain];
                if (Math.Abs(mSpeed) == 1)
                {
                    speed += mSpeed * 5;
                }
                else
                    speed = mSpeed;
                myTrain.SetSpeed(mTrain, speed);
            }
            strResp = strOK;
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }
        private GetResponse ProcessSwitch(string rawURL)
        {
            // decode params
            List<Param> Params = Param.decryptParam(rawURL);
            string strResp = "";
            //check if Params contains anything and is valid
            if (Params == null)
                return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
            if (Params.Count == 0)
                return new GetResponse(GetResponse.ResponseStatus.OK, strResp);

            bool bNoUI = Param.CheckConvertBool(Params, paramNoUI);
            byte mSignalSwitch = Param.CheckConvertByte(Params, paramSignalSwitch);
            if (mSignalSwitch >= mySwitch.NumberSwitches)
                mSignalSwitch = 255;
            bool bValue = Param.CheckConvertBool(Params, paramMode);

            if (!bNoUI)
            {
                strResp = "<HTML><BODY>Status of Switch<p>";
                if (mSignalSwitch == 255)
                    for (byte i = 0; i < mySwitch.NumberSwitches; i++)
                    {
                        strResp += "Switch " + i + ": " + mySwitch.GetSwitch(i) + "</br>";
                    }
                else
                {
                    //change the value
                    mySwitch.ChangeSwitch(mSignalSwitch, bValue);
                    strResp += "Switch " + mSignalSwitch + ": " + bValue + "</br>";
                }
                strResp += "</p></BODY></HTML>";
            }
            else
            {
                if (mSignalSwitch == 255)
                    for (byte i = 0; i < mySwitch.NumberSwitches; i++)
                    {
                        if (mySwitch.GetSwitch(i))
                            strResp += "1";
                        else
                            strResp += "0";
                    }
                else
                {
                    //change status
                    mySwitch.ChangeSwitch(mSignalSwitch, bValue);
                    strResp += strOK;
                }

            }
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }

        private GetResponse ProcessDisplayDefault(string rawUrl)
        {
            string strResp = "";
            // Start HTML document
            strResp += "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">";
            strResp += "<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>Gestion des trains</title>";
            //this is the css to make it nice :-)
            strResp += "<link href=\"file/" + pageCSS + "\" rel=\"stylesheet\" type=\"text/css\" />";
            strResp += "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/></head><body>";
            strResp += "<meta http-equiv=\"Cache-control\" content=\"no-cache\"/>";
            strResp += "<meta http-equiv=\"EXPIRES\" content=\"0\" />";
            //create the script part
            strResp += "<SCRIPT language=\"JavaScript\">";
            strResp += "var xhr = new XMLHttpRequest(); function btnclicked(boxMSG, cmdSend) { boxMSG.innerHTML=\"Waiting\";";
            strResp += "xhr.open('GET', cmdSend + '&" + securityKey + "&_=' + Math.random());";
            strResp += "xhr.onreadystatechange = function() {if (xhr.readyState == 4) {boxMSG.innerHTML=xhr.responseText;}}; xhr.send();}";
            strResp += "</SCRIPT>";
            //strResp = WebServer.OutPutStream(response, strResp);
            // Create one section for each train
            strResp += "<h1>Gestion des trains</h1><TABLE BORDER=\"0\" >";
            for (byte i = 0; i < myParamRail.NumberOfTrains; i++)
            {
                strResp += "<TR><FORM><TD>" + myParamRail.Trains[i].TrainName + "</TD><TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageTrain + "?pw=" + (- myParamRail.Trains[i].Speed) + "&tr=" + i;
                strResp += "')\" value=\"<<\"></TD>";
                strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageTrain + "?pw=-1" + "&tr=" + i;
                strResp += "')\" value=\"<\"></TD>";
                strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageTrain + "?pw=255" + "&tr=" + i;
                //strResp = WebServer.OutPutStream(response, strResp);
                strResp += "')\" value=\"Stop\"></TD>";
                strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageTrain + "?pw=1" + "&tr=" + i;
                strResp += "')\" value=\">\"></TD>";
                strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageTrain + "?pw=" + myParamRail.Trains[i].Speed + "&tr=" + i;
                strResp += "')\" value=\">>\"></TD>";
                strResp += "<TD><span id='train" + i + "'></span></FORM></TD></TR>";
                //strResp = WebServer.OutPutStream(response, strResp);
            }

            for (byte i = 0; i < myParamRail.NumberOfSwitchs; i++)
            {
                strResp += "<TR><FORM><TD>" + myParamRail.Switchs[i].Name + "</TD><TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('switch" + i + "'),'" + pageSwitch + "?si=" + i + "&md=0&no=1')\" value=\"|\"></TD>";
                strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('switch" + i + "'),'" + pageSwitch + "?si=" + i + "&md=1&no=1')\" value=\"/\"></TD>";
                strResp += "<TD><span id='switch" + i + "'></span></FORM></TD></TR>";
            }
            strResp += "</TABLE><br><a href='all.aspx?" + securityKey + "'>Display all page</a><br><a href='circ.aspx?" + securityKey + "'>Circuit</a>";
            strResp += "</body></html>";
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }
    }
}
