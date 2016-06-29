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
        private GetResponse ProcessSignal(string rawURL)
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
            if (mSignalSwitch >= mySignal.NumberSignals)
                mSignalSwitch = 255;
            bool bValue = Param.CheckConvertBool(Params, paramMode);

            if (!bNoUI)
            {
                strResp = "<HTML><BODY>Status of Signals<p>";
                if (mSignalSwitch == 255)
                    for (byte i = 0; i < mySignal.NumberSignals; i++)
                    {
                        strResp += "Signal " + i + ": " + mySignal.GetSignal(i) + "</br>";
                    }
                else
                {
                    //change the value
                    mySignal.ChangeSignal(mSignalSwitch, bValue);
                    strResp += "Signal " + mSignalSwitch + ": " + bValue + "</br>";
                }
                strResp += "</p></BODY></HTML>";
            }
            else
            {
                if (mSignalSwitch == 255)
                    for (byte i = 0; i < mySignal.NumberSignals; i++)
                    {
                        if (mySignal.GetSignal(i))
                            strResp += "1";
                        else
                            strResp += "0";
                    }
                else
                {
                    //change status
                    mySignal.ChangeSignal(mSignalSwitch, bValue);
                    strResp += strOK;
                }

            }
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }
        private GetResponse ProcessTrain(string rawURL)
        {
            List<Param> Params = Param.decryptParam(rawURL);
            string strResp = "";
            int mSpeed = Param.CheckConvertInt32(Params, paramSinglePWM);
            byte mTrain = Param.CheckConvertByte(Params, paramTrain);

            if (mTrain >= myTrain.NumberTrains)
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

        private GetResponse ProcessCircuit(string param)
        {
            string strResp = "";
            // Start HTML document
            strResp += "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">";
            strResp += "<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>Gestion des trains</title>";
            //this is the css to make it nice :-)
            strResp += "<link href=\"file/" + pageCSS + "\" rel=\"stylesheet\" type=\"text/css\" />";
            strResp += "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/></head><body onLoad=\"InitAll();\">";
            strResp += "<meta http-equiv=\"Cache-control\" content=\"no-cache\"/>";
            //create the script part
            strResp += "<SCRIPT language=\"JavaScript\">";
            strResp += "function btnclicked(boxMSG, cmdSend) { var xhr = new XMLHttpRequest(); boxMSG.innerHTML=\"Waiting\";";
            strResp += "xhr.open('GET', cmdSend + '&" + securityKey + "&_=' + Math.random());";
            strResp += "xhr.send(null); xhr.onreadystatechange = function() {if (xhr.readyState == 4) {boxMSG.innerHTML=xhr.responseText;}};}";

            strResp += "function findTop(iobj) { ttop = 0; while (iobj) { ttop += iobj.offsetTop; iobj = iobj.offsetParent; } return ttop; }";
            strResp += "function findLeft(iobj) { tleft = 0; while (iobj) { tleft += iobj.offsetLeft; iobj = iobj.offsetParent; } return tleft; }";
            //request switch change
            strResp += "function swclicked(boxMSG, cmdSend) { var xhr = new XMLHttpRequest(); var mycmd = cmdSend +'&md='; ";
            strResp += "if (boxMSG.src.indexOf('turn.png')>=0) mycmd+='0'; else mycmd += '1';";
            strResp += "xhr.open('GET', 'switch.aspx?' + mycmd + '&" + securityKey + "&_='+Math.random());";
            strResp += "xhr.send(null); xhr.onreadystatechange = function() {if (xhr.readyState == 4) { if(xhr.responseText.indexOf('OK')>=0)";
            strResp += "if (boxMSG.src.indexOf('turn')>=0) boxMSG.src = 'file/str.png'; else boxMSG.src = 'file/turn.png';}};}";
            //request signal change
            strResp += "function siclicked(boxMSG, cmdSend) { var xhr = new XMLHttpRequest(); var mycmd = cmdSend +'&md='; ";
            strResp += "if (boxMSG.src.indexOf('red.png')>=0) mycmd+='0'; else mycmd += '1';";
            strResp += "xhr.open('GET', 'signal.aspx?' + mycmd + '&" + securityKey + "&_='+Math.random());";
            strResp += "xhr.send(null); xhr.onreadystatechange = function() {if (xhr.readyState == 4) { if(xhr.responseText.indexOf('OK')>=0)";
            strResp += "if (boxMSG.src.indexOf('red')>=0) boxMSG.src = 'file/green.png'; else boxMSG.src = 'file/red.png';}};}";
            //create the initial value for the switches
            strResp += "function buildSwitch(boxMSG, num) {var obj = document.getElementById(boxMSG); ";
            strResp += "if (num == 1) obj.src = 'file/str.png'; else obj.src = 'file/turn.png';}";
            strResp += "var NumberSwitch=" + mySwitch.NumberSwitches + ";";
            strResp += "function getswitches() {var xhr2 = new XMLHttpRequest(); xhr2.open('GET', 'switch.aspx?" + securityKey + "&no=1&_=' + Math.random()); xhr2.send(null); xhr2.onreadystatechange = function () {";
            strResp += "if (xhr2.readyState == 4) {";
            strResp += "for (inc = 0; inc < NumberSwitch; inc++) { if(xhr2.responseText.charAt(inc)=='0') mynum = 0; else mynum = 1; buildSwitch('switch'+inc, mynum);}}};}";
            //signals
            strResp += "function buildSignal(boxMSG, num) {var obj = document.getElementById(boxMSG); ";
            strResp += "if (num == 1) obj.src = 'file/green.png'; else obj.src = 'file/red.png';}";
            strResp += "var NumberSignal=" + mySignal.NumberSignals + ";";
            strResp += "function getsignals() {var xhr = new XMLHttpRequest(); xhr.open('GET', 'signal.aspx?" + securityKey + "&no=1&_=' + Math.random()); xhr.send(null); xhr.onreadystatechange = function () {";
            strResp += "if (xhr.readyState == 4) {";
            strResp += "for (inc = 0; inc < NumberSignal; inc++) { if(xhr.responseText.charAt(inc)=='0') mynum = 0; else mynum = 1; buildSignal('signal'+inc, mynum);}}};}";
            //initialization
            strResp += "function InitAll() {getsignals();getswitches();}";
            strResp += "</SCRIPT>";
            //strResp = WebServer.OutPutStream(response, strResp);
            // Create one section for each train
            strResp += "<h1>Gestion des trains</h1><TABLE BORDER=\"0\" >";
            for (byte i = 0; i < myParamRail.NumberOfTrains; i++)
            {
                strResp += "<TR><FORM><TD>" + myParamRail.Trains[i].TrainName + "</TD><TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageTrain + "?pw=" + (-myParamRail.Trains[i].Speed) + "&tr=" + i;
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
            }
            strResp += "</TABLE><br>";
            for (byte i = 0; i < myParamRail.NumberOfSwitchs; i++)
            {
                strResp += "<span style='position: absolute;margin-left:" + myParamRail.Switchs[i].Left + "px; margin-top:" + myParamRail.Switchs[i].Top + "px; top:findTop(document.all.MyImage); left:findLeft(document.all.MyImage);' >";
                strResp += "<img border='0' src=\"\" id='switch" + i + "' onClick=\"swclicked(document.getElementById('switch" + i + "'),'si=" + i + "&no=1');\" /></span>";
            }
            for (byte i = 0; i < myParamRail.NumberOfSignals; i++)
            {
                strResp += "<span style='position: absolute;margin-left:" + myParamRail.Signals[i].Left + "px; margin-top:" + myParamRail.Signals[i].Top + "px; top:findTop(document.all.MyImage); left:findLeft(document.all.MyImage);' >";
                strResp += "<img border='0' src=\"\" id='signal" + i + "' onClick=\"siclicked(document.getElementById('signal" + i + "'),'si=" + i + "&no=1');\" /></span>";
            }
            strResp += "<img alt='Map of the city' id='MyImage' src='file/circuit.png' /><a href='all.aspx?" + securityKey + "'>Display all page</a>";
            strResp += "</body></html>";

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
                strResp += "<TR><FORM><TD>" + myParamRail.Trains[i].TrainName + "</TD><TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageTrain + "?pw=" + (-myParamRail.Trains[i].Speed) + "&tr=" + i;
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
            }
            strResp += "<TR><TD>&nbsp</TD></TR>";
            for (byte i = 0; i < myParamRail.NumberOfSwitchs; i++)
            {
                strResp += "<TR><FORM><TD>" + myParamRail.Switchs[i].Name + "</TD><TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('switch" + i + "'),'" + pageSwitch + "?si=" + i + "&md=0&no=1')\" value=\"|\"></TD>";
                strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('switch" + i + "'),'" + pageSwitch + "?si=" + i + "&md=1&no=1')\" value=\"/\"></TD>";
                strResp += "<TD><span id='switch" + i + "'></span></FORM></TD></TR>";
            }
            strResp += "<TR><TD>&nbsp</TD></TR>";
            for (byte i = 0; i < myParamRail.NumberOfSignals; i++)
            {
                strResp += "<TR><FORM><TD>Signal " + myParamRail.Signals[i].Name + "</TD><TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('signal" + i + "'),'" + pageSignal + "?si=" + i + "&md=0&no=1')\" value=\"|\"></TD>";
                strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('signal" + i + "'),'" + pageSignal + "?si=" + i + "&md=1&no=1')\" value=\"/\"></TD>";
                strResp += "<TD><span id='signal" + i + "'></span></FORM></TD></TR>";
            }

            strResp += "</TABLE><br><a href='circ.aspx?" + securityKey + "'>Circuit</a>";
            strResp += "</body></html>";
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }
    }
}
