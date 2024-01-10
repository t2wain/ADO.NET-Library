namespace ADOLib
{
    public class DataResultEventArgs : EventArgs {

        private object _dataResult;
        private Exception _exception;

        public DataResultEventArgs(Exception exception, object dataResult) {
            this._exception = exception;
            this._dataResult = dataResult;
        }

        public bool IsData {
            get { return (this._exception != null && this._dataResult != null); }
        }

        public bool IsError {
            get { return (this._exception != null); }
        }

        public Exception Exception {
            get { return this._exception; }
        }

        public object DataResult {
            get { return this._dataResult; }
        }
    }
}
