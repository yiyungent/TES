using Domain;
using Service;
using Manager;
using Component.Base;

namespace Component
{
    public class NormTypeComponent : BaseComponent<NormType, NormTypeManager>, NormTypeService
    {
        public bool Exist(string normTypeCode, int exceptId = 0)
        {
            return manager.Exist(normTypeCode: normTypeCode, exceptId: exceptId);
        }
    }
}
