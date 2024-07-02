using System;
using System.Collections.Generic;

namespace HolyShift;

public partial class ShiftsEmployee
{
    public int ShiftEmployeeId { get; set; }

    public int ShiftId { get; set; }

    public int EmployeeId { get; set; }

    public string Status { get; set; }

    public virtual Employee Employee { get; set; }

    public virtual Shift Shift { get; set; }
}
