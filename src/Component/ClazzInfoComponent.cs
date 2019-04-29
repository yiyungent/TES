using Component.Base;
using Domain;
using Manager;
using Service;

namespace Component
{
    public class ClazzInfoComponent : BaseComponent<ClazzInfo, ClazzInfoManager>, ClazzInfoService
    {
    }
}
