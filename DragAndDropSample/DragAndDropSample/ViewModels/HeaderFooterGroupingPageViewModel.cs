using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using DragAndDropSample.Navigables;
using DragAndDropSample.Services;

using Sharpnado.CollectionView.Paging;
using Sharpnado.CollectionView.Services;
using Sharpnado.Presentation.Forms;

using Xamarin.Forms;

namespace DragAndDropSample.ViewModels
{
    public class HeaderFooterGroupingPageViewModel : ANavigableViewModel
    {
        private const int PageSize = 20;
        private readonly ISillyDudeService _sillyDudeService;

        private List<IDudeItem> _sillyPeople;
        private int _currentIndex = -1;

        private List<SillyDude> _listSource = new List<SillyDude>();

        public HeaderFooterGroupingPageViewModel(INavigationService navigationService, ISillyDudeService sillyDudeService)
            : base(navigationService)
        {
            _sillyDudeService = sillyDudeService;

            InitCommands();

            SillyPeople = new List<IDudeItem>();
            SillyPeoplePaginator = new Paginator<SillyDude>(LoadSillyPeoplePageAsync, pageSize: PageSize);
            SillyPeopleLoaderNotifier = new TaskLoaderNotifier<IReadOnlyCollection<SillyDude>>();

            GoBackCommand = new TaskLoaderCommand(() => NavigationService.NavigateBackAsync());
        }

        public int CurrentIndex
        {
            get => _currentIndex;
            set => SetAndRaise(ref _currentIndex, value);
        }

        public ICommand GoBackCommand { get; }

        public ICommand TapCommand { get; private set; }

        public ICommand OnScrollBeginCommand { get; private set; }

        public ICommand OnScrollEndCommand { get; private set; }

        public TaskLoaderNotifier<IReadOnlyCollection<SillyDude>> SillyPeopleLoaderNotifier { get; }

        public Paginator<SillyDude> SillyPeoplePaginator { get; }

        public List<IDudeItem> SillyPeople
        {
            get => _sillyPeople;
            set => SetAndRaise(ref _sillyPeople, value);
        }

        public override void Load(object parameter)
        {
            SillyPeople = new List<IDudeItem>();

            SillyPeopleLoaderNotifier.Load(async _ => (await SillyPeoplePaginator.LoadPage(1)).Items);
        }

        private void InitCommands()
        {
            TapCommand = new TaskLoaderCommand<SillyDudeVmo>(
                (vm) => NavigationService.DisplayAlert("Dude Tapped", $"{vm.Name} was tapped !", "OK"));

            OnScrollBeginCommand = new Command(
                () => System.Diagnostics.Debug.WriteLine("SillyInfiniteGridPeopleVm: OnScrollBeginCommand"));
            OnScrollEndCommand = new Command(
                () => System.Diagnostics.Debug.WriteLine("SillyInfiniteGridPeopleVm: OnScrollEndCommand"));
        }

        private async Task<PageResult<SillyDude>> LoadSillyPeoplePageAsync(int pageNumber, int pageSize, bool isRefresh)
        {
            PageResult<SillyDude> resultPage = await _sillyDudeService.GetSillyPeoplePage(pageNumber, pageSize);

            var dudes = resultPage.Items;

            if (isRefresh)
            {
                SillyPeople = new List<IDudeItem>();
                _listSource = new List<SillyDude>();
            }

            var result = new List<IDudeItem> { new DudeHeader() };
            _listSource.AddRange(dudes);
            foreach (var group in _listSource.OrderByDescending(d => d.SillinessDegree)
                .GroupBy((dude) => dude.SillinessDegree))
            {
                result.Add(new DudeGroupHeader { StarCount = group.Key});
                result.AddRange(group.Select(dude => new SillyDudeVmo(dude, TapCommand)));
            }

            result.Add(new DudeFooter());

            SillyPeople = result;

            return resultPage;
        }
    }
}