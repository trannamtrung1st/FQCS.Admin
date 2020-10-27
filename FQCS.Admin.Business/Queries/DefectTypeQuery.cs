using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FQCS.Admin.Business.Queries
{
    public static class DefectTypeQuery
    {
        public static IQueryable<DefectType> Id(this IQueryable<DefectType> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<DefectType> IdOnly(this IQueryable<DefectType> query)
        {
            return query.Select(o => new DefectType { Id = o.Id });
        }

        public static bool Exists(this IQueryable<DefectType> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<DefectType> Ids(this IQueryable<DefectType> query, IEnumerable<int> ids)
        {
            return query.Where(o => ids.Contains(o.Id));
        }

        #region IQueryable<DefectType>
        public static IQueryable<DefectType> Sort(this IQueryable<DefectType> query,
            DefectTypeQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case DefectTypeQuerySort.NAME:
                        {
                            query = asc ? query.OrderBy(o => o.Name) :
                                query.OrderByDescending(o => o.Name);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<DefectType> Filter(
            this IQueryable<DefectType> query, DefectTypeQueryFilter filter)
        {
            if (filter.id != null)
                query = query.Where(o => o.Id == filter.id);
            if (filter.name_contains != null)
                query = query.Where(o => o.Name.Contains(filter.name_contains));
            if (filter.code != null)
                query = query.Where(o => o.Code == filter.code);
            return query;
        }

        public static IQueryable<DefectType> Project(
            this IQueryable<DefectType> query, DefectTypeQueryProjection projection)
        {
            foreach (var f in projection.GetFieldsArr())
                if (DefectTypeQueryProjection.MAPS.ContainsKey(f))
                    foreach (var prop in DefectTypeQueryProjection.MAPS[f])
                        query = query.Include(prop);
            return query;
        }
        #endregion
    }
}
