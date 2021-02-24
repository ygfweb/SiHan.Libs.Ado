using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SiHan.Libs.Ado
{
    /// <summary>
    /// DbConnection扩展类
    /// </summary>
    public static class DbConnectionExtensions
    {
        /// <summary>
        /// 默认映射规则
        /// </summary>
        public static MapScheme DefaultMapScheme { get; set; } = MapScheme.OriginalName;

        /// <summary>
        /// 查询实体列表
        /// </summary>
        public static async Task<List<T>> SelectAsync<T>(this DbConnection connection, string sql, object param = null) where T : BaseEntity
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentNullException(nameof(sql));
            }
            List<T> objList = new List<T>();
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                if (param != null)
                {
                    command.Parameters.Clear();
                    command.AppendAnonymousParameters(param);
                }
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        T obj = reader.ToEntity<T>();
                        objList.Add(obj);
                    }
                }
            }
            return objList;
        }

        /// <summary>
        /// 查询实体列表
        /// </summary>
        public static async Task<List<T>> SelectAsync<T>(this DbConnection connection, SqlBuilder builder) where T : BaseEntity
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            var buildResult = builder.GetQuery();
            string sql = buildResult.Item1;
            Dictionary<string, object> param = buildResult.Item2;
            List<T> objList = new List<T>();
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                if (param != null)
                {
                    command.Parameters.Clear();
                    command.AppendDictionaryParameters(param);
                }
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        T obj = reader.ToEntity<T>();
                        objList.Add(obj);
                    }
                }
            }
            return objList;
        }

        /// <summary>
        /// 获取表中的所有记录
        /// </summary>
        public static async Task<List<T>> GetAllAsync<T>(this DbConnection connection) where T : BaseEntity
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            List<T> objList = new List<T>();
            TableMapper mapper = MappingCachePool.GetOrAdd<T>();
            using (DbCommand command = connection.CreateCommand())
            {

                command.CommandText = $"select * from {mapper.TableName};";
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        T obj = reader.ToEntity<T>();
                        objList.Add(obj);
                    }
                }
            }
            return objList;
        }

        /// <summary>
        /// 通过主键值查询单个实体
        /// </summary>
        public static async Task<T> SingleByIdAsync<T>(this DbConnection connection, object idValue) where T : BaseEntity
        {
            if (idValue == null || string.IsNullOrWhiteSpace(idValue.ToString()))
            {
                throw new ArgumentNullException(nameof(idValue));
            }
            TableMapper mapper = MappingCachePool.GetOrAdd<T>();
            ColumnMapper column = mapper.KeyColumn;
            if (column == null)
            {
                throw new Exception($"The {mapper.TypeName} class does not define a primary key.");
            }
            else
            {
                string sql = $"select * from {mapper.TableName} where {column.ColumnName} = @p;";
                object dbValue = idValue;
                if (column.ValueConvert != null)
                {
                    dbValue = column.ValueConvert.Write(idValue);
                }
                List<T> list = await connection.SelectAsync<T>(sql, new { p = dbValue });
                if (list.Count == 0)
                {
                    return null;
                }
                else
                {
                    return list[0];
                }
            }
        }


        /// <summary>
        /// 查询单个实体
        /// </summary>
        public static async Task<T> FirstOrDefaultAsync<T>(this DbConnection connection, string sql, object param = null) where T : BaseEntity
        {
            List<T> list = await SelectAsync<T>(connection, sql, param);
            if (list.Count == 0)
            {
                return null;
            }
            else
            {
                return list[0];
            }
        }

        /// <summary>
        /// 获取实体表内的记录总数
        /// </summary>
        public static async Task<int> CountAsync<T>(this DbConnection connection) where T : BaseEntity
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            TableMapper tableMapper = MappingCachePool.GetOrAdd<T>();
            using (DbCommand command = connection.CreateCommand())
            {
                string sql = $"select count(*) from {tableMapper.TableName};";
                command.CommandText = sql;
                object result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }

        /// <summary>
        /// 获取标量值
        /// </summary>
        public static async Task<T> ScalarAsync<T>(this DbConnection connection, string sql, object param = null)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentNullException(nameof(sql));
            }
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Clear();
                if (param != null)
                {
                    command.AppendAnonymousParameters(param);
                }
                object result = await command.ExecuteScalarAsync();
                return (T)result;
            }
        }

        /// <summary>
        /// 获取标量值
        /// </summary>
        public static async Task<T> ScalarAsync<T>(this DbConnection connection, SqlBuilder builder)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            var buildResult = builder.GetQuery();
            string sql = buildResult.Item1;
            Dictionary<string, object> param = buildResult.Item2;
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Clear();
                if (param != null)
                {
                    command.AppendDictionaryParameters(param);
                }
                object result = await command.ExecuteScalarAsync();
                return (T)result;
            }
        }

        /// <summary>
        /// 执行非查询命令
        /// </summary>
        public static async Task<int> ExecuteNonQueryAsync(this DbConnection connection, string sql, object param = null, DbTransaction transaction = null)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentNullException(nameof(sql));
            }
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                if (transaction != null)
                {
                    command.Transaction = transaction;
                }
                command.Parameters.Clear();
                if (param != null)
                {
                    command.AppendAnonymousParameters(param);
                }
                return await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// 插入实体对象
        /// </summary>
        public static async Task<int> InsertAsync<T>(this DbConnection connection, T obj, DbTransaction transaction = null) where T : BaseEntity
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            using (DbCommand command = connection.CreateCommand())
            {
                if (transaction != null)
                {
                    command.Transaction = transaction;
                }
                string sql = CodeSegmentHelper.GenerateInsertql<T>();
                command.CommandText = sql;
                command.Parameters.Clear();
                command.AppendEntityParameters<T>(obj);
                return await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// 插入实体列表
        /// </summary>
        public static async Task<int> InsertAsync<T>(this DbConnection connection, IEnumerable<T> objList, DbTransaction transaction = null) where T : BaseEntity
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (objList == null)
            {
                throw new ArgumentNullException(nameof(objList));
            }
            int count = 0;
            using (DbCommand command = connection.CreateCommand())
            {
                if (transaction != null)
                {
                    command.Transaction = transaction;
                }
                string sql = CodeSegmentHelper.GenerateInsertql<T>();
                command.CommandText = sql;
                foreach (var item in objList)
                {
                    command.Parameters.Clear();
                    command.AppendEntityParameters<T>(item);
                    int result = await command.ExecuteNonQueryAsync();
                    count = result + count;
                }
            }
            return count;
        }

        /// <summary>
        /// 从数据库中删除一个对象
        /// </summary>
        public static async Task<int> DeleteAsync<T>(this DbConnection connection, T obj, DbTransaction transaction = null) where T : BaseEntity
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            TableMapper tableMapper = MappingCachePool.GetOrAdd<T>();
            using (DbCommand command = connection.CreateCommand())
            {
                if (transaction != null)
                {
                    command.Transaction = transaction;
                }
                command.CommandText = CodeSegmentHelper.GenerateDeleteSql<T>();
                command.Parameters.Clear();
                command.AppendEntityParameters<T>(obj);
                return await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        public static async Task<int> DeleteAsync<T>(this DbConnection connection, IEnumerable<T> objList, DbTransaction transaction) where T : BaseEntity
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (objList == null)
            {
                throw new ArgumentNullException(nameof(objList));
            }
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            int count = 0;
            using (DbCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = CodeSegmentHelper.GenerateDeleteSql<T>();
                foreach (var item in objList)
                {
                    if (item != null)
                    {
                        command.Parameters.Clear();
                        command.AppendEntityParameters<T>(item);
                        int result = await command.ExecuteNonQueryAsync();
                        count = result + count;
                    }
                }
            }
            return count;
        }


        /// <summary>
        /// 通过ID值删除一个对象
        /// </summary>
        public static async Task<int> DeleteByIdAsync<T>(this DbConnection connection, object idValue, DbTransaction transaction = null) where T : BaseEntity
        {
            if (idValue == null || string.IsNullOrWhiteSpace(idValue.ToString()))
            {
                throw new ArgumentNullException(nameof(idValue));
            }
            TableMapper tableMapper = MappingCachePool.GetOrAdd<T>();
            ColumnMapper keyColumn = tableMapper.KeyColumn;
            if (tableMapper.KeyColumn == null)
            {
                throw new Exception(tableMapper.TypeName + " does not define a primary key");
            }
            object dbValue = idValue;
            if (keyColumn.ValueConvert != null)
            {
                dbValue = keyColumn.ValueConvert.Write(idValue);
            }
            string sql = $"delete from {tableMapper.TableName} where {keyColumn.ColumnName} = @p;";
            return await ExecuteNonQueryAsync(connection, sql, new { p = dbValue }, transaction);
        }


        /// <summary>
        /// 删除表中所有数据
        /// </summary>
        public static async Task<int> DeleteAllAsync<T>(this DbConnection connection, DbTransaction transaction = null) where T : BaseEntity
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            TableMapper tableMapper = MappingCachePool.GetOrAdd<T>();
            using (DbCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = $"DELETE FROM {tableMapper.TableName};";
                command.Parameters.Clear();
                return await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        public static async Task<int> UpdateAsync<T>(this DbConnection connection, T obj, DbTransaction transaction = null) where T : BaseEntity
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            using (DbCommand command = connection.CreateCommand())
            {
                if (transaction != null)
                {
                    command.Transaction = transaction;
                }
                string sql = CodeSegmentHelper.GenerateUpdateSql<T>();
                command.CommandText = sql;
                command.Parameters.Clear();
                command.AppendEntityParameters<T>(obj);
                return await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        public static async Task<int> UpdateAsync<T>(this DbConnection connection, IEnumerable<T> objList, DbTransaction transaction) where T : BaseEntity
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (objList == null)
            {
                throw new ArgumentNullException(nameof(objList));
            }
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            int count = 0;
            using (DbCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                string sql = CodeSegmentHelper.GenerateUpdateSql<T>();
                command.CommandText = sql;
                foreach (var item in objList)
                {
                    if (item != null)
                    {
                        command.Parameters.Clear();
                        command.AppendEntityParameters<T>(item);
                        int result = await command.ExecuteNonQueryAsync();
                        count = result + count;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        public static async Task CreateTableAsync<T>(this DbConnection connection) where T : BaseEntity
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            await connection.ExecuteNonQueryAsync(DbTableHelper.CreateTableSQL<T>());
            string createIndexSql = DbTableHelper.CreateIndexSQL<T>();
            if (!string.IsNullOrWhiteSpace(createIndexSql))
            {
                await connection.ExecuteNonQueryAsync(createIndexSql);
            }
        }
    }
}

