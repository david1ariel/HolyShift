using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyShift
{
    public class EmployeesLogic : BaseLogic
    {
        public EmployeesLogic(HolyShiftContext db) : base(db) { }

        public EmployeeModel GetSingleEmployee(int employeeId)
        {
            return new EmployeeModel(DB.Employees.Find(employeeId));
        }

        public EmployeeModel GetSingleEmployee(string username)
        {
            return new EmployeeModel(DB.Employees.FirstOrDefault(p => p.Username == username));
        }
    }
}
