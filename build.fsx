#r "paket:
nuget Fake.Core.Target 
nuget Fake.StaticGen
nuget Fake.StaticGen.Html
nuget Fake.StaticGen.Markdown
nuget Fake.StaticGen.Rss
nuget Nett //"
#load "./.fake/build.fsx/intellisense.fsx"
#if !FAKE
    #r "Facades/netstandard" // Intellisense fix, see FAKE #1938
    #r "netstandard"
#endif

open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.StaticGen
open Fake.StaticGen.Html
open Fake.StaticGen.Html.ViewEngine
open Fake.StaticGen.Markdown
open Fake.StaticGen.Rss

open Markdig
open Nett

open System
open System.IO

let now = DateTime.UtcNow

type Config =
    { Title : string
      Author : string
      Description : string }

[<NoComparison>]
type Page =
    | Page of title : string * htmlContent : string
    | Post of Post
    | PostsOverview of Overview<Post>
    | PostsArchive of Page<Post> seq
    | TagsOverview of (string * string * int) seq
    | TagPage of tag: string * posts: Page<Post> seq

and Post =
    { Title : string
      Tags : string []
      Date : DateTime
      Blurb : string option
      Image : string option
      HtmlContent : string }

and [<NoComparison>]
Overview<'page> =
    { Index : int
      NextUrl : string option
      PreviousUrl : string option
      Pages : Page<'page> seq }

let postChooser page = 
    match page.Content with 
    | Post post -> Some { Url = page.Url; Content = post }
    | _ -> None

let posts (site : StaticSite<Config, Page>) = 
    site.Pages
    |> Seq.choose postChooser
    |> Seq.sortByDescending (fun p -> p.Content.Date)

let assetUrlRewrite (filename : string) =
    let dir = filename |> Path.getLowestDirectory
    let file = filename |> Path.GetFileName
    sprintf "/%s/%s/%s/%s/%s"
        dir.[0..3]
        dir.[4..5]
        dir.[6..7]
        dir.[9..]
        file

let tomlTryGet key (toml : TomlTable) =
    if toml.ContainsKey(key) 
    then Some (toml.[key].Get())
    else None

let parseConfig config =
    let toml = Toml.ReadString(config)
    { Title = toml.["title"].Get()
      Author = toml.["author"].Get()
      Description = toml.["description"].Get() }

let parsePage path (Some frontmatter) renderedMarkdown =
    let slug = path |> Path.GetFileNameWithoutExtension
    let toml = Toml.ReadString(frontmatter)
    { Url = slug 
      Content = Page (toml.["title"].Get(), renderedMarkdown) }

let parsePost path (Some frontmatter) renderedMarkdown =
    let filename = path |> Path.GetFileNameWithoutExtension
    let slug = filename.Substring(9)

    let toml = Toml.ReadString(frontmatter)
    let post = 
        { Title = toml.["title"].Get()
          Tags = toml.["tags"].Get()
          Date = toml.["date"].Get()
          Blurb = toml |> tomlTryGet "blurb"
          Image = toml |> tomlTryGet "image" |> Option.map assetUrlRewrite
          HtmlContent = renderedMarkdown }

    { Url = sprintf "%04i/%02i/%02i/%s" post.Date.Year post.Date.Month post.Date.Day slug
      Content = Post post }

let postsOverview (site : StaticSite<Config, Page>) =
    let url index = if index = 0 then "/" else sprintf "/page/%i" (index + 1)
    let chunks = posts site |> Seq.chunkBySize 6
    chunks
    |> Seq.mapi (fun i posts ->
        let content = 
            { Index = i
              PreviousUrl = if i = 0 then None else Some (url (i - 1))
              NextUrl = if i = Seq.length chunks - 1 then None else Some (url (i + 1))
              Pages = posts }
        { Url = url i; Content = PostsOverview content })

let postsArchive (site : StaticSite<Config, Page>) =
    { Url = "/archives"; Content = PostsArchive (posts site) }

let tagUrl (tag : string) = sprintf "/tags/%s" (tag.Replace("#", "sharp") |> Url.slugify)

let tagsOverview (site : StaticSite<Config, Page>) =
    let tags = 
        posts site 
        |> Seq.collect (fun p -> p.Content.Tags) 
        |> Seq.countBy id
        |> Seq.sortByDescending snd
    let overview = tags |> Seq.map (fun (t, count) -> t, tagUrl t, count)
    let overviewPage = { Url = "/tags"; Content = TagsOverview overview }
    let tagPages = 
        tags
        |> Seq.map (fun (tag, count) ->
            let posts = posts site |> Seq.filter (fun p -> p.Content.Tags |> Array.contains tag)
            { Url = tagUrl tag; Content = TagPage (tag, posts) })
    Seq.singleton overviewPage |> Seq.append tagPages

let template (site : StaticSite<Config, Page>) page = 
    let postDetailSpan (post : Page<Post>) =
        let tagList = 
            post.Content.Tags 
            |> Seq.map (fun t -> [ a [ _href (tagUrl t) ] [ str t ]; str ", " ])
            |> Seq.concat
        let tagList = tagList |> Seq.take ((tagList |> Seq.length) - 1)
        span [ _class "post-details" ] [ 
            yield str (post.Content.Date.ToShortDateString())
            yield rawText " &boxv; "
            yield! tagList
        ]

    let postListItem (post : Page<Post>) =
        article [] [
            match post.Content.Image with Some link -> yield a [ _href post.Url ] [ img [ _src link ] ] | _ -> ()
            yield h1 [] [ a [ _href post.Url ] [ str post.Content.Title ] ]
            yield postDetailSpan post
            match post.Content.Blurb with Some blurb -> yield p [] [ str blurb ] | _ -> ()
        ]

    let shortPostListItem (post : Page<Post>) =
        article [] [
            h1 [] [ a [ _href post.Url ] [ str post.Content.Title ] ]
            postDetailSpan post
        ]

    let titleText =
        match page.Content with
        | Page (title, _) -> title
        | Post post -> post.Title
        | PostsOverview o -> if o.Index = 0 then "Blog" else sprintf "Page %i" o.Index
        | PostsArchive _ -> "Archives"
        | TagsOverview _ -> "Tags"
        | TagPage (tag, _) -> sprintf "Tag: %s" tag

    let content = 
        match page.Content with
        | Page (_, content) -> rawText content
        | Post post -> rawText post.HtmlContent
        | PostsOverview { Pages = posts }
        | TagPage (_, posts) ->
            ul [ _class "post-overview" ] [ for p in posts -> li [ _class "post" ] [ postListItem p ] ]
        | PostsArchive posts ->
            let perYear = posts |> Seq.groupBy (fun p -> p.Content.Date.Year)
            ul [ _class "post-overview" ] [ 
                for (year, posts) in perYear do
                    yield li [ _class "year-header" ] [ strf "%i" year ]
                    yield! [ for p in posts -> li [ _class "post" ] [ shortPostListItem p ] ]
            ]
        | TagsOverview tags ->
            ul [] [ for (t, url, count) in tags -> li [] [ a [ _href url ] [ str t ]; strf " (%i)" count ] ]

    let keywords = 
        match page.Content with
        | Post post -> post.Tags |> String.concat ","
        | _ -> ""

    html [] [
        head [ ] [ 
            meta [ _httpEquiv "Content-Type"; _content "text/html; charset=utf-8" ]
            title [] [ strf "%s - %s" titleText site.Config.Title ]
            link [ _rel "stylesheet"; _type "text/css"; _href "/style.css" ]
            meta [ _name "author"; _content site.Config.Author ]
            meta [ _name "description"; _content site.Config.Description ]
            meta [ _name "keywords"; _content keywords ]
            meta [ _name "generator"; _content "Fake.StaticGen" ]
            meta [ _name "viewport"; _content "width=device-width, initial-scale=1" ]
            link [ _rel "canonical"; _href (site.AbsoluteUrl page.Url) ] 
        ]
        body [ ] [ 
            div [ _id "content" ] [ content ]
            footer [] [
                span [] [ rawText "&copy; "; strf "%i Arthur Rump" now.Year ]
                span [] [ 
                    str "Powered by " 
                    a [ _href "https://github.com/arthurrump/Fake.StaticGen" ] [ str "Fake.StaticGen" ]
                    str " and themed with handwritten CSS"
                ]
            ]
        ] 
    ]

let rssFeed (site : StaticSite<Config, Page>) =
    let items = 
        site.Pages
        |> Seq.choose postChooser
        |> Seq.sortByDescending (fun p -> p.Content.Date)
        |> Seq.map (fun post ->
            Rss.Item(
                title = post.Content.Title,
                link = site.AbsoluteUrl post.Url,
                guid = Rss.Guid(site.AbsoluteUrl post.Url, isPermaLink = true),
                pubDate = post.Content.Date))

    Rss.Channel(
        title = site.Config.Title,
        link = site.BaseUrl,
        description = site.Config.Description,
        copyright = sprintf "Copyright (c) %i, Arthur Rump" now.Year,
        generator = "Fake.StaticGen",
        pubDate = now,
        items = (items |> Seq.toList))

let withMarkdownPages files parse =
    let pipeline =
        MarkdownPipelineBuilder()
            .UseEmphasisExtras()
            .UseLinkUrlRewrite(fun link ->
                if link.Url.TrimStart('/').StartsWith("assets/") 
                then assetUrlRewrite link.Url
                else link.Url)
            .Build()

    StaticSite.withPagesFromCustomMarkdown pipeline files parse

Target.create "Generate" <| fun _ ->
    StaticSite.fromConfigFile "https://www.arthurrump.com" "config.toml" parseConfig
    |> withMarkdownPages (!! "content/*.md") parsePage
    |> withMarkdownPages (!! "content/posts/*.md") parsePost
    |> StaticSite.withOverviewPages postsOverview
    |> StaticSite.withOverviewPage postsArchive
    |> StaticSite.withOverviewPages tagsOverview
    |> StaticSite.withRssFeed rssFeed "/feed.xml"
    |> StaticSite.withFilesFromSources (!! "content/posts/assets/**/*") assetUrlRewrite
    |> StaticSite.withFilesFromSources (!! "rootfiles/*") Path.GetFileName
    |> StaticSite.withFilesFromSources (!! "icons/*") Path.GetFileName
    |> StaticSite.withFilesFromSources (!! "code/*") Path.GetFileName
    |> StaticSite.generateFromHtml "public" template

Target.runOrDefault "Generate"