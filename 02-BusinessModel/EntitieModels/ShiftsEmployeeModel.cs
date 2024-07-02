using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyShift
{
    internal class ShiftsEmployeeModel
    {
        public int ShiftEmployeeId { get; set; }

        public int ShiftId { get; set; }

        public int EmployeeId { get; set; }

        public string Status { get; set; }

        public ShiftsEmployeeModel() { }

        public ShiftsEmployeeModel(ShiftsEmployee shiftsEmployee)
        {
            ShiftEmployeeId = shiftsEmployee.ShiftEmployeeId;
            ShiftId = shiftsEmployee.ShiftId;
            EmployeeId = shiftsEmployee.EmployeeId;
            Status = shiftsEmployee.Status;
        }

        public ShiftsEmployee ConvertToShiftsEmployee()
        {
            return new ShiftsEmployee
            {
                ShiftEmployeeId = ShiftEmployeeId,
                ShiftId = ShiftId,
                EmployeeId = EmployeeId,
                Status = Status
            };
        }
    }
}
