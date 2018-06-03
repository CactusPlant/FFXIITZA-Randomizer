using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FFXIITZARandomizer
{
    public class LicenseRandomizer : ObservableObject
    {
        //Class Variables
        Random rand = new Random();
        int[,] emptyBoard;
        List<int> licenses;
        int seed = 0;
        ObservableCollection<string> _log = new ObservableCollection<string>();
        bool _randoBoard = false;

        public bool RandoBoard {
            get { return _randoBoard; }
            set { _randoBoard = value;RaisePropertyChangedEvent("RandoBoard") ; } }

        public IEnumerable<string> Log { get {return _log; } }

        public int Seed { get {return seed; }
            set {seed = value; RaisePropertyChangedEvent("Seed"); } }

        public int[,] RandomizeLicense(int[,] b, List<int> lb)
        {
            //Variable assignment
            int[,] board = b;
            List<int> licensesRemaining = lb;
            List<int> usable = new List<int>();
            List<int> used = new List<int>();
            List<int> usedLicense = new List<int>();

            // Set Starting position
            int start = rand.Next(0, 576);
            board[start % 24, start / 24] = 0x1F00;
            licensesRemaining.Remove(start);
            usable = CheckUsable(start, used, usable);
            usedLicense.Add(0x1C00);
            used.Add(start);
            licensesRemaining.Remove(start);
            Console.WriteLine("Starting Position: {0} x {1}", start % 24, start / 24);

            int numOfLicenses = rand.Next(75, 150);
            for (int i = 0; i < numOfLicenses; i++)
            {
                int selection = usable[rand.Next(0, usable.Count() - 1)];

                //Selects "random" license
                int sel_lic = rand.Next(0, licensesRemaining.Count - 1);
                licensesRemaining.Remove(sel_lic);

                //Insert into board
                int position = usable[rand.Next(0, usable.Count - 1)];
                byte[] bArr = BitConverter.GetBytes(sel_lic);
                board[position % 24, position / 24] = bArr[0] << 8 | bArr[1];
                usable = CheckUsable(position, used, usable);
                used.Add(position);
                usable.Remove(position);


            }


            return board;


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

        public ICommand LicRando { get {return new DelegateCommand(RandoLoop); }}

        private void RandoLoop()
        {
            if (_randoBoard)
            {
                rand = new Random(Seed);
                Directory.CreateDirectory("TZARandoSeed_" + seed + "\\ps2data\\image\\ff12\\test_battle\\in\\binaryfile");
                for (int i = 1; i < 13; i++)
                {
                    licenses = NewLicense();
                    emptyBoard = NewBoard();
                    int[,] board = RandomizeLicense(emptyBoard, licenses);
                    WriteFile(board, i, seed);
                    _log.Add("Licenese board_" + i + " written to file");
                }
            }
        }

        //Creates and returns an empty Board
        public int[,] NewBoard()
        {
            emptyBoard = new int[24, 24];
            for (int col = 0; col < 576; col++)
            {
                //Console.WriteLine("Writing line {0}", col);
                emptyBoard[col % 24, col / 24] = 0xFFFF;
            }

            return emptyBoard;
        }

        //Handles writing of files for mod
        public void WriteFile(int[,] board, int boardnum, int seed)
        {
            using (FileStream
            fileStream = new FileStream("TZARandoSeed_" + seed + "\\ps2data\\image\\ff12\\test_battle\\in\\binaryfile\\board_" + boardnum + ".bin",
            FileMode.Create))
            {
                //Write Header
                fileStream.WriteByte(0x6C);
                fileStream.WriteByte(0x69);
                fileStream.WriteByte(0x63);
                fileStream.WriteByte(0x64);
                fileStream.WriteByte(0x18);
                fileStream.WriteByte(0x00);
                fileStream.WriteByte(0x18);
                fileStream.WriteByte(0x00);

                //Iterate through each piece of the board from 0:0 to 24:24 and write corresponding bytes
                for (int i = 0; i < 576; i++)
                {
                    int row = i / 24;
                    int col = i % 24;
                    byte[] final_bytes = BitConverter.GetBytes(board[col, row]);
                    //Console.WriteLine("Row:{0} Col:{1} Byte1:{2} | Byte2:{3}",row,col,final_bytes[1],final_bytes[0]);
                    fileStream.WriteByte(final_bytes[1]);
                    fileStream.WriteByte(final_bytes[0]);

                }
            }
        }
        //Handles licenses list
        public List<int> NewLicense()
        {
            licenses = new List<int>();
            for (int i = 0; i < 359; i++)
            {
                licenses.Add(i);
            }
            
            return licenses;
        }


        

    }
}
