using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FQCS.Admin.Business.Queries
{
    public static class AppConfigQuery
    {
        public static IQueryable<AppConfig> Id(this IQueryable<AppConfig> query, string id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<AppConfig> IsDefault(this IQueryable<AppConfig> query, bool val = true)
        {
            return query.Where(o => o.IsDefault == val);
        }

        public static IQueryable<AppConfig> IdOnly(this IQueryable<AppConfig> query)
        {
            return query.Select(o => new AppConfig { Id = o.Id });
        }

        public static bool Exists(this IQueryable<AppConfig> query, string id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<AppConfig> Ids(this IQueryable<AppConfig> query, IEnumerable<string> ids)
        {
            return query.Where(o => ids.Contains(o.Id));
        }

        #region IQueryable<AppConfig>
        public static IQueryable<AppConfig> Sort(this IQueryable<AppConfig> query,
            AppConfigQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case AppConfigQuerySort.NAME:
                        {
                            query = asc ? query.OrderBy(o => o.Name) :
                                query.OrderByDescending(o => o.Name);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<AppConfig> Filter(
            this IQueryable<AppConfig> query, AppConfigQueryFilter filter)
        {
            if (filter.id != null)
                query = query.Where(o => o.Id == filter.id);
            if (filter.name_contains != null)
                query = query.Where(o => o.Name.Contains(filter.name_contains));
            if (filter.is_default != null)
                query = query.Where(o => o.IsDefault == filter.is_default);
            return query;
        }

        public static IQueryable<AppConfig> Project(
            this IQueryable<AppConfig> query, AppConfigQueryProjection projection)
        {
            foreach (var f in projection.GetFieldsArr())
                if (AppConfigQueryProjection.MAPS.ContainsKey(f))
                    foreach (var prop in AppConfigQueryProjection.MAPS[f])
                        query = query.Include(prop);
            return query;
        }
        #endregion
    }
}
