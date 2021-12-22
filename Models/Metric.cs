using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AENC.Models
{
    public class Metric
    {
        public int MetricID { get; set; }
        public string MetricLvl { get; set; }
        public string MetricName { get; set; }
        public string MetricDescription { get; set; }
        public string MetricDetails { get; set; }
        public bool IsSelected { get; set; }
    }
}
