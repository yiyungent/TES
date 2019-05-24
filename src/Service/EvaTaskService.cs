using Domain;
using Service.Base;

namespace Service
{
    public interface EvaTaskService : BaseService<EvaTask>
    {
        bool Exist(string evaTaskCode, int exceptId = 0);
    }
}
