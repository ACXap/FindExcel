using System.Collections.ObjectModel;

namespace FindExcel
{
    public class DataFind
    {
        public ObservableCollection<FileExcel> CollectionFiles { get; set; }
        public ObservableCollection<string> CollectionSearchWords { get; set; }
        public ObservableCollection<Result> CollectionResults { get; set; }
        public ObservableCollection<Result> CollectionResolutMasterFile { get; set; }
        public string ColumnSearch { get; set; }
        public string ColumnSearchMastrFile { get; set; }
        public string ColumnData { get; set; }
        public string FileForResult { get; set; }
        public string ColumnForResult { get; set; }
    }
}