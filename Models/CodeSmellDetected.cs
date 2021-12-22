using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AENC.Models
{
    public class CodeSmellDetected
    {
        public string CodeSmellName { get; set; }
        public string CodeSmellClass { get; set; }
        public string CodeSmellFunction { get; set; }
        public int CodeSmellLine { get; set; }
    }
}
