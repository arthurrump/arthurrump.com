---
title: "\"Good explanation, but now what?\" - Thoughts on getting people started with .NET"
category: 
  - .NET
tags:
  - .NET
  - .NET Renaissance
  - Teaching
---

I was listening to a .NET Rocks! [episode with Ian Cooper](http://dotnetrocks.com/?show=1458) about starting a .NET Renaissance, analogous to the Java Renaissance started in 2011, after the decline in usage of the language. Ian lists plenty of evidence for a similar decline in C# usage, and what might have replaced it, in [his blog post](https://medium.com/altdotnet/on-the-need-for-a-c-renaissance-634078d4e865), so I won't get into that, but there is another aspect which I want to talk about. One of the problems briefly mentioned in the podcast is the 'leaky bucket' problem: it is inevitable to 'lose' some developers to other platforms, so unless we fill up the bucket with new ones, usage of C# is going to decline to zero at some point.

There are multiple ways to fill that bucket: showing people who left that we can do all the cool stuff (running on Linux, being blazingly fast, being open source) now too, or showing people who failed with other problems that we can deliver what they need. But the way I want to focus on is getting people completely new to programming to use .NET, since it is something I've tried to do before and something I feel C# has lacked in since forever. Yes, there are people who start their programming journey with .NET: my first real programming language was Visual Basic, but I don't think we should count on people being like me. It's 2017 and I use a Lumia; I rest my case.

Let's be honest: C# is a Microsoft technology, of course it's not one of the cool beginner languages. It's uncool by definition. Yes, there are probably languages that are simply easier to start with (it depends on your definition of easy), but in that case, we should at least be level with Java, and I don't feel like we are. You'll find more Java tutorials on YouTube than videos on C#. I was comparing Computer Science programs at universities in the Netherlands, and almost every one of them teaches Java, and none teaches C#. Even more anecdotally: a friend of mine started programming and he chose Java. Yes, I've talked to him, laughed at him, repelled some myths about Visual Studio, but he didn't switch. C# is just uncool.

We can't do much about the fact that C# is a Microsoft technology. It's also why the enterprisy developers, the traditional .NET crowd, love C#. It's backed by a strong company, with a great track record on backward compatibility and long term support. This enterprisy focus has also heavily influenced the C# learning materials, which are designed to throw huge amounts of information at you, [Jump Start](https://mva.microsoft.com/en-US/training-courses/programming-in-c-jump-start-14254) style, so you know everything as fast as possible and go on with writing the software you should've finished last week. This kind of education is great if you know what you want to build, or what you have to build, but for people who are getting into programming as a hobby, it misses a lot of guidance and examples.

I know this because I've created such Jump Start-like content myself: in 2015 I published some videos on C# on the Dutch website [JorCademy](http://www.jorcademy.nl/), a website mostly focused on getting children into programming, but we know it's watched by people of all ages. Most of the feedback I got on my videos was something along the point of "that's a very good explanation, very thorough, and easy to understand, but I'm missing some examples of what I can do with it." Roughly said: "Good explanation, but now what?" And I feel like this applies not only to my videos but also to the larger stage of C# learning materials.

On the contrary, the web is full of content to get you started with all the cool languages, with short videos, and quick demos. They lack the deep explanations I've come to love about our content. I really believe the best way to learn something is to get a deep understanding of the underlying stack, but it turns out people are excited to get coding quickly and don't want to sit through a twenty-minute monologue about the pros and cons of compiling vs interpreting and how JIT is different from both. Historically .NET just hasn't been a platform for getting started quickly, and you had plenty of time for such lectures because Visual Studio needed another 6 hours to install anyway. And maybe Visual Studio isn't the most beginner friendly program:

<blockquote class="twitter-tweet" data-lang="en"><p lang="en" dir="ltr">Learning to code is an essential skill in the modern world. Here&#39;s a handy diagram showing you how to get started. <a href="https://t.co/E5YH6A6WaO">pic.twitter.com/E5YH6A6WaO</a></p>&mdash; Dylan Beattie 🇪🇺 (@dylanbeattie) <a href="https://twitter.com/dylanbeattie/status/832326857798348800">February 16, 2017</a></blockquote>
<script async src="//platform.twitter.com/widgets.js" charset="utf-8"></script>

Luckily times have changed and with .NET Core and Visual Studio Code, you have your code running in ten minutes and a full-blown editing experience with IntelliSense and debugging in another ten, across Windows, macOS, and Linux. .NET has everything to be a good platform for beginners and getting people excited about programming, now we just need to get a good tutorial out there. I'll see what I can do, stay tuned. 😉

*Thanks for reading! Enjoyed this post? Please share it with your friends using the buttons below! Have some questions? Comments can be found below too! And on a final note: grammar corrections are always welcome! English is not my primary language, but that's not a reason to don't learn how to use the language correctly.*