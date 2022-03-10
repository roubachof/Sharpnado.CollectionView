using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using DragAndDropSample.Navigables;
using DragAndDropSample.Services;

using Sharpnado.CollectionView.Paging;
using Sharpnado.CollectionView.Services;
using Sharpnado.CollectionView.ViewModels;
using Sharpnado.Presentation.Forms;
using Sharpnado.Tasks;

using Xamarin.Forms;

namespace DragAndDropSample.ViewModels
{
    public enum ListMode
    {
        Grid = 0,
        Horizontal = 1,
        Vertical = 2,
    }

    public class GridPageViewModel : ANavigableViewModel
    {
        private const int PageSize = 20;
        private readonly ISillyDudeService _sillyDudeService;

        private ObservableRangeCollection<SillyDudeVmo> _sillyPeople;
        private ListMode _mode;
        private int _currentIndex = -1;

        private int? _selectedDudeId;

        public GridPageViewModel(INavigationService navigationService, ISillyDudeService sillyDudeService)
            : base(navigationService)
        {
            _sillyDudeService = sillyDudeService;

            InitCommands();

            SillyPeople = new ObservableRangeCollection<SillyDudeVmo>();
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

        public ICommand OnDragStarted { get; private set; }

        public ICommand OnDragEnded { get; private set; }

        public TaskLoaderNotifier<IReadOnlyCollection<SillyDude>> SillyPeopleLoaderNotifier { get; }

        public ListMode Mode
        {
            get => _mode;
            set => SetAndRaise(ref _mode, value);
        }

        public Paginator<SillyDude> SillyPeoplePaginator { get; }

        public ObservableRangeCollection<SillyDudeVmo> SillyPeople
        {
            get => _sillyPeople;
            set => SetAndRaise(ref _sillyPeople, value);
        }

        public int? SelectedDudeId
        {
            get => _selectedDudeId;
            set => SetAndRaise(ref _selectedDudeId, value);
        }

        public override void Load(object parameter)
        {
            SillyPeople = new ObservableRangeCollection<SillyDudeVmo>();

            SillyPeopleLoaderNotifier.Load(async _ => (await SillyPeoplePaginator.LoadPage(1)).Items);
        }

        private void InitCommands()
        {
            TapCommand = new Command<SillyDudeVmo>(
                async (vm) => await NavigationService.DisplayAlert("Dude Tapped", $"{vm.Name} was tapped !", "OK"));

            OnScrollBeginCommand = new Command(
                () => System.Diagnostics.Debug.WriteLine("SillyInfiniteGridPeopleVm: OnScrollBeginCommand"));
            OnScrollEndCommand = new Command(
                () => System.Diagnostics.Debug.WriteLine("SillyInfiniteGridPeopleVm: OnScrollEndCommand"));

            OnDragStarted = new Command(
                (info) =>
                {
                    var dragInfo = (DragAndDropInfo)info;
                    System.Diagnostics.Debug.WriteLine($"OnDragStarted( from: {dragInfo.From}, to: {dragInfo.To} )");
                });

            OnDragEnded = new Command(
                (info) =>
                {
                    var dragInfo = (DragAndDropInfo)info;
                    System.Diagnostics.Debug.WriteLine($"OnDragEnded( from: {dragInfo.From}, to: {dragInfo.To} )");
                });
        }

        private async Task<PageResult<SillyDude>> LoadSillyPeoplePageAsync(int pageNumber, int pageSize, bool isRefresh)
        {
            //if (pageNumber > 1)
            //{
            //    return PageResult<SillyDude>.Empty;
            //}

            PageResult<SillyDude> resultPage = await _sillyDudeService.GetSillyPeoplePage(pageNumber, pageSize);
            var viewModels = resultPage.Items.Select(dude => new SillyDudeVmo(dude, TapCommand)).ToList();

            if (isRefresh)
            {
                SillyPeople = new ObservableRangeCollection<SillyDudeVmo>();
            }

            SillyPeople.AddRange(viewModels);

            // Uncomment to test CurrentIndex property
            //TaskMonitor.Create(
            //    async () =>
            //    {
            //        await Task.Delay(2000);
            //        CurrentIndex = 15;
            //    });

            // Uncomment to test Reset action
            // TaskMonitor.Create(
            //   async () =>
            //   {
            //       await Task.Delay(6000);
            //       SillyPeople.Clear();
            //       await Task.Delay(3000);
            //       SillyPeople = new ObservableRangeCollection<SillyDudeVmo>(viewModels);
            //   });

            return resultPage;
        }
    }
}