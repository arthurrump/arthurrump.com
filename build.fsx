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
open Fake.StaticGen.Html.ViewEngine.Accessibility
open Fake.StaticGen.Markdown
open Fake.StaticGen.Rss

open Markdig
open Nett

open System
open System.Globalization
open System.IO
open System.Net
open System.Text
open System.Text.RegularExpressions

let now = DateTime.UtcNow

type Config =
    { Title : string
      Author : string
      AuthorTwitter : string
      ProfileImage : string
      Description : string
      DefaultImage : string
      DisqusId : string
      Navigation : NavItem list
      SocialLinks : Link list }
and NavItem = 
    { Name : string
      Url : string }
and Link =
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
    | Project of Project
    | ProjectsOverview of ProjectType * Page<Project> seq
    | ErrorPage of code: string * text: string

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

and Project =
    { Type : ProjectType
      Title : string
      Tagline : string
      Tech : string []
      Tags : string []
      Color : string
      Image : string option
      ImageAlt : string option
      Order : int
      Links : Link list
      Paragraphs : string list }

and [<RequireQualifiedAccess>]
ProjectType =
    | Project
    | Research

let postChooser page = 
    match page.Content with 
    | Post post -> Some { Url = page.Url; Content = post }
    | _ -> None

let posts (site : StaticSite<Config, Page>) = 
    site.Pages
    |> Seq.choose postChooser
    |> Seq.sortByDescending (fun p -> p.Content.Date)

let projectChooser page =
    match page.Content with
    | Project project when project.Type = ProjectType.Project -> 
        Some { Url = page.Url; Content = project }
    | _ -> 
        None

let projects (site : StaticSite<Config, Page>) =
    site.Pages
    |> Seq.choose projectChooser
    // TODO: sorting

let researchChooser page =
    match page.Content with
    | Project project when project.Type = ProjectType.Research -> 
        Some { Url = page.Url; Content = project }
    | _ -> 
        None

let research (site : StaticSite<Config, Page>) =
    site.Pages
    |> Seq.choose researchChooser
    // TODO: sorting

let postAssetUrlRewrite (filename : string) =
    let dir = filename |> Path.getLowestDirectory
    let file = filename |> Path.GetFileName
    sprintf "/%s/%s/%s/%s/%s"
        dir.[0..3]
        dir.[4..5]
        dir.[6..7]
        dir.[9..]
        file

let projectUrl = function
    | ProjectType.Project -> "project"
    | ProjectType.Research -> "research"

let projectsPageTitle = function
    | ProjectType.Project -> "Projects"
    | ProjectType.Research -> "Research"

let projectAssetUrlRewrite type' (filename : string) =
    let dir = filename |> Path.getLowestDirectory
    let file = filename |> Path.GetFileName
    sprintf "/%s/%s/%s" (projectUrl type') dir file

let tomlTryGet key (toml : TomlTable) =
    if toml.ContainsKey(key) 
    then Some (toml.[key].Get())
    else None

let parseConfig config =
    let toml = Toml.ReadString(config)
    { Title = toml.["title"].Get()
      Author = toml.["author"].Get()
      AuthorTwitter = toml.["author-twitter"].Get()
      ProfileImage = toml.["profile-image"].Get()
      Description = toml.["description"].Get()
      DefaultImage = toml.["default-image"].Get()
      DisqusId = toml.["disqus-id"].Get()
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
          Image = toml |> tomlTryGet "image" |> Option.map postAssetUrlRewrite
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

let parseProject type' path (Some frontmatter) renderedMarkdown =
    let toml = Toml.ReadString frontmatter
    let project =
        { Type = type'
          Title = toml.["title"].Get()
          Tagline = toml.["tagline"].Get()
          Tech = toml.["tech"].Get()
          Tags = toml.["tags"].Get()
          Color = toml.["color"].Get()
          Image = toml |> tomlTryGet "image" |> Option.map (projectAssetUrlRewrite type')
          ImageAlt = toml |> tomlTryGet "image-alt"
          Order = toml.["order"].Get()
          Links = 
            toml.["links"].Get<TomlTableArray>().Items
            |> Seq.map (fun link -> 
                { Name = link.["name"].Get()
                  Icon = link.["icon"].Get()
                  Url = link.["url"].Get() })
            |> Seq.toList
          Paragraphs = renderedMarkdown |> String.splitStr "<hr />" }
    { Url = path |> Path.GetFileNameWithoutExtension |> sprintf "/%s/%s" (projectUrl type')
      Content = Project project }

let projectsOverview (site : StaticSite<Config, Page>) =
    { Url = "/projects"; Content = ProjectsOverview (ProjectType.Project, projects site) }

let researchOverview (site : StaticSite<Config, Page>) =
    { Url = "/research"; Content = ProjectsOverview (ProjectType.Research, research site) }

let template (site : StaticSite<Config, Page>) page = 
    let _property = XmlEngine.attr "property"

    let mutable iconList = set [ "md-close" ]
    let icon name = 
        iconList <- Set.add name iconList
        XmlEngine.tag "svg" [ _class "icon"; _ariaHidden "true" ] [ 
            XmlEngine.tag "use" [ XmlEngine.attr "xlink:href" ("#icon-" + name) ] [] ]

    let iconsCombined () =
        XmlEngine.tag "svg" [
            XmlEngine.attr "xmlns" "http://www.w3.org/2000/svg"
            _style "display: none;" 
        ] (iconList 
           |> Set.map (fun icon -> 
                site.Files 
                |> Seq.find (fun p -> p.Url |> Path.GetFileNameWithoutExtension = icon) 
                |> fun p -> p.Content 
                |> Encoding.UTF8.GetString
                |> String.replace "xmlns=\"http://www.w3.org/2000/svg\"" ""
                |> String.replace "<svg" ("<symbol id=\"icon-" + icon + "\"")
                |> String.replace "</svg" "</symbol"
                |> rawText)
           |> Set.toList)

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
        | Project project -> project.Title
        | ProjectsOverview (type', _) -> projectsPageTitle type'
        | ErrorPage (code, text) -> code + " " + text

    let pageHeader =
        let navItem (item : NavItem) = 
            span [ _class "nav-item" ] [ a [ _href item.Url ] [ str item.Name ] ]

        header [ _id "main-header" ] [
            span [ _id "title" ] [ a [ _href "/" ] [ str "Arthur Rump" ] ]
            button [ _id "menu-toggle"
                     _roleButton
                     _ariaLabel "Menu"
                     _onclick ("var img = document.querySelector('#menu-toggle > svg > use');"
                        + "var nav = document.getElementById('main-nav');"
                        + "if (nav.classList.contains('opened')) {"
                        + "  nav.classList.remove('opened'); img.setAttribute('xlink:href', '#icon-md-menu'); }"
                        + "else { nav.classList.add('opened'); img.setAttribute('xlink:href', '#icon-md-close'); }") ] [
                icon "md-menu"
            ]
            nav [ _id "main-nav"] (site.Config.Navigation |> List.map navItem)
        ]

    let profile () =
        aside [ _class "profile" ] [
            img [ _class "profile-pic"; _src site.Config.ProfileImage ]
            span [ _class "name" ] [ str site.Config.Author ]
            span [ _class "motto" ] [ str site.Config.Description ]
            a [ _class "expand-social-links"
                _roleButton
                _onclick ("var sl = document.querySelector('ul.social-links');"
                   + "sl.classList.toggle('opened');") ] [ 
                str "Find me elsewhere" 
            ]
            ul [ _class "social-links links-list" ] [ 
                for link in site.Config.SocialLinks ->
                    li [] [
                        a [ _href link.Url ] [
                            icon link.Icon
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
                    li [] [ 
                        a [ _href url ] [ str t ]
                        span [ _class "page-count" ] [ strf "(%i)" count ]
                    ] 
            ]
        ]

    let shareBox url title = 
        let enc = WebUtility.UrlEncode
        let url = url |> site.AbsoluteUrl |> enc
        let title = enc title
        let sharelink iconName text url = a [ _href url; _title text; _ariaLabel text; _target "blank"; _rel "noopener noreferrer" ] [ icon iconName ]
        div [ _class "sharebox" ] [
            span [] [ str "Share this:" ]
            div [ _class "links" ] [
                sharelink "facebook" "Facebook" (sprintf "https://www.facebook.com/sharer/sharer.php?u=%s" url)
                sharelink "twitter" "Twitter" (sprintf "https://twitter.com/intent/tweet?text=%s&url=%s&via=%s" title url (enc (site.Config.AuthorTwitter.TrimStart('@'))))
                sharelink "linkedin" "LinkedIn" (sprintf "https://www.linkedin.com/sharing/share-offsite/?url=%s" url)
                sharelink "reddit" "Reddit" (sprintf "http://www.reddit.com/submit?url=%s&title=%s" url title)
            ]
        ]

    let readAlsoBox text url tags =
        let related = 
            posts site
            |> Seq.filter (fun p -> p.Url <> url)
            |> Seq.map (fun p -> p, p.Content.Tags |> Seq.filter (fun t -> tags |> Seq.contains t) |> Seq.length)
            |> Seq.sortByDescending snd
            |> Seq.truncate 3
            |> Seq.takeWhile (fun (_, rel) -> rel > 0)
            |> Seq.map fst
        let related = 
            if related |> Seq.isEmpty 
            then posts site |> Seq.filter (fun p -> p.Url <> url) |> Seq.truncate 3 
            else related
        div [ _class "readalsobox" ] [
            span [] [ str text ]
            ul [ _class "post-overview" ] (related |> Seq.map postListItem |> Seq.toList)
        ]

    let commentsBox url = 
        div [ _class "commentsbox" ] [
            rawText (sprintf """<div id="disqus_thread"></div>
<script>
var disqus_config = function () {
this.page.url = '%s';
this.page.identifier = '%s';
};

(function() { // DON'T EDIT BELOW THIS LINE
var d = document, s = d.createElement('script');
s.src = 'https://%s.disqus.com/embed.js';
s.setAttribute('data-timestamp', +new Date());
(d.head || d.body).appendChild(s);
})();
</script>
<noscript>Please enable JavaScript to view the <a href="https://disqus.com/?ref_noscript">comments powered by Disqus.</a></noscript>
"""             (site.AbsoluteUrl url) url (site.Config.DisqusId)) ]

    let projectHeader titleWrapper imgWrapper project =
        div [ _class "project-header"
              _style (sprintf "background-color: %s;" project.Color) ] [
            match project.Image with
            | Some image ->
                div [ _class "left" ] [ 
                    imgWrapper (img [ 
                        _class "header-image"; 
                        _src image; 
                        _alt (project.ImageAlt |> Option.defaultValue "")
                    ]) 
                ]
            | None -> 
                ()
            div [ _class (if project.Image.IsSome then "right" else "full") ] [ 
                titleWrapper (h1 [] [ str project.Title ])
                span [ _class "tagline" ] [ str project.Tagline ]
                rawText project.Paragraphs.Head
                ul [ _class "links-list" ] [
                    yield str "Links:"
                    for link in project.Links ->
                        li [] [
                        a [ _href link.Url ] [
                            icon link.Icon
                            str link.Name 
                        ]
                    ]
                ]
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
                yield shareBox page.Url post.Title
                yield commentsBox page.Url
                yield readAlsoBox "Read also:" page.Url post.Tags
            ]
        | PostsOverview overview ->
            let pagination = 
                let older = overview.NextUrl |> Option.map (fun n -> a [ _href n; _class "older" ] [ str "Older"; icon "md-arrow-forward" ])
                let newer = overview.PreviousUrl |> Option.map (fun p -> a [ _href p; _class "newer" ] [ icon "md-arrow-back"; str "Newer" ])
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
        | Project project ->
            div [ _class "project" ] [
                yield projectHeader id id project
                yield div [ _class "tech"] [
                    span [] [ str "Built with" ]
                    ul [ _class "tag-list" ] [
                        for t in project.Tech ->
                            li [] [ str t ]
                    ]
                ]
                for par in project.Paragraphs |> List.tail ->
                    section [ _class "text" ] [
                        rawText par
                    ]
            ]
        | ProjectsOverview (type', projects) ->
            let projects = projects |> Seq.sortByDescending (fun p -> p.Content.Order)
            div [ _class "titeled-container projects-overview" ] [
                yield h1 [] [ str (projectsPageTitle type') ]
                for p in projects -> section [] [ 
                    projectHeader 
                        (fun h -> a [ _href p.Url ] [ h ]) 
                        (fun img -> a [ _href p.Url; _class "image-link" ] [ img ])
                        p.Content 
                ]
            ]
        | ErrorPage (code, text) ->
            div [ _id "error-page" ] [
                span [ _class "status-code" ] [ str code ]
                span [ _class "status-text" ] [ str text ]
                readAlsoBox "Maybe you find this interesting:" page.Url []
            ]

    let frame content =
        match page.Content with
        | Post _ | TagsOverview _ | Project _ | ProjectsOverview _ | ErrorPage _  ->
            content
        | Page _ | PostsOverview _ ->
            div [ _class "columns" ] [ content; profile () ]
        | TagPage _ | PostsArchive _ ->
            div [ _class "columns" ] [ content; tagList (tags site) ]

    let matomo =
        rawText """<script type="text/javascript">
  var _paq = window._paq || [];
  /* tracker methods like "setCustomDimension" should be called before "trackPageView" */
  _paq.push(["disableCookies"]);
  _paq.push(["setDomains", ["*.www.arthurrump.com","*.arthurrump.gitlab.io"]]);
  _paq.push(['trackPageView']);
  _paq.push(['enableLinkTracking']);
  (function() {
    var u="//analytics.arthurrump.com/";
    _paq.push(['setTrackerUrl', u+'matomo.php']);
    _paq.push(['setSiteId', '2']);
    var d=document, g=d.createElement('script'), s=d.getElementsByTagName('script')[0];
    g.type='text/javascript'; g.async=true; g.defer=true; g.src=u+'matomo.js'; s.parentNode.insertBefore(g,s);
  })();
</script>"""

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
        | Project project ->
            [ meta [ _name "keywords"; _content (project.Tags |> String.concat ",") ]
              meta [ _name "title"; _content project.Title ]
              meta [ _name "description"; _content project.Tagline ]
              meta [ _name "twitter:card"; _content "summary_large_image" ]
              meta [ _name "twitter:creator"; _content site.Config.AuthorTwitter ]
              meta [ _property "og:title"; _content project.Title ]
              meta [ _property "og:type"; _content "website" ]
              match project.Image with Some image -> meta [ _property "og:image"; _content (site.AbsoluteUrl image) ] | None -> ()
              meta [ _property "og:description"; _content project.Tagline ] ]
        | _ ->
            [ meta [ _name "description"; _content site.Config.Description ]
              meta [ _property "og:title"; _content titleText ]
              meta [ _property "og:type"; _content "website" ] ]

    let fullBody = 
        div [ _id "background" ] [ 
            div [ _id "container" ] [ 
                pageHeader
                frame content 
            ]
        ]

    html [] [
        head [ ] ([ 
            meta [ _httpEquiv "Content-Type"; _content "text/html; charset=utf-8" ]
            title [] [ strf "%s - %s" titleText site.Config.Title ]
            link [ _rel "stylesheet"; _type "text/css"; _href "/style.css" ]
            link [ _rel "stylesheet"; _type "text/css"; _href "/colorcode.css" ]
            link [ _rel "icon"; _href "favicon.ico" ]
            link [ _rel "icon"; _href "favicon-16x16.png"; _type "image/png"; _sizes "16x16" ]
            link [ _rel "icon"; _href "favicon-32x32.png"; _type "image/png"; _sizes "32x32" ]
            link [ _rel "icon"; _href "icon_192.png"; _type "image/png"; _sizes "192x192" ]
            link [ _rel "apple-touch-icon"; _href "apple-touch-icon.png"; _type "image/png"; _sizes "180x180" ]
            meta [ _name "author"; _content site.Config.Author ]
            meta [ _name "copyright"; _content (sprintf "Copyright %i %s" now.Year site.Config.Author) ]
            meta [ _name "description"; _content site.Config.Description ]
            meta [ _name "generator"; _content "Fake.StaticGen" ]
            meta [ _name "viewport"; _content "width=device-width, initial-scale=1" ]
            link [ _rel "canonical"; _href (site.AbsoluteUrl page.Url) ]
            meta [ _property "og:url"; _content (site.AbsoluteUrl page.Url) ]
            meta [ _property "og:site_name"; _content site.Config.Title ] 
            meta [ _name "twitter:dnt"; _content "on" ]
            matomo
        ] @ headerTags)
        body [ ] [ 
            iconsCombined ()
            fullBody
            footer [] [
                span [] [ a [ _href "/archives" ] [ str "Archive" ] ]
                span [] [ a [ _href "/tags" ] [ str "Tags" ] ]
                span [] [ a [ _href "/feed.xml" ] [ str "RSS Feed" ] ]
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

let colorCodeCss =
    let removeBodyRule (css : string) = Regex.Replace(css, "body\s*\{[^\}]*\}", "")
    let lightCss = ColorCode.HtmlClassFormatter(ColorCode.Styling.StyleDictionary.DefaultLight).GetCSSString()
    let darkCss = ColorCode.HtmlClassFormatter(ColorCode.Styling.StyleDictionary.DefaultDark).GetCSSString()
    sprintf "%s @media (prefers-color-scheme: dark) { %s }" (removeBodyRule lightCss) (removeBodyRule darkCss)

let withMarkdownPages files parse =
    let pipeline =
        MarkdownPipelineBuilder()
            .UseEmphasisExtras()
            .UseGenericAttributes()
            .UsePipeTables()
            .UseImageAsFigure(onlyWithTitle = true)
            .UseUrlRewriter(fun link ->
                if link.Url.TrimStart('/').StartsWith("assets/") 
                then postAssetUrlRewrite link.Url
                else link.Url)
            .UseSyntaxHighlighting(ColorCode.Styling.StyleDictionary.DefaultLight, false)
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
    |> withMarkdownPages (!! "content/projects/*.md") (parseProject ProjectType.Project)
    |> StaticSite.withOverviewPage projectsOverview
    |> withMarkdownPages (!! "content/research/*.md") (parseProject ProjectType.Research)
    |> StaticSite.withOverviewPage researchOverview
    |> StaticSite.withFileFromString colorCodeCss "/colorcode.css"
    |> StaticSite.withFilesFromSources (!! "content/posts/assets/**/*" --"content/posts/assets/**/ignore/**/*") postAssetUrlRewrite
    |> StaticSite.withFilesFromSources (!! "content/projects/assets/**/*" --"content/projects/assets/**/ignore/**/*") (projectAssetUrlRewrite ProjectType.Project)
    |> StaticSite.withFilesFromSources (!! "content/research/assets/**/*" --"content/research/assets/**/ignore/**/*") (projectAssetUrlRewrite ProjectType.Research)
    |> StaticSite.withFilesFromSources (!! "assets/**/*") (String.regex_replace ".*assets[/\\\\]" "")
    |> StaticSite.withFilesFromSources (!! "code/*") Path.GetFileName
    |> StaticSite.withFilesFromSources (!! "icons/**/*") (fun path -> "icons/" + (Path.GetFileName path))
    |> StaticSite.withPage (ErrorPage ("404", "Not Found")) "/404.html"
    |> StaticSite.generateFromHtml "public" template

Target.runOrDefault "Generate"