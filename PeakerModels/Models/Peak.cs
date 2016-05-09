using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Data.Entity.Spatial;
using System.Collections.Generic;

namespace peaker.Models
{
    public class Peak
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int STCTYFIPS { get; set; }
        public int ELEV_METER { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public virtual IEnumerable<SummitCompletion> SummitCompletions { get; set; }
    }

}
