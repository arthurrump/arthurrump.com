+++
title = "versionsof.net"
tagline = "Bringing clarity to versions of .NET"
tech = [ "F#", "Fake.StaticGen" ]
tags = [ ".NET", ".NET Versions", "F#", "Fable", "Elmish" ]
color = "#512bd4"
image = "assets/versionsof-net/screenshot.png"
image-alt = "A tablet showing the versionsof.net website"

[[links]]
name = "Website"
icon = "ios-globe"
url = "https://versionsof.net/"
[[links]]
name = "GitHub repository"
icon = "github"
url = "https://github.com/arthurrump/versionsof.net"
+++

.NET Core versioning is a mess. There are lots and lots and lots of version numbers, from runtime to SDK, from Visual Studio to languages and of course .NET Standard. So what SDK corresponds to which runtime again? I know there some sort of a system, but it differs per release. I created this website based on information published in the .NET Core repo to give an overview of all the versions of .NET.

---

## Data from the source
The data for this website is directly retrieved from information released by the teams developing .NET. For .NET Core, these are the releases.json files and release notes Markdown files in the [dotnet/core](https://github.com/dotnet/core) repo, see also the [NetCore.Versions](/projects/netcore-versions) project.

---

## Responsive tables
On larger screens, most information is shown in tables. This makes it easy to get an overview and compare different releases, but for small screens this results in a lot of scrolling. For mobile devices the tables are reduced to a cards based UI, with a pure CSS solution using flexbox and grid.