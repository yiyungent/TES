using Domain;
using Service;
using Manager;
using Component.Base;

namespace Component
{
    public class EvaResultComponent : BaseComponent<EvaResult, EvaResultManager>, EvaResultService
    {
    }
}
