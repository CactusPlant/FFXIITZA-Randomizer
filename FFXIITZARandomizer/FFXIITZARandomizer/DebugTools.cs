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
    //Debug Tools was used to find the binary differences between two files and show the
    //Location of that changed byte, to find Bazaar data
    class DebugTools : ObservableObject
    {
        private ObservableCollection<LogEntry> _log = new ObservableCollection<LogEntry>();
        private ObservableCollection<Item> _itemSearchResults = new ObservableCollection<Item>();
        private ObservableCollection<Item> _sortedItemsList = new ObservableCollection<Item>();
        private List<Item> _masterItemList = new List<Item>();
        private string _oldFileName = "Old File";
        private string _newFileName = "New File";
        private int _comparisonLVIndex = -1;
        private LogEntry _selectedItem;
        private Item _dbSortItem1;
        private Item _dbSortItem2;
        private string _dbItemSearchText = "Search Text/Hex";

        public LogEntry SelectedItem { get => _selectedItem; set {
                _selectedItem = value;
                RaisePropertyChangedEvent("SelectedItem");
            } }
        public int ComparisonLVIndex { get => _comparisonLVIndex; set { _comparisonLVIndex = value; RaisePropertyChangedEvent("NewFileName"); } }
        public string OldFileName { get => _oldFileName;  set { _oldFileName = value;RaisePropertyChangedEvent("OldFileName") ; } }
        public string NewFileName { get => _newFileName;  set { _newFileName = value; RaisePropertyChangedEvent("NewFileName"); } }
        public Item DbSortItem1 { get => _dbSortItem1; set { _dbSortItem1 = value; RaisePropertyChangedEvent("DBSortItem1"); } }
        public Item DbSortItem2 { get => _dbSortItem2; set { _dbSortItem2 = value; RaisePropertyChangedEvent("DBSortItem2"); } }
        public string DbItemSearchText { get => _dbItemSearchText; set { _dbItemSearchText = value;
                RaisePropertyChangedEvent("DBItemSearchText");
                SearchItems();
            } }
        public IEnumerable<LogEntry> Log { get { return _log; } }
        public IEnumerable<Item> ItemSearchResults { get => _itemSearchResults; }
        public IEnumerable<Item> SortedItemsList { get => _sortedItemsList; }
        

        public ICommand CompareBytes { get { return new DelegateCommand(Compare); } }
        //Compare's the bytes of Old and New file, displaying differneces
        private void Compare()
        {
            Console.WriteLine("Starting Comparison");
            _log.Clear();
            byte[] fNew = File.ReadAllBytes(NewFileName);
            byte[] fOld = File.ReadAllBytes(OldFileName);


            for (int i = 0; i < fNew.Length; i++)
            {
                if (fNew[i] != fOld[i])
                {
                    _log.Add(new LogEntry("0x"+i.ToString("X"), "0x"+fOld[i].ToString("X"), "0x"+fNew[i].ToString("X")));
                }
            }


        }
        public ICommand SetFileName1 { get { return new DelegateCommand(SetNameOld); } }
        private void SetNameOld()
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "Comparison File",
                DefaultExt = ".bin",
                Filter = "Binary Files (.bin)|*.bin"
            };

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                OldFileName = dlg.FileName;
            }
        }
        public ICommand SetFileName2 { get { return new DelegateCommand(SetNameNew); } }


        private void SetNameNew()
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "Comparison File",
                DefaultExt = ".bin",
                Filter = "Binary Files (.bin)|*.bin"
            };

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                NewFileName = dlg.FileName;
            }
        }

        public ICommand DBMoveItem { get { return new DelegateCommand(MoveItem); } }

        private void MoveItem()
        {
            _sortedItemsList.Add(_dbSortItem1);

        }

        public ICommand DBSearchItems { get { return new DelegateCommand(SearchItems); } }

        //Sorts master items list into results to be displayed by ListView
        //Creates List if doesn't exist
        private void SearchItems()
        {
            if (_masterItemList.Count == 0)
            {
                string[] buffer = File.ReadAllLines("C:\\Users\\Cactus\\Documents\\ItemsMaster.csv");
                foreach (string s in buffer)
                {
                    string n = s.Split(';')[0];
                    string h = s.Split(';')[1];

                    _masterItemList.Add(new Item(n, h));
                }
            }
            
            _itemSearchResults.Clear();
            foreach(Item i in _masterItemList)
            {
                if(i.Name.ToLower().Contains(_dbItemSearchText.ToLower()) || i.HexValue.Contains(_dbItemSearchText))
                {
                    _itemSearchResults.Add(i);
                }
            }

        }


    }

    class LogEntry
    {
        private string _name = "";
        private string _oldByte = "";
        private string _newByte = "";

        public string Name { get { return _name; }}
        public string OldByte { get { return _oldByte; } }
        public string NewByte { get { return _newByte; } }

        public LogEntry(string n, string b1, string b2)
        {
            _name = n;
            _oldByte = b1;
            _newByte = b2;
        }

    }

    class Item
    {
        private string _name = "";
        private string _hexValue = "";
        private int _hexInt = 0;

        public string Name { get => _name; set => _name = value; }
        public string HexValue { get => _hexValue; set => _hexValue = value; }
        public int HexInt { get => _hexInt; set => _hexInt = value; }

        public Item(string n, string hex)
        {
            _name = n;
            _hexValue = hex;
            

        }
    }
}
