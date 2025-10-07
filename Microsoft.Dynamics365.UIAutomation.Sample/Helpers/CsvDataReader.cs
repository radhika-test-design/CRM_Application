using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Microsoft.Dynamics365.UIAutomation.Sample.Helpers
{
   
        public class CsvDataReader
        {
        public static IEnumerable<object[]> GetTestData(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var data = new List<object[]>();

            // Skip header row
            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                data.Add(values);
            }

            return data;
        }
    }
    }
