using DapperRepository.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DapperRepository.Data
{
    public class DapperSqlQuery<T> : ISqlQueryUtil<T>
    {
        private IDictionary<string, string> _columns;
        private string _tableName;
        public string keyfield { get; private set; }

        public DapperSqlQuery()
        {
            _columns = new Dictionary<string, string>();
            SetFieldNames();
        }

        public void SetFieldNames(bool ignorePK = false)
        {
            var tableName = typeof(T).GetCustomAttribute<TableAttribute>();
            _tableName = tableName == null ? typeof(T).Name : tableName.Name;

            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (var item in properties)
            {
                var notMapped = item.GetCustomAttribute<NotMappedAttribute>();
                if (notMapped != null)
                    continue;

                var column = item.GetCustomAttribute<ColumnAttribute>();
                if (column != null)
                    _columns.Add(column.Name, item.Name);
                else
                    _columns.Add(item.Name, item.Name);

                var key = item.GetCustomAttribute<KeyAttribute>();
                if (key != null)
                    keyfield = item.Name;


            }

        }

        public string Insert()
        {
            StringBuilder builder = new StringBuilder();
            _columns = _columns.Where(p => p.Value != keyfield).ToDictionary(p => p.Key, p => p.Value);

            builder.Append(@"INSERT INTO " + _tableName + "  ( ");
            foreach (var item in _columns)
            {
                builder.Append(item.Value);
                if (_columns.Last().Value != item.Value)
                    builder.Append(",");
            }

            builder.Append(")");
            builder.AppendLine(" VALUES ( ");
            foreach (var item in _columns)
            {
                builder.Append("@" + item.Value);
                if (_columns.Last().Value != item.Value)
                    builder.Append(",");
            }

            builder.Append(")");
            return builder.ToString();
        }

        public string Delete(string idField = null)
        {
            StringBuilder builder = new StringBuilder();
            //Set the real Primary Key
            if (!String.IsNullOrEmpty(idField))
                keyfield = idField;

            builder.Append("DELETE FROM " + _tableName + " WHERE " + keyfield + " = @" + keyfield);
            return builder.ToString();
        }

        public string SelectAll(IDictionary<string, string> columns = null)
        {
            StringBuilder builder = new StringBuilder();
            if (columns != null)
                _columns = columns.ToDictionary(p => p.Key, p => p.Value);

            builder.AppendLine("SELECT ");
            foreach (var item in _columns)
            {
                if (_columns.First().Value == item.Value)
                    builder.AppendLine(item.Key + " AS " + item.Value);
                else
                    builder.AppendLine("," + item.Key + " AS " + item.Value);

            }
            builder.AppendLine("FROM " + _tableName);

            return builder.ToString();

        }

        public string SelectById(string idField = null, IDictionary<string, string> columns = null)
        {
            StringBuilder builder = new StringBuilder();
            // Set the real Primary Key
            if (!String.IsNullOrEmpty(idField))
                keyfield = idField;

            builder.Append(this.SelectAll(columns));
            builder.AppendLine(" WHERE " + keyfield + " = @" + keyfield);
            return builder.ToString();
        }

        public string Update(string idField = null, IList<string> columns = null)
        {
            StringBuilder builder = new StringBuilder();
            //Set the real Primary Key
            if (!String.IsNullOrEmpty(idField))
                keyfield = idField;

            //Set only the column that will be updated
            if (columns != null)
                _columns = columns.ToDictionary(p => p, p => p);


            //Remove the primary key from the update command 
            _columns = _columns.Where(p => p.Value != keyfield).ToDictionary(p => p.Key, p => p.Value);

            //Builds the sql string
            builder.Append(" UPDATE " + _tableName + " SET ");
            foreach (var item in _columns)
            {
                if (_columns.First().Value == item.Value)
                    builder.AppendLine(item.Value + " = @" + item.Value);
                else
                    builder.AppendLine("," + item.Value + " = @" + item.Value);

            }
            builder.Append(" WHERE " + keyfield + " = @" + keyfield);

            return builder.ToString();

        }


    }
}
