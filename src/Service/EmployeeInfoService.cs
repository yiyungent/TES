using Domain;
using Service.Base;
namespace Service
{
    public interface EmployeeInfoService : BaseService<EmployeeInfo>
    {
        bool Exist(string employeeCode, int exceptId = 0);

        /// <summary>
        /// 是否已经绑定此用户
        /// </summary>
        /// <param name="employeeCode">此员工工号</param>
        /// <param name="exceptUserId">排除的用户ID</param>
        /// <returns></returns>
        bool IsBindUser(string employeeCode, int exceptUserId);

        /// 是否已经绑定用户
        /// </summary>
        /// <param name="employeeCode">此员工工号</param>
        /// <returns>此员工已经绑定，返回 True</returns>
        bool IsBindUser(string employeeCode);
    }
}
