using GalaSoft.MvvmLight;

namespace FindExcel
{
    public class Result: ViewModelBase
    {
        public string Address { get; set; }
        public int RowNumber { get; set; }
        public string WorkSheetName { get; set; }
        public string FoundString { get; set; }
        public string Data { get; set; }
        public string NameFile { get; set; }
        public string CellForResult { get; set; }


        private bool _isCheck = false;
        public bool IsCheck
        {
            get => _isCheck;
            set => Set("IsCheck", ref _isCheck, value);
        }
    }
}