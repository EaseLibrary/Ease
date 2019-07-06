using Prism.Mvvm;
using Prism.Navigation;
using System.Threading.Tasks;

namespace Ease.TestCommon
{
	public class VM : BindableBase
	{
		public VM(INavigationService navigationService, IRepo repo)
		{
			NavigationService = navigationService;
			Repo = repo;
		}

		private INavigationService NavigationService { get; }
		private IRepo Repo { get; }

		public void DoSaveData()
		{
			Repo.SaveData();
		}

		public string MyRepoProperty { get => Repo.MyProperty; }
		public string MyStringProperty { get; set; }

		public async Task DoNavigation(string target)
		{
			await NavigationService.NavigateAsync(target);
		}

		public async Task DoNavigationWithParameters(string target)
		{
			await NavigationService.NavigateAsync(target, new NavigationParameters("x=1"));
		}
	}
}
