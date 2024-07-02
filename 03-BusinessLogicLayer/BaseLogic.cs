using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyShift
{
    public class BaseLogic
    {
        protected readonly HolyShiftContext DB;
        public BaseLogic(HolyShiftContext dB)
        {
            DB = dB;
        }
    }
}
