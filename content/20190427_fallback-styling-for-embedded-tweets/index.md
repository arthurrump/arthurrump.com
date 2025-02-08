---
title: "Fallback styling for embedded Tweets"
tags: [ "CSS", "Twitter", "Embed Tweet" ]
category: Posts
---

Sometimes you want to embed a Tweet in your blog posts. At least I do. With a bit of CSS, you won't have to depend on an external script to make your embedded Tweet recognizable as a Tweet.

Embedding a Tweet on your website is easy to do. Find the tweet, click on Embed Tweet, copy, paste, done.

Let's first take a look at the code you just copied:

```html
<blockquote class="twitter-tweet" data-dnt="true">
    <p lang="en" dir="ltr">
        Announcing <a href="https://twitter.com/hashtag/fsharp?src=hash&amp;ref_src=twsrc%5Etfw">#fsharp</a> 4.6 <a href="https://t.co/hk3V7T5Efc">https://t.co/hk3V7T5Efc</a>
    </p>
    &mdash; Phillip Carter (@_cartermp) <a href="https://twitter.com/_cartermp/status/1113122340966064128?ref_src=twsrc%5Etfw">2 april 2019</a>
</blockquote>
<script async src="https://platform.twitter.com/widgets.js" charset="utf-8"></script>
```

> Sidenote: please add the `data-dnt="true"` attribute to tell Twitter you don't want them to track your visitors.

Note how all the actual content of the Tweet is just text in a blockquote. That's great because if the script does not load, the content you wanted to share is still there and readable. This could happen in situations where Twitter is entirely blocked (whether by a company or a nation-state), the user has JavaScript disabled, or because the script is blocked by Content Blocking in Firefox. However, this means that Tweets will be rendered as blockquotes by default. Here's a screenshot from my website with two embedded tweets:

![Two embedded tweets formatted with standard blockquote styling]({attach}blockquotes.png "Tweets styled like quotes with my old website theme")

These are simply styled as every other blockquote, which is not a problem in itself, but it does not give the reader an obvious clue to the source of the content. Sure, they will probably figure it out from the use of t.co links and @username, but I'm often a bit confused when I see a tweet presented like this, especially since you're expecting the Tweet to be displayed in the widget as created by Twitter's script.

This is the reason I decided to add some distinct styling for Tweets when redoing the design for this site. Here's a comparison between my styling and the widget as rendered by Twitter:

![Comparison between my custom styling of the blockquote and Twitter's widget, with a Tweet by Scott Hanselman]({attach}fallback-shanselman.png "My custom blockquote styling on the left, the Twitter widget on the right")

![Another comparison, this time by Immo Landwerth]({attach}fallback-terrajobst.png "My custom blockquote styling on the left, the Twitter widget on the right")

We only need a little bit of CSS:

```css
blockquote.twitter-tweet {
    background-image: url("/ionicons/logo-twitter.svg");
    background-repeat: no-repeat;
    background-position: top 0.5rem right 0.5rem;
    background-size: 1.25rem 1.25rem;
    padding: 1rem 2rem 1rem 1rem;
    border: 1px solid #e1e8ed;
    border-radius: 5px;
}

blockquote.twitter-tweet > p {
    margin: 0;
}
```

Restricting the selector to only elements of type `blockquote` makes sure to only apply the styling in case the script has not loaded. The script will replace the blockquote with an `iframe` containing Twitter's widget and I don't want to mess that up. The Twitter bird is added as a background image in the upper right corner and the text is padded to make sure it doesn't flow over the logo. The `border` and `border-radius` properties match the values the script would set.

Since I'm already using [Ionicons](https://ionicons.com/) on this site, I just linked to the logo provided by Ionicons (with a little modification to set the color), but if you want a pure CSS solution, you can of course embed the SVG directly:

```css
background-image: url("data:image/svg+xml;charset=utf-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20viewBox%3D%220%200%2072%2072%22%3E%3Cpath%20fill%3D%22none%22%20d%3D%22M0%200h72v72H0z%22%2F%3E%3Cpath%20class%3D%22icon%22%20fill%3D%22%231da1f2%22%20d%3D%22M68.812%2015.14c-2.348%201.04-4.87%201.744-7.52%202.06%202.704-1.62%204.78-4.186%205.757-7.243-2.53%201.5-5.33%202.592-8.314%203.176C56.35%2010.59%2052.948%209%2049.182%209c-7.23%200-13.092%205.86-13.092%2013.093%200%201.026.118%202.02.338%202.98C25.543%2024.527%2015.9%2019.318%209.44%2011.396c-1.125%201.936-1.77%204.184-1.77%206.58%200%204.543%202.312%208.552%205.824%2010.9-2.146-.07-4.165-.658-5.93-1.64-.002.056-.002.11-.002.163%200%206.345%204.513%2011.638%2010.504%2012.84-1.1.298-2.256.457-3.45.457-.845%200-1.666-.078-2.464-.23%201.667%205.2%206.5%208.985%2012.23%209.09-4.482%203.51-10.13%205.605-16.26%205.605-1.055%200-2.096-.06-3.122-.184%205.794%203.717%2012.676%205.882%2020.067%205.882%2024.083%200%2037.25-19.95%2037.25-37.25%200-.565-.013-1.133-.038-1.693%202.558-1.847%204.778-4.15%206.532-6.774z%22%2F%3E%3C%2Fsvg%3E");
```

(This is also what the Twitter widget does, this is the SVG they use for it.)

And there it is in practice:

<blockquote class="twitter-tweet" data-dnt="true"><p lang="en" dir="ltr">Announcing <a href="https://twitter.com/hashtag/fsharp?src=hash&amp;ref_src=twsrc%5Etfw">#fsharp</a> 4.6 <a href="https://t.co/hk3V7T5Efc">https://t.co/hk3V7T5Efc</a></p>&mdash; Phillip Carter (@_cartermp) <a href="https://twitter.com/_cartermp/status/1113122340966064128?ref_src=twsrc%5Etfw">2 april 2019</a></blockquote>

Looks quite nice, I think.