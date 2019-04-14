+++
title = "Tales from the Development of Wasp Podcatcher - Episode 5: I'm still here"
tags = [ "Wasp Podcatcher", "F#" ]
date = 2018-04-08 16:40:52
+++


Time flies when you're having fun. It has already been over a month since the last post, which is quite long since I intended this to be a weekly series. But don't worry, I'm still here. I had some trouble with satellite television which took up quite some time for a week or two, and study is never standing still, but Wasp Podcatcher is progressing.

When I had free time last month I have been trying to get FSharp.Data.GraphQL to talk to the database, without having to specify all the types in the database again. Luckily F# has a really cool thing called Type Providers, which allow you to do compile-time checking of types inferred from other places, in my case a SQL database. This way I don't have to specify the SQL tables again in the F# type system, but I do get the compile-time type check I can't live without.


I came across two big type providers for databases: [FSharp.Data.SqlClient](fsprojects.github.io/FSharp.Data.SqlClient/) and [SQLProvider](fsprojects.github.io/SQLProvider/). I chose to go with the latter because it works with other databases instead of only SQL Server. Even though it looks like I will use an Azure SQL database for this project, I might go to a PostgreSQL database on AWS if that's cheaper (i.e. AWS gives me more credit for free, because I'm a student). However, to allow the SQL type provider to make sense of my Anchor modelled database, I did write some views in SQL. I'm not sure if those can easily be ported PostgreSQL, but I don't think it will be too hard.

## What's next?
I will continue to work on the GraphQL API to enable both a normal overview of the data and a way to just query for changes. I currently only have a subscriptions field in the root query, which returns podcasts and their episodes the user is subscribed to, so there is still quite a lot of work to do. After that, there are probably some improvements in the number of queries to the database to be made, but I will focus on making it work too.

I'm also planning to give the [promo website](https://wasppodcatcher.com) an update and maybe add a mailing list for people to subscribe to news on the project. In the meantime, you can, of course, follow [@wasppodcatcher](https://twitter.com/wasppodcatcher) on Twitter to stay up to date! See you next week! (Hopefully for real this time. 😉)