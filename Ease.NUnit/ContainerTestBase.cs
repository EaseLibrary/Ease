using System;
using Moq;
using NUnit.Framework;

namespace Ease.NUnit
{
	public abstract class ContainerTestBase
	{
		abstract protected T ResolveType<T>()
			where T : class;
		abstract protected void RegisterType<T>() 
			where T : class;
		abstract protected void RegisterType<TInterface, TImplementation>() 
			where TInterface : class
			where TImplementation : TInterface, new();
		abstract protected void RegisterTypeFactory<T>(Func<T> factory)
			where T : class;
		abstract protected void RegisterMockType<T>(Func<Action<Mock<T>>> onCreatedCallbackFactory)
			where T : class;

		private Action _onPerTestSetup;

		[SetUp]
		public void PerTestSetup()
		{
			_onPerTestSetup?.Invoke();
		}

		protected void RegisterPerTestSetup(Action perTestSetup)
		{
			_onPerTestSetup += perTestSetup;
		}

		protected Mock<T> GetMock<T>() 
			where T : class
		{
			var instance = ResolveType<T>();
			var mock = Mock.Get<T>(instance);
			return mock;
		}
	}
}
