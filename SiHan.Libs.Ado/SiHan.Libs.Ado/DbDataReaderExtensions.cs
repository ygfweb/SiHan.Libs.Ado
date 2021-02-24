using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace SiHan.Libs.Ado
{
    public static class DbDataReaderExtensions
    {
        /// <summary>
        /// 将DbDataReader转换为实体
        /// </summary>
        public static T ToEntity<T>(this DbDataReader reader) where T : BaseEntity
        {
            TableMapper mapper = MappingCachePool.GetOrAdd<T>();
            T obj = Activator.CreateInstance<T>();
            foreach (var item in mapper.Columns)
            {
                ColumnMapper column = item.Value;
                PropertyInfo property = column.PropertyInfo;
                object value = reader[column.ColumnName];
                if (value == DBNull.Value)
                {
                    property.SetValue(obj, null);
                }
                else
                {
                    if (column.ValueConvert != null)
                    {
                        property.SetValue(obj, column.ValueConvert.Read(value));
                    }
                    else
                    {
                        if (column.IsGuidString)
                        {
                            Guid guid = (Guid)value;
                            property.SetValue(obj, guid.ToString());
                        }
                        else if (column.IsEnum)
                        {
                            property.SetValue(obj, Enum.ToObject(property.PropertyType, Convert.ToInt32(value)), null);
                        }
                        else
                        {
                            property.SetValue(obj, value);
                        }
                    }
                }
            }
            return obj;
        }
    }
}
