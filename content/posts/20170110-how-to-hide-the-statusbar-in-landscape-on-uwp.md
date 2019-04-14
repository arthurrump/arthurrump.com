+++
title = "How to hide the StatusBar in Landscape on UWP"
tags = [ "UWP", "Windows 10 Mobile" ]
date = 2017-01-10 22:21:07
blurb = "The StatusBar takes up quite some space, here's how you can hide it in your app when switching to landscape."
+++


Windows 10 Mobile has lost a lot of the awesome UI/UX from the good ol' Windows Phone 7 era, but there's one thing that they kept around: the enormous amount of space the statusbar takes up in landscape view. I still love the look and feel of the old Windows Phone, but this thing has been bugging me since, well, my first smartphone.

![Windows Phone 7 also had a wide landscape statusbar on the side](/assets/20170110-how-to-hide-the-statusbar-in-landscape-on-uwp/wp7.jpg "The StatusBar on WP7 (screenshots were impossible, back then)")

In my apps, I prefer to hide the statusbar when the user goes into landscape mode, but just let it stay where it is when in portrait mode. It includes some useful information, after all, and in portrait the amount of used space is acceptable. Here's my solution:

------
tl;dr:
```csharp
if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
{
    Window.Current.SizeChanged += async (sender, e) =>
    {
        var statusBar = StatusBar.GetForCurrentView();
        if (ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Landscape)
            await statusBar.HideAsync();
        else
            await statusBar.ShowAsync();
    };
}
```
------

To get access to the statusbar, we first need to reference the Mobile Extensions, as Mobile's the only SKU of Windows 10 where the statusbar is available. This can be done using the 'Add reference' dialog, which can be found by right-clicking 'References' in the Solution Explorer in Visual Studio. After navigating to Universal Windows - Extensions in the left menu, make sure you **check the checkbox** before the version of the Mobile Extensions you're targeting:

![Windows Mobile Extensions for the UWP selected in the 'Add reference' dialog](/2017/01/10/how-to-hide-the-statusbar-in-landscape-on-uwp/references.png "The 'Add reference' dialog")

Now we can get access to the statusbar, how do we do that? The `StatusBar` class has a static method called `GetForCurrentView()` to get access to the statusbar, so this is easy:

```csharp
using Windows.UI.ViewManagement;
var statusBar = StatusBar.GetForCurrentView();
```

Now we want to hide the statusbar if we're in landscape orientation, else we want to show the statusbar. To find out the orientation we can use the `ApplicationView` class, which is also in the `Windows.UI.ViewManagement` namespace:

```csharp
if (ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Landscape)
    await statusBar.HideAsync();
else
    await statusBar.ShowAsync();
```

We want to execute this every time the orientation of the device changes, but unfortunately, there is no `OrientationChanged` event. What we want to use is the `Window.Current.SizeChanged` event in the `Windows.UI.Xaml` namespace. We can hook this all up this way:

```csharp
Window.Current.SizeChanged += async (sender, e) =>
{
    var statusBar = StatusBar.GetForCurrentView();
    if (ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Landscape)
        await statusBar.HideAsync();
    else
        await statusBar.ShowAsync();
};
```

However, when this code is executed on a non-Mobile Windows 10 device, it will generate a runtime exception. Of course, that's not what we want; this code should just be ignored if it doesn't apply. To accomplish this we can check if the `StatusBar` type is available, before executing the code:

```csharp
if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
{
    Window.Current.SizeChanged += async (sender, e) =>
    {
        var statusBar = StatusBar.GetForCurrentView();
        if (ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Landscape)
            await statusBar.HideAsync();
        else
            await statusBar.ShowAsync();
    };
}
```

Finally, where should we place this snippet? I want my entire application to behave this way, so I put the code to set this up in the `OnInitializeAsync()` method of the [Template 10](https://aka.ms/template10) BootStrapper. You can also choose to enable this functionality later in the runtime of your app. I'd recommend moving the event handler into its own method, so the functionality can easily be disabled by unsubscribing the event:

```csharp
Window.Current.SizeChanged += HideStatusBarIfLandscape;
Window.Current.SizeChanged -= HideStatusBarIfLandscape; // Disabling by unsubscribing
```

In this case, you should also check for availability of the statusbar, before subscribing to the event. If the code inside the handler won't run anyway, subscribing is just unnecessary overhead.

I think this little snippet of code belongs in almost every app, and I'll definitely use it in most of mine.