using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThAmCo.Events.Data
{
    public class GuestBooking
    {
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public int EventId { get; set; }

        public Event Event { get; set; }

        public int BookingId { get { return CustomerId * 10 + EventId; } }

        public bool Attended { get; set; }
    }
}
