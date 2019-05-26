using Component.Base;
using Domain;
using Manager;
using Service;

namespace Component
{
    public class EmployeeInfoComponent : BaseComponent<EmployeeInfo, EmployeeInfoManager>, EmployeeInfoService
    {
        public bool Exist(string employeeCode, int exceptId = 0)
        {
            return manager.Exist(employeeCode, exceptId: exceptId);
        }

        /// <summary>
        /// 是否已经绑定此用户
        /// </summary>
        /// <param name="employeeCode">此员工工号</param>
        /// <param name="exceptUserId">排除的用户ID</param>
        /// <returns></returns>
        public bool IsBindUser(string employeeCode, int exceptUserId)
        {
            return manager.IsBindUser(employeeCode, exceptUserId);
        }

        /// <summary>
        /// 是否已经绑定用户
        /// </summary>
        /// <param name="employeeCode">此员工工号</param>
        /// <returns>此员工已经绑定，返回 True</returns>
        public bool IsBindUser(string employeeCode)
        {
            return manager.IsBindUser(employeeCode);
        }
    }
}
