﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.Admin.WebAdmin.Models
{
    public interface IReturnViewModel
    {
        string Layout { get; set; }
        PageInfo Info { get; set; }
    }
}
