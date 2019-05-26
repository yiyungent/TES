using Component.Base;
using Domain;
using Manager;
using Service;

namespace Component
{
    public class StudentInfoComponent : BaseComponent<StudentInfo, StudentInfoManager>, StudentInfoService
    {
        public bool Exist(string studentCode, int exceptId = 0)
        {
            return manager.Exist(studentCode, exceptId: exceptId);
        }

        /// <summary>
        /// 是否已经绑定此用户
        /// </summary>
        /// <param name="studentCode">此学生学号</param>
        /// <param name="exceptUserId">排除的用户ID</param>
        /// <returns></returns>
        public bool IsBindUser(string studentCode, int exceptUserId)
        {
            return manager.IsBindUser(studentCode, exceptUserId);
        }

        /// <summary>
        /// 是否已经绑定用户
        /// </summary>
        /// <param name="studentCode">此学生学号</param>
        /// <returns>此学生已经绑定，返回 True</returns>
        public bool IsBindUser(string studentCode)
        {
            return manager.IsBindUser(studentCode);
        }
    }
}
