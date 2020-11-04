# Sharpnado.Presentation.Forms

| Lib | Version                                                                                                                             |
| -------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| Sharpnado.Presentation.Forms  | ![Sharpnado.Presentation.Forms](https://img.shields.io/nuget/v/Sharpnado.Presentation.Forms.svg) |
| Sharpnado.Forms.HorizontalListView      | ![Sharpnado.Forms.HorizontalListView](https://img.shields.io/nuget/v/Sharpnado.Forms.HorizontalListView.svg) |

| Platform | Build Status                                                                                                                             |
| -------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| Android  | [![Build status](https://build.appcenter.ms/v0.1/apps/23f44cf3-7656-4932-9d82-f654db6afc82/branches/master/badge)](https://appcenter.ms) |
| iOS      | [![Build status](https://build.appcenter.ms/v0.1/apps/ddd14409-1f42-4521-ae8d-6f9891de2714/branches/master/badge)](https://appcenter.ms) |


## MUST READ: Big refactoring ?

The big sharpnado refactoring is over.

Each sharpnado's component has now its own repo.

* Sharpnado.Tabs have now their own repo
* The Sharpnado.Presentation.Forms repo now only contains the source code for the HorizontalListView

Latest version of Sharpnado.Presentation.Forms (v1.7.1) doesn't have all the sharpnado nugets up to date.

Preferred way of using packages is now to install only the one needed.

## Sample App: the Silly! app

All the following components are presented in the Silly! app in the following repository:

https://github.com/roubachof/Xamarin-Forms-Practices

If you want to know how to use the components, it's the best place to start.

## Initialization

**IMPORTANT:** On platform projects, call `SharpnadoInitializer.Initialize()` after `Xamarin.Forms.Forms.Init()` and before `LoadApplication(new App())`.


## Featured Components 

Xamarin Forms custom components and renderers starring:

### [``Sharpnado.Tabs``](https://github.com/roubachof/Sharpnado.Tabs)

<img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/Tabs/logo_2_1.png" width="200" />

* Fully customizable
* Underlined tabs, bottom tabs, Segmented control, scrollable tabs
* BadgeView
* Component oriented architecture
* Layout your tabs and ```ViewSwitcher``` as you want
* Shadows included in `TabHost`
* Bindable

![banner](https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/Tabs/github_banner.jpg)

### [``Sharpnado.Shadows``](https://github.com/roubachof/Sharpnado.Shadows)

<img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/Shadows/shadows.png" width="200" />

* Add as **many** **custom** shadows as you like to any `Xamarin.Forms` view (`Android`, `iOS`, `UWP`). 
* You can specify each shadow `Color`, `Opacity`, `BlurRadius`, and `Offset`
* Simply implement `Neumorphism`
* You can add one shadow, 3 shadows, 99 shadows, to any `Xamarin.Forms` element
* Animate any of these property and make the shadows dance around your elements
* No `AndroidX` or `SkiaSharp` dependency required, only `Xamarin.Forms`

![Presentation](https://raw.githubusercontent.com/roubachof/Sharpnado.Shadows/master/Docs/github_banner.png)

### [``Sharpnado.MaterialFrame``](https://github.com/roubachof/Sharpnado.MaterialFrame)

<img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/MaterialFrame/material_frame.png" width="200" />

  * 4 built-in themes: AcrylicBlur/Acrylic/Dark/Light
  * 3 Blur Styles: Light/ExtraLight/Dark
  * Based on `RealtimeBlurView` on Android and `UIVisualEffectView` on iOS
  * Dark elevation
  * LightBackground color
  * CornerRadius
  * Performance

![banner](https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/MaterialFrame/github_banner.png)


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


### [```Sharpnado.TaskLoaderView 2.0``` handles all your task loading states](https://github.com/roubachof/Sharpnado.TaskLoaderView)

<img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/TaskLoaderView/tlv_icon_tos.png" width="150" />

  * Handles error with custom messages and icons
  * Handles empty states
  * Show snackbar errors for refresh scenarios (if data is already shown)
  * Handles retry with button
  * Support Xamarin.Forms.Skeleton
  * Can override any state views with your own custom ones

<p float="left" align="middle">
  <img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/tlv_skeleton.gif" width="320" hspace="20"/>
  <img src="https://github.com/roubachof/Sharpnado.Presentation.Forms/wiki/Images/tlv_user_views.gif" width="320" hspace="20"/>
</p>

**IMPORTANT:** On platform projects, call SharpnadoInitializer.Initialize() after Xamarin.Forms.Forms.Init() and before LoadApplication(new App()).

Those components are used and tested in the Silly! app:  https://github.com/roubachof/Xamarin-Forms-Practices.

## Open Source licenses and inspirations

* Special thanks to Daniel John Causer (https://causerexception.com) for inspiring the horizontal list.
* Thanks to alex dunn for his ```MaterialFrame``` idea.
* Thanks to Vladislav Zhukov (https://github.com/mrxten/XamEffects) for its ```TapCommand``` and ```TouchFeedbackColor``` effects, Copyright (c) 2017 Vladislav Zhukov, under MIT License (MIT).
* I greet his grace Stephen Cleary (https://github.com/StephenCleary) who cast his holy words on my async soul (https://www.youtube.com/watch?v=jjaqrPpdQYc). ```NotifyTask``` original code, Copyright (c) 2015 Stephen Cleary, under MIT License (MIT).
