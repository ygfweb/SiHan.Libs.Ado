using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace SiHan.Libs.Ado
{
    /// <summary>
    /// DbCommand扩展类
    /// </summary>
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
                Type paramType = paramObject.GetType();
                foreach (PropertyInfo pInfo in paramType.GetProperties())
                {
                    var parameter = command.CreateParameter();
                    string name = pInfo.Name;
                    object value = pInfo.GetValue(paramObject, null);
                    parameter.ParameterName = "@" + name;
                    if (value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else if (pInfo.PropertyType.IsEnum)
                    {
                        parameter.Value = Convert.ToInt16(value);
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
                        if (item.Value.IsEnum)
                        {
                            parameter.Value = Convert.ToInt16(value);
                        }
                        else if (item.Value.IsGuidString)
                        {
                            parameter.Value = Guid.Parse(value.ToString());
                        }
                        else
                        {
                            parameter.Value = value;
                        }
                    }
                }
                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// 使用字典为命令追加参数
        /// </summary>
        public static void AppendDictionaryParameters(this DbCommand command, Dictionary<string, object> keyValues)
        {
            if (keyValues == null)
            {
                throw new ArgumentNullException(nameof(keyValues));
            }
            foreach (var item in keyValues)
            {
                var parameter = command.CreateParameter();
                string name = item.Key;
                object value = item.Value;
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
}
