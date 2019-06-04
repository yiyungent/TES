using Domain;
using Framework.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Admin.Models.EvaResultVM
{
    public class EvaResultListViewModel
    {
        public IList<EvaResultVMItem> List { get; set; }

        public PageInfo PageInfo { get; set; }
    }

    /// <summary>
    /// 每一项: eg: 评价任务1--教师2--关联的全部（全部评价类型）评价记录  
    /// </summary>
    public class EvaResultVMItem
    {
        public EvaTask EvaTask { get; set; }

        /// <summary>
        /// 被评价教师/员工
        /// </summary>
        public EmployeeInfo EvaedEmployee { get; set; }

        /// <summary>
        /// 某个评价类型为多少分
        /// </summary>
        public Dictionary<NormType, decimal> ScoreDic { get; set; }

        /// <summary>
        /// 各个评价类型加起来的 总分
        /// </summary>
        public decimal ScoreSum
        {
            get
            {
                decimal rtnSum = 0;
                foreach (var item in ScoreDic)
                {
                    rtnSum += item.Key.Weight * item.Value;
                }

                return rtnSum;
            }
        }
    }
}