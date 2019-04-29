using Component.Base;
using Domain;
using Manager;
using Service;

namespace Component
{
    public class TeacherInfoComponent : BaseComponent<TeacherInfo, TeacherInfoManager>, TeacherInfoService
    {
    }
}
