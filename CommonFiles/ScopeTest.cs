using Moq;
using System;
using Ease.TestCommon;
using Prism.Navigation;

#if IS_MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Ease.MsTest.PrismForms.Tests
#elif IS_NUNIT
using NUnit.Framework;
#if IS_DRYIOC
namespace Ease.NUnit.DryIoc.PrismForms.Tests
#elif IS_UNITY
namespace Ease.NUnit.Unity.PrismForms.Tests
#endif
#endif
{
#if IS_MSTEST
	[TestClass]
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

#if IS_MSTEST
		public ScopeTest()
		{
			onIRepoMockCreated = configureIRepoMockWithDefaultValue;
		}

		protected override void RegisterTypes()
		{
			base.RegisterTypes();
			RegisterMockType(() => onIRepoMockCreated);
		}
#else
		public ScopeTest()
		{
			RegisterMockType(() => onIRepoMockCreated);
			RegisterPerTestSetup(() =>
			{
				onIRepoMockCreated = configureIRepoMockWithDefaultValue;
			});
		}
#endif

#if IS_MSTEST
		[TestMethod]
#else
		[Test]
#endif
		public void IRepoIsSetupWithDefaultCreatedCallback()
		{
			var vm = ResolveType<VM>();
			Assert.AreEqual(_iRepoDefaultMyPropertyValue, vm.MyRepoProperty);
		}


#if IS_MSTEST
		[TestMethod]
#else
		[Test]
#endif
		public void IRepoIsSetupWithOverridenCreatedCallback()
		{
			onIRepoMockCreated += configureIRepoWithOverridenValue;
			var vm = ResolveType<VM>();
			Assert.AreEqual(_iRepoOverridenMyPropertyValue, vm.MyRepoProperty);
			Assert.IsNull(vm.MyStringProperty);
		}


#if IS_MSTEST
		[TestMethod]
#else
		[Test]
#endif
		public void IRepoIsSetupWithNoCallback()
		{
			onIRepoMockCreated = null;
			var vm = ResolveType<VM>();
			Assert.IsNull(vm.MyRepoProperty);
		}


#if IS_MSTEST
		[TestMethod]
#else
		[Test]
#endif
		public void VmCallsRepoSaveDataWhenDoSaveData()
		{
			onIRepoMockCreated += configureIRepoWithOverridenValue;
			var vm = ResolveType<VM>();
			vm.DoSaveData();
			ValidateMock<IRepo>(m => m.Verify(i => i.SaveData(), Times.Once));
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
#else
		[Test]
		public void RepoSaveDataCallHistoryIsResetBetweenCalls([Range(1, 10)]int time)
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
#else
		[Test]
		public void ObjectsAreResetBewteenCalls([Range(1, 10)]int time)
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
			Assert.AreEqual(expected, vm.MyStringProperty);
		}


#if IS_MSTEST
		[TestMethod]
#else
		[Test]
#endif
		public void VmCallsNavigationServiceWithTargetWhenDoNavigation()
		{
			var target = "TargetPath";
			var vm = ResolveType<VM>();
			vm.DoNavigation(target).Wait();
			VerifyNavigation(target, Times.Once);
		}

#if IS_MSTEST
		[TestMethod]
#else
		[Test]
#endif
		public void VmCallsNavigationServiceWithTargetWhenDoNavigationWithParameters()
		{
			var target = "TargetPath";
			var vm = ResolveType<VM>();
			vm.DoNavigationWithParameters(target).Wait();
			VerifyNavigation(target, null as INavigationParameters, Times.Once);
		}

#if IS_MSTEST
		[TestMethod]
#else
		[Test]
#endif
		public void VmCallsNavigationServiceWithParameterValidationWhenDoNavigationWithParameters()
		{
			var target = "TargetPath";
			var vm = ResolveType<VM>();
			vm.DoNavigationWithParameters(target).Wait();
			VerifyNavigation(target, p => p.ContainsKey("x") && p.GetValue<string>("x").Equals("1"), Times.Once);
		}

#if IS_MSTEST
		[TestMethod]
#else
		[Test]
#endif
		public void VmCallsNavigationServiceWithTargetWhenDoNavigationWithParametersCheckingSpecificParameters()
		{
			var target = "TargetPath";
			var vm = ResolveType<VM>();
			vm.DoNavigationWithParameters(target).Wait();
			VerifyNavigation(target, new Prism.Navigation.NavigationParameters("x=1"), Times.Once);
		}


	}
}
