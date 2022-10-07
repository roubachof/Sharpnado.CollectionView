# Sharpnado's CollectionView
* Performance oriented
* Horizontal, Grid, Carousel or Vertical layout
* Header, Footer and GroupHeader
* Reveal custom animations
* Drag and Drop
* Column count
* Infinite loading with Paginator component
* Snapping on first or middle element
* Padding and item spacing
* Handles NotifyCollectionChangedAction Add, Remove and Reset actions
* View and data template recycling
* RecyclerView on Android
* UICollectionView on iOS

## Installation

* In Core project, in `MauiProgram.cs`:

```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp()
        .UseSharpnadoCollectionView(loggerEnabled: false);
}
```

## Usage

