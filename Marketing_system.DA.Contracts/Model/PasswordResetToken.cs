using Marketing_system.DA.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketing_system.DA.Contracts.Model;

public class PasswordResetToken : Entity
{
    public string Token { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsUsed { get; set; }
    public int UserId { get; set; }
}
