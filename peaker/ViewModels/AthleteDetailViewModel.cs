using peaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace peaker.ViewModels
{
    public class AthleteDetailViewModel
    {
        public string AthleteName { get; set; }
        public int AthleteId { get; set; }
        public int IndexedActivityCount { get; set; }
        public IEnumerable<SummitCompletion> AthleteSummitCompletions { get; set; }
    }
}