using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricTrain.Controller
{
    partial class TrainManagement
    {
        #region All const string
        // security key
        static string MySecurityKey = "Key1234";
        const string paramSecurityKey = "sec";
        static string securityKey = "";
        // parameters
        static private string strFileProgram = "ParamTrain.txt";
        private const char ParamSeparator = '&';
        private const char ParamStart = '?';
        private const char ParamEqual = '=';
        private const string paramNumberSignal = "nsi";
        private const string paramNumberSwitch = "nai";
        private const string paramNameSwitch = "ain";
        private const string paramNameSignal = "sna";
        private const string paramNumberTrain = "ntr";
        private const string paramTrainName = "tn";
        private const string paramParamChannel = "tc";
        private const string paramRedBlue = "tr";
        private const string paramSecurity = "sec";
        private const string paramSerial = "dbg";
        private const string paramCantonOrigin = "co";
        private const string paramCantonEnd = "cf";
        private const string paramSwitchOrigin = "ao";
        private const string paramSwitchStraight = "ad";
        private const string paramSwitchTurn = "ac";
        private const string paramNumberCantons = "nca";
        private const string paramGoto = "gt";
        private const string paramDetecteur = "d";
        private const string paramTrainSpeed = "tv";
        private const string paramWeb = "wb";
        private const string paramtop = "pt";
        private const string paramleft = "pl";
        private const string paramSignaltop = "spt";
        private const string paramSignalleft = "spl";
        private const string paramSwitchMinDur = "smi";
        private const string paramSwitchMaxDur = "sma";
        private const string paramSwitchMinAng = "ami";
        private const string paramSwitchMaxAng = "ama";
        private const string paramSwitchAngle = "sa";
        // Strings to be used for the page names
        const string pageDefault = "default.aspx";
        const string pageUtil = "util.aspx";
        const string pageTrain = "train.aspx";
        const string pageSignal = "signal.aspx";
        const string pageSwitch = "switch.aspx";
        const string pageCircuit = "circ.aspx";
        const string pageCSS = "train.css";
        // Strings to be used for the param names
        const string paramComboBlue = "bl";
        const string paramComboRed = "rd";
        const string paramChannel = "ch";
        const string paramSinglePWM = "pw";
        const string paramTrain = "tr";
        const string paramComboPWM1 = "p1";
        const string paramComboPWM2 = "p2";
        const string paramContinuousFct = "fc";
        const string paramMode = "md";
        const string paramNoUI = "no";
        const string paramILSNum = "il";
        const string paramSignalSwitch = "si";
        // Strings to be used for separators and returns
        //HTTP/1.1 200 OK\r\nContent-Type: text/html; charset=utf-8\r\nCache-Control: no-cache\r\nConnection: close\r\n\r\n
        const string strOK = "OK";
        const string strProblem = "Problem";
        const char strEndFile = '\r';

        #endregion All const string

    }
}
