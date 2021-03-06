﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FQCS.Admin.Business.Queries
{
    public static class QCEventQuery
    {
        public static IQueryable<QCEvent> Id(this IQueryable<QCEvent> query, string id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<QCEvent> IdOnly(this IQueryable<QCEvent> query)
        {
            return query.Select(o => new QCEvent { Id = o.Id });
        }

        public static bool Exists(this IQueryable<QCEvent> query, string id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<QCEvent> Ids(this IQueryable<QCEvent> query, IEnumerable<string> ids)
        {
            return query.Where(o => ids.Contains(o.Id));
        }

        public static IQueryable<QCEvent> OfBatch(this IQueryable<QCEvent> query, int batchId)
        {
            return query.Where(o => o.ProductionBatchId == batchId);
        }

        public static IQueryable<QCEvent> Passed(this IQueryable<QCEvent> query)
        {
            return query.Where(o => !o.Details.Any());
        }

        public static IQueryable<QCEvent> SortByTime(this IQueryable<QCEvent> query, bool asc)
        {
            return asc ? query.OrderBy(o => o.CreatedTime) :
                query.OrderByDescending(o => o.CreatedTime);
        }

        #region IQueryable<QCEvent>
        public static IQueryable<QCEvent> Sort(this IQueryable<QCEvent> query,
            QCEventQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case QCEventQuerySort.TIME:
                        {
                            query = query.SortByTime(asc);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<QCEvent> Filter(
            this IQueryable<QCEvent> query, QCEventQueryFilter filter)
        {
            if (filter.id != null)
                query = query.Where(o => o.Id == filter.id);
            if (filter.ids != null && filter.ids.Length > 0)
                query = query.Ids(filter.ids);
            if (filter.batch_id != null)
                query = query.Where(o => o.ProductionBatchId == filter.batch_id);
            if (filter.seen != null)
                query = query.Where(o => o.Seen == filter.seen);
            return query;
        }

        public static IQueryable<QCEvent> Project(
            this IQueryable<QCEvent> query, QCEventQueryProjection projection)
        {
            foreach (var f in projection.GetFieldsArr())
                if (QCEventQueryProjection.MAPS.ContainsKey(f))
                    foreach (var prop in QCEventQueryProjection.MAPS[f])
                        query = prop.Compile()(query);
            return query;
        }
        #endregion
    }
}
