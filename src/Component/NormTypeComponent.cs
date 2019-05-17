using Domain;
using Service;
using Manager;
using Component.Base;

namespace Component
{
    public class NormTypeComponent : BaseComponent<NormType, NormTypeManager>, NormTypeService
    {
    }
}
