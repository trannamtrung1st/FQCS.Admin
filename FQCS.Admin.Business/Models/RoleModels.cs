using System;
using System.Collections.Generic;
using System.Text;
using FQCS.Admin.Data.Models;

namespace FQCS.Admin.Business.Models
{
    public class CreateRoleModel : MappingModel<AppRole>
    {
        public CreateRoleModel()
        {
        }

        public CreateRoleModel(AppRole entity) : base(entity)
        {
        }

        public string Name { get; set; }
    }

    public class UpdateRoleModel : MappingModel<AppRole>
    {
        public UpdateRoleModel()
        {
        }

        public UpdateRoleModel(AppRole entity) : base(entity)
        {
        }

        public string Name { get; set; }
    }
}
