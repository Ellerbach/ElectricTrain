using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricTrain.Models
{
    class ParamRail
    {
        public int NumberOfTrains { get; set; }
        public int NumberOfSignals { get; set; }
        public int NumberOfSwitchs { get; set; }
        public ParamTrain[] Trains { get; set; }
        public ParamSwitchs[] Switchs { get; set; }
        public ParamSignal[] Signals { get; set; }
        public string SecurityKey { get; set; }

    }
}
