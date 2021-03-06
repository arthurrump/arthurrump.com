+++
title = "Fake.StaticGen"
tagline = "A fully customizable static site generator using the power of FAKE"
tech = [ "F#", "FAKE" ]
tags = [ "FAKE", "static site", "staticgen", "F#" ]
color = "#19232e"
image = "assets/fake-staticgen/logo.png"
image-alt = "Fake.StaticGen logo"
order = 20

[[links]]
name = "GitHub repository"
icon = "github"
url = "https://github.com/arthurrump/Fake.StaticGen"
[[links]]
name = "NuGet package"
icon = "nuget"
url = "https://nuget.org/packages/Fake.StaticGen"
+++

Fake.StaticGen is a toolkit for generating static websites using [FAKE](https://fake.build) build scripts. It provides a framework for including pages and files and writing those out to disk in the correct folder structure. Fake.StaticGen tries to be very flexible and give you full control of the way you want your site to be organized, while still being helpful in common scenarios. Both this website and [versionsof.net](https://versionsof.net) are examples of what you can do with Fake.StaticGen.

---

## Extensible model
Fake.StaticGen has an extensible model and a lot of functionality is provided through extensions, such as Fake.StaticGen.Html, which provides a DSL based on the Giraffe View Engine to define your templates in F#; Fake.StaticGen.Rss, which helps you generate RSS feeds; or Fake.StaticGen.Markdown, which contains some helpers for working with Markdown content in your site.