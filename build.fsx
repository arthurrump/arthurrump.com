#r "paket:
nuget Fake.Core.Target 
nuget Fake.StaticGen
nuget Fake.StaticGen.Html
nuget Fake.StaticGen.Markdown
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
    |> StaticSite.withFilesFromSources (!! "rootfiles/*") Path.GetFileName
    |> StaticSite.withFilesFromSources (!! "icons/*") Path.GetFileName
    |> StaticSite.generateFromHtml "public" template

Target.runOrDefault "Generate"