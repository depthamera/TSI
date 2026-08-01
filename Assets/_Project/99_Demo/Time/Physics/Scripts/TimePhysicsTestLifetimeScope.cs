using TSI.Core.Time;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TSI.Demo.Time
{
    public class TimePhysicsTestLifetimeScope : LifetimeScope
    {
        [SerializeField] private TimeHierarchyProfileSO _timeHierarchy;
        [SerializeField] private ProjectTimeSettingsSO _projectTimeSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterTimeSystem(options, _timeHierarchy);

            builder.RegisterInstance(_projectTimeSettings);
        }
    }
}
