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
    }
}

