using System.Data;

namespace ADOLib
{
    /// <summary>
    /// Using OleDB and Microsoft ACE engine
    /// to work with Microsoft Access database.
    /// </summary>
    public class AccessDatabase : OleDbDatabase {

        public AccessDatabase() : base() { }

        public override string GetConnectionString() {
            return "Provider=Microsoft.ACE.OLEDB.12.0;Persist Security Info=False;"
                + "Data Source=" + this.FileName;
        }

        public virtual string FileName { get; set; } = null!;

        public override void DeriveParameters(IDbCommand cmd) {
            throw new NotImplementedException();
        }

    }

}
