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

        /// <summary>
        /// Provider=[Microsoft.Jet.OLEDB.4.0|Microsoft.ACE.OLEDB.12.0|Microsoft.ACE.OLEDB.16.0];
        /// Data Source=[full file path];
        /// Extended Properties="[Excel 8.0|Excel 12.0 Xml];HDR=[YES|NO];ReadOnly=[true|false];MaxScanRows=[8|1-16|0];[IMEX=1;]"
        /// </summary>
        public override string GetConnectionString() {
            return "Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source="
                + this.FileName
                + ";Mode=ReadWrite|Share Deny None;"
                + "Extended Properties=\"Excel 12.0 Xml;HDR=YES;\";Jet OLEDB:System database=\"\";"
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
