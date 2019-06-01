using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbaPlayers
{
    class Meta
    {
        public int Total_Pages { get; set; }
        public int Current_Page { get; set; }
        public int? Next_Page { get; set; }
        public int Per_Page { get; set; }
        public int Total_Count { get; set; }
    }
}
