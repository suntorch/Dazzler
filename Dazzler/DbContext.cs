using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using Dazzler.Interfaces;
using Dazzler.Models;

namespace Dazzler
{
    public class DbContext : IDbContext
    {
        public IDbConnection DbConnection { get; internal set; }


        public DbContext(IDbConnection db) => this.DbConnection = db;
        public DbContext(DbContextOptions options) { }
        public DbContext() { }


        protected List<T> Query<T>(string name, object args
            , int? offset = null
            , int? limit = null
            , int? timeout = null
            , bool? noevent = null
            , object state = null
            , ResultInfo ri = null)
        {
            return DbConnection.Query<T>(CommandType.StoredProcedure, name, args
                , offset: offset
                , limit: limit
                , timeout: timeout
                , noevent: noevent
                , state: state
                , ri: ri);
        }
        protected List<T> Query<T>(object args
            , int? offset = null
            , int? limit = null
            , int? timeout = null
            , bool? noevent = null
            , object state = null
            , ResultInfo ri = null
            , [CallerMemberName] string name = null)
        {
            return DbConnection.Query<T>(CommandType.StoredProcedure, name, args
                , offset: offset
                , limit: limit
                , timeout: timeout
                , noevent: noevent
                , state: state
                , ri: ri);
        }

        protected List<T> QueryText<T>(string sql, object args
            , int? offset = null
            , int? limit = null
            , int? timeout = null
            , bool? noevent = null
            , object state = null
            , ResultInfo ri = null)
        {
            return DbConnection.Query<T>(CommandType.Text, sql, args
                , offset: offset
                , limit: limit
                , timeout: timeout
                , noevent: noevent
                , state: state
                , ri: ri);
        }


        protected T Scalar<T>(string name, object args
            , int? timeout = null
            , bool? noevent = null
            , object state = null
            , ResultInfo ri = null)
        {
            return DbConnection.Scalar<T>(CommandType.StoredProcedure, name, args
                , timeout: timeout
                , noevent: noevent
                , state: state
                , ri: ri);
        }
        protected T Scalar<T>(object args
            , int? timeout = null
            , bool? noevent = null
            , object state = null
            , ResultInfo ri = null
            , [CallerMemberName] string name = null)
        {
            return DbConnection.Scalar<T>(CommandType.StoredProcedure, name, args
                , timeout: timeout
                , noevent: noevent
                , state: state
                , ri: ri);
        }

        protected T ScalarText<T>(string sql, object args
            , int? timeout = null
            , bool? noevent = null
            , object state = null
            , ResultInfo ri = null)
        {
            return DbConnection.Scalar<T>(CommandType.Text, sql, args
                , timeout: timeout
                , noevent: noevent
                , state: state
                , ri: ri);
        }


        protected int NonQuery(string name, object args
            , int? timeout = null
            , bool? noevent = null
            , object state = null
            , ResultInfo ri = null)
        {
            return DbConnection.NonQuery(CommandType.StoredProcedure, name, args
                , timeout: timeout
                , noevent: noevent
                , state: state
                , ri: ri);
        }
        protected int NonQuery(object args
            , int? timeout = null
            , bool? noevent = null
            , object state = null
            , ResultInfo ri = null
            , [CallerMemberName] string name = null)
        {
            return DbConnection.NonQuery(CommandType.StoredProcedure, name, args
                , timeout: timeout
                , noevent: noevent
                , state: state
                , ri: ri);
        }

        protected int NonQueryText(string sql, object args
            , int? timeout = null
            , bool? noevent = null
            , object state = null
            , ResultInfo ri = null)
        {
            return DbConnection.NonQuery(CommandType.Text, sql, args
                , timeout: timeout
                , noevent: noevent
                , state: state
                , ri: ri);
        }

    }

    public class DbContextOptions
    {
        public string ConnectionString { get; set; }
    }
}
