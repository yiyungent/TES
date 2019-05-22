using Domain;
using Manager.Base;
using NHibernate.Criterion;

namespace Manager
{
    public class StudentInfoManager : BaseManager<StudentInfo>
    {
        public bool Exists(string studentCode)
        {
            bool isExist = Count(Expression.Eq("StudentCode", studentCode)) > 0;

            return isExist;
        }
    }
}
