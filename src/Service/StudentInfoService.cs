using Domain;
using Service.Base;

namespace Service
{
    public interface StudentInfoService : BaseService<StudentInfo>
    {
        bool Exists(string studentCode);
    }
}
