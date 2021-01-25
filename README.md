# Sharpnado.HorizontalListView

<p align="left"><img src="Docs/logo.png" height="180"/>

Get it from NuGet:

[![Nuget](https://img.shields.io/nuget/v/Sharpnado.Forms.HorizontalListView.svg)](https://www.nuget.org/packages/Sharpnado.Forms.HorizontalListView)

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

    Sharpnado.HorizontalListView.Initializer.Initialize(true, false);
    ...
}
```

* On `iOS` add this line before `Xamarin.Forms.Forms.Init()` and `LoadApplication(new App())`.

```csharp
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    SharpnadoInitializer.Initialize();

    global::Xamarin.Forms.Forms.Init();
    LoadApplication(new App());
}
```

* On `Android` add this line before `Xamarin.Forms.Forms.Init()` and `LoadApplication(new App())`.

```csharp
public override OnCreate(Bundle savedInstanceState)
{
    SharpnadoInitializer.Initialize();

    global::Xamarin.Forms.Forms.Init();
    LoadApplication(new App());
}

```

## Version 1.8 breaking change

Namespace changed from `Sharpnado.Presentation.Forms.HorizontalListView` to `Sharpnado.HorizontalListView`.

`HorizontalListView`, like `MaterialFrame`, `Tabs` and `Shadows`, now uses the same xml namespace: http://sharpnado.com.

Because of how works xaml compilation, you need to add code in your `App.xaml.cs` referencing the sharpnado assembly:

```csharp
public App()
{
    InitializeComponent();

    Sharpnado.HorizontalListView.Initializer.Initialize(true, false);
    ...
}
```

## Presentation

 * Horizontal, Grid, Carousel or Vertical layout
 * Reveal custom animations
 * Drag and Drop feature
 * Column count
 * Infinite loading with ```Paginator``` component
 * Snapping on first or middle element
 * Padding and item spacing
 * Handles ```NotifyCollectionChangedAction``` Add, Remove and Reset actions
 * View recycling
 * ```RecyclerView``` on Android
 * ```UICollectionView``` on iOS


## Linear layout

```csharp
public HorizontalListViewLayout ListLayout { get; set; } = HorizontalListViewLayout.Linear;
```
By default the layout is in ```Linear``` mode, which means you will have only one row.
You can specify the ```ItemWidth``` and ```ItemHeight```.
You can also specify ```ItemSpacing``` and ```CollectionPadding```.

*GridPage.xaml*

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

<sho:HorizontalListView x:Name="HorizontalListView"
    CollectionPadding="10,30,10,75"
    CurrentIndex="{Binding CurrentIndex}"
    InfiniteListLoader="{Binding SillyPeoplePaginator}"
    ItemHeight="260"
    ItemWidth="260"
    ItemTemplate="{StaticResource HorizontalDudeTemplate}"
    ItemsSource="{Binding SillyPeople}"
    ListLayout="Linear"
    ListLayoutChanging="ListLayoutChanging"
    ScrollBeganCommand="{Binding OnScrollBeginCommand}"
    ScrollEndedCommand="{Binding OnScrollEndCommand}"
    SnapStyle="Center"
    TapCommand="{Binding TapCommand}" />
```

As you can see ```TapCommand``` and ```TouchFeedbackColor``` (aka Ripple) are brought to you by the awesome effects created by mrxten (https://github.com/mrxten/XamEffects). 
It's the best ripple effect plugin so far on Xamarin.Forms since it always worked for me.
With other maybe more known plugins, I had some issues on iOS.

<p align="center">
  A <code>HorizontalListView</code> with <code>SnapStyle=Center</code> and <code>ItemWidth/ItemHeight</code> set.
</p>
<p align="center">
  <img src="Docs/hlv_horizontal_android.png" width="250" />
</p>



### ColumnCount property

You can also decide to just specify the number of column you want, the ```ColumnCount``` property, and the ```ItemWidth``` will be computed for you.

```xml
<sho:HorizontalListView
    x:Name="HorizontalListView"
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
  A <code>HorizontalListView</code> with <code>ColumnCount=2</code>.
</p>
<p align="center">
  <img src="Images/../Docs/hlv_horizontal_android_col2.png" width="250" />
</p>

### Carousel Layout

You can set ```ListLayout``` to ```Carousel```.
In this mode you can't specify ```ItemWidth``` (obviously).
If you don't specify the ```ItemHeight```, it will be automatically computed for you.

```xml
<renderedViews:HorizontalListView Grid.Row="3"
                                  Margin="-16,8"
                                  CollectionPadding="8,8"
                                  ItemSpacing="8"
                                  ListLayout="Carousel"
                                  ItemsSource="{Binding SillyPeopleLoader.Result}"
                                  SnapStyle="Center">
    ...
</renderedViews:HorizontalListView>
```

<p align="center">
  A <code>HorizontalListView</code> with <code>ListLayout=Carousel</code>.
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

<sho:HorizontalListView x:Name="HorizontalListView"
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

You can also use Sharpnado's `HorizontalListView` like a regular list view.

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

<sho:HorizontalListView x:Name="HorizontalListView"
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
  A <code>HorizontalListView</code> with <code>ListLayout=Vertical</code>.
</p>
<p align="center">
  <img src="Docs/hlv_drag_iphone.png" width="250" />
</p>

Of course drag and drop is also available with this layout.

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

        HorizontalListView.PreRevealAnimationAsync = async (viewCell) =>
        {
            viewCell.View.Opacity = 0;

            if (HorizontalListView.ListLayout == HorizontalListViewLayout.Vertical)
            {
                viewCell.View.RotationX = 90;
            }
            else
            {
                viewCell.View.RotationY = -90;
            }
        };

        HorizontalListView.RevealAnimationAsync = async (viewCell) =>
        {
            await viewCell.View.FadeTo(1);

            if (HorizontalListView.ListLayout == HorizontalListViewLayout.Vertical)
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


## Infinite Loading

You can achieve infinite loading really easily by using the ```Paginator``` component, and bind it to the ```InfiniteListLoader``` property.
All is explained here:

https://www.sharpnado.com/paginator-platform-independent/

## Drag and drop

If you want to have both drag and drop enabled and still be able to tap the item, you need to use the ```TapCommand``` on the `HorizontalListView` instead of the ```xamEffects:Commands.Tap``` on the `DataTemplate` content.
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

### Since 1.8.1

`EnableDragAndDrop` is now a bindable property, so you can enable it at runtime.

You can now also specify a custom animation when the `EnableDragAndDrop` is set to ture:

```csharp
HorizontalListView.DragAndDropEnabledAnimationAsync = async (viewCell, token) =>
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
<sho:HorizontalListView
    ...
    iOSDragAndDropOnPanGesture="True" />
```


**Remark:** You don't have to inherit from `DraggableViewCell`, any `ViewCell` can be dragged.

### DraggableViewCell

The `DraggableViewCell` is useful for using triggers on the `IsDragAndDropping` property (changing its background color or elevation during drag and drop for example).

You can also disable the drag and drop for certain cells thanks to the `IsDraggable` property.

## Others properties

### Properties available with both layout mode

```csharp
public static readonly BindableProperty ListLayoutProperty = BindableProperty.Create(
    nameof(ListLayout),
    typeof(HorizontalListViewLayout),
    typeof(HorizontalListView),
    HorizontalListViewLayout.Linear,
    propertyChanged: OnListLayoutChanged,
    propertyChanging: OnListLayoutChanging);

public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
    nameof(ItemsSource),
    typeof(IEnumerable),
    typeof(HorizontalListView),
    default(IEnumerable<object>),
    BindingMode.TwoWay,
    propertyChanged: OnItemsSourceChanged);

public static readonly BindableProperty InfiniteListLoaderProperty = BindableProperty.Create(
    nameof(InfiniteListLoader),
    typeof(IInfiniteListLoader),
    typeof(HorizontalListView));

public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
    nameof(ItemTemplate),
    typeof(DataTemplate),
    typeof(HorizontalListView),
    default(DataTemplate));

public static readonly BindableProperty ItemHeightProperty = BindableProperty.Create(
    nameof(ItemHeight),
    typeof(double),
    typeof(HorizontalListView),
    defaultValue: 0D,
    defaultBindingMode: BindingMode.OneWayToSource);

public static readonly BindableProperty ItemWidthProperty = BindableProperty.Create(
    nameof(ItemWidth),
    typeof(double),
    typeof(HorizontalListView),
    defaultValue: 0D,
    defaultBindingMode: BindingMode.OneWayToSource);

public static readonly BindableProperty CollectionPaddingProperty = BindableProperty.Create(
    nameof(CollectionPadding),
    typeof(Thickness),
    typeof(HorizontalListView),
    defaultValue: new Thickness(0, 0),
    defaultBindingMode: BindingMode.OneWayToSource);

public static readonly BindableProperty ItemSpacingProperty = BindableProperty.Create(
    nameof(ItemSpacing),
    typeof(int),
    typeof(HorizontalListView),
    defaultValue: 0,
    defaultBindingMode: BindingMode.OneWayToSource);

public static readonly BindableProperty TapCommandProperty = BindableProperty.Create(
    nameof(TapCommand),
    typeof(ICommand),
    typeof(HorizontalListView));

public static readonly BindableProperty ScrollBeganCommandProperty = BindableProperty.Create(
    nameof(ScrollBeganCommand),
    typeof(ICommand),
    typeof(HorizontalListView));

public static readonly BindableProperty ScrollEndedCommandProperty = BindableProperty.Create(
    nameof(ScrollEndedCommand),
    typeof(ICommand),
    typeof(HorizontalListView));

public static readonly BindableProperty CurrentIndexProperty = BindableProperty.Create(
    nameof(CurrentIndex),
    typeof(int),
    typeof(HorizontalListView),
    defaultValue: -1,
    defaultBindingMode: BindingMode.TwoWay,
    propertyChanged: OnCurrentIndexChanged);

public static readonly BindableProperty VisibleCellCountProperty = BindableProperty.Create(
    nameof(VisibleCellCount),
    typeof(int),
    typeof(HorizontalListView),
    defaultValue: 0,
    defaultBindingMode: BindingMode.TwoWay,
    propertyChanged: OnVisibleCellCountChanged);

public static readonly BindableProperty DisableScrollProperty = BindableProperty.Create(
    nameof(DisableScroll),
    typeof(bool),
    typeof(HorizontalListView),
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
    typeof(HorizontalListView));

public static readonly BindableProperty DragAndDropEndedCommandProperty = BindableProperty.Create(
    nameof(DragAndDropEndedCommand),
    typeof(ICommand),
    typeof(HorizontalListView));

public static readonly BindableProperty IsDragAndDroppingProperty = BindableProperty.Create(
    nameof(IsDragAndDropping),
    typeof(bool),
    typeof(HorizontalListView),
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
