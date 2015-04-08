using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JFx.Utils
{
    /// <summary>
    /// 分页信息
    /// </summary>
    public class PageItem
    {
        private int pageIndex;
        /// <summary>
        /// 当前页下标
        /// </summary>
        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value; }
        }

        private int pageSize;
        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }

        private int totalCount;
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount
        {
            get { return totalCount; }
            set { totalCount = value; }
        }
    }
}
