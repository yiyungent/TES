using Domain;
using Service;
using Manager;
using Component.Base;

namespace Component
{
    public class EvaTaskComponent : BaseComponent<EvaTask, EvaTaskManager>, EvaTaskService
    {
        public bool Exist(string evaTaskCode, int exceptId = 0)
        {
            return manager.Exist(evaTaskCode: evaTaskCode, exceptId: exceptId);
        }
    }
}
