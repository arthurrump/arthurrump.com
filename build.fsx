#r "paket:
nuget Fake.Core.Target 
nuget Fake.StaticGen
nuget Fake.StaticGen.Html //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO.Globbing.Operators
open Fake.StaticGen

Target.create "Generate" <| fun _ ->
    StaticSite.fromConfig "https://www.arthurrump.com" ()
    |> StaticSite.withFilesFromSources (!! "rootfiles/*") id
    |> StaticSite.withFilesFromSources (!! "icons/*") id
    |> StaticSite.generate "public" (fun _ _ -> "")

Target.runOrDefault "Generate"