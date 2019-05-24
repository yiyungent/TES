using Domain;
using Service.Base;

namespace Service
{
    public interface NormTypeService : BaseService<NormType>
    {
        bool Exist(string normTypeCode, int exceptId = 0);
    }
}
