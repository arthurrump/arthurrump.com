+++
title = "Introducing versionsof.net"
category = ".NET"
tags = [ ".NET", "F#", "Fable", "Elmish" ]
date = 2018-12-30 17:39:25
+++


.NET Core versioning is a mess. There are lots and lots and lots of version numbers, from runtime to SDK, from Visual Studio to languages and of course .NET Standard. So what SDK corresponds to which runtime again? I know there some sort of a system, but it differs per release. Here's my solution:

![Screenshot of the versionsof.net website](/2018/12/30/introducing-versionsof-net/screenshot.png "Bringing clarity to versions of .NET")

I called it [versionsof.net](https://versionsof.net).
<!-- more -->

And people seem to like it:

<blockquote class="twitter-tweet"><p lang="en" dir="ltr">Great web site built by <a href="https://twitter.com/ArthurRump?ref_src=twsrc%5Etfw">@ArthurRump</a>. Thanks <a href="https://twitter.com/_cartermp?ref_src=twsrc%5Etfw">@_cartermp</a> for showing it to me! <a href="https://t.co/ob2hxzVeZf">https://t.co/ob2hxzVeZf</a></p>&mdash; Immo Landwerth (@terrajobst) <a href="https://twitter.com/terrajobst/status/1075889623178477568?ref_src=twsrc%5Etfw">December 20, 2018</a></blockquote>
<blockquote class="twitter-tweet"><p lang="en" dir="ltr">Absolutely lovely website by <a href="https://twitter.com/ArthurRump?ref_src=twsrc%5Etfw">@ArthurRump</a> that shows <a href="https://twitter.com/dotnet?ref_src=twsrc%5Etfw">@dotnet</a> versions! <a href="https://t.co/kbjoacrwDG">https://t.co/kbjoacrwDG</a></p>&mdash; Scott Hanselman (@shanselman) <a href="https://twitter.com/shanselman/status/1075894394396467200?ref_src=twsrc%5Etfw">December 20, 2018</a></blockquote> <script async src="https://platform.twitter.com/widgets.js" charset="utf-8"></script> 

The idea started when I came across some files in the [dotnet/core](https://github.com/dotnet/core) repo: `releases.json` gives you a list of every release of .NET Core. The reason I found it was the announcement that it was being deprecated, but luckily there was already a replacement in the form of `releases-index.json` and multiple `releases.json` files. And then I built a website around those files, simple as that.

Actually, it was a bit more complicated, because the consistency in both structure and content was lacking. If you look at the requests made from the website, you'll notice that they go to my own fork of the dotnet/core repo, because an update to these files might very well break the parsing logic. For the last months, I've almost permanently had an open pull request with little fixes in these files. These issues seem to be mostly resolved now and I'm discussing options to add a consistency check on new pull requests with [Lee Coward](https://github.com/leecow), one of the maintainers of the dotnet/core repo. I'm hoping to switch over to the real production version of the JSON files in the coming month, once some kind of validation is in place.

## Fable
This is the first real project I did using the [Fable](https://fable.io) F# to JavaScript compiler. It's awesome. Being able to write in a strongly typed language with an amazing type system and have a website come out at the other end is just great. Even better: it's a static website that can be hosted for free at GitHub or GitLab Pages. I love it when I don't have to pay for servers.

The only thing I'm not quite a fan of is the tooling setup. Because you're with one leg in .NET and with the other in JavaScript, you have to combine tooling from both ecosystems. I found especially [webpack](https://webpack.js.org/), which seems to be the preferred way of packing up your Fable application, to be very confusing. Turns out you should just copy the example [webpack.config.js](https://github.com/fable-compiler/webpack-config-template) file and not try to understand it. The [dotnet-fable](https://www.nuget.org/packages/dotnet-fable/) tool tries to make things easier, but a lack of documentation does not help.

I do really enjoy the [Elmish](https://elmish.github.io) programming model. It is so simple that the source code is literally copied into the [documentation](https://elmish.github.io/elmish/program.html), and it's actually helpful. Following The Elm Architecture results in neatly organized code, that is also [easy to componetize](https://www.youtube.com/watch?v=-Oc4xJivY78). Code sharing between Fable and .NET is a bit tricky, and I'm still figuring that part out, but at least it is possible.

## What's next
As I said, I hope to directly link to the JSON files instead of my own fork. If it doesn't work out, I'll try to add some automatic merging with the consistency tests integrated.

As for features on the website, I'm planning to show the release notes directly on the website, but for that I'll first want to rebuild the table, this time also keeping mobile usability in mind. Another thing I want to add is URL handling for the selected filter. URL handling is also something that is necessary when multiple pages are added. I'll first look into adding .NET Standard, but if there's a good data source for .NET Framework or Mono releases, those would be interesting additions too.

These suggestions are also in the [GitHub Issues](https://github.com/arthurrump/versionsof.net/issues), which is also the place to add your own ideas. And don't forget to bookmark [versionsof.net](https://versionsof.net)!

*Thanks for reading! Enjoyed this post? Please share it with your friends using the buttons below! Have some questions? Comments can be found below too! And on a final note: grammar corrections are always welcome! English is not my primary language, but that's not a reason to don't learn how to use the language correctly.*