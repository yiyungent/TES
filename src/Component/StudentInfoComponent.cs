using Component.Base;
using Domain;
using Manager;
using Service;

namespace Component
{
    public class StudentInfoComponent : BaseComponent<StudentInfo, StudentInfoManager>, StudentInfoService
    {
    }
}
