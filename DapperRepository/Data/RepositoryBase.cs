using Dapper;
using DapperRepository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DapperRepository.Data
{
    public class RepositoryBase<TEntity> : IDisposable, IRepositoryBase<TEntity> where TEntity : class
    {
        protected readonly ISqlQueryUtil<TEntity> _sqlQueryUtil;
        protected readonly IDbConnection _connection;
        public RepositoryBase(ISqlQueryUtil<TEntity> sqlQueryUtil, IDbConnection connection)
        {
            _sqlQueryUtil = sqlQueryUtil;
            _connection = connection;

        }


        public virtual void Add(TEntity obj)
        {

            _connection.Execute(this._sqlQueryUtil.Insert(), obj);


        }

        public void Dispose()
        {

        }

        public virtual IEnumerable<TEntity> GetAll()
        {

            return _connection.Query<TEntity>(this._sqlQueryUtil.SelectAll()).ToList();

        }

        public virtual TEntity GetById(int id)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add(this._sqlQueryUtil.keyfield, id);
            return _connection.Query<TEntity>(this._sqlQueryUtil.SelectById(), dynamicParameters).FirstOrDefault();

        }

        public virtual void Remove(TEntity obj)
        {

            _connection.Query<TEntity>(this._sqlQueryUtil.Delete(), obj);


        }

        public virtual void Update(TEntity obj)
        {
            _connection.Query<TEntity>(this._sqlQueryUtil.Update(), obj);

        }



        


    }
}
