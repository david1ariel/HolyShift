using System;
using System.Collections.Generic;

namespace HolyShift;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string Username { get; set; }

    public byte[] PasswordHash { get; set; }

    public string Role { get; set; }

}
