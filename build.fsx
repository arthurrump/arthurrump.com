#r "paket:
nuget Fake.Core.Target 
nuget Fake.StaticGen
nuget Fake.StaticGen.Html
nuget Fake.StaticGen.Markdown
nuget Fake.StaticGen.Rss
nuget MarkdigExtensions.UrlRewriter
nuget MarkdigExtensions.ImageAsFigure
nuget MarkdigExtensions.SyntaxHighlighting
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
open System.Globalization
open System.IO

let now = DateTime.UtcNow

type Config =
    { Title : string
      Author : string
      AuthorTwitter : string
      Description : string
      DefaultImage : string
      Navigation : NavItem list
      SocialLinks : SocialLink list }
and NavItem = 
    { Name : string
      Url : string }
and SocialLink =
    { Name : string
      Icon : string
      Url : string }

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
      AuthorTwitter = toml.["author-twitter"].Get()
      Description = toml.["description"].Get()
      DefaultImage = toml.["default-image"].Get()
      Navigation = 
        toml.["nav"].Get<TomlTableArray>().Items 
        |> Seq.map (fun navItem -> 
            { Name = navItem.["name"].Get() 
              Url = navItem.["url"].Get() }) 
        |> Seq.toList
      SocialLinks = 
        toml.["social-links"].Get<TomlTableArray>().Items
        |> Seq.map (fun navItem ->
            { Name = navItem.["name"].Get()
              Icon = navItem.["icon"].Get()
              Url = navItem.["url"].Get() })
        |> Seq.toList }

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

let tags site = 
    posts site 
    |> Seq.collect (fun p -> p.Content.Tags) 
    |> Seq.countBy id
    |> Seq.sortByDescending snd
    |> Seq.map (fun (t, count) -> t, tagUrl t, count)

let tagsOverview (site : StaticSite<Config, Page>) =
    let tags = tags site
    let overviewPage = { Url = "/tags"; Content = TagsOverview tags }
    let tagPages = 
        tags
        |> Seq.map (fun (tag, url, _) ->
            let posts = posts site |> Seq.filter (fun p -> p.Content.Tags |> Array.contains tag)
            { Url = url; Content = TagPage (tag, posts) })
    Seq.singleton overviewPage |> Seq.append tagPages

let template (site : StaticSite<Config, Page>) page = 
    let _property = XmlEngine.attr "property"

    let postDetailSpan (post : Post) =
        let tagList = 
            post.Tags 
            |> Seq.map (fun t -> [ a [ _href (tagUrl t) ] [ str t ]; str ", " ])
            |> Seq.concat
        let tagList = tagList |> Seq.take ((tagList |> Seq.length) - 1)
        span [ _class "post-details" ] [ 
            yield span [ _class "dateline" ] [ str (post.Date.ToString("MMMM dd, yyyy", CultureInfo.GetCultureInfo("en"))) ]
            yield rawText " &boxv; "
            yield! tagList
        ]

    let postListItem (post : Page<Post>) =
        li [ _class "post text" ] [
            match post.Content.Image with Some link -> yield a [ _href post.Url; _class "image-link" ] [ img [ _src link ] ] | _ -> ()
            yield h1 [] [ a [ _href post.Url ] [ str post.Content.Title ] ]
            yield postDetailSpan post.Content
            match post.Content.Blurb with Some blurb -> yield p [] [ str blurb ] | _ -> ()
        ]

    let shortPostListItem (post : Page<Post>) =
        li [ _class "post text" ] [
            h1 [] [ a [ _href post.Url ] [ str post.Content.Title ] ]
            postDetailSpan post.Content
        ]

    let titleText =
        match page.Content with
        | Page (title, _) -> title
        | Post post -> post.Title
        | PostsOverview o -> if o.Index = 0 then "Blog" else sprintf "Page %i" o.Index
        | PostsArchive _ -> "Archives"
        | TagsOverview _ -> "Tags"
        | TagPage (tag, _) -> sprintf "Tag: %s" tag

    let pageHeader =
        let navItem (item : NavItem) = 
            span [ _class "nav-item" ] [ a [ _href item.Url ] [ str item.Name ] ]

        header [ _id "main-header" ] [
            span [ _id "title" ] [ a [ _href "/" ] [ str "Arthur Rump" ] ]
            button [ _id "menu-toggle"
                     _onclick ("var img = document.querySelector('#menu-toggle > img');"
                        + "var nav = document.getElementById('main-nav');"
                        + "if (nav.classList.contains('opened')) {"
                        + "  nav.classList.remove('opened'); img.setAttribute('src', '/ionicons/md-menu.svg'); }"
                        + "else { nav.classList.add('opened'); img.setAttribute('src', '/ionicons/md-close.svg'); }") ] [
                img [ _alt "Menu"; _src "/ionicons/md-menu.svg" ] 
            ]
            nav [ _id "main-nav"] (site.Config.Navigation |> List.map navItem)
        ]

    let profile =
        aside [ _class "profile" ] [
            img [ _class "profile-pic"; _src "/android-chrome-192x192.png" ]
            span [ _class "name" ] [ str site.Config.Author ]
            span [ _class "motto" ] [ str site.Config.Description ]
            a [ _class "expand-social-links"
                Accessibility._roleButton
                _onclick ("var sl = document.querySelector('ul.social-links');"
                   + "sl.classList.toggle('opened');") ] [ 
                str "Find me elsewhere" 
            ]
            ul [ _class "social-links" ] [ 
                for link in site.Config.SocialLinks ->
                    li [] [
                        a [ _href link.Url ] [
                            img [ _src (sprintf "/simpleicons/%s.svg" link.Icon) ]
                            str link.Name 
                        ]
                    ]
            ]
        ]
    
    let tagList tags =
        div [ _class "titeled-container" ] [
            h1 [] [ str "Tags" ]
            ul [ _class "tag-list" ] [ 
                for (t, url, count) in tags -> 
                    li [] [ a [ _href url ] [ str t ]; strf " (%i)" count ] 
            ]
        ]

    let content = 
        match page.Content with
        | Page (title, content) -> 
            div [ _class "titeled-container" ] [
                h1 [] [ str title ]
                div [ _class "page text" ] [ 
                    rawText content 
                ]
            ]
        | Post post -> 
            div [ _class "titeled-container post-container" ] [
                match post.Blurb with | Some blurb -> yield p [ _class "blurb" ] [ str blurb ] | _ -> ()
                yield h1 [] [ str post.Title ]
                yield postDetailSpan post
                yield div [ _class "text" ] [
                    article [] [ 
                        rawText post.HtmlContent 
                    ]
                ]
            ]
        | PostsOverview overview ->
            let pagination = 
                let older = overview.NextUrl |> Option.map (fun n -> a [ _href n; _class "older" ] [ str "Older"; img [ _src "/ionicons/md-arrow-forward.svg" ] ])
                let newer = overview.PreviousUrl |> Option.map (fun p -> a [ _href p; _class "newer" ] [ img [ _src "/ionicons/md-arrow-back.svg" ]; str "Newer" ])
                let buttons = [ newer; older ] |> List.choose id
                div [ _class "pagination" ] buttons
            div [ _class "titeled-container overview-container" ] [ 
                yield h1 [] [ str "Blog" ]
                if overview.Index <> 0 then yield pagination
                yield ul [ _class "post-overview" ] [ 
                    for p in overview.Pages -> 
                        postListItem p
                ]
                yield pagination
            ]
        | TagPage (tag, posts) ->
            div [ _class "titeled-container overview-container" ] [
                h1 [] [ str tag ]
                ul [ _class "post-overview" ] [ 
                    for p in posts -> 
                        postListItem p
                ]
            ]
        | PostsArchive posts ->
            let perYear = posts |> Seq.groupBy (fun p -> p.Content.Date.Year)
            div [ _class "titeled-container overview-container" ] [
                h1 [] [ str "Archive" ]
                ul [ _class "post-overview" ] [ 
                    for (year, posts) in perYear do
                        yield li [ _class "year-header" ] [ strf "%i" year ]
                        yield! [ for p in posts -> shortPostListItem p ]
                ]
            ]
        | TagsOverview tags ->
            tagList tags

    let frame content =
        match page.Content with
        | Post _ ->
            content
        | Page _ | PostsOverview _ ->
            div [ _class "columns" ] [ content; profile ]
        | TagPage _ | PostsArchive _ ->
            div [ _class "columns" ] [ content; tagList (tags site) ]
        | TagsOverview _ ->
            content

    let headerTags =
        match page.Content with
        | Post post ->
            [ yield meta [ _name "keywords"; _content (post.Tags |> String.concat ",") ]
              yield meta [ _name "title"; _content post.Title ]
              yield meta [ _name "description"; _content (post.Blurb |> Option.defaultValue site.Config.Description) ]
              yield meta [ _name "twitter:card"; _content (if post.Image.IsSome then "summary_large_image" else "summary") ]
              yield meta [ _name "twitter:creator"; _content site.Config.AuthorTwitter ]
              yield meta [ _property "og:title"; _content post.Title ]
              yield meta [ _property "og:type"; _content "article" ]
              yield meta [ _property "og:image"; _content (site.AbsoluteUrl (post.Image |> Option.defaultValue site.Config.DefaultImage)) ]
              yield meta [ _property "og:description"; _content (post.Blurb |> Option.defaultValue site.Config.Description) ]
              yield meta [ _property "article:author"; _content site.Config.Author ]
              yield meta [ _property "article:published_time"; _content (post.Date.ToString("s")) ]
              for t in post.Tags -> meta [ _property "article:tag"; _content t ] ]
        | _ ->
            [ meta [ _name "description"; _content site.Config.Description ]
              meta [ _property "og:title"; _content titleText ]
              meta [ _property "og:type"; _content "website" ] ]

    html [] [
        head [ ] ([ 
            meta [ _httpEquiv "Content-Type"; _content "text/html; charset=utf-8" ]
            title [] [ strf "%s - %s" titleText site.Config.Title ]
            link [ _rel "stylesheet"; _type "text/css"; _href "/style.css" ]
            meta [ _name "author"; _content site.Config.Author ]
            meta [ _name "copyright"; _content (sprintf "Copyright %i %s" now.Year site.Config.Author) ]
            meta [ _name "description"; _content site.Config.Description ]
            meta [ _name "generator"; _content "Fake.StaticGen" ]
            meta [ _name "viewport"; _content "width=device-width, initial-scale=1" ]
            link [ _rel "canonical"; _href (site.AbsoluteUrl page.Url) ]
            meta [ _property "og:url"; _content (site.AbsoluteUrl page.Url) ]
            meta [ _property "og:site_name"; _content site.Config.Title ] 
            meta [ _name "twitter:dnt"; _content "on" ]
        ] @ headerTags)
        body [ ] [ 
            div [ _id "background" ] [ 
                div [ _id "container" ] [ 
                    pageHeader
                    frame content 
                ]
            ]
            footer [] [
                span [] [ a [ _href "/archives" ] [ str "Archive" ] ]
                span [] [ a [ _href "/tags" ] [ str "Tags" ] ]
                span [] [ a [ _href "https://github.com/arthurrump/Fake.StaticGen" ] [ str "Generated with Fake.StaticGen" ] ]
                span [] [ rawText "&copy; "; strf "%i %s" now.Year site.Config.Author ]
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
            .UseGenericAttributes()
            .UseImageAsFigure(onlyWithTitle = true)
            .UseUrlRewriter(fun link ->
                if link.Url.TrimStart('/').StartsWith("assets/") 
                then assetUrlRewrite link.Url
                else link.Url)
            .UseSyntaxHighlighting()
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
    |> StaticSite.withFilesFromSources (!! "content/posts/assets/**/*" --"content/posts/assets/**/ignore/**/*") assetUrlRewrite
    |> StaticSite.withFilesFromSources (!! "rootfiles/*") Path.GetFileName
    |> StaticSite.withFilesFromSources (!! "icons/*") Path.GetFileName
    |> StaticSite.withFilesFromSources (!! "code/*") Path.GetFileName
    |> StaticSite.withFilesFromSources (!! "ionicons/*") (fun path -> "ionicons/" + (Path.GetFileName path))
    |> StaticSite.withFilesFromSources (!! "simpleicons/*") (fun path -> "simpleicons/" + (Path.GetFileName path))
    |> StaticSite.generateFromHtml "public" template

Target.runOrDefault "Generate"