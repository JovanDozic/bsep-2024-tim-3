using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketing_system.BL.Contracts.DTO
{
    public class AdvertisementDto
    {
        public int Id { get; set; } = 0;
        public string Slogan {  get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Description { get; set; }
        public int ClientId { get; set; }
        public DateOnly Deadline { get; set; }
        public int Status { get; set; }
    }
}
