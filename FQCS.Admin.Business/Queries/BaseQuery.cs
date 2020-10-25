using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FQCS.Admin.Business.Models;

namespace FQCS.Admin.Business.Queries
{
    public static class BaseQuery
    {
        public static IQueryable<T> SelectPage<T>(
            this IQueryable<T> query, int page, int limit)
        {
            page = page - 1;
            return query.Skip(page * limit).Take(limit);
        }

    }
}
