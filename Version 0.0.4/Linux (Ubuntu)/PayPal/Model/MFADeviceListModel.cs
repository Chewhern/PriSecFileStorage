using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriSecFileStorageAPI.Model
{
    public class MFADeviceListModel
    {
        public String Status { get; set; }

        public String[] MFADeviceID { get; set; }
    }
}
