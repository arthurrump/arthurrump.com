+++
title = "Tales from the Development of Wasp Podcatcher - Episode 1: Kicking it off"
tags = [ "Wasp Podcatcher" ]
date = 2018-02-10 21:55:51
+++


Late last year I made a big decision. I bought an Android phone, as a replacement for my Lumia 950. As we all know, Windows 10 Mobile wasn't too alive anymore and when I left, the last Insider build had already shipped. And then I got a very good deal on a Nokia 8, so it was hard to say no. But my Lumia is still with me most of the time, for two big reasons: first of all is the camera on the 950 way better than that dual camera joke on the back of the Nokia 8. Secondly, and most importantly for this post, it is still my podcast player.
<!-- more -->

I've been using the [Podcasts (beta)](https://www.microsoft.com/en-us/store/p/podcasts-beta/9nblggh1zj3r) UWP app for a long time and even though it's still in beta (and will probably be forever), it's a very solid app. The thing I love most about it is the synchronization of subscriptions and episode states to OneDrive, so I have the same data across all my devices. Except for my new phone. I've been looking around for apps that can synchronize between my devices, but even with the vast amount of podcast apps available on Android, I haven't found a single one that also has a UWP (or even a traditional Windows) counterpart.

Granted, I found two options that were close: the very popular [PocketCasts](https://www.shiftyjelly.com/pocketcasts/), which exists as an Android app and is also available as a website and there's gPodder (Windows) and AntennaPod (Android), both of which can sync to the [gpodder.net](http://gpodder.net) service. I discarded the first because I want an offline application that can download episodes in the background, so I can watch a video podcast on my laptop when I'm on the train. The second came closer, but the Windows application looked very dated and when browsing around on the website, it didn't seem to have the latest episodes for quite some podcasts, which also ruled out building a UWP app to integrate with their service.

## Wasp Podcatcher
That's when I decided to build something myself. I've been pretty busy in the last months, but recently I've found more time and started developing Wasp Podcatcher, which will consist of a Xamarin Forms application and an Azure Mobile App Service to enable synchronization and lift the burden of checking for new episodes from the clients and let the server push new episodes to them. That's at least how I'm currently planning it to be.

![The Wasp Podcatcher logo, with a wasp sitting on a microphone](/assets/20180210-wasp-podcatcher-01/wasp-podcatcher.png "Wasp Podcatcher")

It's still very early days for this project and I have only worked on the server yet, which is also far from finished, so there's quite a lot that still has to be done. I've been thinking quite a bit on how I want to store the data and synchronize it between the server and multiple clients, and the models for that are mostly done. I'll explain it when I've done some more work on it

Why am I writing this post at such an early stage of the project? Mainly because I want to see if there's interest from other people, and possibly gather some good suggestions. I'm planning to write a little post every week, with updates on where the project is at and what I did in that week. Not just for your engagement, but also to give myself some motivation to work on it and see the progress I have made.

### Last week
So, to kick things off: this week I spent some time designing the logo, and I was very pleasantly surprised with the result. That accidental click on the Bahnschrift font worked out pretty well. I also have to thank Gilad Fried and Chad Remsing from [the Noun Project](https://thenounproject.com/) for their [wasp](https://thenounproject.com/term/wasp/50533) and [microphone](https://thenounproject.com/term/microphone-ready/578132) icons. I've modified both to fit them together, but they were a great start.

I also registered a domain name and created a simple website, on which I will also link to these posts. Since GitLab [shut down the custom domain service](https://about.gitlab.com/2018/02/05/gitlab-pages-custom-domain-validation/), due to a security issue, [wasppodcatcher.com](https://wasppodcatcher.com) will just link through for now, but that will be fixed when GitLab releases their security update. And if you don't want to use the comments below to talk about Wasp, you can mail me at [arthur@wasppodcatcher.com](mailto:arthur@wasppodcatcher.com).

### What's next
In the coming time, I'm of course going back to the actual implementation, first getting the server to somewhat reliably store and return all the data. And I'll keep pondering if I will open source this project, keep that off until a later moment, or keep it as my private pet project. I'm not sure yet, but we'll see.

In the meantime, follow [me](https://twitter.com/arthurrump) and [Wasp](https://twitter.com/wasppodcatcher) on Twitter to stay up to date. See you in the next one!

*Thanks for reading! Enjoyed this post? Please share it with your friends using the buttons below! Have some questions? Comments can be found below too! And on a final note: grammar corrections are always welcome! English is not my primary language, but that's not a reason to don't learn how to use the language correctly.*