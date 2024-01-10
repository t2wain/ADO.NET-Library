using Microsoft.Extensions.Logging;

namespace ADOApp
{
    /// <summary>
    /// An example of a business logic class
    /// using a DB repository.
    /// </summary>
    public class Examples : IDisposable
    {
        private RepoExample _repo;
        private ILogger _logger;

        public Examples(RepoExample repo, ILogger<Examples> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public void Run()
        {
            var t = 2;

            switch (t)
            {
                case 0:
                    _repo.OpenClose();
                    break;
                case 1:
                    GetTable();
                    break;
                case 2:
                    GetDataReader();
                    GetDataReader2();
                    break;
                case 3:
                    GetDataAdpater();
                    break;
            }
        }

        public void GetTable()
        {
            var t = _repo.GetTable();
            var cnt = t.Rows.Count;

            t = _repo.GetTable2();
            cnt = t.Rows.Count;

            _logger.LogInformation(string.Format("Return record count: {0}", cnt));
        }

        public void GetDataReader()
        {
            using var res = _repo.GetReader();
            using var dr = res.DataReader;
            var cnt = 0;
            while (dr.Read())
            {
                var v = dr[0];
                cnt++;
            }
            _logger.LogInformation(string.Format("Return record count: {0}", cnt));
        }

        public void GetDataReader2()
        {
            using var res = _repo.GetReader2();
            using var dr = res.DataReader;
            var cnt = 0;
            while (dr.Read())
            {
                var v = dr[0];
                cnt++;
            }
            _logger.LogInformation(string.Format("Return record count: {0}", cnt));
        }

        public void GetDataAdpater()
        {
            using var ad = _repo.GetAdapter();
            var t = ad.DataTable;
            var cnt = t.Rows.Count;
            _logger.LogInformation(string.Format("Return record count: {0}", cnt));
        }

        public void Dispose() 
        {
            _repo.Dispose();
            _repo = null!;
            _logger = null!;
        }
    }
}
