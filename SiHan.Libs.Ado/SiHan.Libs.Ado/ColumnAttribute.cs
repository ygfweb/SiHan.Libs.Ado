using System;
using System.Collections.Generic;
using System.Text;

namespace SiHan.Libs.Ado
{
    /// <summary>
    /// 列特性
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 列名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 值转换器
        /// </summary>
        public Type Convert { get; set; } = null;

        /// <summary>
        /// 数据库类型（用于生成数据表）
        /// </summary>
        public string DbType { get; set; } = "";

        /// <summary>
        /// 数据库默认值（用于生成数据表）
        /// </summary>
        public string DbDefaultValue { get; set; } = "";
    }
}

