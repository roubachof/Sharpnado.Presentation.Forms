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
			<th>Bottom bar tabs</th>
      <th>Fixed tabs</th>
		</tr>
	</thead>
	<tbody>
		<tr>
      <td><img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/dark/ios_bottom_tabs.gif" width="300" /></td>
  		<td><img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/dark/android_fixed_tabs.gif" width="300" /></td>
		</tr>
    <tr>
      <td>BottomTabItem</td>
			<td>UnderlinedTabItem</td>
    </tr>   
  </tbody>
</table>

<table>
	<thead>
		<tr>			
      <th>Scrollable tabs</th>
      <th>Custom tabs</th>
		</tr>
	</thead>
	<tbody>
		<tr>
      <td><img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/dark/ios_scrollable_tabs.gif" width="300" /></td>
      <td><img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/dark/android_spam_tabs.gif" width="300" /></td>
		</tr>
    <tr>
      <td>TabType.Scrollable</td>
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
  <img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/dark/android_hlv.gif" width="320" hspace="20"/>
  <img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/dark/ios_hlv_carousel.gif" width="320" hspace="20"/>
</p>

### [```Grid``` collection view (```ListLayout``` = ```Grid```)](https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/HorizontalListView-Grid-And-Carousel#grid-layout)
  * Column count (if equal to 1 then you have a classic ```ListView``` ;)
  * Infinite loading with ```Paginator``` component
  * Drag and Drop
  * Padding and item spacing
  * Handles ```NotifyCollectionChangedAction``` Add, Remove and Reset actions
  * View recycling

<p float="left" align="middle">
  <img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/dark/android_list.gif" width="320" hspace="20"/>
  <img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/dark/ios_grid.gif" width="320" hspace="20"/>
</p>


### [```TaskLoaderView 2.0``` handles all your task loading states](https://github.com/roubachof/Sharpnado.TaskLoaderView)
  * Handles error with custom messages and icons
  * Handles empty states
  * Show snackbar errors for refresh scenarios (if data is already shown)
  * Handles retry with button
  * Support Xamarin.Forms.Skeleton
  * Can override any state views with your own custom ones

<p float="left" align="middle">
  <img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/dark/tlv_skeleton.gif" width="320" hspace="20"/>
  <img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/dark/tlv_user_views.gif" width="320" hspace="20"/>
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
