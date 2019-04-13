+++
title = "Explore your Akavache cache on UWP"
tags = [ "UWP", "Akavache" ]
date = 2017-01-25 20:07:37
+++


[Akavache](https://github.com/akavache/akavache) is an awesome library for almost every .NET desktop and mobile application platform to store both important user data and expiring local cache data. I've been using Akavache in a UWP app to cache results from a web service. In the Akavache README, the [Akavache Explorer](https://github.com/paulcbetts/AkavacheExplorer) application is recommended for debugging the cache.
<!-- more -->

There are no binaries of the Akavache Explorer available for download, so you'll have to compile it yourself. The README mentions Visual Studio 2010 is required (yes, this application is old), but it compiles just fine in Visual Studio 2017 RC. After starting the application, things get a little harder: the explorer needs a path to the cache file in order to work. However, you've probably never specified a location for that file in your code, so where can you find it?

![Screenshot of the Akavache Explorer asking for a file path](/assets/20170125-explore-your-akavache-cache-on-uwp/akavache_explorer.png "Akavache Explorer asks for a path")

The application data for a UWP app is stored in the `<user>\AppData\Local\Packages\<package name>` folder, which can be quickly found using the `%LocalAppData%\Packages\<package name>` shortcut. If you're unsure what the name of your package is, you can look it up in the Package.appxmanifest file, on the Packaging tab.

![The Packaging tab of Package.appxmanifest](/assets/20170125-explore-your-akavache-cache-on-uwp/package_name.png "The package name can be found in the Package.appxmanifest file")

The different types of cache all have their own cache file:
- LocalMachine: `\LocalState\BlobCache\blobs.db`
- UserAccount: `\RoamingState\BlobCache\userblobs.db`
- Secure: `\RoamingState\SecretCache\secret.db`

So in order to explore your LocalMachine cache, you'll have to enter `%LocalAppData%\Packages\<package name>\LocalState\BlobCache\blobs.db` into the Path field of the Akavache Explorer. You'll have to check the 'Open as SQLite3 Cache' option for every type of cache, the 'Open as Encrypted Cache' is for the Secure cache.

Notice that both the UserAccount and Secure caches are stored in the `RoamingState` folder, which means that these files are automatically synced across all Windows 10 devices logged in with the same Microsoft account. In other words, you shouldn't use these for caching web requests, but they are great for storing settings and authentication.

If you know where to look, debugging your Akavache cache is a rather easy task.

*Thanks for reading! Enjoyed this post? Please share it with your friends using the buttons below! Have some questions? Comments can be found below too! And on a final note: grammar corrections are always welcome! English is not my primary language, but that's not a reason to don't learn how to use the language correctly.*