---
title: "Tales from the Development of Wasp Podcatcher - Episode 2: Stop fighting Entity Framework"
category:
  - Tales from Wasp Podcatcher
tags:
  - Wasp Podcatcher
  - Entity Framework
---

Last week I worked on the models as they will be stored in the database and that will be sent from the client to the server and back. So in this post, we'll take a look at what the data model looks like.

## The basic data model
I'm building a podcast application, so to start we need a podcast class, containing details like title, logo and most importantly the feed. Of course, a podcast has episodes, with a title, a description, a release date, a link to the media file and possibly a logo specific to this episode. <!-- more --> On the client an episode also has a state, which consists of the playhead, the time into the episode where the user last stopped listening, and a played/unplayed status. On the server, this is a separate model, with a link to an episode and a user. When a user is subscribed to a podcast there is a subscription object linking the user with a podcast stored in the server database. The client simply only stores podcasts the user subscribed to. Lastly, there is a playlist, which consists of playlistitems, which links an episode to the index in the playlist, on the server side also referencing the user.

The complete model described above represents one state in the system, but when synchronizing multiple devices, things can get complicated very easily if you don't want to send over the entire state every time you sync. And when you listen to a couple of podcasts, of which some have over 1500 episodes (*cough* .NET Rocks! *cough*), sending over all those episodestates every single time is quite unnecessary (how many times would the state of some random episode from 2004 change?) and just wasteful of sometimes limited bandwidth. So we would prefer to only send over the changes that are made since the last time the client checked.

## Storing updates
To enable this we will store a complete history of changes with their timestamps on the server, and send the changes since some timestamp when the client requests them. For the initial sync of a new device (and maybe a future web app?), the current state is also stored. The changes we need for each model described before are listed here:

* Episode: add, update (yes, the description can be updated, the media-url can change, etc.), remove (yes, episodes can be published by accident, etc.)
* Podcast: update (when the details of a podcast change)
* Subscription: add, remove
* EpisodeState: update (there is a default of playhead 0 and unplayed, states cannot be removed)
* PlaylistItem: add, remove (maybe an update for reordering, might be easier as a remove, add)

These update models contain a complete snapshot of some object at that point in time because some additions might not be available in the current data anymore, so this complete copy is required.

## Goodbye Entity Framework
As always, I happily started to implement this and tried to link it to a database with Entity Framework. I even figured out how to contain a complete copy of an object inside the update entities, by referencing a derivative of those objects and marking those derivatives as complex types. Yes, that works and I build a little test application to actually see that it works. EF can do really interesting things when you try to do weird things like that, but it always seems to get confused when my model is contained in multiple projects. Because the client and server model are so similar, I created a common type which both can derive from. And even though in the end all the models referenced in the cloud project are contained in the cloud project (deriving from models in a core project), EF comes up with helpful errors like these:

```
Wasp.Cloud.Models.Podcast: Name: Each type name in a schema must be unique. Type name 'Podcast' is already defined.
Wasp.Cloud.Models.Podcast: Name: Each type name in a schema must be unique. Type name 'Podcast' is already defined.
```

I could've understood that error if there were indeed three different models called Podcast, but even when I renamed all so they had different names, this error kept coming up. Oh well.

I'm currently looking around at other available ORMs and the two that I'm considering now, mainly based on their ongoing development and wider use are [Dapper](https://github.com/StackExchange/Dapper) (created and used by StackOverflow) and [NHibernate](http://nhibernate.info/), but I'm open for other suggestions. With the first one, it looks like I'll have to write SQL, instead of using statically typed LINQ, and the second uses XML for the mapping configuration, but that should be circumventable by using [Fluent NHibernate](http://www.fluentnhibernate.org/). And NHibernate is LGPL licensed, and even though I know that first L makes things a lot better, I'm still nervous using a library with a license that sounds GPL-ish. I'm going to think about this a bit.

I really love the idea of code-first database generations and the automatically generated migrations. But I don't want to fight my tools every time I do something. Then it is time to say goodbye and cease the fight.

*Thanks for reading! Enjoyed this post? Please share it with your friends using the buttons below! Have some questions? Comments can be found below too! And on a final note: grammar corrections are always welcome! English is not my primary language, but that's not a reason to don't learn how to use the language correctly.*