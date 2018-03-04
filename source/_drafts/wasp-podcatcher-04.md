---
title: >-
  Tales from the Development of Wasp Podcatcher - Episode 4: Go GraphQL
category:
  - Tales from Wasp Podcatcher
tags:
  - Wasp Podcatcher
  - GraphQL
  - F#
---

Last week I introduced Anchor Modeling and the model for the server I created with that. This week I wrote some stored procedures to insert new data into the database with all the proper timestamps and ties. Then I started thinking about querying the data and what the API should look like. Then I came across [GraphQL](https://graphql.org).

I first heard of GraphQL in July 2017 on [a .NET Rocks episode](https://dotnetrocks.com/?show=1462) with Steve Faulkner. I didn't really have a use for it at the time, maybe only when integrating with an API that was built on it. And there aren't many of those, with GitHub as a notable exception. Today, however, I'm going to build an API that's involved with multiple objects and relationships between them, and that's exactly where GraphQL shines.

<!-- more -->

## What is GraphQL?
GraphQL is a query-language that can be used as an alternative to REST APIs. Instead of telling the server what you want depending on the endpoint and HTTP method, there is just one endpoint to which you can send your query. In that query, you can also detail exactly which properties to include with an object, so there's no bandwidth wasted on information the client doesn't need. The thing I like most is the strong typing it gives your API: you'll always get the objects and properties you query for, or an error if you got your types wrong. This also makes it very easy to add information to objects in your API without breaking any clients.

Another neat aspect of GraphQL is the integrated documentation, which is quite like the XML documentation comments in .NET. It is not a complete replacement for separate docs with tutorials and walkthroughs, but it does give you a nice description of all API's. That documentation can be read by sending so-called introspection queries to the GraphQL endpoint, or by using a more visual tool like [Graph*i*QL](https://github.com/graphql/graphiql), which gives you a way to browse the documentation and also write queries with IntelliSense-like autocompletion. Check out [GitHub's API explorer](https://developer.github.com/v4/explorer/) to check out the awesomeness.

## The stack behind the API
I decided to go F#, because that would allow me to use [FSharp.Data.GraphQL](https://fsprojects.github.io/FSharp.Data.GraphQL/). The strong typing and succinct syntax of F# couple really well with the ideas of GraphQL. I was first looking at using Azure Functions to host the API, but that would probably mean rebuilding the entire GraphQL schema on every request, which is urgingly advised against for really good reasons. And Azure Functions is not part of the free Azure for Students I get through Microsoft Imagine via university, so I would have to get a credit card, which is just a hassle as a student without a job, for a card for which I have no use otherwise.

So I started looking for a more tradition web framework for F#. And wow, there's quite a lot of them. The one I already knew about is [Suave](https://suave.io/). I also quickly found [Freya](https://freya.io/), [Giraffe](https://github.com/giraffe-fsharp/giraffe) and a bunch of others. Of course, I could also just go with ASP.NET MVC or [Nancy](http://nancyfx.org/), both of which also work just fine in F#. I decided to go with Giraffe because they have some nice sample applications that were easy to build and play with, and because it's just middleware for ASP.NET Core, a system I already know and a lot of resources are available for. It will also allow me to plug in other middleware for e.g. authentication.

So, next week I will work on the API, building out the GraphQL schema further to support all the types of requests I need, and also find a way to get that data from the database in a somewhat efficient manner. For the time-based requests, I might even go and write some SQL Server views. Yay! I'll let you know how that works out, see you next week!

*Thanks for reading! Enjoyed this post? Please share it with your friends using the buttons below! Have some questions? Comments can be found below too! And on a final note: grammar corrections are always welcome! English is not my primary language, but that's not a reason to don't learn how to use the language correctly.*