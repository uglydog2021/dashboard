using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_ReportForms.Core.Models
{
    public class ExcelRun
    {
        public List<string> ColumnNameList { get; set; }

        public IEnumerable<dynamic> ViewData { get; set; }

        public string SheetName { get; set; }
    }
}
