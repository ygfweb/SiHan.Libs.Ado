using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SiHan.Libs.Ado
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    internal class ReflectionHelper
    {
        /// <summary>
        /// 获取翻译后的名称
        /// </summary>
        public static string GetName(string originalName)
        {
            if (string.IsNullOrWhiteSpace(originalName))
            {
                throw new ArgumentNullException(nameof(originalName));
            }
            if (DbConnectionExtensions.DefaultMapScheme == MapScheme.UnderScoreCase)
            {
                return StringHelper.PascalCaseToUnderscores(originalName);
            }
            else
            {
                return originalName.Trim();
            }
        }

        /// <summary>
        /// 检查类型是否是可空泛型
        /// </summary>
        public static bool IsNullable(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static TableMapper ToMapper<T>() where T : BaseEntity
        {
            Type type = typeof(T);
            TableAttribute tableAttribute = type.GetCustomAttribute<TableAttribute>();
            string tableName;
            if (tableAttribute != null && !string.IsNullOrWhiteSpace(tableAttribute.Name))
            {
                tableName = tableAttribute.Name.Trim();
            }
            else
            {
                tableName = GetName(type.Name);
            }
            TableMapper tableMapper = new TableMapper() { TableName = tableName, Type = type, TypeName = type.Name };
            List<PropertyInfo> pList = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            foreach (PropertyInfo property in pList)
            {
                // 检查 IgnoreAttribute
                IgnoreAttribute ignore = property.GetCustomAttribute<IgnoreAttribute>();
                if (ignore != null)
                {
                    continue;
                }
                // 检查 ColumnAttribute
                bool isKey;
                bool isAuto = false;
                KeyAttribute key = property.GetCustomAttribute<KeyAttribute>();
                if (key != null)
                {
                    isKey = true;
                    isAuto = key.IsAuto;
                }
                else if (string.Equals(property.Name, "id", StringComparison.OrdinalIgnoreCase))
                {
                    // 当属性名为ID，但又未指定Key特性时，按以下规则判断，如果是整数，则为自动增量，否则不是自动增量。
                    string propertyTypeName = property.PropertyType.FullName;
                    if (propertyTypeName == "System.Int32" || propertyTypeName == "System.UInt32" || propertyTypeName == "System.Int64" || propertyTypeName == "System.UInt64")
                    {
                        isAuto = true;
                    }
                    else
                    {
                        isAuto = false;
                    }
                    isKey = true;
                }
                else
                {
                    isKey = false;
                    isAuto = false;
                }
                ColumnAttribute columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
                IndexAttribute indexAttribute = property.GetCustomAttribute<IndexAttribute>();
                string columnName = "";
                BaseValueConvert valueConvert = null;
                if (columnAttribute != null)
                {
                    // 获取列名
                    if (!string.IsNullOrWhiteSpace(columnAttribute.Name))
                    {
                        columnName = columnAttribute.Name.Trim();
                    }
                    else
                    {
                        columnName = GetName(property.Name);
                    }
                    // 获取 ValueConvert
                    if (columnAttribute.Convert != null)
                    {
                        Type convertType = columnAttribute.Convert;
                        if (convertType.IsAbstract)
                        {
                            throw new Exception($"{property.Name} value convert cannot be an abstract class.");
                        }
                        if (!convertType.IsSubclassOf(typeof(BaseValueConvert)))
                        {
                            string name = typeof(BaseValueConvert).Name;
                            throw new Exception($"{property.Name} value convert must inherit {name}.");
                        }
                        valueConvert = Activator.CreateInstance(convertType) as BaseValueConvert;
                    }
                }
                else
                {
                    columnName = GetName(property.Name);
                }
                // 创建映射实例
                ColumnMapper columnMapper = new ColumnMapper()
                {
                    ColumnName = columnName,
                    IsPrimarykey = isKey,
                    PropertyInfo = property,
                    PropertyName = property.Name,
                    ValueConvert = valueConvert,
                    IsAuto = isAuto,
                    IsEnum = property.PropertyType.IsEnum,
                    IsGuidString = property.PropertyType == typeof(string) && columnName.ToLower().Contains("id"),
                    IsCanNull = IsNullable(property.PropertyType),
                    IsIndex = indexAttribute != null,
                    DbType = (columnAttribute != null && !string.IsNullOrWhiteSpace(columnAttribute.DbType)) ? columnAttribute.DbType : "",
                    DbDefaultValue = (columnAttribute != null && !string.IsNullOrWhiteSpace(columnAttribute.DbDefaultValue)) ? columnAttribute.DbDefaultValue : ""
                };
                tableMapper.Add(columnMapper);
            }
            return tableMapper;
        }
    }
}
