# Sharpnado.Presentation.Forms

| Lib | Version                                                                                                                             |
| -------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| Sharpnado.Presentation.Forms  | ![Sharpnado.Presentation.Forms](https://img.shields.io/nuget/v/Sharpnado.Presentation.Forms.svg) |
| Sharpnado.Forms.HorizontalListView      | ![Sharpnado.Forms.HorizontalListView](https://img.shields.io/nuget/v/Sharpnado.Forms.HorizontalListView.svg) |

| Platform | Build Status                                                                                                                             |
| -------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| Android  | [![Build status](https://build.appcenter.ms/v0.1/apps/23f44cf3-7656-4932-9d82-f654db6afc82/branches/master/badge)](https://appcenter.ms) |
| iOS      | [![Build status](https://build.appcenter.ms/v0.1/apps/ddd14409-1f42-4521-ae8d-6f9891de2714/branches/master/badge)](https://appcenter.ms) |

Xamarin Forms custom components and renderers starring:

### ["Pure" ```Xamarin.Forms``` Tabs (no renderers)](https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Pure-Xamarin.Forms-tabs)
* Fully customizable
* Stylable
* Component oriented architecture
* Layout your tabs and ```ViewSwitcher``` as you want

<table>
	<thead>
		<tr>
			<th>Fixed tabs</th>
			<th>Bottom bar tabs</th>
      <th>Scrollable tabs</th>
		</tr>
	</thead>
	<tbody>
		<tr>
      <td><img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/android_top_tabs.png" width="250" height=500/></td>
  		<td><img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/bottom_tabs.gif" width="250" height=500/></td>
      <td><img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/scrollable_tabs_underline_text.gif" width="250" /></td>
		</tr>
    <tr>
      <td>UnderlinedTabItem</td>
			<td>BottomTabItem</td>
      <td>TabType.Scrollable</td>
      
    </tr>   
  </tbody>
</table>

<table>
	<thead>
		<tr>			
      <th>Circle button</th>
			<th>Underline all</th>
      <th>Custom tabs</th>
		</tr>
	</thead>
	<tbody>
		<tr>
      <td><img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/tab_circle_button.png" width="250" /></td>
      <td><img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/slide_tabs_whole.gif" width="250" /></td>
      <td><img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/spam_tabs.gif" width="250" height=500 /></td>
		</tr>
    <tr>
      <td>TabButton</td>
      <td>UnderlineAllTab=True</td>
      <td>inherit from TabItem</td>
    </tr>   
  </tbody>
</table>

### [```HorizontalListView``` for Xamarin Forms](https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/HorizontalListView-Grid-And-Carousel)
  * Carousel layout
  * Column count
  * Infinite loading with ```Paginator``` component
  * Snapping on first or middle element
  * Padding and item spacing
  * Handles ```NotifyCollectionChangedAction``` Add, Remove and Reset actions
  * View recycling
  * ```RecyclerView``` on Android
  * ```UICollectionView``` on iOS
  * This implementation is in fact very close in terms of philosophy and implementation to what will provide the future Xamarin ```CollectionView```.

<p float="left" align="middle">
  <img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/horizontal_snap_center.gif" width="250" hspace="20"/>
  <img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/carousel-ios.gif" width="250" hspace="20"/>
</p>

### [```Grid``` collection view (```ListLayout``` = ```Grid```)](https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/HorizontalListView-Grid-And-Carousel#grid-layout)
  * Column count (if equal to 1 then you have a classic ```ListView``` ;)
  * Infinite loading with ```Paginator``` component
  * Drag and Drop
  * Padding and item spacing
  * Handles ```NotifyCollectionChangedAction``` Add, Remove and Reset actions
  * View recycling

<p float="left" align="middle">
  <img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/drag_and_drop.gif" width="250" hspace="20"/>
  <img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/listview-dragandrop.gif" width="250" hspace="20"/>
</p>

### [```TaskLoaderView``` displays an ```ActivityLoader``` while loading](https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/TaskLoaderView)
  * Handles error with custom messages and icons
  * Handles empty states
  * Don't show activity loader for refresh scenarios (if data is already shown)
  * Handles retry with button
  * Pure Xamarin Forms view: no renderers

<p align="center">
  <img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/task_loader_view.gif" width="250"  />
</p>


It's available in 2 Nuget flavors:

* **Sharpnado.Presentation.Forms** (which include several others components like the increeeedible ```TaskLoaderView```)
* **Sharpnado.Forms.HorizontalListView** (```HorizontalListView``` only with only ```TapCommand``` and ```MaterialFrame```)

**IMPORTANT:** On platform projects, call SharpnadoInitializer.Initialize() after Xamarin.Forms.Forms.Init() and before LoadApplication(new App()).

Those components are used and tested in the Silly! app:  https://github.com/roubachof/Xamarin-Forms-Practices.

## Open Source licenses and inspirations

* Special thanks to Daniel John Causer (https://causerexception.com) for inspiring the horizontal list.
* Thanks to alex dunn for his ```MaterialFrame``` idea.
* Thanks to Vladislav Zhukov (https://github.com/mrxten/XamEffects) for its ```TapCommand``` and ```TouchFeedbackColor``` effects, Copyright (c) 2017 Vladislav Zhukov, under MIT License (MIT).
* I greet his grace Stephen Cleary (https://github.com/StephenCleary) who cast his holy words on my async soul (https://www.youtube.com/watch?v=jjaqrPpdQYc). ```NotifyTask``` original code, Copyright (c) 2015 Stephen Cleary, under MIT License (MIT).
