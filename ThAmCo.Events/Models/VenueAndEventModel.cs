using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.Events.Data;

namespace ThAmCo.Events.Models
{
    public class VenueAndEventModel
    {
        public EventVenueDto Venue { get; set; }

        public Event Event { get; set; }
    }
}
