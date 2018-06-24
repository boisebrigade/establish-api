using System;
using System.Collections.Generic;
using System.Text;

namespace CFABB.SelfRescue.Data
{
    public class EntryCategory
    {
        public int EntryId { get; set; }
        public Entry Entry { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
