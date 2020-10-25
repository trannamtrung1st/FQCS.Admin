using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FQCS.Admin.Data
{

    public static class Constants
    {
        public static class Data
        {
            public const string CONN_STR = "Server=localhost;Database=FQCS;Trusted_Connection=False;User Id=sa;Password=123456;MultipleActiveResultSets=true";
        }

        public static class RoleName
        {
            public const string ADMIN = "Administrator";
        }
    }

}
