using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiHan.Libs.Ado
{
    /// <summary>
    /// 表映射
    /// </summary>
    internal class TableMapper
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; } = "";

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; } = "";

        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; set; } = null;

        /// <summary>
        /// 列映射集合
        /// </summary>
        public Dictionary<string, ColumnMapper> Columns = new Dictionary<string, ColumnMapper>();

        /// <summary>
        /// 主键列
        /// </summary>
        public ColumnMapper KeyColumn { get; set; }

        public void Add(ColumnMapper column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }
            if (column.IsPrimarykey)
            {
                KeyColumn = column;
            }
            this.Columns.Add(column.ColumnName, column);
        }

        public bool IsExist(string columnName)
        {
            return this.Columns.ContainsKey(columnName);
        }
    }
}

