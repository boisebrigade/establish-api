using System;
using System.Collections.Generic;
using System.Text;

namespace CFABB.SelfRescue.Data {
    public class Tag {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<EntryTag> Entries { get; set; }
    }
}
