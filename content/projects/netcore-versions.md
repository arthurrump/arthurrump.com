+++
title = "NetCore.Versions"
tagline = "Working with .NET Core's releases.json files"
tech = [ "F#", "Giraffe", "Unquote", "GitHub Checks" ]
tags = [ ".NET Core", ".NET Core Versions", ".NET Core Releases", "releases.json" ]
color = "#512bd4"
image = "assets/netcore-versions/screenshot.png"
image-alt = "A tablet showing GitHub with a failed check for releases.json files in my fork of dotnet/core"
order = 30

[[links]]
name = "GitHub repository"
icon = "github"
url = "https://github.com/arthurrump/NetCore.Versions"
[[links]]
name = "NuGet package"
icon = "nuget"
url = "https://nuget.org/packages/NetCoreVersions"
[[links]]
name = "GitHub Marketplace"
icon = "github"
url = "https://github.com/apps/netcore-versions-checks"
+++

To display version information about .NET Core on [versionsof.net](/projects/versionsof-net), I had to decode the `releases.json` files in the [dotnet/core](https://github.com/dotnet/core) repo. Not a big problem, you'd think. These are just JSON files, how hard can it be? That's fair, but the schema, nor the data of these files turned out to be really consistent. To help with this, I created the NetCore.Versions.Checks GitHub app that checks these files on every commit.

---

## Schema NuGet package
The schema I use is available on NuGet as a package containing the schema expressed in F# records and decoders for use with [Thoth.Json.Net](https://www.nuget.org/packages/Thoth.Json.Net). One of the biggest benefits of using Thoth.Json are its crystal clear error messages, explaining exactly what is wrong with the provided JSON files. This was really helpful for building the GitHub app.

---

## GitHub Checks app
To help with the consistency of the `releases.json` files, I created a GitHub app that runs a set of checks on every commit. It is implemented as a [Giraffe](https://github.com/giraffe-fsharp/Giraffe) web app, running on the free Azure App Service tier. To execute the checks, I created a small testing framework with support for tree structured tests on data objects, using the excellent [Unquote](https://github.com/SwensenSoftware/unquote) library to provide clear error messages.