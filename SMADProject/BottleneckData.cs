using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMADProject
{
    public class BottleneckData
    {
        public string LineName { get; set; }
        public TimeSpan BottleneckTime { get; set; }
        public TimeSpan TotalDowntime { get; set; }
        public string Recommendations { get; set; }
    }
}
