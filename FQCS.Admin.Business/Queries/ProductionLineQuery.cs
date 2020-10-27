using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FQCS.Admin.Business.Queries
{
    public static class ProductionLineQuery
    {
        public static IQueryable<ProductionLine> Id(this IQueryable<ProductionLine> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<ProductionLine> IdOnly(this IQueryable<ProductionLine> query)
        {
            return query.Select(o => new ProductionLine { Id = o.Id });
        }

        public static bool Exists(this IQueryable<ProductionLine> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<ProductionLine> Ids(this IQueryable<ProductionLine> query, IEnumerable<int> ids)
        {
            return query.Where(o => ids.Contains(o.Id));
        }

        #region IQueryable<ProductionLine>
        public static IQueryable<ProductionLine> Sort(this IQueryable<ProductionLine> query,
            ProductionLineQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case ProductionLineQuerySort.CODE:
                        {
                            query = asc ? query.OrderBy(o => o.Code) :
                                query.OrderByDescending(o => o.Code);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<ProductionLine> Filter(
            this IQueryable<ProductionLine> query, ProductionLineQueryFilter filter)
        {
            if (filter.id != null)
                query = query.Where(o => o.Id == filter.id);
            if (filter.code != null)
                query = query.Where(o => o.Code == filter.code);
            return query;
        }

        public static IQueryable<ProductionLine> Project(
            this IQueryable<ProductionLine> query, ProductionLineQueryProjection projection)
        {
            foreach (var f in projection.GetFieldsArr())
                if (ProductionLineQueryProjection.MAPS.ContainsKey(f))
                    foreach (var prop in ProductionLineQueryProjection.MAPS[f])
                        query = query.Include(prop);
            return query;
        }
        #endregion
    }
}
