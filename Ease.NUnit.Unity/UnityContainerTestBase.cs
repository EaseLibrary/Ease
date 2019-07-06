using System;
using Moq;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Registration;

namespace Ease.NUnit.Unity
{
	public class UnityContainerTestBase : ContainerTestBase
	{
		protected UnityContainer Container;
		protected LifetimeResetter Resetter { get; set; }

		public UnityContainerTestBase()
		{
			Resetter = new LifetimeResetter();
			Container = new UnityContainer();
			RegisterPerTestSetup(() => 
			{
				Resetter.Reset();
			});
		}

		protected void RegisterResettableType<T>()
		{
			Container.RegisterType<T>(new ResettableLifetimeManager(Resetter));
		}

		protected void RegisterResettableType<TInterface, TImplementation>()
		{
			Container.RegisterType(typeof(TInterface), typeof(TImplementation), string.Empty, new ResettableLifetimeManager(Resetter), null);
//			UnityContainerExtensions.RegisterType<TInterface, TImplementation>(Container, (new ResettableLifetimeManager(Resetter)) as LifetimeManager, injectionMembers);
		}

		protected void RegisterResettableTypeFactory<T>(Func<T> factory)
		{
			RegisterResettableType<T>(new InjectionFactory(c => factory()));
		}

		protected void RegisterResettableType<T>(params InjectionMember[] injectionMembers)
		{
			Container.RegisterType<T>(new ResettableLifetimeManager(Resetter), injectionMembers);
		}

		protected override void RegisterMockType<T>(Func<Action<Mock<T>>> onCreatedCallbackFactory)
		{
			RegisterResettableType<T>(new InjectionFactory(c => CreateAndInitializeMockInstance(onCreatedCallbackFactory)));
		}

		private static T CreateAndInitializeMockInstance<T>(Func<Action<Mock<T>>> onMockInstanceCreatedFactory) where T : class
		{
			var mock = new Mock<T>();
			var onCreatedCallback = onMockInstanceCreatedFactory();
			onCreatedCallback?.Invoke(mock);
			return mock.Object;
		}

		protected override void RegisterType<T>()
		{
			RegisterResettableType<T>();
		}

		protected override void RegisterType<TInterface, TImplementation>()
		{
			RegisterResettableType<TInterface, TImplementation>();
		}

		protected override void RegisterTypeFactory<T>(Func<T> factory)
		{
			RegisterResettableTypeFactory<T>(factory);
		}

		protected override T ResolveType<T>()
		{
			return Container.Resolve<T>();
		}

		protected void ValidateMock<T>(Action<Mock<T>> validationAction) where T : class
		{
			var instance = ResolveType<T>();
			var mock = Mock.Get(instance);
			validationAction(mock);
		}

		protected class LifetimeResetter
		{
			public Action OnReset;

			public void Reset()
			{
				OnReset?.Invoke();
			}
		}

		protected class ResettableLifetimeManager : LifetimeManager
		{
			private LifetimeResetter LifetimeResetter { get; }

			private object instance;

			public ResettableLifetimeManager(LifetimeResetter lifetimeResetter)
			{
				lifetimeResetter.OnReset += () => RemoveValue(null);
				LifetimeResetter = lifetimeResetter;
			}

			public override void SetValue(object newValue, ILifetimeContainer container = null)
			{
				instance = newValue;
			}

			public override void RemoveValue(ILifetimeContainer container = null)
			{
				instance = null;
			}

			public override object GetValue(ILifetimeContainer container = null)
			{
				return instance;
			}

			protected override LifetimeManager OnCreateLifetimeManager()
			{
				return new ResettableLifetimeManager(LifetimeResetter);
			}
		}


	}
}
