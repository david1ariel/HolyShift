using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyShift
{
    public class ShiftModel
    {
        public int ShiftId { get; set; }

        public int ShiftNth { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int ShiftType { get; set; }

        public ShiftModel() { }
        public ShiftModel(Shift shift)
        {
            ShiftId = shift.ShiftId;
            ShiftNth = shift.ShiftNth;
            StartTime = shift.StartTime;
            EndTime = shift.EndTime;
            ShiftType = shift.ShiftType;
        }

        public Shift ConvertToShift()
        {
            return new Shift
            {
                ShiftId = ShiftId,
                ShiftNth = ShiftNth,
                StartTime = StartTime,
                EndTime = EndTime,
                ShiftType = ShiftType
            };
        }
    }
}
