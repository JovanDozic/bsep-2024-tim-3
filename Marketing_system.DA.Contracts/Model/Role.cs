using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marketing_system.DA.Contracts.Shared;
using Newtonsoft.Json;

namespace Marketing_system.DA.Contracts.Model
{
    public class Role : Entity
    {
        public string Name { get; set; }
        [NotMapped][JsonProperty]
        public List<Permission> Permissions{  get; set; }

        public Role() { }
        public Role(string role, List<Permission> permissions)
        {
            Name = role;
            Permissions = permissions;
        }
    }
}
