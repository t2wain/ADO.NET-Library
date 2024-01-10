using System.Data;

namespace ADOLib
{

    public static class DataParser {

        public record UnkVal(object Value);

        public static UnkVal V(object value) => new UnkVal(value);

        public static bool ParseBoolean(this UnkVal d) {
            bool v = false;
            try {
                if (!d.IsNull())
                    Boolean.TryParse(d.Value.ToString(), out v);
            }
            catch { }
            return v;
        }

        public static int ParseInt(this UnkVal d) {
            int v = 0;
            try {
                if (!d.IsNull())
                    Int32.TryParse(d.Value.ToString(), out v);
            }
            catch { }
            return v;
        }

        public static double ParseDouble(this UnkVal d) {
            double v = 0;
            try {
                if (!d.IsNull())
                    Double.TryParse(d.Value.ToString(), out v);
            }
            catch { }
            return v;
        }

        public static DateTime ParseDateTime(this UnkVal d) {
            DateTime v = DateTime.MinValue;
            try {
                if (!d.IsNull())
                    DateTime.TryParse(d.Value.ToString(), out v);
            }
            catch { }
            return v;
        }

        public static object ParseDBNull(this UnkVal d) {
            object v = d.Value;
            try {
                if (string.IsNullOrWhiteSpace(v?.ToString())) {
                    v = DBNull.Value;
                }
                else if (v.GetType() == typeof(DateTime)) {
                    if ((DateTime)v == DateTime.MinValue)
                        v = DBNull.Value;
                }
                else if (v.GetType() == typeof(Int32)) {
                    if ((Int32)v == 0)
                        v = DBNull.Value;
                }
            }
            catch {}
            return v;
        }

        public static bool IsNull(this UnkVal d) {
            return (d.Value == null || d.Value == DBNull.Value);
        }

        public static string[] GetColumnNames(DataView dv) {
            var lstCol = new List<string>();
            foreach (DataColumn col in dv.Table!.Columns)
                lstCol.Add(col.ColumnName.ToLower());
            return lstCol.ToArray();
        }
    }

}
