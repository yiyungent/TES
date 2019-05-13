using Domain;
using Service.Base;

namespace Service
{
    public interface CourseInfoService : BaseService<CourseInfo>
    {
        bool CheckCourseInfo(CourseInfo model, out string message);
    }
}
