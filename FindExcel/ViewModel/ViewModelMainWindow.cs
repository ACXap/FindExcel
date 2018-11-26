using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Linq;
//using System.Windows;

namespace FindExcel
{
    class ViewModelMainWindow : ViewModelBase
    {
        private readonly ModelMainWindow _modelMainWindow;
        private object _lock = new object();

        private ObservableCollection<FileExcel> _collectionFiles;
        public ObservableCollection<FileExcel> CollectionFiles
        {
            get => _collectionFiles;
            set => Set("CollectionFiles", ref _collectionFiles, value);
        }

        private string _masterFile;
        public string MasterFile
        {
            get => _masterFile;
            set => Set("MasterFile", ref _masterFile, value);
        }

        private string _searchWord = string.Empty;
        public string SearchWord
        {
            get => _searchWord;
            set
            {
                Set("SearchWord", ref _searchWord, value);
                _collectionView.Refresh();
                _collectionMasterFileView.Refresh();
            }
        }

        private string _filterWorkSheet = string.Empty;
        public string FilterWorkSheet
        {
            get => _filterWorkSheet;
            set
            {
                Set("FilterWorkSheet", ref _filterWorkSheet, value);
                _collectionView.Refresh();
            }
        }

        private string _columnSearch;
        public string ColumnSearch
        {
            get => _columnSearch;
            set => Set("ColumnSearch", ref _columnSearch, value);
        }

        private string _columnSearchMasterFile;
        public string ColumnSearchMasterFile
        {
            get => _columnSearchMasterFile;
            set => Set("ColumnSearchMasterFile", ref _columnSearchMasterFile, value);
        }

        private string _columnForResult;
        public string ColumnForResult
        {
            get => _columnForResult;
            set => Set("ColumnForResult", ref _columnForResult, value);
        }

        private ProgressReport _progressReport;
        public ProgressReport ProgressReport
        {
            get => _progressReport;
            set => Set("ProgressReport", ref _progressReport, value);
        }

        private ObservableCollection<Result> _collectionResults = new ObservableCollection<Result>();
        public ObservableCollection<Result> CollectionResults
        {
            get => _collectionResults;
            set => Set("CollectionResults", ref _collectionResults, value);
        }

        private ObservableCollection<Result> _collectionResolutMasterFile = new ObservableCollection<Result>();
        public ObservableCollection<Result> CollectionResolutMasterFile
        {
            get => _collectionResolutMasterFile;
            set => Set("CollectionResolutMasterFile", ref _collectionResolutMasterFile, value);
        }

        private ICollectionView _collectionView;
        public ICollectionView CollectionView
        {
            get => _collectionView;
            set => Set("CollectionView", ref _collectionView, value);
        }

        private ICollectionView _collectionMasterFileView;
        public ICollectionView CollectionMasterFileView
        {
            get => _collectionMasterFileView;
            set => Set("CollectionMasterFileView", ref _collectionMasterFileView, value);
        }

        private RelayCommand _commandAddFiles;
        public RelayCommand CommandAddFiles
        {
            get
            {
                return _commandAddFiles
                    ?? (_commandAddFiles = new RelayCommand(
                    () =>
                    {
                        var list = _modelMainWindow.GetFiles();
                        if (list != null)
                        {
                            if (_collectionFiles == null || !_collectionFiles.Any())
                            {
                                CollectionFiles = new ObservableCollection<FileExcel>(list);
                            }
                            else
                            {
                                foreach(var item in list)
                                {
                                    CollectionFiles.Add(item);
                                }
                            }
                        }
                    }));
            }
        }

        private RelayCommand _commandClearCollection;
        public RelayCommand CommandClearCollection
        {
            get
            {
                return _commandClearCollection
                    ?? (_commandClearCollection = new RelayCommand(
                    () =>
                    {
                        CollectionFiles.Clear();
                    },
                    () => _collectionFiles?.Count > 0
                    && (_progressReport == null || _progressReport.ProcessType != ProcessType.Working)));
            }
        }

        private RelayCommand _commandClearCollectionResult;
        public RelayCommand CommandClearCollectionResult
        {
            get
            {
                return _commandClearCollectionResult
                    ?? (_commandClearCollectionResult = new RelayCommand(
                    () =>
                    {
                        CollectionResults.Clear();
                        CollectionResolutMasterFile.Clear();
                        ProgressReport = null;
                    },
                    () => CollectionResults?.Count > 0));
            }
        }

        private RelayCommand _commandReadFile;
        public RelayCommand CommandReadFile
        {
            get
            {
                return _commandReadFile
                    ?? (_commandReadFile = new RelayCommand(
                    () =>
                    {
                        DataRead dataRead = new DataRead()
                        {
                            CollectionFiles = _collectionFiles,
                            FileForResult = _masterFile,

                            CollectionResolutMasterFile = _collectionResolutMasterFile,
                            CollectionResults = _collectionResults,

                            ColumnSearch = _columnSearch,
                            ColumnSearchMastrFile = _columnSearchMasterFile,
                            ColumnForResult = _columnForResult
                        };

                        ProgressReport = new ProgressReport();

                        _modelMainWindow.ReadFile(dataRead, ProgressReport);
                    },
                    () => _collectionFiles?.Count > 0
                    && !string.IsNullOrEmpty(ColumnSearch)
                    && (_progressReport == null || _progressReport.ProcessType != ProcessType.Working)
                    ));
            }
        }

        private RelayCommand _commandAddFileForResult;
        public RelayCommand CommandAddFileForResult
        {
            get
            {
                return _commandAddFileForResult
                    ?? (_commandAddFileForResult = new RelayCommand(
                    () =>
                    {
                        ProgressReport = new ProgressReport();

                        MasterFile = _modelMainWindow.GetFileForResult(ProgressReport);
                    }));
            }
        }

        private RelayCommand _commandEditMasterFile;
        public RelayCommand CommandEditMasterFile
        {
            get
            {
                return _commandEditMasterFile
                    ?? (_commandEditMasterFile = new RelayCommand(
                    () =>
                    {
                        DataEdit dataEdit = new DataEdit()
                        {
                            CollectionResults = _collectionResults,
                            CollectionResolutMasterFile = _collectionResolutMasterFile,
                            FileForResult = _masterFile,
                            ColumnForResult = _columnForResult
                        };

                        ProgressReport = new ProgressReport();

                        _modelMainWindow.EditMasterFile(dataEdit, ProgressReport);
                    },
                    () =>
                    !string.IsNullOrEmpty(_masterFile)
                    && !string.IsNullOrEmpty(_columnForResult)
                    && CollectionFiles?.Count > 0
                    && CollectionResults?.Count(s => s.IsCheck) > 0
                    && CollectionResolutMasterFile?.Count(s => s.IsCheck) > 0
                    ));
            }
        }

        //private RelayCommand<DragEventArgs> _commandDrop;
        //public RelayCommand<DragEventArgs> CommandDrop
        //{
        //    get
        //    {
        //        return _commandDrop
        //            ?? (_commandDrop = new RelayCommand<DragEventArgs>(p =>
        //            {
        //                if (p.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
        //                {
        //                    var files = (string[])p.Data.GetData(DataFormats.FileDrop, true);
        //                }
        //            }));
        //    }
        //}

        public ViewModelMainWindow()
        {
            _modelMainWindow = new ModelMainWindow();
            BindingOperations.EnableCollectionSynchronization(_collectionResults, _lock);
            BindingOperations.EnableCollectionSynchronization(_collectionResolutMasterFile, _lock);
            CollectionView = CollectionViewSource.GetDefaultView(_collectionResults);
            CollectionMasterFileView = CollectionViewSource.GetDefaultView(_collectionResolutMasterFile);
            CollectionView.Filter = CollectionFilter;
            CollectionMasterFileView.Filter = CollectionMasterFileFilter;
            
        }

        private bool CollectionFilter(object item)
        {
            Result result = item as Result;
            return result.Address.ToLower().Contains(_filterWorkSheet.ToLower()) && result.FoundString.ToLower().Contains(_searchWord.ToLower());
        }

        private bool CollectionMasterFileFilter(object item)
        {
            Result result = item as Result;
            return result.FoundString.ToLower().Contains(_searchWord.ToLower());
        }
    }
}