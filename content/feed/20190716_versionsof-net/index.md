---
title: "versionsof.net"
subtitle: "Bringing clarity to versions of .NET"
tags: [ ".NET" ]
category: Projects
aliases:
- /project/versionsof-net
---

.NET Core versioning is a mess. There are lots and lots and lots of version numbers, from runtime to SDK, from Visual Studio to languages and of course .NET Standard. So what SDK corresponds to which runtime again? I know there some sort of a system, but it differs per release. I created this website based on information published in the .NET Core repo to give an overview of all the versions of .NET.

![A tablet showing the versionsof.net website]({attach}screenshot.png)

Links:

- [Website](https://versionsof.net/)
- [GitHub repository](https://github.com/arthurrump/versionsof.net)

## Data from the source
The data for this website is directly retrieved from information released by the teams developing .NET. For .NET Core, these are the `releases.json` files and release notes Markdown files in the [dotnet/core](https://github.com/dotnet/core) repo, see also the [NetCore.Versions]({filename}../20190709_netcore-versions/index.md) project.

## Including .NET Framework and Mono
There are more versions of .NET besides .NET Core, so in a later update I also added a list of releases for .NET Framework and Mono. For Mono the information about releases is retrieved from [GitHub](https://github.com/mono/website/tree/gh-pages/docs/about-mono/releases), which is the same data as used for the [Mono website](https://www.mono-project.com/docs/about-mono/releases/). For .NET Framework the information is scraped from [Wikipedia](https://en.wikipedia.org/wiki/Template:.NET_Framework_version_history) and the [Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/versions-and-dependencies), using the excellent HtmlProvider in [FSharp.Data](http://fsharp.github.io/FSharp.Data/).

## Responsive tables
On larger screens, most information is shown in tables. This makes it easy to get an overview and compare different releases, but for small screens this results in a lot of scrolling. For mobile devices the tables are reduced to a cards based UI, with a pure CSS solution using flexbox and grid.