# Sharpnado.CollectionView
**Formerly named `HorizontalListView`**

<p align="left"><img src="Docs/logo.png" height="180"/>

Get it from NuGet:

[![Nuget](https://img.shields.io/nuget/v/Sharpnado.CollectionView.svg)](https://www.nuget.org/packages/Sharpnado.CollectionView)

| Supported platforms        |
|----------------------------|
| :heavy_check_mark: Android |
| :heavy_check_mark: iOS     |

![Presentation](Docs/github_banner.jpg)

## Initialization


* On Core project in `App.xaml.cs`:

For the namespace schema to work, you need to call initializer from App.xaml.cs like this:

```csharp
public App()
{
    InitializeComponent();

    Sharpnado.CollectionView.Initializer.Initialize(true, false);
    ...
}
```

* On `iOS` add this line before `Xamarin.Forms.Forms.Init()` and `LoadApplication(new App())`.

```csharp
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    Initializer.Initialize();

    global::Xamarin.Forms.Forms.Init();
    LoadApplication(new App());
}
```

* On `Android` add this line before `Xamarin.Forms.Forms.Init()` and `LoadApplication(new App())`.

```csharp
public override OnCreate(Bundle savedInstanceState)
{
    Initializer.Initialize();

    global::Xamarin.Forms.Forms.Init();
    LoadApplication(new App());
}

```

## Version 2.0 breaking changes: CollectionView

`HorizontalListView` has finally been renamed `CollectionView` \o/.

All references to `HorizontalList` has been renamed to `Collection`, including:

 * namespaces
 * filename
 * class names
 * HorizontalListViewLayout => CollectionViewLayout
 * ListLayout => CollectionLayout

## Presentation

 * Horizontal, Grid, Carousel or Vertical layout
 * Drag and Drop feature
 * Grouping with headers and footers
 * Reveal custom animations
 * Column count
 * Infinite loading with ```Paginator``` component
 * Snapping on first or middle element
 * Padding and item spacing
 * Handles ```NotifyCollectionChangedAction``` Add, Remove and Reset actions
 * View recycling
 * ```RecyclerView``` on Android
 * ```UICollectionView``` on iOS



https://user-images.githubusercontent.com/596903/138864430-108dcbc2-e425-4e2e-a9e1-feb568c0a866.mp4


    
## Horizontal list layout

```csharp
public CollectionViewLayout CollectionLayout { get; set; } = CollectionViewLayout.Horizontal;
```
By default the layout is in ```Linear``` mode, which means you will have only one row.
You can specify the ```ItemWidth``` and ```ItemHeight```.
You can also specify ```ItemSpacing``` and ```CollectionPadding```.


```xml
<DataTemplate x:Key="HorizontalDudeTemplate">
    <sho:DraggableViewCell x:Name="DraggableViewCell">
        <ContentView
            xamEffects:Commands.Tap="{Binding TapCommand}"
            xamEffects:Commands.TapParameter="{Binding .}"
            xamEffects:TouchEffect.Color="{StaticResource Accent}">
            <sho:Shadows
                x:Name="Shadow"
                CornerRadius="10"
                Shades="{StaticResource DarkerNeumorphism}">
                <views:SillyHorizontalCell
                    Margin="16,13,16,13"
                    BackgroundColor="{StaticResource DarkerSurface}"
                    CornerRadius="10">
                    <views:SillyHorizontalCell.Triggers>
                        <DataTrigger
                            Binding="{Binding Source={x:Reference DraggableViewCell}, Path=IsDragAndDropping}"
                            TargetType="views:SillyHorizontalCell"
                            Value="True">
                            <Setter Property="BackgroundColor" Value="{StaticResource DarkSurface}" />
                        </DataTrigger>
                    </views:SillyHorizontalCell.Triggers>
                </views:SillyHorizontalCell>
            </sho:Shadows>
        </ContentView>
    </sho:DraggableViewCell>
</DataTemplate>

...

<sho:CollectionView x:Name="CollectionView"
    CollectionPadding="10,30,10,75"
    CurrentIndex="{Binding CurrentIndex}"
    InfiniteListLoader="{Binding SillyPeoplePaginator}"
    ItemHeight="260"
    ItemWidth="260"
    ItemTemplate="{StaticResource HorizontalDudeTemplate}"
    ItemsSource="{Binding SillyPeople}"
    CollectionLayout="Horizontal"
    ScrollBeganCommand="{Binding OnScrollBeginCommand}"
    ScrollEndedCommand="{Binding OnScrollEndCommand}"
    SnapStyle="Center"
    TapCommand="{Binding TapCommand}" />
```

You can also simply use the the `HorizontalListView` class which is a shorthand to set the layout to `Horizontal`.

```xml
<sho:HorizontalListView 
    CollectionPadding="10,30,10,75"
    CurrentIndex="{Binding CurrentIndex}"
    InfiniteListLoader="{Binding SillyPeoplePaginator}"
    ItemHeight="260"
    ItemWidth="260"
    ItemTemplate="{StaticResource HorizontalDudeTemplate}"
    ItemsSource="{Binding SillyPeople}"
    ScrollBeganCommand="{Binding OnScrollBeginCommand}"
    ScrollEndedCommand="{Binding OnScrollEndCommand}"
    SnapStyle="Center"
    TapCommand="{Binding TapCommand}" />
```


As you can see ```TapCommand``` and ```TouchFeedbackColor``` (aka Ripple) are brought to you by the awesome effects created by mrxten (https://github.com/mrxten/XamEffects). 
It's the best ripple effect plugin so far on Xamarin.Forms since it always worked for me.
With other maybe more known plugins, I had some issues on iOS.

<p align="center">
  A <code>CollectionView</code> with <code>SnapStyle=Center</code> and <code>ItemWidth/ItemHeight</code> set.
</p>
<p align="center">
  <img src="Docs/hlv_horizontal_android.png" width="250" />
</p>



### ColumnCount property

You can also decide to just specify the number of column you want, the ```ColumnCount``` property, and the ```ItemWidth``` will be computed for you.

```xml
<sho:CollectionView
    x:Name="CollectionView"
    CollectionPadding="10,30,10,75"
    ColumnCount="2"
    CurrentIndex="{Binding CurrentIndex}"
    InfiniteListLoader="{Binding SillyPeoplePaginator}"
    ItemHeight="260"
    ItemTemplate="{StaticResource DudeTemplateSelector}"
    ItemsSource="{Binding SillyPeople}"
    ListLayout="Linear"
    TapCommand="{Binding TapCommand}" />
```

<p align="center">
  A <code>CollectionView</code> with <code>ColumnCount=2</code>.
</p>
<p align="center">
  <img src="Images/../Docs/hlv_horizontal_android_col2.png" width="250" />
</p>

### Carousel Layout

You can set ```ListLayout``` to ```Carousel```.
In this mode you can't specify ```ItemWidth``` (obviously).
If you don't specify the ```ItemHeight```, it will be automatically computed for you.

```xml
<renderedViews:CollectionView Grid.Row="3"
                                  Margin="-16,8"
                                  CollectionPadding="8,8"
                                  ItemSpacing="8"
                                  ListLayout="Carousel"
                                  ItemsSource="{Binding SillyPeopleLoader.Result}"
                                  SnapStyle="Center">
    ...
</renderedViews:CollectionView>
```

<p align="center">
  A <code>CollectionView</code> with <code>ListLayout=Carousel</code>.
</p>
<p align="center">
  <img src="Docs/hlv_carousel_iphone.gif" width="250" />
</p>

## Grid Layout

If you set the ```ListLayout``` property to ```Grid```, you will have access to the same properties.

```xml
<DataTemplate x:Key="GridDudeTemplate">
    <sho:DraggableViewCell x:Name="DraggableViewCell">
        <ContentView>
            <sho:Shadows
                x:Name="Shadow"
                CornerRadius="10"
                Shades="{StaticResource ThinDarkerNeumorphism}">
                <views:SillyGridCell
                    Margin="16,13,16,13"
                    BackgroundColor="{StaticResource DarkerSurface}"
                    CornerRadius="10">
                    <views:SillyGridCell.Triggers>
                        <DataTrigger
                            Binding="{Binding Source={x:Reference DraggableViewCell}, Path=IsDragAndDropping}"
                            TargetType="views:SillyGridCell"
                            Value="True">
                            <Setter Property="BackgroundColor" Value="{StaticResource DarkSurface}" />
                        </DataTrigger>
                    </views:SillyGridCell.Triggers>
                </views:SillyGridCell>
            </sho:Shadows>
        </ContentView>
    </sho:DraggableViewCell>
</DataTemplate>

<sho:CollectionView x:Name="CollectionView"
    CollectionPadding="10,30,10,75"
    CurrentIndex="{Binding CurrentIndex}"
    EnableDragAndDrop="True"
    InfiniteListLoader="{Binding SillyPeoplePaginator}"
    ItemHeight="120"
    ItemWidth="120"
    ItemTemplate="{StaticResource GridDudeTemplate}"
    ItemsSource="{Binding SillyPeople}"
    ListLayout="Grid"
    TapCommand="{Binding TapCommand}" />
```

You can use the ```IsDragAndDropping``` property of the ```DraggableViewCell``` to change the background color with a simple ```DataTrigger```.

<p align="center">
A <code>Grid</code> <code>ListLayout</code> with drag and drop enabled.<br>
</p>
<p align="center">
  <img src="Docs/drag_grid.gif" width="250" />
</p>

The ```ColumnCount``` property works also with the grid layout.

## Vertical Layout

You can also use Sharpnado's `CollectionView` like a regular list view.

```xml
<DataTemplate x:Key="VerticalDudeTemplate">
    <sho:DraggableViewCell x:Name="DraggableViewCell">
        <ContentView>
            <sho:Shadows
                x:Name="Shadow"
                CornerRadius="10"
                Shades="{StaticResource ThinDarkerNeumorphism}">
                <views:SillyListCell
                    Margin="16,13,16,13"
                    BackgroundColor="{StaticResource DarkerSurface}"
                    CornerRadius="10">
                    <views:SillyListCell.Triggers>
                        <DataTrigger
                            Binding="{Binding Source={x:Reference DraggableViewCell}, Path=IsDragAndDropping}"
                            TargetType="views:SillyListCell"
                            Value="True">
                            <Setter Property="BackgroundColor" Value="{StaticResource DarkSurface}" />
                        </DataTrigger>
                    </views:SillyListCell.Triggers>
                </views:SillyListCell>
            </sho:Shadows>
        </ContentView>
    </sho:DraggableViewCell>
</DataTemplate>

<sho:CollectionView x:Name="CollectionView"
    CollectionPadding="10,30,10,75"
    CurrentIndex="{Binding CurrentIndex}"
    EnableDragAndDrop="True"
    InfiniteListLoader="{Binding SillyPeoplePaginator}"
    ItemHeight="120"
    ItemTemplate="{StaticResource VerticalDudeTemplate}"
    ItemsSource="{Binding SillyPeople}"
    ListLayout="Vertical"
    TapCommand="{Binding TapCommand}" />
```

<p align="center">
  A <code>CollectionView</code> with <code>ListLayout=Vertical</code>.
</p>
<p align="center">
  <img src="Docs/hlv_drag_iphone.png" width="250" />
</p>

Of course drag and drop is also available with this layout.


## Infinite Loading

You can achieve infinite loading really easily by using the ```Paginator``` component, and bind it to the ```InfiniteListLoader``` property.
All is explained here:

https://www.sharpnado.com/paginator-platform-independent/

## Drag and drop

If you want to have both drag and drop enabled and still be able to tap the item, you need to use the ```TapCommand``` on the `CollectionView` instead of the ```xamEffects:Commands.Tap``` on the `DataTemplate` content.
It's less nice since you won't have the nice color ripple, but it will work :)

The only thing you have to do to enable drag and drop is set `EnableDragAndDrop` to `true`.

The `DragAndDropStartCommand` and `DragAndDropEndedCommand` commands will pass as argument a `DragAndDropInfo` object:

```csharp
public class DragAndDropInfo
{
    public int To { get; }

    public int From { get; }

    public object Content { get; }
}
```

Contributor: Implemented by @jmmortega.

## DragAndDropTrigger and DragAndDropDirection

Since 1.8.2, you can now choose if you want to begin the drag and drop with a ``Pan`` gesture or a `LongPress`.

* `DragAndDropTrigger="Pan"`
* `DragAndDropTrigger="LongTap"`

You can also restrict the drag movement to a given direction:

* For the horizontal layout: `DragAndDropDirection = HorizontalOnly` 
* For the vertical layout: `DragAndDropDirection = VerticalOnly`

It will give a better more precise drag experience, more precise.

### Since 1.8.1

`EnableDragAndDrop` is now a bindable property, so you can enable it at runtime.

You can now also specify a custom animation when the `EnableDragAndDrop` is set to ture:

```csharp
CollectionView.DragAndDropEnabledAnimationAsync = async (viewCell, token) =>
{
    while (!token.IsCancellationRequested)
    {
        await viewCell.View.RotateTo(8);
        await viewCell.View.RotateTo(-8);
    }

    await viewCell.View.RotateTo(0);
};
```

will result in:

<p align="center">
  <img src="Docs/drag_enabled_animation.gif" width="400" />
</p>

You can decide to start the drag without long press on iOS thanks to the iOS specific property `iOSDragAndDropOnPanGesture`:

```xml
<sho:CollectionView
    ...
    iOSDragAndDropOnPanGesture="True" />
```


**Remark:** You don't have to inherit from `DraggableViewCell`, any `ViewCell` can be dragged.


### DraggableViewCell

The `DraggableViewCell` is useful for using triggers on the `IsDragAndDropping` property (changing its background color or elevation during drag and drop for example).

You can also disable the drag and drop for certain cells thanks to the `IsDraggable` property.


## Headers, groups and footers (only for linear layouts)

Since 2.0, you can assign a size to a `DataTemplate` using the `SizedDataTemplate` markup extension. 
This opens the door to the implementation of header/footer/group headers.

All you have to do is to use a `DataTemplateSelector` with `SizedDataTemplate` and set the size of the given `DataTemplate`.

Let's consider the following screen:

<p align="center">
  <img src="Docs/header_android.png" width="300" />
</p>

In our example, we want, a header, a footer, but also a group header (items are grouped by silliness degree, their "star" rating).
So we will be using inheritance on the view model side to achieve that:

```csharp
namespace DragAndDropSample.ViewModels
{
    public interface IDudeItem
    {
    }

    public class DudeHeader : IDudeItem
    {
    }

    public class DudeFooter : IDudeItem
    {
    }

    public class DudeGroupHeader : IDudeItem
    {
        public int StarCount { get; set; }

        public string Text => $"{StarCount} Stars";
    }

    public class SillyDudeVmo : IDudeItem
    {
        public SillyDudeVmo(SillyDude dude, ICommand tapCommand)
        {
            if (dude != null)
            {
                Id = dude.Id;
                Name = dude.Name;
                FullName = dude.FullName;
                Role = dude.Role;
                Description = dude.Description;
                ImageUrl = dude.ImageUrl;
                SillinessDegree = dude.SillinessDegree;
                SourceUrl = dude.SourceUrl;
            }

            TapCommand = tapCommand;
        }

        public bool IsMovable { get; protected set; } = true;

        public ICommand TapCommand { get; set; }

        public int Id { get; }

        public string Name { get; }

        public string FullName { get; }

        public string Role { get; }

        public string Description { get; }

        public string ImageUrl { get; }

        public double SillinessDegree { get; }

        public string SourceUrl { get; }

        public override string ToString()
        {
            return $"{FullName} silly degree: {SillinessDegree}";
        }
    }
}
```

Then after sorting our collection by rating, we will bind our `CollectionView` to the SillyPeople list.

```csharp
public class HeaderFooterGroupingPageViewModel : ANavigableViewModel
{
    public List<IDudeItem> SillyPeople
    {
        get => _sillyPeople;
        set => SetAndRaise(ref _sillyPeople, value);
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
    }
}
```

Thanks god for Linq!

You can see how easy it is to order and create our header view models.

Now let's switch to the XAML world!

We create a template for each of our header types:

```xml
 <ResourceDictionary>
    <DataTemplate x:Key="HeaderTemplate">
        <sho:DraggableViewCell x:Name="DraggableViewCell" IsDraggable="False">

            <ContentView Margin="0" BackgroundColor="{StaticResource DarkerSurface}">
                <Label
                    Style="{StaticResource TextSubhead}"
                    HorizontalOptions="Center"
                    Text="Look at my Nice Header!" />
            </ContentView>
        </sho:DraggableViewCell>
    </DataTemplate>

    <DataTemplate x:Key="FooterTemplate">
        <sho:DraggableViewCell x:Name="DraggableViewCell" IsDraggable="False">
            <StackLayout
                Padding="30,0,15,0"
                Orientation="Horizontal"
                Spacing="15">
                <ActivityIndicator
                    VerticalOptions="Center"
                    IsRunning="True"
                    Color="{StaticResource Accent}" />
                <Label
                    Style="{StaticResource TextSubhead}"
                    VerticalOptions="Center"
                    Text="Loading next dudes..." />
            </StackLayout>
        </sho:DraggableViewCell>
    </DataTemplate>

    <DataTemplate x:Key="GroupHeaderTemplate" x:DataType="viewModels:DudeGroupHeader">
        <sho:DraggableViewCell x:Name="DraggableViewCell" IsDraggable="False">
            <sho:Shadows x:Name="Shadow" Shades="{StaticResource VerticalNeumorphism}">
                <StackLayout
                    Margin="0,15,0,10"
                    Padding="0"
                    BackgroundColor="{StaticResource DarkerSurface}"
                    Orientation="Horizontal"
                    Spacing="0">

                    <Frame
                        WidthRequest="30"
                        HeightRequest="30"
                        Margin="15,0,10,0"
                        Padding="0"
                        HorizontalOptions="End"
                        VerticalOptions="Center"
                        BackgroundColor="{StaticResource Accent}"
                        CornerRadius="10"
                        HasShadow="False">
                        <Label
                            Style="{StaticResource TextTitle}"
                            HorizontalOptions="Center"
                            VerticalOptions="Center"
                            Text="{Binding StarCount}" />
                    </Frame>
                    <Label
                        Style="{StaticResource TextTitle}"
                        VerticalOptions="Center"
                        Text="Stars Dudes" />
                </StackLayout>
            </sho:Shadows>
        </sho:DraggableViewCell>
    </DataTemplate>

    <DataTemplate x:Key="DudeTemplate">
        <sho:DraggableViewCell x:Name="DraggableViewCell">
            <sho:Shadows
                x:Name="Shadow"
                CornerRadius="10"
                Shades="{StaticResource ThinDarkerNeumorphism}">
                <views:SillyListCell
                    Margin="16,13"
                    BackgroundColor="{StaticResource DarkerSurface}"
                    CornerRadius="10">
                    <views:SillyListCell.Triggers>
                        <DataTrigger
                            Binding="{Binding Source={x:Reference DraggableViewCell}, Path=IsDragAndDropping}"
                            TargetType="views:SillyListCell"
                            Value="True">
                            <Setter Property="BackgroundColor" Value="{StaticResource DarkSurface}" />
                        </DataTrigger>
                    </views:SillyListCell.Triggers>
                </views:SillyListCell>
            </sho:Shadows>
        </sho:DraggableViewCell>
    </DataTemplate>
</ResourceDictionary>
```

The last step is to make the correspondance between our header view models, and our headers data templates.
For that, we declare our `DataTemplateSelector`:

```csharp
public class HeaderFooterGroupingTemplateSelector: DataTemplateSelector
{
    public SizedDataTemplate HeaderTemplate { get; set; }

    public SizedDataTemplate FooterTemplate { get; set; }

    public SizedDataTemplate GroupHeaderTemplate { get; set; }

    public DataTemplate DudeTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        switch (item)
        {
            case DudeHeader header:
                return HeaderTemplate;

            case DudeFooter footer:
                return FooterTemplate;

            case DudeGroupHeader groupHeader:
                return GroupHeaderTemplate;

            default:
                return DudeTemplate;
        }
    }
}
```

You can see that all the headers (all the data template with an associated size in fact) need to be a `SizedDataTemplate`.
Then we just assign a fixed size to each template when we declare our `DataTemplateSelector` :

```xml
<views:HeaderFooterGroupingTemplateSelector
    x:Key="HeaderFooterGroupingTemplateSelector"
    DudeTemplate="{StaticResource DudeTemplate}"
    FooterTemplate="{sho:SizedDataTemplate Template={StaticResource FooterTemplate},
                                            Size=60}"
    GroupHeaderTemplate="{sho:SizedDataTemplate Template={StaticResource GroupHeaderTemplate},
                                                Size=75}"
    HeaderTemplate="{sho:SizedDataTemplate Template={StaticResource HeaderTemplate},
                                            Size=40}" />
```

We don't have to assign a size to our item template (here the silly dude), it will pick the `ItemWidth` (for an horizontal layout) or `ItemHeight` (for a vertical one) size.



https://user-images.githubusercontent.com/596903/138863695-1a3c426f-6d3c-4096-a743-20392fe9db3c.mp4



You can find this example in the sample project (click on "Header and Grouping Example" button).



## Reveal animations

Contributor: original idea from @jmmortega.

You can set custom animations on cells that will be triggered when a cell appears for the first time.

*Properties for reveal animations*
```csharp
public Func<ViewCell, Task> PreRevealAnimationAsync { get; set; }

public Func<ViewCell, Task> RevealAnimationAsync { get; set; }

public Func<ViewCell, Task> PostRevealAnimationAsync { get; set; }
```

In the following example I flip the cell on the vertical axis and fade them for grid and linear layout. And flip the cell on the horizontal axis for vertical layout.

*GridPage.xaml.cs*


```csharp
public partial class GridPage : ContentPage
{
    public GridPage()
    {
        InitializeComponent();

        CollectionView.PreRevealAnimationAsync = async (viewCell) =>
        {
            viewCell.View.Opacity = 0;

            if (CollectionView.CollectionLayout == CollectionViewLayout.Vertical)
            {
                viewCell.View.RotationX = 90;
            }
            else
            {
                viewCell.View.RotationY = -90;
            }
        };

        CollectionView.RevealAnimationAsync = async (viewCell) =>
        {
            await viewCell.View.FadeTo(1);

            if (CollectionView.CollectionLayout == CollectionViewLayout.Vertical)
            {
                await viewCell.View.RotateXTo(0);
            }
            else
            {
                await viewCell.View.RotateYTo(0);
            }
        };
    }
}
```

<p align="center">
  <img src="Docs/reveal.gif" width="250" />
</p>

## Others properties

### Properties available with both layout mode

```csharp
public static readonly BindableProperty ListLayoutProperty = BindableProperty.Create(
    nameof(ListLayout),
    typeof(CollectionViewLayout),
    typeof(CollectionView),
    CollectionViewLayout.Linear,
    propertyChanged: OnListLayoutChanged,
    propertyChanging: OnListLayoutChanging);

public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
    nameof(ItemsSource),
    typeof(IEnumerable),
    typeof(CollectionView),
    default(IEnumerable<object>),
    BindingMode.TwoWay,
    propertyChanged: OnItemsSourceChanged);

public static readonly BindableProperty InfiniteListLoaderProperty = BindableProperty.Create(
    nameof(InfiniteListLoader),
    typeof(IInfiniteListLoader),
    typeof(CollectionView));

public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
    nameof(ItemTemplate),
    typeof(DataTemplate),
    typeof(CollectionView),
    default(DataTemplate));

public static readonly BindableProperty ItemHeightProperty = BindableProperty.Create(
    nameof(ItemHeight),
    typeof(double),
    typeof(CollectionView),
    defaultValue: 0D,
    defaultBindingMode: BindingMode.OneWayToSource);

public static readonly BindableProperty ItemWidthProperty = BindableProperty.Create(
    nameof(ItemWidth),
    typeof(double),
    typeof(CollectionView),
    defaultValue: 0D,
    defaultBindingMode: BindingMode.OneWayToSource);

public static readonly BindableProperty CollectionPaddingProperty = BindableProperty.Create(
    nameof(CollectionPadding),
    typeof(Thickness),
    typeof(CollectionView),
    defaultValue: new Thickness(0, 0),
    defaultBindingMode: BindingMode.OneWayToSource);

public static readonly BindableProperty ItemSpacingProperty = BindableProperty.Create(
    nameof(ItemSpacing),
    typeof(int),
    typeof(CollectionView),
    defaultValue: 0,
    defaultBindingMode: BindingMode.OneWayToSource);

public static readonly BindableProperty TapCommandProperty = BindableProperty.Create(
    nameof(TapCommand),
    typeof(ICommand),
    typeof(CollectionView));

public static readonly BindableProperty ScrollBeganCommandProperty = BindableProperty.Create(
    nameof(ScrollBeganCommand),
    typeof(ICommand),
    typeof(CollectionView));

public static readonly BindableProperty ScrollEndedCommandProperty = BindableProperty.Create(
    nameof(ScrollEndedCommand),
    typeof(ICommand),
    typeof(CollectionView));

public static readonly BindableProperty CurrentIndexProperty = BindableProperty.Create(
    nameof(CurrentIndex),
    typeof(int),
    typeof(CollectionView),
    defaultValue: -1,
    defaultBindingMode: BindingMode.TwoWay,
    propertyChanged: OnCurrentIndexChanged);

public static readonly BindableProperty VisibleCellCountProperty = BindableProperty.Create(
    nameof(VisibleCellCount),
    typeof(int),
    typeof(CollectionView),
    defaultValue: 0,
    defaultBindingMode: BindingMode.TwoWay,
    propertyChanged: OnVisibleCellCountChanged);

public static readonly BindableProperty DisableScrollProperty = BindableProperty.Create(
    nameof(DisableScroll),
    typeof(bool),
    typeof(CollectionView),
    defaultValue: false,
    defaultBindingMode: BindingMode.TwoWay);

public event EventHandler<ListLayoutChangedEventArgs> ListLayoutChanging;

public Func<ViewCell, Task> PreRevealAnimationAsync { get; set; }

public Func<ViewCell, Task> RevealAnimationAsync { get; set; }

public Func<ViewCell, Task> PostRevealAnimationAsync { get; set; }

/// In certain scenarios, the first scroll of the list can be smoothen
/// by pre-building some views.
public int ViewCacheSize { get; set; } = 0;

public bool EnableDragAndDrop { get; set; } = false;

public SnapStyle SnapStyle { get; set; } = SnapStyle.None;

public int ColumnCount { get; set; } = 0;

public ScrollSpeed ScrollSpeed { get; set; } = ScrollSpeed.Normal;

```

### Properties available with Grid and Vertical ListLayout

```csharp
public bool EnableDragAndDrop { get; set; } = false;

public static readonly BindableProperty DragAndDropStartedCommandProperty = BindableProperty.Create(
    nameof(DragAndDropStartedCommand),
    typeof(ICommand),
    typeof(CollectionView));

public static readonly BindableProperty DragAndDropEndedCommandProperty = BindableProperty.Create(
    nameof(DragAndDropEndedCommand),
    typeof(ICommand),
    typeof(CollectionView));

public static readonly BindableProperty IsDragAndDroppingProperty = BindableProperty.Create(
    nameof(IsDragAndDropping),
    typeof(bool),
    typeof(CollectionView),
    defaultValue: false);
```

## Some implementation details

### Android

The Android renderer is implemented with a ```RecyclerView```.
Padding and item spacing is computed by an extension of ```ItemDecoration```.
While column computing and item distribution is achieved by a custom ```GridLayoutManager```.
The Snap to first item is implemented with a custom ```LinearSnapHelper```. Drag and drop is handled by an ```ItemTouchHelper.Callback```.

### iOS

The iOS renderer is implemented by a ```UICollectionView```.
Padding and item spacing are natively provided by the ```UICollectionViewFlowLayout```.
Snap to Center item is brought by a little trick on ```DecelerationEnded``` callback.
Drag and drop is handled by a ```UILongPressGestureRecognizer``` followed by calls to the ```xxxInteractiveMovementxxx``` methods.
