using System.Collections.ObjectModel;

namespace FindExcel
{
    public class DataEdit
    {
        public ObservableCollection<Result> CollectionResults { get; set; }
        public ObservableCollection<Result> CollectionResolutMasterFile { get; set; }

        public string FileForResult { get; set; }
        public string ColumnForResult { get; set; }
    }
}