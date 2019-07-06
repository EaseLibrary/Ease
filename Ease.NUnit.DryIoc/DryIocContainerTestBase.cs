using System;
using DryIoc;
using Moq;

namespace Ease.NUnit.DryIoc
{
	public class DryIocContainerTestBase : ContainerTestBase
	{
		private Container Container;
		private IResolverContext ScopeContext;

		protected DryIocContainerTestBase()
		{
			var rules = Rules.Default
				.WithConcreteTypeDynamicRegistrations()
				.WithTrackingDisposableTransients()
				.WithDefaultReuse(new CurrentScopeReuse());
			Container = new Container(rules);
			RegisterPerTestSetup(() =>
			{
				ScopeContext?.Dispose();
				ScopeContext = Container.OpenScope();
			});
		}

		protected override void RegisterType<T>()
		{
			Container.Register<T>();
		}

		protected override void RegisterType<TInterface, TImplementation>()
		{
			Container.Register<TInterface, TImplementation>();
		}

		protected override void RegisterTypeFactory<T>(Func<T> factory)
		{
			Container.RegisterDelegate<T>(c => factory());
		}

		protected override void RegisterMockType<T>(Func<Action<Mock<T>>> onMockInstanceCreatedFactory)
		{
			Container.Register<T>(made: Made.Of<T>(() => CreateAndInitializeMockInstance(onMockInstanceCreatedFactory)));
		}

		private static T CreateAndInitializeMockInstance<T>(Func<Action<Mock<T>>> onMockInstanceCreatedFactory) where T : class
		{
			var mock = new Mock<T>();
			var onCreatedCallback = onMockInstanceCreatedFactory();
			onCreatedCallback?.Invoke(mock);
			return mock.Object;
		}

		protected override T ResolveType<T>()
		{
			return ScopeContext.Resolve<T>();
		}

		protected void ValidateMock<T>(Action<Mock<T>> validationAction) where T : class
		{
			var instance = ResolveType<T>();
			var mock = Mock.Get(instance);
			validationAction(mock);
		}
	}
}
