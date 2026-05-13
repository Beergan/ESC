using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC.CONCOST.ModuleESCCore.Models
{
    public class ContractSampleFileRequest
    {
        public string FileName { get; set; } = string.Empty;

        public string ContentBase64 { get; set; } = string.Empty;
    }
}