using Component.Base;
using Domain;
using Manager;
using Service;


namespace Component
{
    public class UserInfoComponent : BaseComponent<UserInfo, UserInfoManager>, UserInfoService
    {
    }
}
