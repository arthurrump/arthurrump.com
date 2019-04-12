+++
title = "Tales from the Development of Wasp Podcatcher - Episode 3: Figuring out Anchor Modeling"
category = "Tales from Wasp Podcatcher"
tags = [ "Wasp Podcatcher", "Anchor Modeling" ]
date = 2018-02-25 21:22:58
+++


Last week I waved goodbye to Entity Framework and talked about the data scheme I was thinking about. This week I threw most of that scheme away as I started playing with a new tool to model the data.

## Anchor Modeling
That tool is called [Anchor Modeling](http://www.anchormodeling.com/), and it's not really that new as the first presentation on it was given in 2007. It was new for me though, and I discovered it in [Steph Locke's talk at NDC London](https://www.youtube.com/watch?v=2176f9K-cC4). The reason I became interested in the tool is its first class support for modeling changes of data over time, exactly what I need to send only changes in the data when synchronizing multiple devices. Storing the history of an attribute is as simple as toggling the historized property in the graphical tool. 

<!-- more -->

The database scheme the Anchor tool creates is in the sixth normal form, which means that for every attribute and for every relation there is a separate table. This results in a scheme that looks like a total mess, but it makes it easy to set timestamps for every attribute separately. Fortunately, the Anchor tool also generates views to just get the latest version of an anchor and table-valued functions for getting historical versions, both returning the data in a more conventional way (i.e. all attributes in one table).

This is the model I came up with:

![A graph overview of the database structure](/assets/20180225-wasp-podcatcher-03/wasp-database.svg "The Wasp database structure")

In an Anchor Model, a red square is an anchor, which would be an entity in a normal database. The circles connected to these squares are the attributes of that anchor; if it's a double circle it means that it is a historized attribute, meaning that all its changes will be stored. The grey diamond is a tie, which is a relation between two or more anchors. The symbols on the connections from an anchor to a tie indicate if that anchor is an identifier for that tie. Three horizontal bars means it is, one vertical means it isn't.

A very convenient way to think of these identifier indicators is as an how-many-to-how-many indicator in classic database modeling, where a three-bar indicates a many and a one-bar a one. So an episode is part of **one** podcast, and a podcast can have **many** episodes.

### The downsides
Even though I think Anchor Modeling is a really great tool, it does have some downsides. It is not widely used, so there is very little information to find online, and you'll have to figure things out by playing around, looking at the generated SQL-code, reading some scientific papers. There are some videos as well, but they don't go into much detail. I'm planning on writing another blog post with links to all resources I found, and maybe write something up myself once I've figured it out myself.

The tooling is also a bit limited, and I've been switching between Opera and Firefox to use all the saving and loading options, and none of them work in Edge at all. The tool also generates SQL-code to execute against your database to set up the scheme, but I would much rather have a command-line application that can do that for me in CI, so I only have to store the Anchor XML document in source control. There is a thing called [sisula](https://github.com/Roenbaeck/sisula) that is supposed to help with this, but I haven't quite figured out how it should work.

## GitLab fixes custom domains
This week GitLab [finally rolled out](https://about.gitlab.com/2018/02/21/pages-security-fix-rollout/) its fix for the custom domain security issue this week! So [wasppodcatcher.com](https://wasppodcatcher.com/) no longer redirects to the gitlab.io site, but just shows you the page as it should. Yay.

## What's next
In the coming week, I'll try to get a better grasp of Anchor Modeling and hopefully write most of the stored procedures to do all of the database operations. Since most of the logic of the server will now be written in SQL instead of beautiful LINQ-queries I'm thinking about switching to a more lightweight framework instead of the large ASP.NET MVC with Azure Mobile App Service integration I was planning on. Maybe I'll even use F# and/or go serverless, who knows? See you next week!

*Thanks for reading! Enjoyed this post? Please share it with your friends using the buttons below! Have some questions? Comments can be found below too! And on a final note: grammar corrections are always welcome! English is not my primary language, but that's not a reason to don't learn how to use the language correctly.*