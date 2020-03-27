using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace iBank.IntegrationTests.TestTools
{
    public abstract class IntegrationTestBase
    {
        private readonly string _localDbConnectionString = ConfigurationManager.ConnectionStrings["local"].ToString();

        protected void Create(string filePath)
        {
            if (!File.Exists(filePath)) throw new Exception(string.Format("No file found at path [{0}]", filePath));

            var sb = new StringBuilder();
            using (var sr = new StreamReader(filePath))
            {
                var line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (LineEqualsGoKeyword(line)) continue;

                    sb.Append(line);
                    sb.Append(Environment.NewLine);
                }
            }

            if (sb.Length == 0) throw new Exception(string.Format("No text found at file path [{0}]", filePath));

            ExecuteNonQuery(sb.ToString(), _localDbConnectionString);

        }

        protected void CreateFromDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) throw new Exception(string.Format("Directory path [{0}] does not exist.", directoryPath));

            var files = Directory.GetFiles(directoryPath);

            foreach (var file in files)
            {
                using (var sr = new StreamReader(file))
                {
                    var sb = new StringBuilder();
                    var line = "";

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (LineEqualsGoKeyword(line)) continue;

                        if (CurrencyConversionTableRecordToSkip(file, line)) continue;

                        sb.Append(line);
                        sb.Append(Environment.NewLine);
                    }

                    if (sb.Length == 0) throw new Exception(string.Format("No text found at file path [{0}}]", file));

                    ExecuteNonQuery(sb.ToString(), _localDbConnectionString);
                }
            }
        }

        protected static string SetPath(string fileName)
        {
            var temp = AppDomain.CurrentDomain.BaseDirectory;

            var path = Path.GetFullPath(Path.Combine(temp, @"..\..\"));

            return path + fileName;
        }

        private void ExecuteNonQuery(string sql, string connectionString)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                cmd.CommandTimeout = 180;
                cmd.ExecuteNonQuery();
            }
        }

        private bool LineEqualsGoKeyword(string s)
        {
            return s.Equals("GO", StringComparison.OrdinalIgnoreCase);
        }

        private bool CurrencyConversionTableRecordToSkip(string file, string line)
        {
            //only convert GBP and USD, need to include IDENTITY to allow for IDENTITY INSERT clauses
            if (file.Contains("curconversion") && !(line.Contains("GBP") || line.Contains("USD") || line.Contains("IDENTITY"))) return true;

            return false;
        }
    }
}
