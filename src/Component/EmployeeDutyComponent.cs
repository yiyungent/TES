using Component.Base;
using Domain;
using Manager;
using Service;

namespace Component
{
    public class EmployeeDutyComponent : BaseComponent<EmployeeDuty, EmployeeDutyManager>, EmployeeDutyService
    {
    }
}
