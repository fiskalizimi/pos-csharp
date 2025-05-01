using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fiskalizimi
{
    public class CsrRequest
    {
        public string Country { get; set; }
        public string BusinessName { get; set; }
        public ulong Nui { get; set; }
        public ulong BranchId { get; set; }
        public ulong PosId { get; set; }
    }

}
