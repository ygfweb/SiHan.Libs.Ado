using System;
using System.Collections.Generic;
using System.Text;

namespace SiHan.Libs.Ado
{
    /// <summary>
    /// 将数据库中的GUID类型转换成字符串
    /// </summary>
    public class GuidValueConvert : BaseValueConvert
    {
        public override object Read(object dbValue)
        {
            Guid guid = (Guid)dbValue;
            if (guid == null)
            {
                return "";
            }
            else
            {
                return guid.ToString();
            }
        }

        public override object Write(object propertyValue)
        {
            if (propertyValue == null || string.IsNullOrWhiteSpace(propertyValue.ToString()))
            {
                return null;
            }
            else
            {
                return Guid.Parse(propertyValue.ToString());
            }
        }
    }
}
