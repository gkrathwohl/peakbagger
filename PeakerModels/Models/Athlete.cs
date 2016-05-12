using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Data.Entity.Spatial;
using System.Collections.Generic;

namespace peaker.Models
{
    public class Athlete
    {
        public int Id { get; set; }
        public int StravaId { get; set; }
        public string Name { get; set; }
        public string AccessToken { get; set; }
        public string UserId { get; set; }

        public virtual IEnumerable<SummitCompletion> SummitCompletions { get; set; }
    }

}
