using GalaSoft.MvvmLight;

namespace FindExcel
{
    public class ProgressReport :ViewModelBase
    {
        private string _processedFile;
        public string ProcessedFile
        {
            get => _processedFile;
            set => Set("ProcessedFile", ref _processedFile, value);
        }

        private string _processedMessage;
        public string ProcessedMessage
        {
            get => _processedMessage;
            set
            { 
                Set("ProcessedMessage", ref _processedMessage, value);
                if(!string.IsNullOrEmpty(value))
                {
                    IsOpenMessage = true;
                }
            }
        }

        private int _progressValue = 0;
        public int ProgressValue
        {
            get => _progressValue;
            set => Set("ProgressValue", ref _progressValue, value);
        }

        private ProcessType _processType = ProcessType.NotReady;
        public ProcessType ProcessType
        {
            get => _processType;
            set => Set("ProcessType", ref _processType, value);
        }

        private bool _isOpenMessage = false;
        public bool IsOpenMessage
        {
            get => _isOpenMessage;
            set => Set("IsOpenMessage", ref _isOpenMessage, value);
        }
    }
}