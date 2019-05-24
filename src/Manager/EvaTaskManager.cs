using Domain;
using Manager.Base;
using NHibernate.Criterion;

namespace Manager
{
    public class EvaTaskManager : BaseManager<EvaTask>
    {
        public bool Exist(string evaTaskCode, int exceptId = 0)
        {
            bool isExist = Count(Expression.And(
                                Expression.Eq("EvaTaskCode", evaTaskCode),
                                Expression.Not(Expression.Eq("ID", exceptId))
                            )) > 0;

            return isExist;
        }
    }
}
