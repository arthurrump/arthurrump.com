#r "paket:
nuget Fake.Core.Target 
nuget Fake.StaticGen
nuget Fake.StaticGen.Html
nuget Fake.StaticGen.Markdown
nuget Fake.StaticGen.Rss
nuget Nett //"
#load "./.fake/build.fsx/intellisense.fsx"
#if !FAKE
    #r "netstandard"
#endif

open Fake.Core
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

let strf fmt = Printf.ksprintf str fmt

type Page =
    | Page of title : string * htmlContent : string
    | Post of Post

and Post =
    { Title : string
      Category : string
      Tags : string []
      Date : DateTime
      HtmlContent : string }

let postChooser page = match page.Content with Post post -> Some { Url = page.Url; Content = post } | _ -> None

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
          Category = toml.["category"].Get()
          Tags = toml.["tags"].Get()
          Date = toml.["date"].Get()
          HtmlContent = renderedMarkdown }

    { Url = sprintf "%i/%i/%i/%s" post.Date.Year post.Date.Month post.Date.Day slug
      Content = Post post }

let template site page = 
    let titleText =
        match page.Content with
        | Page (title, _) -> title
        | Post post -> post.Title

    let content = 
        match page.Content with
        | Page (_, content) -> rawText content
        | Post post -> rawText post.HtmlContent

    html [] [
        head [ ] [ title [] [ strf "%s - Arthur Rump" titleText ] ]
        body [ ] [ content ]
    ]

let rssFeed (site : StaticSite<unit, Page>) =
    let now = DateTime.UtcNow

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
        title = "Arthur Rump",
        link = site.BaseUrl,
        description = "TODO",
        copyright = sprintf "Copyright (c) %i, Arthur Rump" now.Year,
        generator = "Fake.StaticGen",
        pubDate = now,
        items = (items |> Seq.toList))

let withMarkdownPages files parse =
    let pipeline =
        MarkdownPipelineBuilder()
            .UseEmphasisExtras()
            .Build()
    StaticSite.withPagesFromCustomMarkdown pipeline files parse

Target.create "Generate" <| fun _ ->
    StaticSite.fromConfig "https://www.arthurrump.com" ()
    |> withMarkdownPages (!! "content/*.md") parsePage
    |> withMarkdownPages (!! "content/posts/*.md") parsePost
    |> StaticSite.withRssFeed rssFeed "/feed.xml"
    |> StaticSite.withFilesFromSources (!! "rootfiles/*") Path.GetFileName
    |> StaticSite.withFilesFromSources (!! "icons/*") Path.GetFileName
    |> StaticSite.generateFromHtml "public" template

Target.runOrDefault "Generate"