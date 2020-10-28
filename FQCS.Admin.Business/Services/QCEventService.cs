using Microsoft.EntityFrameworkCore;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Business.Queries;
using FQCS.Admin.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace FQCS.Admin.Business.Services
{
    public class QCEventService : Service
    {
        public QCEventService(ServiceInjection inj) : base(inj)
        {
        }

        #region Create QCEventService
        protected void PrepareCreate(QCEvent entity)
        {
            entity.LastUpdated = entity.CreatedTime;
        }

        public QCEvent CreateQCEvent(QCEvent entity)
        {
            PrepareCreate(entity);
            return context.QCEvent.Add(entity).Entity;
        }
        #endregion

        #region Validation
        public ValidationData ValidateQCMessage(
            QCEventMessage model)
        {
            return new ValidationData();
        }
        #endregion

    }
}
