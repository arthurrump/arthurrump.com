#r "paket:
nuget Fake.Core.Target 
nuget Fake.StaticGen
nuget Fake.StaticGen.Html
nuget Fake.StaticGen.Markdown //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO.Globbing.Operators
open Fake.StaticGen
open Fake.StaticGen.Html
open Fake.StaticGen.Html.ViewEngine
open Fake.StaticGen.Markdown

open Markdig

open System.IO

type Page =
    | Page of htmlContent : string
    | Post of htmlContent : string

let parsePage path frontmatter renderedMarkdown =
    let slug = path |> Path.GetFileNameWithoutExtension
    { Url = slug; Content = Page renderedMarkdown }

let parsePost path frontmatter renderedMarkdown =
    let filename = path |> Path.GetFileNameWithoutExtension
    let slug = filename.Substring(9)
    { Url = slug; Content = Post renderedMarkdown }

let template site page = 
    match page.Content with
    | Page content
    | Post content ->
        html [] [
            head [ ] [ title [] [ str "Arthur Rump" ] ]
            body [ ] [ rawText content ]
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