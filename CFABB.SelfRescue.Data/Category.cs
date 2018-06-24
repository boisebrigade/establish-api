using System;
using System.Collections.Generic;
using System.Text;

namespace CFABB.SelfRescue.Data {
    public class Category {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }

        public ICollection<EntryTag> Entries { get; set; }
    }
}
