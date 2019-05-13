using Component.Base;
using Domain;
using Manager;
using NHibernate.Criterion;
using Service;
using System.Collections.Generic;
using System.Linq;

namespace Component
{
    public class CourseInfoComponent : BaseComponent<CourseInfo, CourseInfoManager>, CourseInfoService
    {
        public bool CheckCourseInfo(CourseInfo model, out string message)
        {
            bool isCan = true;
            message = "课程信息可用";

            // 查找 已经具有此课程名的 (非当前) 的课程
            if (!string.IsNullOrEmpty(model.Name))
            {
                CourseInfo use = Query(new List<ICriterion>
                {
                    Expression.And(
                        Expression.Eq("Name", model.Name.Trim()),
                        Expression.Not(Expression.Eq("ID", model.ID))
                    )
                }).FirstOrDefault();
                if (use != null)
                {
                    isCan = false;
                    message = "课程名已经存在，请更换课程名";
                    return isCan;
                }
            }

            if (!string.IsNullOrEmpty(model.CourseCode))
            {
                CourseInfo use = Query(new List<ICriterion>
                {
                    Expression.And(
                        Expression.Eq("CourseCode", model.Name.Trim()),
                        Expression.Not(Expression.Eq("ID", model.ID))
                    )
                }).FirstOrDefault();
                if (use != null)
                {
                    isCan = false;
                    message = "课程代号已经存在，请更换课程代号";
                    return isCan;
                }
            }

            return isCan;
        }
    }
}
