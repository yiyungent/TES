using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core;
using Domain;
using NHibernate.Criterion;
using Service;

namespace WebUI.Infrastructure.CaculateScore
{
    public class CaculateScore : ICaculateScore
    {
        /// <summary>
        /// 计算分数并保存评价结果
        /// </summary>
        /// <param name="evaTask"></param>
        /// <param name="normType"></param>
        /// <param name="employeeInfo"></param>
        public void Caculate(EvaTask evaTask, NormType normType, EmployeeInfo employeeInfo)
        {
            IList<NormTarget> allTarget = Container.Instance.Resolve<NormTargetService>().GetAll();
            IList<EvaRecord> allRecord = Container.Instance.Resolve<EvaRecordService>().Query(new List<ICriterion>
            {
                Expression.Eq("EvaluateTask.ID", evaTask.ID),
                Expression.Eq("NormType.ID", normType.ID),
                Expression.Eq("Teacher.ID", employeeInfo.ID)
            });

            // 按评价人排序
            //allRecord = allRecord.OrderBy(m => m.SysUser.ID).ToList();

            // 计算分数和+参评人员
            int personCount = 0; // 参评人数
            int userId = 0; // 评价人ID，用于计算参评人数
            decimal sumScore = 0;
            decimal avgScore = 0;
            foreach (var item in allRecord)
            {
                if (item.Evaluator.ID != userId)
                {
                    personCount = personCount + 1;

                    userId = item.Evaluator.ID;
                }
                // 递归确定权重（含归一化处理）
                decimal weight = GetWeight(item.NormTarget, allTarget);

                sumScore = sumScore + item.Options.Score * weight;
            }
            // 求平均数
            if (personCount != 0)
            {
                avgScore = sumScore / personCount;
            }
            // 判断是否已经存在 此评价结果-创建 / 更新（重新计算）
            EvaResult existEvaResult = Container.Instance.Resolve<EvaResultService>().Query(
                new List<ICriterion> {
                    Expression.Eq("Teacher.ID", employeeInfo.ID),
                    Expression.Eq("NormType.ID", normType.ID),
                    Expression.Eq("EvaluateTask.ID", evaTask.ID)
            }
            ).FirstOrDefault();
            if (existEvaResult != null)
            {
                // 更新 计算结果EvaResult
                existEvaResult.CaculateTime = DateTime.Now;
                existEvaResult.Score = avgScore;
                Container.Instance.Resolve<EvaResultService>().Edit(existEvaResult);
            }
            else
            {
                // 创建 计算结果EvaResult
                Container.Instance.Resolve<EvaResultService>().Create(new EvaResult
                {
                    Teacher = employeeInfo,
                    NormType = normType,
                    CaculateTime = DateTime.Now, // 计算分数时间
                    EvaluateTask = evaTask,
                    Score = avgScore
                });
            }
        }

        public void Caculate(int evaTaskId, int normTypeId, int employeeInfoId)
        {
            Caculate(new EvaTask { ID = evaTaskId }, new NormType { ID = normTypeId }, new EmployeeInfo { ID = employeeInfoId });
        }

        #region 递归获取权重（含归一化处理）
        private decimal GetWeight(NormTarget self, IList<NormTarget> all)
        {
            if (self.ParentTarget == null)
            {
                var findBrother = from m in all
                                  where m.ParentTarget == null
                                  && m.NormType != null
                                  && self.NormType != null
                                  && m.NormType.ID == self.NormType.ID
                                  select m;

                decimal sumWeight = 0;
                foreach (var item in findBrother)
                {
                    sumWeight = sumWeight + item.Weight;
                }

                if (sumWeight == 0) sumWeight = 1; // 避免0做除数

                return self.Weight / sumWeight;
            }
            else
            {
                var findBrother = from m in all
                                  where m.ParentTarget != null
                                  && m.ParentTarget != null
                                  && self.ParentTarget != null
                                  && m.ParentTarget.ID == self.ParentTarget.ID
                                  && m.NormType != null
                                  && self.NormType != null
                                  && m.NormType.ID == self.NormType.ID
                                  select m;
                decimal sumWeight = 0;
                foreach (var item in findBrother)
                {
                    sumWeight = sumWeight + item.Weight;
                }
                if (sumWeight == 0) sumWeight = 1;

                // 需要递归
                return self.Weight / sumWeight * GetWeight(self.ParentTarget, all);
            }
        }
        #endregion
    }
}