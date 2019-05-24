using Domain;
using Service;
using Manager;
using Component.Base;

namespace Component
{
    public class NormTypeComponent : BaseComponent<NormType, NormTypeManager>, NormTypeService
    {
        public bool Exists(string normTypeCode, int exceptId = 0)
        {
            return manager.Exists(normTypeCode: normTypeCode, exceptId: exceptId);
        }
    }
}
