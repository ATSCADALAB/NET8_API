using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Order
{
    public class ImportResult
    {
        public int SuccessCount { get; set; }
        public int SkippedCount { get; set; }
        public List<string> SkippedRows { get; set; }
        public List<string> Errors { get; set; }
    }
}
