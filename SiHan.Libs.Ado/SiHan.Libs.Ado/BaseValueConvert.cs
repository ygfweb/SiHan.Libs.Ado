using System;
using System.Collections.Generic;
using System.Text;

namespace SiHan.Libs.Ado
{
    /// <summary>
    /// 值转换器抽象基类
    /// </summary>
    public abstract class BaseValueConvert
    {
        public BaseValueConvert() { }
        /// <summary>
        /// 将数据库中读取的值转换成对象的属性值
        /// </summary>
        public abstract object Read(object dbValue);

        /// <summary>
        /// 将对象的属性值转换成待更新的数据值
        /// </summary>
        public abstract object Write(object propertyValue);
    }
}

