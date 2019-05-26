using Domain;
using Service.Base;

namespace Service
{
    public interface StudentInfoService : BaseService<StudentInfo>
    {
        bool Exist(string studentCode, int exceptId = 0);

        /// <summary>
        /// 是否已经绑定此用户
        /// </summary>
        /// <param name="studentCode">此学生学号</param>
        /// <param name="exceptUserId">排除的用户ID</param>
        /// <returns></returns>
        bool IsBindUser(string studentCode, int exceptUserId);

        /// <summary>
        /// 是否已经绑定用户
        /// </summary>
        /// <param name="studentCode">此学生学号</param>
        /// <returns>此学生已经绑定，返回 True</returns>
        bool IsBindUser(string studentCode);
    }
}
