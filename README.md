# GroupedObservableCollection
An implementation of ObservableCollection that allows for grouped data to be presented in a XAML type of UI, providing support for updates to the underlying data. 

![Demo of the collection in use in a grouped ListView in a UWP app](https://raw.githubusercontent.com/mikegoatly/GroupedObservableCollection/master/docs/demo.gif)

## Quick start
_The demo UWP app in the repo shows most of these principals._

Create a new collection from an existing set of data:
```c#
var carList = GetListOfCarsFromSomewhere();

// Imaginary type called Car with a string property "Model"
// The delegate instructs the collection what data should be used to group entries together
var groupedCollection = new GroupedObservableCollection<string, Car>(c => c.Model, carList);
```

Bind that to a CollectionViewSource (or alternatively set it programmatically in code behind):
```xml
<Grid Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
    <Grid.Resources>
        <CollectionViewSource IsSourceGrouped="True" x:Name="Source" Source="{Binding GroupedCollection}" />
    </Grid.Resources>
    ...
```

Configure a ListView to work from the CollectionViewSource:
```xml
<ListView ItemsSource="{Binding Source={StaticResource Source}}">
    <ListView.GroupStyle>
        <GroupStyle>
            <GroupStyle.HeaderTemplate>
                <DataTemplate>
                    <TextBlock Text="{Binding Key}" />
                </DataTemplate>
            </GroupStyle.HeaderTemplate>
        </GroupStyle>
    </ListView.GroupStyle>
    <ListView.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Name}" />
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

And then you can manipulate the collection as much as you like behind the scenes - the UI will stay in sync.

## Simple collection operations
```c#
// Add an item - the correct group will automatically be appended to
myCollection.Add(new Car { ... });

// Remove an item from - the correct group with be removed from. If the item is the last in the group,
// the group itself will be removed
myCollection.Remove(car);

// Test for the existence of an item
myCollection.Contains(car);

// Enumerate all the items in the collection in group order
var allCars = myCollection.EnumerateItems().ToList();

// Enumerate all the group keys in the collection
var allKeys = myCollection.Keys.ToList();
```

## Advanced collection operations
### ReplaceWith(GroupedObservableCollection<TKey, TElement> replacement, IEqualityComparer<TElement> comparer)
This method supports the scenario where an initial set of data needs to be refreshed with an updated list:
* Items that exist in the current collection and in the replacement will remain
* Items that are in the current collection, but are no longer in the replacement will be removed
* New items unique to the replacement collection will be added
* The order of items will be updated in the current collection to be consistent with the replacement collection

_ReplaceWith feels like a bad name. I considered MergeWith, but it's not really a merge because items are deleted from the collection as well as added, i.e. the replacement always wins. Alternative suggestions welcome :)_

## Current limitations
* Outside of initial collection creation and ReplaceWith calls, no sort order is applied to items within a group. i.e. if you programatically call Add, the new item will always be added to the end of the group it belongs to.
* Ordering of groups _is_ done, but according to the default comparison logic for the key type and only in ascending order.
