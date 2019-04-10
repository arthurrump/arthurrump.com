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

type Config =
    { Title : string
      Author : string
      Description : string }

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
          Category = toml.["category"].Get()
          Tags = toml.["tags"].Get()
          Date = toml.["date"].Get()
          HtmlContent = renderedMarkdown }

    { Url = sprintf "%i/%i/%i/%s" post.Date.Year post.Date.Month post.Date.Day slug
      Content = Post post }

let template (site : StaticSite<Config, Page>) page = 
    let titleText =
        match page.Content with
        | Page (title, _) -> title
        | Post post -> post.Title

    let content = 
        match page.Content with
        | Page (_, content) -> rawText content
        | Post post -> rawText post.HtmlContent

    let keywords = 
        match page.Content with
        | Post post -> post.Tags |> String.concat ","
        | Page _ -> ""

    html [] [
        head [ ] [ 
            title [] [ strf "%s - %s" titleText site.Config.Title ]
            meta [ _name "author"; _content site.Config.Author ]
            meta [ _name "description"; _content site.Config.Description ]
            meta [ _name "keywords"; _content keywords ]
            meta [ _name "generator"; _content "Fake.StaticGen" ]
            meta [ _name "viewport"; _content "width=device-width, initial-scale=1" ]
            link [ _rel "canonical"; _content (site.AbsoluteUrl page.Url) ] ]
        body [ ] [ content ] ]

let rssFeed (site : StaticSite<Config, Page>) =
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
            .Build()
    StaticSite.withPagesFromCustomMarkdown pipeline files parse

Target.create "Generate" <| fun _ ->
    StaticSite.fromConfigFile "https://www.arthurrump.com" "config.toml" parseConfig
    |> withMarkdownPages (!! "content/*.md") parsePage
    |> withMarkdownPages (!! "content/posts/*.md") parsePost
    |> StaticSite.withRssFeed rssFeed "/feed.xml"
    |> StaticSite.withFilesFromSources (!! "rootfiles/*") Path.GetFileName
    |> StaticSite.withFilesFromSources (!! "icons/*") Path.GetFileName
    |> StaticSite.generateFromHtml "public" template

Target.runOrDefault "Generate"