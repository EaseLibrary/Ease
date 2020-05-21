using Moq;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#if IS_MSTEST
namespace Ease.MsTest.PrismForms
#elif IS_DRYIOC
namespace Ease.NUnit.DryIoc.PrismForms
#elif IS_UNITY
namespace Ease.NUnit.Unity.PrismForms
#endif
{
	public class PrismFormsTestBase
#if IS_MSTEST
	: DryIocContainerTestBase
#elif IS_DRYIOC
	: DryIocContainerTestBase
#elif IS_UNITY
	: UnityContainerTestBase
#endif
	{
		protected Action<Mock<INavigationService>> OnINavigationServiceMockCreated;

		protected Action<Mock<IPageDialogService>> OnIPageDialogServiceMockCreated;

		protected Action<Mock<IEventAggregator>> OnIEventAggregatorMockCreated;

		public PrismFormsTestBase()
		{
#if IS_MSTEST
		}

		protected override void RegisterTypes()
		{
			base.RegisterTypes();
#endif
			RegisterMockType(() => OnINavigationServiceMockCreated);
			RegisterMockType(() => OnIPageDialogServiceMockCreated);
			RegisterMockType(() => OnIEventAggregatorMockCreated);
		}

#region INavigationServiceValidation

		protected void AddNavigationCallback(Action<Uri, NavigationParameters, bool?, bool> callback)
		{
			var navServiceMock = Mock.Get(ResolveType<INavigationService>());
			navServiceMock.Setup(s => s.NavigateAsync(It.IsAny<Uri>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
				.Callback(callback);
		}

		protected void AddNavigationCallback(Action<String, NavigationParameters, bool?, bool> callback)
		{
			var navServiceMock = Mock.Get(ResolveType<INavigationService>());
			navServiceMock.Setup(s => s.NavigateAsync(It.IsAny<String>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
				.Callback(callback);
		}

		protected void VerifyNavigation(Uri uri, Func<Times> times)
		{
			var navServiceMock = Mock.Get(ResolveType<INavigationService>());
			navServiceMock.Verify(
				n => (n.NavigateAsync(uri)), times);
		}

		protected void VerifyNavigation(Uri uri, INavigationParameters parameters, Func<Times> times)
		{
			var navServiceMock = Mock.Get(ResolveType<INavigationService>());
			navServiceMock.Verify(
				n => (n.NavigateAsync(uri, parameters)), times);
		}

		protected void VerifyNavigation(Uri uri, System.Linq.Expressions.Expression<Func<INavigationParameters, bool>> parameterValidation, Func<Times> times)
		{
			var navServiceMock = Mock.Get(ResolveType<INavigationService>());
			navServiceMock.Verify(n => n.NavigateAsync(uri, It.Is(parameterValidation)), times);
		}

		protected void VerifyNavigation(string path, Func<Times> times)
		{
			var navServiceMock = Mock.Get(ResolveType<INavigationService>());
			navServiceMock.Verify(
				n => (n.NavigateAsync(path)), times);
		}

		protected void VerifyNavigation(string path, INavigationParameters parameters, Func<Times> times)
		{
			var navServiceMock = Mock.Get(ResolveType<INavigationService>());
			if (parameters != null)
			{
				navServiceMock.Verify(n => (n.NavigateAsync(path, parameters)), times);
			}
			else
			{
				navServiceMock.Verify(n => (n.NavigateAsync(path, It.IsAny<INavigationParameters>())), times);
			}
		}

		protected void VerifyNavigation(string path, System.Linq.Expressions.Expression<Func<INavigationParameters, bool>> parameterValidation, Func<Times> times)
		{
			var navServiceMock = Mock.Get(ResolveType<INavigationService>());
			navServiceMock.Verify(n => n.NavigateAsync(path, It.Is(parameterValidation)), times);
		}

		protected void VerifyNavigationGoBackAsync(Func<Times> times)
		{
			var navServiceMock = Mock.Get(ResolveType<INavigationService>());
			navServiceMock.Verify(n => n.GoBackAsync(), times);
		}

		protected void VerifyNavigationGoBackAsync(System.Linq.Expressions.Expression<Func<INavigationParameters, bool>> parameterValidation, Func<Times> times)
		{
			var navServiceMock = Mock.Get(ResolveType<INavigationService>());
			navServiceMock.Verify(n => n.GoBackAsync(It.Is(parameterValidation)), times);
		}

		#endregion

		protected NavigationParameters CreateNavigationParameters(NavigationMode navigationMode, params KeyValuePair<string, object>[] parameters)
		{
			var navParams = new NavigationParameters();
			var internalParams = navParams as INavigationParametersInternal;
			internalParams.Add("__NavigationMode", navigationMode);

			foreach (var parameter in parameters)
			{
				navParams.Add(parameter.Key, parameter.Value);
			}

			return navParams;
		}

		protected T ResolveAndCallOnNavigatedTo<T>(NavigationMode navigationMode, params KeyValuePair<string, object>[] parameters)
			where T : BindableBase,  INavigatedAware
		{
			var vm = ResolveType<T>();
			var navParams = CreateNavigationParameters(navigationMode, parameters);
			vm.OnNavigatedTo(navParams);
			return vm;
		}
	}
}