using Component.Base;
using Domain;
using Manager;
using Service;

namespace Component
{
    public class EmployeeInfoComponent : BaseComponent<EmployeeInfo, EmployeeInfoManager>, EmployeeInfoService
    {
        public bool Exists(string employeeCode)
        {
            return manager.Exists(employeeCode);
        }
    }
}
