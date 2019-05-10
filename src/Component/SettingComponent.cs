using Component.Base;
using Domain;
using Manager;
using Service;

namespace Component
{
    public class SettingComponent : BaseComponent<Setting, SettingManager>, SettingService
    {
    }
}
