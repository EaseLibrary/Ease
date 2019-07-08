using DryIoc;
using Moq;
using System;
using System.Collections.Generic;

namespace Ease.MsTest
{
	public class DryIocContainerTestBase
	{
		private static Dictionary<Type, Container> Containers { get; set; } = new Dictionary<Type, Container>();

		private Container Container;
		private IResolverContext ScopeContext;

		public DryIocContainerTestBase()
		{
			var key = this.GetType();
			if (!Containers.ContainsKey(key))
			{
				var rules = Rules.Default
					.WithConcreteTypeDynamicRegistrations()
					.WithTrackingDisposableTransients()
					.WithDefaultReuse(new CurrentScopeReuse());
				Container = new Container(rules);
				RegisterTypes();
				if (!_baseRegisterTypesWasCalled)
				{
					throw new InvalidOperationException("Inherited classes must call base.RegisterTypes() when overriding");
				}
				Containers.Add(key, Container);
			}
			else
			{
				Container = Containers[key];
			}

			ScopeContext = Container.OpenScope();

		}

		private bool _baseRegisterTypesWasCalled = false;

		protected virtual void RegisterTypes()
		{
			_baseRegisterTypesWasCalled = true;
		}

		protected void RegisterType<T>() where T : class
		{
			Container.Register<T>();
		}

		protected void RegisterMockType<T>(Func<Action<Mock<T>>> onMockInstanceCreatedFactory) where T : class
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

		protected T ResolveType<T>()
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

