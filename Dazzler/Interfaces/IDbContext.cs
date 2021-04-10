using System;
using System.Data;

namespace Dazzler.Interfaces
{
    public interface IDbContext
    {
        IDbConnection DbConnection { get; }
    }
}
