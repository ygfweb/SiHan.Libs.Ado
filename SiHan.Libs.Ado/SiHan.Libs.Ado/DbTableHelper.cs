using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiHan.Libs.Ado
{
    internal static class DbTableHelper
    {
        private static Type GetPropertyType(ColumnMapper mapper)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            if (ReflectionHelper.IsNullable(mapper.PropertyInfo.PropertyType))
            {
                return Nullable.GetUnderlyingType(mapper.PropertyInfo.PropertyType);
            }
            else
            {
                return mapper.PropertyInfo.PropertyType;
            }
        }
        private static string GetDbType(ColumnMapper mapper)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            if (!string.IsNullOrWhiteSpace(mapper.DbType))
            {
                return mapper.DbType;
            }
            else
            {
                Type type = GetPropertyType(mapper);
                if (type == typeof(string))
                {
                    return "varchar(500)";
                }
                else if (type == typeof(Guid))
                {
                    return "uuid";
                }
                else if (type == typeof(int))
                {
                    return "int";
                }
                else if (type == typeof(DateTime))
                {
                    return "TIMESTAMP";
                }
                else if (type == typeof(bool))
                {
                    return "boolean";
                }
                else if (type == typeof(double))
                {
                    return "numeric(18,2)";
                }
                else if (type.IsEnum)
                {
                    return "smallint";
                }
                else
                {
                    throw new Exception("不支持该数据类型：" + type.FullName);
                }
            }
        }
        private static string CreateColumn(ColumnMapper mapper)
        {
            StringBuilder sb = new StringBuilder();
            string dbType = GetDbType(mapper);
            sb.Append($"{mapper.ColumnName} {dbType} ");
            if (!mapper.IsCanNull)
            {
                sb.Append("not null ");
            }
            if (!string.IsNullOrWhiteSpace(mapper.DbDefaultValue))
            {
                sb.Append($"default {mapper.DbDefaultValue}");
            }
            sb.Append(",");
            return sb.ToString();
        }

        private static string CreateColumns<T>() where T : BaseEntity
        {
            StringBuilder sb = new StringBuilder();
            TableMapper tableMapper = MappingCachePool.GetOrAdd<T>();
            // 生成主键
            if (tableMapper.KeyColumn != null)
            {
                sb.AppendLine($"{tableMapper.KeyColumn.ColumnName} {GetDbType(tableMapper.KeyColumn)} PRIMARY KEY,");
            }
            // 生成其他列
            var columns = tableMapper.Columns.Where(p => p.Value.IsPrimarykey == false).ToList();
            foreach (var item in columns)
            {
                var column = item.Value;
                sb.AppendLine(CreateColumn(column));
            }
            string sql = sb.ToString().Trim().Trim(',');
            return sql;
        }



        public static string CreateTableSQL<T>() where T : BaseEntity
        {
            TableMapper mapper = MappingCachePool.GetOrAdd<T>();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"CREATE TABLE {mapper.TableName}(");
            sb.AppendLine(CreateColumns<T>());
            sb.AppendLine(");");
            return sb.ToString();
        }

        public static string CreateIndexSQL<T>() where T : BaseEntity
        {
            TableMapper tableMapper = MappingCachePool.GetOrAdd<T>();
            StringBuilder sb = new StringBuilder();
            var columns = tableMapper.Columns.Where(p => p.Value.IsIndex == true).ToList();
            foreach (var item in columns)
            {
                var column = item.Value;
                sb.AppendLine($"CREATE INDEX idx_{tableMapper.TableName}_{column.ColumnName} on {tableMapper.TableName}({column.ColumnName});");
            }
            return sb.ToString();
        }
    }
}
