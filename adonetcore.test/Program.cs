using adonetcore.core.Implementations;
using System;
using System.Data;

namespace adonetcore.test
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var con = new DBHandler("Data Source=mydata.db;Cache=Shared", "system.data.sqlite"))
            {
                string all = "select * from SavingPlan;";
                var result = con.GetDataReaderText(all);
                while (result.Read())
                {
                    Console.WriteLine("text " + result["Id"]);
                }
            }
        }
    }
}
