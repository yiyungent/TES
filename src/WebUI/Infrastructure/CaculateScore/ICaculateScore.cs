using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUI.Infrastructure.CaculateScore
{
    public interface ICaculateScore
    {
        /// <summary>
        /// 计算分数并保存评价结果
        /// </summary>
        /// <param name="evaTask"></param>
        /// <param name="normType"></param>
        /// <param name="employeeInfo"></param>
        void Caculate(EvaTask evaTask, NormType normType, EmployeeInfo employeeInfo);
    }
}
