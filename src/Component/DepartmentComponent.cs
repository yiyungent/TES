using Component.Base;
using Domain;
using Manager;
using Service;

namespace Component
{
    public class DepartmentComponent : BaseComponent<Department, DepartmentManager>, DepartmentService
    {
    }
}
