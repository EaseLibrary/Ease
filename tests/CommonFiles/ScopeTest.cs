﻿using Moq;
using System;
using Ease.TestCommon;
using Prism.Navigation;
using System.Threading.Tasks;

#if IS_MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if IS_DRYIOC
namespace Ease.MsTest.DryIoc.PrismForms.Tests
#elif IS_UNITY
namespace Ease.MsTest.Unity.PrismForms.Tests
#endif
#elif IS_NUNIT
using NUnit.Framework;
#if IS_DRYIOC
namespace Ease.NUnit.DryIoc.PrismForms.Tests
#elif IS_UNITY
namespace Ease.NUnit.Unity.PrismForms.Tests
#endif
#elif IS_XUNIT
using Xunit;
#if IS_DRYIOC
namespace Ease.XUnit.DryIoc.PrismForms.Tests
#elif IS_UNITY
namespace Ease.XUnit.Unity.PrismForms.Tests
#endif
#endif
{
#if IS_MSTEST
	[TestClass]
#elif IS_NUNIT
	[TestFixture]
#endif
	public class ScopeTest : PrismFormsTestBase
	{

		private static readonly string _iRepoDefaultMyPropertyValue = "DefaultValue";
		private static readonly string _iRepoOverridenMyPropertyValue = "NewValue";

		private static Action<Mock<IRepo>> onIRepoMockCreated;
		private static Action<Mock<IRepo>> configureIRepoMockWithDefaultValue = mock =>
		{
			mock.SetupGet(s => s.MyProperty)
				.Returns(_iRepoDefaultMyPropertyValue);
		};
		private static Action<Mock<IRepo>> configureIRepoWithOverridenValue = mock =>
		{
			mock.SetupGet(s => s.MyProperty)
				.Returns(_iRepoOverridenMyPropertyValue);
		};

		public ScopeTest()
		{
#if (IS_MSTEST || IS_XUNIT)
			onIRepoMockCreated = configureIRepoMockWithDefaultValue;
#elif IS_NUNIT
			RegisterPerTestSetup(() =>
			{
				onIRepoMockCreated = configureIRepoMockWithDefaultValue;
			});
#endif
		}

		protected override void RegisterTypes()
		{
			base.RegisterTypes();

			RegisterMockType(() => onIRepoMockCreated);
			RegisterMockType<IService>();
		}

#if IS_MSTEST
		[TestMethod]
#elif IS_NUNIT
		[Test]
#elif IS_XUNIT
		[Fact]
#endif
		public void IRepoIsSetupWithDefaultCreatedCallback()
		{
			var vm = ResolveType<VM>();

#if IS_XUNIT
			Assert.Equal(_iRepoDefaultMyPropertyValue, vm.MyRepoProperty);
#else
			Assert.AreEqual(_iRepoDefaultMyPropertyValue, vm.MyRepoProperty);
#endif
		}


#if IS_MSTEST
		[TestMethod]
#elif IS_NUNIT
		[Test]
#elif IS_XUNIT
		[Fact]
#endif
		public void IRepoIsSetupWithOverridenCreatedCallback()
		{
			onIRepoMockCreated += configureIRepoWithOverridenValue;
			var vm = ResolveType<VM>();

#if IS_XUNIT
			Assert.Equal(_iRepoOverridenMyPropertyValue, vm.MyRepoProperty);
			Assert.Null(vm.MyStringProperty);
#else
			Assert.AreEqual(_iRepoOverridenMyPropertyValue, vm.MyRepoProperty);
			Assert.IsNull(vm.MyStringProperty);
#endif
		}


#if IS_MSTEST
		[TestMethod]
#elif IS_NUNIT
		[Test]
#elif IS_XUNIT
		[Fact]
#endif
		public void IRepoIsSetupWithNoCallback()
		{
			onIRepoMockCreated = null;
			var vm = ResolveType<VM>();
#if IS_XUNIT
			Assert.Null(vm.MyRepoProperty);
#else
			Assert.IsNull(vm.MyRepoProperty);
#endif
		}


#if IS_MSTEST
		[TestMethod]
#elif IS_NUNIT
		[Test]
#elif IS_XUNIT
		[Fact]
#endif
		public void VmCallsRepoSaveDataWhenDoSaveData()
		{
			onIRepoMockCreated += configureIRepoWithOverridenValue;
			var vm = ResolveType<VM>();

			vm.DoSaveData();

			ValidateMock<IRepo>(m => m.Verify(i => i.SaveData(), Times.Once));
		}

#if IS_MSTEST
		[TestMethod]
#elif IS_NUNIT
		[Test]
#elif IS_XUNIT
		[Fact]
#endif
		public async Task VmCallsServiceLongRunningTaskWhenDoLongRunningTask()
		{
			var vm = ResolveType<VM>();

			await vm.DoLongRunningTask();

			ValidateMock<IService>(m => m.Verify(i => i.LongRunningTask(), Times.Once));
		}


#if IS_MSTEST
		[DataTestMethod]
		[DataRow(1)]
		[DataRow(2)]
		[DataRow(3)]
		[DataRow(4)]
		[DataRow(5)]
		[DataRow(6)]
		[DataRow(7)]
		[DataRow(8)]
		[DataRow(9)]
		[DataRow(10)]
		public void RepoSaveDataCallHistoryIsResetBetweenCalls(int time)
#elif IS_NUNIT
		[Test]
		public void RepoSaveDataCallHistoryIsResetBetweenCalls([Range(1, 10)]int time)
#elif IS_XUNIT
		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(3)]
		[InlineData(4)]
		[InlineData(5)]
		[InlineData(6)]
		[InlineData(7)]
		[InlineData(8)]
		[InlineData(9)]
		[InlineData(10)]
		public void RepoSaveDataCallHistoryIsResetBetweenCalls(int time)
#endif
		{
			Func<Times> expected = Times.Never;
			var vm = ResolveType<VM>();
			if (time % 2 == 0)
			{
				expected = Times.Once;
				vm.DoSaveData();

			}
			ValidateMock<IRepo>(m => m.Verify(i => i.SaveData(), expected));
		}


#if IS_MSTEST
		[DataTestMethod]
		[DataRow(1)]
		[DataRow(2)]
		[DataRow(3)]
		[DataRow(4)]
		[DataRow(5)]
		[DataRow(6)]
		[DataRow(7)]
		[DataRow(8)]
		[DataRow(9)]
		[DataRow(10)]
		public void ObjectsAreResetBewteenCalls(int time)
#elif IS_NUNIT
		[Test]
		public void ObjectsAreResetBewteenCalls([Range(1, 10)]int time)
#elif IS_XUNIT
		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(3)]
		[InlineData(4)]
		[InlineData(5)]
		[InlineData(6)]
		[InlineData(7)]
		[InlineData(8)]
		[InlineData(9)]
		[InlineData(10)]
		public void ObjectsAreResetBewteenCalls(int time)
#endif
		{
			var testValue = "test";
			var expected = default(string);
			var vm = ResolveType<VM>();
			if (time % 2 == 0)
			{
				vm.MyStringProperty = testValue;
				expected = testValue;
			}
#if IS_XUNIT
			Assert.Equal(expected, vm.MyStringProperty);
#else
			Assert.AreEqual(expected, vm.MyStringProperty);
#endif
		}


#if IS_MSTEST
		[TestMethod]
#elif IS_NUNIT
		[Test]
#elif IS_XUNIT
		[Fact]
#endif
		public async Task VmCallsNavigationServiceWithTargetWhenDoNavigation()
		{
			var target = "TargetPath";
			var vm = ResolveType<VM>();

			await vm.DoNavigationAsync(target);

			VerifyPrismNavigateAsync(target, Times.Once);
		}

#if IS_MSTEST
		[TestMethod]
#elif IS_NUNIT
		[Test]
#elif IS_XUNIT
		[Fact]
#endif
		public async Task VmCallsNavigationServiceWithTargetWhenDoNavigationWithParameters()
		{
			var target = "TargetPath";
			var vm = ResolveType<VM>();
			
			await vm.DoNavigationAsync(target, null);

			VerifyPrismNavigateAsync(target, null as INavigationParameters, Times.Once);
		}

#if IS_MSTEST
		[TestMethod]
#elif IS_NUNIT
		[Test]
#elif IS_XUNIT
		[Fact]
#endif
		public async Task VmCallsNavigationServiceWithParameterValidationWhenDoNavigationWithParameters()
		{
			var target = "TargetPath";
			string parameterKey = "x";
			string parameterValue = "1";

			var vm = ResolveType<VM>();
			var navigationParameters = new NavigationParameters()
			{
				{ parameterKey, parameterValue }
			};

			await vm.DoNavigationAsync(target, navigationParameters);

			VerifyPrismNavigateAsync(target, p => p.ContainsKey(parameterKey) && p.GetValue<string>(parameterKey).Equals(parameterValue), Times.Once);
		}

#if IS_MSTEST
		[TestMethod]
#elif IS_NUNIT
		[Test]
#elif IS_XUNIT
		[Fact]
#endif
		public async Task VmCallsNavigationServiceWithTargetWhenDoNavigationWithParametersCheckingSpecificParameters()
		{
			var target = "TargetPath";
			var vm = ResolveType<VM>();
			var navigationParameters = new NavigationParameters()
			{
				{ "x", "1" }
			};

			await vm.DoNavigationAsync(target, navigationParameters);

			VerifyPrismNavigateAsync(target, navigationParameters, Times.Once);
		}

#if IS_MSTEST
		[TestMethod]
#elif IS_NUNIT
		[Test]
#elif IS_XUNIT
		[Fact]
#endif
		public async Task VmCallsNavigationServiceWhenGoBackAsync()
		{
			var vm = ResolveType<VM>();

			await vm.DoGoBackAsync();

			VerifyPrismGoBackAsync(Times.Once);
		}

#if IS_MSTEST
		[TestMethod]
#elif IS_NUNIT
		[Test]
#elif IS_XUNIT
		[Fact]
#endif
		public async Task VmCallsNavigationServiceWhenGoBackAsyncParametersCheckingSpecificParameters()
		{
			var vm = ResolveType<VM>();
			var navigationParameters = new NavigationParameters()
			{
				{ "x", "1" }
			};

			await vm.DoGoBackAsync(navigationParameters);

			VerifyPrismGoBackAsync(navigationParameters, Times.Once);
		}

#if IS_MSTEST
		[TestMethod]
#elif IS_NUNIT
		[Test]
#elif IS_XUNIT
		[Fact]
#endif
		public async Task VmCallsNavigationServiceWithParameterValidationWhenGoBackAsyncWithParameters()
		{
			string parameterKey = "x";
			string parameterValue = "1";

			var vm = ResolveType<VM>();
			var navigationParameters = new NavigationParameters()
			{
				{ parameterKey, parameterValue }
			};

			await vm.DoGoBackAsync(navigationParameters);

			VerifyPrismGoBackAsync(p => p.ContainsKey(parameterKey) && p.GetValue<string>(parameterKey).Equals(parameterValue), Times.Once);
		}

#if IS_MSTEST
		[TestMethod]
#elif IS_NUNIT
		[Test]
#elif IS_XUNIT
		[Fact]
#endif
		public async Task VmCallsNavigationServiceWhenGoBackAsyncWithModalNavigation()
		{
			var vm = ResolveType<VM>();
			var navigationParameters = new NavigationParameters();

			await vm.DoGoBackAsync(navigationParameters, true, true);

			VerifyPrismGoBackAsync(navigationParameters, true, true, Times.Once);
		}

#if IS_MSTEST
		[TestMethod]
#elif IS_NUNIT
		[Test]
#elif IS_XUNIT
		[Fact]
#endif
		public async Task AddNavigationCallbackInvokedWhenDoNavigationAsyncWithString()
		{
			bool callbackInvoked = false;
			var vm = ResolveType<VM>();

			AddNavigationCallback((string path, INavigationParameters parameters, bool? useModalNavigation, bool animated) =>
			{
				callbackInvoked = true;
			});

			await vm.DoNavigationAsync("target_path", new NavigationParameters(), false, false );

#if IS_XUNIT
			Assert.True(callbackInvoked);
#else
			Assert.IsTrue(callbackInvoked);
#endif
		}

#if IS_MSTEST
		[TestMethod]
#elif IS_NUNIT
		[Test]
#elif IS_XUNIT
		[Fact]
#endif
		public async Task AddNavigationCallbackInvokedWhenDoNavigationAsyncWithUri()
		{
			bool callbackInvoked = false;
			var vm = ResolveType<VM>();

			AddNavigationCallback((Uri uri, INavigationParameters parameters, bool? useModalNavigation, bool animated) =>
			{
				callbackInvoked = true;
			});

			await vm.DoNavigationAsync(new Uri("target/path", UriKind.RelativeOrAbsolute), new NavigationParameters(), false, false);

#if IS_XUNIT
			Assert.True(callbackInvoked);
#else
			Assert.IsTrue(callbackInvoked);
#endif
		}
	}
}
