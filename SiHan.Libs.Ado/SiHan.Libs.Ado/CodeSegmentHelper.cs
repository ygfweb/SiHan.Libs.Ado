using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SiHan.Libs.Ado
{
    internal static class CodeSegmentHelper
    {
        /// <summary>
        /// 生成insert语句
        /// </summary>
        public static string GenerateInsertql<T>() where T : BaseEntity
        {
            TableMapper tableMapper = MappingCachePool.GetOrAdd<T>();
            StringBuilder sb = new StringBuilder();
            sb.Append($"INSERT INTO {tableMapper.TableName} ");
            sb.Append("(");
            StringBuilder cols = new StringBuilder();
            var columns = tableMapper.Columns.Where(p=>p.Value.IsAuto==false).Select(p=>p.Value).ToList();
            foreach (var item in columns)
            {
                cols.Append($"{item.ColumnName},");
            }
            sb.Append(cols.ToString().Trim(','));
            sb.Append(") ");
            sb.Append("VALUES (");
            cols = new StringBuilder();
            foreach (var item in columns)
            {
                cols.Append($"@{item.PropertyName},");
            }
            sb.Append(cols.ToString().Trim(','));
            sb.Append(");");
            return sb.ToString();
        }

        public static string GenerateUpdateSql<T>() where T : BaseEntity
        {
            TableMapper tableMapper = MappingCachePool.GetOrAdd<T>();
            ColumnMapper keyColumn = tableMapper.KeyColumn;
            if (keyColumn == null)
            {
                throw new Exception($"{tableMapper.TypeName} class does not define a primary key");
            }
            StringBuilder sb = new StringBuilder();
            sb.Append($"UPDATE {tableMapper.TableName} SET ");
            var columns = tableMapper.Columns.Where(p => p.Value.IsPrimarykey == false).ToList();
            StringBuilder cols = new StringBuilder();
            foreach (var item in columns)
            {
                cols.Append($"{item.Value.ColumnName}=@{item.Value.PropertyName},");
            }
            sb.Append(cols.ToString().Trim(','));
            sb.Append(" WHERE ");
            sb.Append($"{keyColumn.ColumnName} = @{keyColumn.PropertyName};");
            return sb.ToString();
        }

        public static string GenerateDeleteSql<T>() where T:BaseEntity
        {
            TableMapper tableMapper = MappingCachePool.GetOrAdd<T>();
            ColumnMapper keyColumn = tableMapper.KeyColumn;
            if (keyColumn == null)
            {
                throw new Exception($"{tableMapper.TypeName} class does not define a primary key");
            }
            return $"delete from {tableMapper.TableName} where {keyColumn.ColumnName} = @{keyColumn.PropertyName};";
        }
    }
}


