---
title: Hello world!
category:
- Metablog
tags:
- Hello world
---

```csharp
Console.WriteLine("Hello world!");
```

I'd like to use the opportunity of this first post to tell about the technology behind this blog, as that's something I'm always interested in myself. Maybe a little too interested. However, this website is build using the [Hexo](https://hexo.io/) static site generator, hosted on [GitLab Pages](https://pages.gitlab.io/).

## Static gen
Using a static site generator isn't a choice I expected to make. I've tried lots of other options before going with Hexo, and with all of them there was something I just didn't like. And there's always the problem of hosting. I was looking for something highly customizable, but simple to use. A really great writing experience was also a big priority, as that's what I'm hoping to do a lot.

Some of the first systems I tried were full blown CMSs, build in .NET and C#. I like C#, it's my favorite programming language. C# can be used to build websites. It made sense to me to use my favorite language for my blog. I tried both [Orchard](http://www.orchardproject.net/) and [Umbraco](http://umbraco.com/), both of them really nice and very powerful systems. A little too powerful for me, and not a very nice writing experience for just writing a blog post. Another problem with .NET based websites is hosting: you'll need to host on a Windows machine. (That's not completely true anymore, especially with .NET Core, but it was when I tried these things.) Generally this rule applies: hosting on Windows is more expensive than hosting on Linux, and you'll have to do a lot more yourself. There are just a couple of .NET hosting providers that are relatively easy to use: [Microsoft Azure](https://azure.microsoft.com) and [AppHarbor](https://appharbor.com/). Both of them have a free tier, but as soon as you want to add a custom domain name, pricing rises up like a rocket when compared to a shared Linux hoster. Sure, the quality of the hosting is a lot better than the average shared Linux hosting, but my personal blog just doesn't need to have 101% uptime and distributed hosting across datacenters all over the world. That's were .NET is failing for me.

A CMS that seemed to fix the shortcomings of the CMSs mentioned above is [Ghost](https://ghost.org/). It has a beautiful writing experience and it's very simple to use. 

    - uses nodejs -> Linux
        but not hostable on average providers

- wordpress: complicated, php