using System;
using System.Collections.Generic;
using System.Text;

namespace CFABB.SelfRescue.Data
{
    public class EntryTag
    {
        public int EntryId { get; set; }
        public Entry Entry { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
