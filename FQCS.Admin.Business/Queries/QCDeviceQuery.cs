using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Data.Models;

namespace FQCS.Admin.Business.Queries
{
    public static class QCDeviceQuery
    {
        public static IQueryable<QCDevice> Id(this IQueryable<QCDevice> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<QCDevice> IdOnly(this IQueryable<QCDevice> query)
        {
            return query.Select(o => new QCDevice { Id = o.Id });
        }

        public static bool Exists(this IQueryable<QCDevice> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<QCDevice> Ids(this IQueryable<QCDevice> query, IEnumerable<int> ids)
        {
            return query.Where(o => ids.Contains(o.Id));
        }

        #region IQueryable<QCDevice>
        public static IQueryable<QCDevice> Sort(this IQueryable<QCDevice> query,
            QCDeviceQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case QCDeviceQuerySort.CODE:
                        {
                            query = asc ? query.OrderBy(o => o.Code) :
                                query.OrderByDescending(o => o.Code);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<QCDevice> Filter(
            this IQueryable<QCDevice> query, QCDeviceQueryFilter filter)
        {
            if (filter.id != null)
                query = query.Where(o => o.Id == filter.id);
            if (filter.code != null)
                query = query.Where(o => o.Code == filter.code);
            return query;
        }
        #endregion
    }
}
