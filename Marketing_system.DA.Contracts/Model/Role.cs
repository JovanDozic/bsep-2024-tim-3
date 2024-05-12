using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marketing_system.DA.Contracts.Shared;

namespace Marketing_system.DA.Contracts.Model
{
    public class Role : Entity
    {
        public string Name { get; set; }

        public Role() { }
        public Role(string role)
        {
            Name = role;
        }
    }
}
