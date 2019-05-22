using Domain;
using Service.Base;
namespace Service
{
    public interface EmployeeInfoService : BaseService<EmployeeInfo>
    {
        bool Exists(string employeeCode);
    }
}
