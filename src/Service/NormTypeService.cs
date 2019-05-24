using Domain;
using Service.Base;

namespace Service
{
    public interface NormTypeService : BaseService<NormType>
    {
        bool Exists(string normTypeCode, int exceptId = 0);
    }
}
