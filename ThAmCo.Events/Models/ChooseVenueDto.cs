using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThAmCo.Events.Models
{
    public class ChooseVenueDto
    {
        [Key, MinLength(5), MaxLength(5)]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(1, Int32.MaxValue)]
        public int Capacity { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Range(0.0, Double.MaxValue)]
        public double CostPerHour { get; set; }
    }
}
