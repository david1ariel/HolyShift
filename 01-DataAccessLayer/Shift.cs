using System;
using System.Collections.Generic;

namespace HolyShift;

public partial class Shift
{
    public int ShiftId { get; set; }

    public int ShiftNth { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int ShiftType { get; set; }
}
