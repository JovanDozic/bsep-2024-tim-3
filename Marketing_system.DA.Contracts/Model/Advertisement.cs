using Marketing_system.DA.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Marketing_system.DA.Contracts.Model
{
    public class Advertisement : Entity
    {
        public string? Slogan {  get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Description { get; set; }
        public int ClientId { get; set; }
        public DateOnly Deadline {  get; set; }
        public AdvertisementStatus Status { get; set; }

        public Advertisement(string? slogan, DateOnly startDate, DateOnly endDate, string description, int clientId,DateOnly deadline, AdvertisementStatus status )
        {
            Slogan = slogan; 
            StartDate = startDate;
            EndDate = endDate;
            Description = description;
            ClientId = clientId;
            Deadline = deadline;
            Status = status;
        }

    }
}
