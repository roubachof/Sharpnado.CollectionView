using System.Collections.Generic;
using System.Threading.Tasks;

using Sharpnado.CollectionView;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DragAndDropSample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GridPage : ContentPage
    {
        private string[] _pickerData = new string[] { "Auto", "1", "2", "3" };

        public GridPage()
        {
            InitializeComponent();

            HorizontalListView.PreRevealAnimationAsync = async (viewCell) =>
                {
                    viewCell.View.Opacity = 0;

                    if (HorizontalListView.CollectionLayout == CollectionViewLayout.Vertical)
                    {
                        viewCell.View.RotationX = 90;
                    }
                    else
                    {
                        viewCell.View.RotationY = -90;
                    }

                    await Task.Delay(200);
                };

            HorizontalListView.RevealAnimationAsync = async (viewCell) =>
                {
                    await viewCell.View.FadeTo(1);

                    if (HorizontalListView.CollectionLayout == CollectionViewLayout.Vertical)
                    {
                        await viewCell.View.RotateXTo(0);
                    }
                    else
                    {
                        await viewCell.View.RotateYTo(0);
                    }
                };

            HorizontalListView.DragAndDropEnabledAnimationAsync = async (viewCell, token) =>
            {
                while (!token.IsCancellationRequested)
                {
                    await viewCell.View.RotateTo(8);
                    await viewCell.View.RotateTo(-8);
                }

                await viewCell.View.RotateTo(0);
            };

            foreach (var item in _pickerData)
            {
                ColumnPicker.Items.Add(item);
            }

            ColumnPicker.SelectedIndex = 0;
            ColumnPicker.SelectedIndexChanged += (sender, args) =>
            {
                if (ColumnPicker.SelectedIndex == -1)
                {
                    HorizontalListView.ColumnCount = 0;
                }
                else
                {
                    HorizontalListView.ColumnCount = ColumnPicker.SelectedIndex == 0 ? -1 : ColumnPicker.SelectedIndex;
                    if (ColumnPicker.SelectedIndex == 0)
                    {
                        HorizontalListView.ItemWidth = 120;
                    }
                }
            };
        }

        private void ListLayoutChanging(object sender, CollectionLayoutChangedEventArgs e)
        {
            ColumnCountContainer.IsVisible = e.ListLayout == CollectionViewLayout.Grid;

            switch (e.ListLayout)
            {
                case CollectionViewLayout.Horizontal:
                    HorizontalListView.BatchBegin();
                    HorizontalListView.ItemWidth = 260;
                    HorizontalListView.ItemHeight = 260;
                    HorizontalListView.ColumnCount = 0;
                    HorizontalListView.BatchCommit();
                    HorizontalListView.DragAndDropDirection = DragAndDropDirection.HorizontalOnly;
                    HorizontalListView.Margin = Device.RuntimePlatform == Device.Android
                        ? new Thickness(0, 60, 0, 0)
                        : new Thickness(0, -60, 0, 0);

                    break;

                case CollectionViewLayout.Grid:
                    HorizontalListView.BatchBegin();
                    HorizontalListView.ItemWidth = 120;
                    HorizontalListView.ItemHeight = 120;
                    HorizontalListView.BatchCommit();
                    HorizontalListView.Margin = new Thickness(0);
                    HorizontalListView.DragAndDropDirection = DragAndDropDirection.Free;

                    HorizontalListView.ColumnCount = ColumnPicker.SelectedIndex == 0 ? -1 : ColumnPicker.SelectedIndex;
                    break;

                case CollectionViewLayout.Vertical:
                    HorizontalListView.BatchBegin();
                    HorizontalListView.ItemWidth = 0;
                    HorizontalListView.ItemHeight = 120;
                    HorizontalListView.BatchCommit();
                    HorizontalListView.Margin = new Thickness(0);
                    HorizontalListView.DragAndDropDirection = DragAndDropDirection.VerticalOnly;
                    break;
            }
        }
    }
}