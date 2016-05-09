using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Data.Entity.Spatial;

namespace peaker.Models
{
    public class IndexedActivity
    {
        public int Id { get; set; }
        public int StravaActivityId { get; set; }

        public Athlete Athlete { get; set; }
    }

}
