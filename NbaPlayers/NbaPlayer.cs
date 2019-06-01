using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbaPlayers
{
    class NbaPlayer
    {
        public int ID { get; set; }
        public string First_Name { get; set; }
        public int? Height_Feet { get; set; }
        public int? Height_Inches { get; set; }
        public string Last_Name { get; set; }
        public string Position { get; set; }

        public TeamInfo Team { get; set; }
        public class TeamInfo
        {
            public int ID { get; set; }
            public string Abbreviation { get; set; }
            public string City { get; set; }
            public string Conference { get; set; }
            public string Division { get; set; }
            public string Full_Name { get; set; }
            public string Name { get; set; }

        }

        public int? Weight_Pounds { get; set; }

    }
}
