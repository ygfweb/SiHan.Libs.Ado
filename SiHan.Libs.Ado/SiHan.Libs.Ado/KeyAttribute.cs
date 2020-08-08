using System;
using System.Collections.Generic;
using System.Text;

namespace SiHan.Libs.Ado
{
    /// <summary>
    /// 主键特性
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class KeyAttribute : Attribute
    {
        /// <summary>
        /// 是否由数据库自动生成
        /// </summary>
        public bool IsAuto { get; set; } = false;
    }
}
