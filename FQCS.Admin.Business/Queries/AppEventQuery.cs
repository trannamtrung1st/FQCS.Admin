using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Data.Models;

namespace FQCS.Admin.Business.Queries
{
    public static class AppEventQuery
    {
        public static IQueryable<AppEvent> Id(this IQueryable<AppEvent> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<AppEvent> IdOnly(this IQueryable<AppEvent> query)
        {
            return query.Select(o => new AppEvent { Id = o.Id });
        }

        public static bool Exists(this IQueryable<AppEvent> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<AppEvent> Ids(this IQueryable<AppEvent> query, IEnumerable<int> ids)
        {
            return query.Where(o => ids.Contains(o.Id));
        }

        #region IQueryable<AppEvent>
        public static IQueryable<AppEvent> Sort(this IQueryable<AppEvent> query,
            AppEventQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case AppEventQuerySort.CREATED_TIME:
                        {
                            query = asc ? query.OrderBy(o => o.CreatedTime) :
                                query.OrderByDescending(o => o.CreatedTime);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<AppEvent> Filter(
            this IQueryable<AppEvent> query, AppEventQueryFilter filter)
        {
            if (filter.from_date != null)
                query = query.Where(o => o.CreatedTime.Date >= filter.from_date);
            if (filter.to_date != null)
                query = query.Where(o => o.CreatedTime.Date <= filter.to_date);
            return query;
        }
        #endregion
    }
}
