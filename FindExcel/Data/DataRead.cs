using System.Collections.ObjectModel;

namespace FindExcel
{
    public class DataRead
    {
        public ObservableCollection<FileExcel> CollectionFiles { get; set; }
        public ObservableCollection<Result> CollectionResults { get; set; }
        public ObservableCollection<Result> CollectionResolutMasterFile { get; set; }
        public string ColumnSearch { get; set; }
        public string ColumnSearchMastrFile { get; set; }

        public string FileForResult { get; set; }
        public string ColumnForResult { get; set; }
    }
}