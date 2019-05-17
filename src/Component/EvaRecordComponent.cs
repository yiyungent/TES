using Domain;
using Service;
using Manager;
using Component.Base;

namespace Component
{
    public class EvaRecordComponent : BaseComponent<EvaRecord, EvaRecordManager>, EvaRecordService
    {
    }
}
