using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FQCS.Admin.Business.Queries
{
    public static class ProductModelQuery
    {
        public static IQueryable<ProductModel> Id(this IQueryable<ProductModel> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<ProductModel> IdOnly(this IQueryable<ProductModel> query)
        {
            return query.Select(o => new ProductModel { Id = o.Id });
        }

        public static bool Exists(this IQueryable<ProductModel> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<ProductModel> Ids(this IQueryable<ProductModel> query, IEnumerable<int> ids)
        {
            return query.Where(o => ids.Contains(o.Id));
        }

        #region IQueryable<ProductModel>
        public static IQueryable<ProductModel> Sort(this IQueryable<ProductModel> query,
            ProductModelQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case ProductModelQuerySort.NAME:
                        {
                            query = asc ? query.OrderBy(o => o.Name) :
                                query.OrderByDescending(o => o.Name);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<ProductModel> Filter(
            this IQueryable<ProductModel> query, ProductModelQueryFilter filter)
        {
            if (filter.id != null)
                query = query.Where(o => o.Id == filter.id);
            if (filter.name_contains != null)
                query = query.Where(o => o.Name.Contains(filter.name_contains));
            if (filter.code != null)
                query = query.Where(o => o.Code == filter.code);
            return query;
        }

        public static IQueryable<ProductModel> Project(
            this IQueryable<ProductModel> query, ProductModelQueryProjection projection)
        {
            foreach (var f in projection.GetFieldsArr())
                if (ProductModelQueryProjection.MAPS.ContainsKey(f))
                    foreach (var prop in ProductModelQueryProjection.MAPS[f])
                        query = query.Include(prop);
            return query;
        }
        #endregion
    }
}
