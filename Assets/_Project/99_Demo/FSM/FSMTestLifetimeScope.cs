using MessagePipe;
using TSI.Core.Time;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TSI.Demo
{
    public class FSMTestLifetimeScope : LifetimeScope
    {
        [SerializeField] private TimeHierarchyProfileSO _timeHierarchy;

        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterTimeSystem(options, _timeHierarchy);
        }
    }
}
