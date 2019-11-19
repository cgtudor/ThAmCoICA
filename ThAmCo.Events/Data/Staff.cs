using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Events.Data
{
    public class Staff
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public bool Firstaider { get; set; }

        public List<Staffing> Staffing { get; set; }

        public String FullName { get { return FirstName + " " + Surname; } }

    }
}
