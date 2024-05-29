using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketing_system.BL.Contracts.DTO;

public class ResetPasswordDto
{
    public string Token { get; set; }
    public string NewPassword { get; set; }
}
