---
title: Hello world!
category:
  - Metablog
tags:
  - Hello world
date: 2016-12-23 19:32:26
---


```csharp
Console.WriteLine("Hello world!");
```

I'd like to use the opportunity of this first post to tell about the technology behind this blog, as that's something I'm always interested in myself. This website is build using the [Hexo](https://hexo.io/) static site generator, hosted on [GitLab Pages](https://pages.gitlab.io/). The theme is based on Icarus, which can be found [here on GitHub](https://github.com/ppoffice/hexo-theme-icarus) (MIT License). All the changes I've made are available via [the repo](https://gitlab.com/arthurrump/arthurrump.gitlab.io) for this site.

Using a static site generator isn't a choice I expected to make. I've tried lots of other options before going with Hexo, and with all of them, there was something I just didn't like. And there's always the problem of hosting. I was looking for something highly customizable, but simple to use. A really great writing experience was also a big priority, as that's what I'm hoping to do a lot.

Some of the first systems I tried were full blown CMSs, build in .NET and C#. I like C#, it's my favorite programming language, and C# can be used to build websites. It made sense to me to use my favorite technologies to build my blog. I tried both [Orchard](http://www.orchardproject.net/) and [Umbraco](http://umbraco.com/), both of them really great and very powerful systems. A little too powerful for me, and not the neat writing experience for just writing a blog post that I was looking for. I've also created a little CMS myself, but it suffers the same problem for every .NET based website: hosting. You'll want to host the website on a Windows machine. (That's not completely true anymore, especially with .NET Core, but it was when I tried these things.) Generally, this rule applies: hosting on Windows is more expensive than hosting on Linux, and you'll have to do a lot more yourself. There are just a couple of .NET hosting providers that are relatively easy to use: [Microsoft Azure](https://azure.microsoft.com) and [AppHarbor](https://appharbor.com/). Both of them have a free tier, but as soon as you want to add a custom domain name, pricing rises up like a rocket when compared to simple shared Linux hosting. Sure, the quality of the hosting is a lot better than the average shared Linux hosting, but my personal blog just doesn't need to have 101% uptime and distributed hosting across datacenters all over the world. That's where .NET is failing for me.

A CMS that seemed to fix the shortcomings of the CMSs mentioned above is [Ghost](https://ghost.org/). It has a beautiful writing experience and it's very simple to use. It's also built on Node.js, so it runs completely fine on Linux. It turns out though that on most simple shared hosting you don't get the control you need to start the Node server. How simple and clean Ghost may be, hosting it is too much of a hassle, i.e. it costs too much money.

The first CMS most people think of is probably [Wordpress](https://wordpress.org/). Being build on PHP it's way easier to host than the aforementioned alternatives, as PHP is widely supported on cheap hosting. But it's Wordpress: big, complicated and built on PHP. No offenses to anyone, but I honestly don't think PHP should be used for anything serious, or at least I wouldn't want to. In short: no Wordpress for me.

The biggest problem thus far: hosting. When looking at the blogs of other people I found something interesting: I was seeing a couple of them hosting on [GitHub Pages](https://pages.github.com/). For free. Without extra fees for adding a custom domain. Pretty awesome, right? So I started looking into it, finding that the most common way to do a blog on a static site host like GitHub Pages is using a [static site generator](http://www.staticgen.com/). GitHub Pages works especially well with [Jekyll](http://jekyllrb.com/), but that doesn't (officially) support Windows, which is my OS of choice. Other generators will have to be executed on your own PC, pushing the generated files into Git, which, like publishing your binaries into Git, feels a little dirty. Luckily GitHub isn't the only repository hosting service with a logo that resembles a cat and an option to host static websites: there's also something called [GitLab](https://gitlab.com/), which offers [GitLab Pages](https://pages.gitlab.io/). The beauty of GitLab's version of hosting is found in the integrated build system they offer: it can run any static site generator that can run in a Docker container on Linux. You don't have to publish the generated site into Git, just the source. GitLab will then generate the static site and publish it on the web. This system gives you a lot of flexibility for the price of, well, nothing.

Just one thing left: why [Hexo](https://hexo.io/)? When scrolling through [staticgen.com](http://www.staticgen.com/) there are lots of options and there are probably many more out there that aren't on that list. Why did I pick Hexo? First of all, it's one of the demos on the [GitLab Pages account](https://gitlab.com/pages), so I could just copy their build file, instead of trying to configure something myself. It's also very easy to install Hexo using [npm](https://www.npmjs.com/) and, to be honest, the site just gave me a good feeling. Sometimes that's all it takes.

So there you have it: this is the technology stack supporting this blog. I hope this time everything works out well and I'll be able to use this blog for a very long time.

*Thanks for reading! Enjoyed this post? Please share it with your friends using the buttons below! Have some questions? Comments can be found below too! And on a final note: grammar corrections are always welcome! English is not my primary language, but that's not a reason to don't learn how to use the language correctly.*