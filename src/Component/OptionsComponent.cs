using Domain;
using Service;
using Manager;
using Component.Base;

namespace Component
{
    public class OptionsComponent : BaseComponent<Options, OptionsManager>, OptionsService
    {
    }
}
