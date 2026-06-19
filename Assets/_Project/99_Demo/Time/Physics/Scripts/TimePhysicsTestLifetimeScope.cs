using TSI.App.Input;
using TSI.App.UI;
using TSI.Core.Time;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using TSI.Core;

namespace TSI.Demo.Time
{
    public class TimePhysicsTestLifetimeScope : LifetimeScope
    {
        [SerializeField] private TimeHierarchyProfileSO _timeHierarchy;
        [SerializeField] private ProjectTimeSettingsSO _projectTimeSettings;

        // TODO: 이거 등록하고 사용해서 기존 데모 씬에 수정된 내용들 반영.
    
        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<TimeLayerSO, TickMessage>(options);

            builder.RegisterComponent(_projectTimeSettings);
            builder.Register<PhysicsInterpolationManager>(Lifetime.Singleton);

            builder.RegisterComponent(_timeHierarchy);

            builder.Register<ITimeHierarchyFactory, SOTimeHierarchyFactory>(Lifetime.Singleton);

            builder.RegisterEntryPoint<TimeManager>().AsSelf();
        }
    }
}
