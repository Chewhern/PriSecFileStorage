using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriSecFileStorageAPI.Model
{
    public class AllowedUserListModel
    {
        public String Status { get; set; }

        public String[] AllowedUserID { get; set; }
    }
}
