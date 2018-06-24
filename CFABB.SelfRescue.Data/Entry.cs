using System;
using System.Collections.Generic;
using System.Text;

namespace CFABB.SelfRescue.Data {
    public class Entry {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public ICollection<EntryTag> Tags { get; set; }
        public ICollection<EntryCategory> Categories { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<PhoneNumber> PhoneNumbers { get; set; }
        public ICollection<Hours> OperatingHours { get; set; }
    }
}
