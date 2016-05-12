using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Data.Entity.Spatial;
using System;

namespace peaker.Models
{
    public class SummitCompletion
    {
        public int Id { get; set; }
        public int StravaActivityId { get; set; }
        public DateTime Date { get; set; }

        public Athlete Athlete { get; set; }
        public Peak Peak { get; set; }

    }

}
