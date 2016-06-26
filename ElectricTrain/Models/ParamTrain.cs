using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricTrain.Models
{
    class ParamTrain
    {
        public string TrainName { get; set; }
        public int Speed { get; set; }
        static public int NUMBER_TRAIN_MAX { get { return 8; } }
    }
}
