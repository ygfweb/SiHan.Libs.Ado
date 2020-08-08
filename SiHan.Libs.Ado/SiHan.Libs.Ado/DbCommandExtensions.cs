using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace SiHan.Libs.Ado
{
    public static class DbCommandExtensions
    {
        /// <summary>
        /// 使用匿名对象为命令追加参数
        /// </summary>
        /// <param name="command">命令对象</param>
        /// <param name="paramObject">匿名对象</param>
        public static void AppendAnonymousParameters(this DbCommand command, object paramObject)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            if (paramObject != null)
            {
                foreach (PropertyInfo pInfo in paramObject.GetType().GetProperties())
                {
                    var parameter = command.CreateParameter();
                    string name = pInfo.Name;
                    object value = pInfo.GetValue(paramObject, null);
                    parameter.ParameterName = "@" + name;
                    if (value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else
                    {
                        parameter.Value = value;
                    }
                    command.Parameters.Add(parameter);
                }
            }
        }
        /// <summary>
        /// 使用实体类为命令追加参数
        /// </summary>
        public static void AppendEntityParameters<T>(this DbCommand command, T obj) where T : BaseEntity
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            TableMapper tableMapper = MappingCachePool.GetOrAdd<T>();
            var columns = tableMapper.Columns;
            foreach (var item in columns)
            {
                DbParameter parameter = command.CreateParameter();
                parameter.ParameterName = "@" + item.Value.PropertyName;
                object value = item.Value.GetPropertyValue(obj);
                if (value == null)
                {
                    parameter.Value = DBNull.Value;
                }
                else
                {
                    BaseValueConvert valueConvert = item.Value.ValueConvert as BaseValueConvert;
                    if (valueConvert != null)
                    {
                        parameter.Value = valueConvert.Write(value);
                    }
                    else
                    {
                        parameter.Value = value;
                    }
                }
                command.Parameters.Add(parameter);
            }
        }
    }
}
