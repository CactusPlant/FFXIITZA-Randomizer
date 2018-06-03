using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIITZARandomizer
{
    //Model to be used in release with logic
    class LicenseCell
    {
        public int HexValue { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int FromCenter { get; set; }

        public LicenseCell()
        {

        }


    }
}
