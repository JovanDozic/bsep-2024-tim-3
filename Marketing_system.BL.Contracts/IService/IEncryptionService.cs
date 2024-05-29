using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketing_system.BL.Contracts.IService;

public interface IEncryptionService
{
    string Encrypt(string input);
    string Decrypt(string input);
}
