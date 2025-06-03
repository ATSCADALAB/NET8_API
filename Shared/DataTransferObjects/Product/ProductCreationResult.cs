using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Product
{
    public class ProductCreationResult
    {
        public string TagID { get; set; }
        public bool IsSuccess { get; set; }
        public int? ProductId { get; set; }
        public string ErrorMessage { get; set; }
    }
}
