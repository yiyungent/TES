using Domain;
using Manager.Base;
using NHibernate.Criterion;

namespace Manager
{
    public class EmployeeInfoManager : BaseManager<EmployeeInfo>
    {
        public bool Exists(string employeeCode)
        {
            bool isExist = Count(Expression.Eq("EmployeeCode", employeeCode)) > 0;

            return isExist;
        }
    }
}
