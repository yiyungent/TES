using Domain;
using Manager.Base;
using NHibernate.Criterion;
using System.Collections.Generic;

namespace Manager
{
    public class StudentInfoManager : BaseManager<StudentInfo>
    {
        public bool Exist(string studentCode, int exceptId = 0)
        {
            bool isExist = Count(Expression.And(
                                Expression.Eq("StudentCode", studentCode),
                                Expression.Not(Expression.Eq("ID", exceptId))
                            )) > 0;

            return isExist;
        }

        /// <summary>
        /// 是否已经绑定此用户
        /// </summary>
        /// <param name="studentCode">此学生学号</param>
        /// <param name="exceptUserId">排除的用户ID</param>
        /// <returns></returns>
        public bool IsBindUser(string studentCode, int exceptUserId)
        {
            bool isBind = Count(Expression.And(
                               Expression.Eq("StudentCode", studentCode),
                               Expression.Not(Expression.Eq("UID", exceptUserId))
                           )) > 0;
            if (isBind)
            {
                isBind = Count(Expression.And(
                               Expression.Eq("StudentCode", studentCode),
                               Expression.IsNull("UID")
                           )) <= 0;
            }

            return isBind;
        }

        /// <summary>
        /// 是否已经绑定用户
        /// </summary>
        /// <param name="studentCode">此学生学号</param>
        /// <returns>此学生已经绑定，返回 True</returns>
        public bool IsBindUser(string studentCode)
        {
            bool isBind = Count(Expression.And(
                               Expression.Eq("StudentCode", studentCode),
                               Expression.IsNotEmpty("UID")
                           )) <= 0;
            if (isBind)
            {
                // 注意：null 值在数据库中属于特殊，它代表未知，等于任何值
                // 还有一种情况，即 UID 为 null 时也表示 未绑定
                isBind = Count(Expression.And(
                               Expression.Eq("StudentCode", studentCode),
                               Expression.IsNotNull("UID")
                           )) <= 0;
            }

            return isBind;
        }
    }
}
