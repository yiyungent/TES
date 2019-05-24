using Domain;
using Manager.Base;
using NHibernate.Criterion;

namespace Manager
{
    public class NormTypeManager : BaseManager<NormType>
    {
        public bool Exist(string normTypeCode, int exceptId = 0)
        {
            bool isExist = Count(Expression.And(
                                Expression.Eq("NormTypeCode", normTypeCode),
                                Expression.Not(Expression.Eq("ID", exceptId))
                            )) > 0;

            return isExist;
        }
    }
}
