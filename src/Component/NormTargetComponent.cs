using Domain;
using Service;
using Manager;
using Component.Base;

namespace Component
{
    public class NormTargetComponent : BaseComponent<NormTarget, NormTargetManager>, NormTargetService
    {
    }
}
