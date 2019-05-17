using Domain;
using Service;
using Manager;
using Component.Base;

namespace Component
{
    public class EvaTaskComponent : BaseComponent<EvaTask, EvaTaskManager>, EvaTaskService
    {
    }
}
