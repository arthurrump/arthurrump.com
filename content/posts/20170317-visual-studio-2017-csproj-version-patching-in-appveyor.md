+++
title = "Visual Studio 2017 .csproj version patching in AppVeyor"
tags = [ ".NET", ".csproj", "AppVeyor", "Continuous Deployment" ]
date = 2017-03-17 19:10:16
blurb = "How to set your package versions dynamically without creating an AssemblyInfo.cs file."
+++


After the debacle with project.json and .xproj, Microsoft settled on a simpler, more modern version of the old .csproj project system with Visual Studio 2017. One of the changes advanced users will notice when creating a new project in Visual Studio 2017, is the absence of AssemblyInfo.cs. You can add it back yourself, if you want to use some advanced options that aren't available in .csproj, like the ability to make internal types visible to other assemblies, but by default, it's gone.

This brings us to the topic of this post: the awesome [AppVeyor](https://www.appveyor.com/) Continuous Delivery service. One of the features offered by AppVeyor is so-called AssemblyInfo patching, which could change the version of your assembly to the one set in `appveyor.yml`, so the build number could be included, for example. However, with AssemblyInfo.cs no longer there, this feature, of course, won't work and you'll have to change your version number manually. Luckily, it's pretty easy:

```powershell
$path = (Get-Item .\Project\Project.csproj).FullName
$csproj = [xml](Get-Content $path)
$csproj.Project.PropertyGroup.Version = $Env:APPVEYOR_BUILD_VERSION
$csproj.Save($path)
```

Adding this PowerShell script to the `before_build` step will change the version number in the .csproj file to the version set in your `appveyor.yml` file, so your assemblies will have the correct version again.

## Update
As Lee Campbell [commented](http://disq.us/p/1j8rpnx) below, you can also use an MSBuild flag in your build command to set the version:

```powershell
dotnet build /p:Version=$Env:APPVEYOR_BUILD_VERSION
```

It feels like this is the right solution for the problem, instead of fiddling around and changing files.

*Thanks for reading! Enjoyed this post? Please share it with your friends using the buttons below! Have some questions? Comments can be found below too! And on a final note: grammar corrections are always welcome! English is not my primary language, but that's not a reason to don't learn how to use the language correctly.*