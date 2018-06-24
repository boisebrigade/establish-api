using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFABB.SelfRescue.Models
{
    public class Entry
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Address[] Addresses { get; set; }
        public PhoneNumber[] PhoneNumbers { get; set; }
        public OperatingHours[] HoursOfOperation { get; set; }
    }
}
