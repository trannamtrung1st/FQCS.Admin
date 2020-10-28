using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FQCS.Admin.Business.Queries
{
    public static class ProductionBatchQuery
    {
        public static IQueryable<ProductionBatch> InLine(this IQueryable<ProductionBatch> query, int lineId)
        {
            return query.Where(o => o.ProductionLineId == lineId);
        }

        public static IQueryable<ProductionBatch> RunningAtTime(this IQueryable<ProductionBatch> query, DateTime time)
        {
            return query.Where(o => o.StartedTime <= time && (o.FinishedTime == null || o.FinishedTime >= time));
        }

        public static IQueryable<ProductionBatch> Id(this IQueryable<ProductionBatch> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<ProductionBatch> IdOnly(this IQueryable<ProductionBatch> query)
        {
            return query.Select(o => new ProductionBatch { Id = o.Id });
        }

        public static bool Exists(this IQueryable<ProductionBatch> query, int id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<ProductionBatch> Ids(this IQueryable<ProductionBatch> query, IEnumerable<int> ids)
        {
            return query.Where(o => ids.Contains(o.Id));
        }

        #region IQueryable<ProductionBatch>
        public static IQueryable<ProductionBatch> Sort(this IQueryable<ProductionBatch> query,
            ProductionBatchQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case ProductionBatchQuerySort.CODE:
                        {
                            query = asc ? query.OrderBy(o => o.Code) :
                                query.OrderByDescending(o => o.Code);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<ProductionBatch> Filter(
            this IQueryable<ProductionBatch> query, ProductionBatchQueryFilter filter)
        {
            if (filter.id != null)
                query = query.Where(o => o.Id == filter.id);
            if (filter.code != null)
                query = query.Where(o => o.Code == filter.code);
            return query;
        }

        public static IQueryable<ProductionBatch> Project(
            this IQueryable<ProductionBatch> query, ProductionBatchQueryProjection projection)
        {
            foreach (var f in projection.GetFieldsArr())
                if (ProductionBatchQueryProjection.MAPS.ContainsKey(f))
                    foreach (var prop in ProductionBatchQueryProjection.MAPS[f])
                        query = query.Include(prop);
            return query;
        }
        #endregion
    }
}
