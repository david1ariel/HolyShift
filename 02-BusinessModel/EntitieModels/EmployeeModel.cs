using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyShift
{
    public class EmployeeModel
    {
        public int EmployeeId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public byte[] PasswordHash { get; set; }

        public string Role { get; set; }
        public EmployeeModel() { }

        public EmployeeModel(Employee employee)
        {
            EmployeeId = employee.EmployeeId;
            Name = employee.Name;
            Email = employee.Email;
            Username = employee.Username;
            PasswordHash = employee.PasswordHash;
            Role = employee.Role;
        }

        public Employee ConvertToEmployeeModel()
        {
            return new Employee
            {
                EmployeeId = EmployeeId,
                Name = Name,
                Email = Email,
                Username = Username,
                PasswordHash = PasswordHash,
                Role = Role
            };
        }
    }
}
