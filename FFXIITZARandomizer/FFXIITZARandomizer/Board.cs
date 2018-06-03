using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIITZARandomizer
{
    //Board class used to handle creating the license board and checking logic
    class Board
    {
        private int[,] _spaces = new int[24, 24];
        private List<LicenseCell> _deck = new List<LicenseCell>();

        private void ClearBoard()
        {
            _spaces = new int[24, 24];
            for (int col = 0; col < 576; col++)
            {
                //Console.WriteLine("Writing line {0}", col);
                _spaces[col % 24, col / 24] = 0xFFFF;
            }
        }


        public List<int> CheckUsable(int num, List<int> used, List<int> usables)
        {
            //Check each direction, and add to usable if slot is unused, not on list, and in range

            if (num / 24 - 1 >= 0 && !used.Contains(num - 24) && !usables.Contains(num - 24))
            { usables.Add(num - 24); }

            if (num / 24 + 1 < 24 && !used.Contains(num + 24) && !usables.Contains(num + 24))
            { usables.Add(num + 24); }

            if (num % 24 - 1 >= 0 && !used.Contains(num - 1) && !usables.Contains(num - 1))
            { usables.Add(num - 1); }

            if (num % 24 + 1 < 24 && !used.Contains(num + 1) && !usables.Contains(num + 1))
            { usables.Add(num + 1); }

            return usables;
        }
    }
}
