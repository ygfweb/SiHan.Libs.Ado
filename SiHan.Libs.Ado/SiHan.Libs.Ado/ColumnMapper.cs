using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SiHan.Libs.Ado
{
    /// <summary>
    /// 列映射
    /// </summary>
    internal class ColumnMapper
    {
        /// <summary>
        /// 列名称
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsPrimarykey { get; set; }

        /// <summary>
        /// 值转换器
        /// </summary>
        public BaseValueConvert ValueConvert { get; set; }

        /// <summary>
        /// 是否自动增量
        /// </summary>
        public bool IsAuto { get; set; } = false;

        /// <summary>
        /// 是否是枚举类型
        /// </summary>
        public bool IsEnum { get; set; } = false;

        /// <summary>
        /// 是否是Guid的字符串类型
        /// </summary>
        public bool IsGuidString { get; set; } = false;

        /// <summary>
        /// 是否可为空
        /// </summary>
        public bool IsCanNull { get; set; } = false;

        /// <summary>
        /// 是否是索引列
        /// </summary>
        public bool IsIndex { get; set; } = false;

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType { get; set; } = "";

        /// <summary>
        /// 数据库默认值
        /// </summary>
        public string DbDefaultValue { get; set; } = "";

        /// <summary>
        /// 获取实例对象的属性值
        /// </summary>
        public object GetPropertyValue(object objectInstance)
        {
            return PropertyInfo.GetValue(objectInstance);
        }
        /// <summary>
        /// 获取实例对象待更新的数据库值
        /// </summary>
        public object GetDataValue(object objectInstance)
        {
            object pValue = this.GetPropertyValue(objectInstance);
            if (ValueConvert!=null)
            {
                return ValueConvert.Write(pValue);
            }
            else
            {
                return pValue;
            }
        }
    }
}
