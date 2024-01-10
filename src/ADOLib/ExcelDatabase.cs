using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace ADOLib
{
    /// <summary>
    /// Using OledDB and Microst ACE engine 
    /// to work with Excel file like a DB.
    /// </summary>
    public class ExcelDatabase : OleDbDatabase {

        public ExcelDatabase() : base() { }

        public override string GetConnectionString() {
            return "Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source="
                + this.FileName
                + ";Mode=ReadWrite|Share Deny None;"
                + "Extended Properties=\"Excel 8.0;HDR=YES;\";Jet OLEDB:System database=\"\";"
                + "Jet OLEDB:Registry Path=\"\";Jet OLEDB:Engine Type=35;"
                + "Jet OLEDB:Database Locking Mode=0;Jet OLEDB:Global Partial Bulk Ops=2;"
                + "Jet OLEDB:Global Bulk Transactions=1;Jet OLEDB:Create System Database=False;"
                + "Jet OLEDB:Encrypt Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;"
                + "Jet OLEDB:Compact Without Replica Repair=False;Jet OLEDB:SFP=False";
        }

        public virtual string FileName { get; set; } = null!;

        public override void DeriveParameters(IDbCommand cmd) {
            throw new NotImplementedException();
        }

        public DataTable GetTableData(string excelTableName) {
            DbCommand cmd = this.CreateCommand(excelTableName);
            cmd.CommandType = CommandType.TableDirect;
            return this.ExecuteTable(cmd, excelTableName);
        }

        public void GetTableData(string excelTableName, DataTable table) {
            DbCommand cmd = this.CreateCommand(excelTableName);
            cmd.CommandType = CommandType.TableDirect;
            this.ExecuteTable(cmd, table);
        }

        public static List<string> GetExcelSheets(string fileName) {
            List<string> ltables = new List<string>();;
            using (ExcelDatabase db = new ExcelDatabase()) {
                db.FileName = fileName;
                OleDbConnection cnn = db.Connection as OleDbConnection;
                DataTable t = cnn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                    new object[] { null, null, null, "TABLE" });
                foreach (DataRow r in t.Rows)
                    ltables.Add(r[2].ToString());
            }
            return ltables;
        }

        public static bool ValidateConnection(string fileName) {
            bool success = false;
            using (ExcelDatabase db = new ExcelDatabase()) {
                db.FileName = fileName;
                try { 
                    DbConnection cnn = db.Connection;
                    success = true;
                }
                catch { }
            }
            return success;
        }

    }

}
