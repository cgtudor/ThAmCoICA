using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Events.Data
{
    public class Staffing
    {
        public int StaffId { get; set; }

        public int EventId { get; set; }

        public Staff Staff { get; set; }

        public Event Event { get; set; }

        public int StaffingId { get { return EventId * 10 + StaffId; } }
    }
}
