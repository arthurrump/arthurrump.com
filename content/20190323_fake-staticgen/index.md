---
title: "Fake.StaticGen"
subtitle: "A fully customizable static site generator using the power of FAKE"
tags: [ "F#", "FAKE" ]
category: Projects
---

![Fake.StaticGen logo]({attach}logo.svg){ style="margin: 2em; max-width: 300px; float: right" } 
Fake.StaticGen is a toolkit for generating static websites using [FAKE](https://fake.build) build scripts. It provides a framework for including pages and files and writing those out to disk in the correct folder structure. Fake.StaticGen tries to be very flexible and give you full control of the way you want your site to be organized, while still being helpful in common scenarios. Both this website and [versionsof.net](https://versionsof.net) are examples of what you can do with Fake.StaticGen.

Links:

- [GitHub repository](https://github.com/arthurrump/Fake.StaticGen)
- [NuGet package](https://nuget.org/packages/Fake.StaticGen)

## Extensible model
Fake.StaticGen has an extensible model and a lot of functionality is provided through extensions, such as Fake.StaticGen.Html, which provides a DSL based on the Giraffe View Engine to define your templates in F#; Fake.StaticGen.Rss, which helps you generate RSS feeds; or Fake.StaticGen.Markdown, which contains some helpers for working with Markdown content in your site.