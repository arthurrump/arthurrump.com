---
title: "Git-based versioning using FAKE"
tags: [ "F#", "FAKE", "Versioning", "Git" ]
category: Posts
---

How to use FAKE to automatically set version numbers based on your Git history.

For [Fake.StaticGen](https://github.com/arthurrump/Fake.StaticGen), I wanted to have a semi-automated way of determining the version number for each release. Inspired by [Nerdbank.GitVersioning](https://github.com/AArnott/Nerdbank.GitVersioning), I liked the idea of having a unique version for every commit, but I didn't quite like how this tool took over the build process. (Or I would have to try and integrate it into MSBuild, which seems to be possible. But MSBuild...)

## The versioning system

I came up with the following system, which would be relatively easy to do with the [FAKE](https://fake.build) build script that I was already using. Version information is provided in two files, *version* and *version-pre* in the root of the repository. In *version* the base version for the current commit is specified and *version-pre* contains the prerelease tag (the "-text" part of a version) if the version is a prerelease. The actual version is calculated based on the 'distance' of the commit to the last commit where the *version* file was changed.

Example:

| #    | Commit changes                                   | Version    |
| ---- | ------------------------------------------------ | ---------- |
| A    | `version` is set to 1.1, `version-pre` to `beta` | 1.1.0-beta |
| B    | code updates                                     | 1.1.1-beta |
| C    | `version-pre` removed                            | 1.1.2      |
| D    | more updates                                     | 1.1.3      |
| E    | `version` is set to 1.2                          | 1.2.0      |
| F    | more updates                                     | 1.2.1      |

Now when we're still at commit F and we start making changes, the patch version will go up one and a -dirty prerelease flag is added, so we end up with version 1.2.2-dirty.

These are the versions as they are built on the master branch or a tagged commit. If they are built on a different branch, the branch label and the short commit hash are added to the prerelease flags. For commit F on the dev branch, that would result in 1.2.1-dev-F.

## Some helper functions

FAKE comes with a wide variety of modules, two of which are particularly useful for our scenario: [Fake.Core.SemVer](https://fake.build/apidocs/v5/fake-core-semver.html) for working with versions, and [Fake.Tools.Git](https://fake.build/apidocs/v5/index.html#Fake.Tools.Git) for executing Git commands. To use some functions of these modules more easily, we'll define a couple of helper functions.

Let's start with some helper functions to get the relevant information from Git:

```f#
let [<Literal>] repo = "."

module GitHelpers =
    let isTagged () = 
        Git.CommandHelper.directRunGitCommand repo "describe --exact-match HEAD"

    let previousChangeCommit file = 
        Git.CommandHelper.runSimpleGitCommand repo ("log --format=%H -1 -- " + file)

    let fileChanged file =
        Git.FileStatus.getChangedFilesInWorkingCopy repo "HEAD" 
        |> Seq.exists (fun (_, f) -> f = file)
```

- `isTagged` checks if the current commit is associated with a tag. If there is no tag, the command fails and `directRunGitCommand` returns false.
- `previousChangeCommit` gets the commit where a file was last changed. It requests the log for this file, printing only the commit hash using the `%H` format string and limiting the number of commits to 1. `runSimpleGitCommand` returns the output of the command as a string.
- `fileChanged` checks if a file is changed in the working copy and not yet committed.

We'll also define some helper functions for modifying versions:

```f#
module Version =
    let withPatch patch version =
        { version with Patch = patch; Original = None }

    let appendPrerelease suffix version =
        let pre = 
            match suffix, version.PreRelease with
            | Some s, Some p -> PreRelease.TryParse (sprintf "%O-%s" p s)
            | Some s, None -> PreRelease.TryParse s
            | None, p -> p
        { version with PreRelease = pre; Original = None }
```

- `withPatch` sets a new number to the patch number of a version.
- `appendPrerelease` adds a suffix to the prerelease tag of a version, accounting for the fact that there might not be a prerelease tag yet.

## Calculating the version

Now we can write our logic to calculate the correct version. We'll start with the simple version, which does not care about the branch rules yet:

```f#
let [<Literal>] versionFile = "version"
let [<Literal>] versionPreFile = "version-pre"

module Version =
    let getCleanVersion () = 
        Trace.trace "Determining version based on Git history"
        let version = File.readAsString versionFile |> SemVer.parse

        let height =
            if GitHelpers.fileChanged versionFile then 
                0
            else
                let previousVersionChange = GitHelpers.previousChangeCommit versionFile
                let height = Git.Branches.revisionsBetween repo previousVersionChange "HEAD"
                if Git.Information.isCleanWorkingCopy repo then height else height + 1

        let pre = 
            if File.exists versionPreFile
            then Some (File.readAsString versionPreFile)
            else None

        version |> withPatch (uint32 height) |> appendPrerelease pre
```

The most interesting part is the calculation of the 'git height', the number of commits between the current commit and the commit where the *version* file was last changed. If the file is changed in the current working copy, we reset the height to 0. Otherwise, we retrieve the commit where *version* was last changed, using our `previousChangeCommit` helper function. Then we use the built-in FAKE function `revisionsBetween` to get the 'height' of our current commit. If the working directory is clean, this is the result, otherwise, we add 1.

The height is set as the patch number of the version read from the *version* file (don't forget to convert to an unsigned integer) and combined with the prerelease flags read from *version-pre* to create the correct version.

### Other branch rules

To keep things a bit orderly we'll set the correct dirty- and branch-flags in a new function:

```f#
module Version =
    let getVersionWithPrerelease () =
        let pre = 
            let branch = 
                match Git.Information.getBranchName repo with
                | "NoBranch" -> None
                | branch -> Some branch
            let isClean = Git.Information.isCleanWorkingCopy repo
            if isClean && (branch = Some "master" || GitHelpers.isTagged ()) then 
                None 
            else 
                let commit = Git.Information.getCurrentSHA1 repo |> fun s -> s.Substring(0, 7)
                let dirty = if isClean then None else Some "dirty"
                [ branch; Some commit; dirty ] |> List.choose id |> String.concat "-" |> Some

        getCleanVersion () |> appendPrerelease pre
```

First, we get the name of the branch, where Git returning "NoBranch" means that there is no branch checked out. If we are in a clean working copy (i.e. there are no changes since the last commit) and the branch is the master branch or the current commit is tagged, then we don't add any prerelease tags.

If that condition does not hold, we get the abbreviated commit hash, set the dirty flag if necessary and put them together into a dash-separated string. The prerelease flag is then appended onto the 'clean' version.

That's it. Now we can retrieve the calculated version by calling `Version.getVersionWithPrerelease ()`.

You can, of course, use this process with any type of application, but chances are you're using FAKE with a .NET project. So let's look at how we can set the correct version when building with the dotnet CLI.

## Setting the version in a .NET project

There are some nested records involved when configuring a .NET build in FAKE, so we'll first define some helper functions on the MSBuild parameters:

```f#
[<AutoOpen>]
module MSBuildParamHelpers =
    let withVersion version (param : MSBuild.CliArguments) =
        { param with Properties = ("Version", string version)::param.Properties }

    let withNoWarn warnings (param : MSBuild.CliArguments) =
        { param with 
            NoWarn = 
                param.NoWarn 
                |> Option.defaultValue []
                |> List.append warnings
                |> Some }

    let withDefaults version =
        withVersion version >> withNoWarn [ "FS2003" ]
```

The `withDefaults` function sets the version and adds a flag to disable warnings for FS2003, which will complain about the `AssemblyInformationalVersionAttribute` being set to a string that is not a valid assembly version number, such as a version with a prerelease flag. According to [documentation](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assemblyinformationalversionattribute), "this warning is harmless" and it can safely be ignored. For a C# project, the corresponding warning is [CS1607](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/cs1607).

Now you can build a project with the correct version:

```f#
let version = Version.getVersionWithPrerelease ()
DotNet.build 
    (fun o -> { o with MSBuildParams = o.MSBuildParams |> withDefaults version })
    projectOrSolutionPath
```

## Using with Azure DevOps Pipelines

When you're running your builds on Azure Pipelines, there are two more things to consider:

- When your Git repo is cloned, the commit is checked out, not the branch, so `getBranchName` will always return "NoBranch".
- You might want to set the build number to match your calculated version.

To do these two things, here are a couple more helper functions:

```f#
module AzureDevOps =
    let tryGetSourceBranch () =
        System.Environment.GetEnvironmentVariable("BUILD_SOURCEBRANCHNAME") 
        |> Option.ofObj

    let updateBuildNumber version =
        sprintf "\n##vso[build.updatebuildnumber]%O" version
        |> System.Console.WriteLine
```

- `tryGetSourceBranch` tries to read the BUILD_SOURCEBRANCHNAME environment variable, where [Azure Pipelines stores the branch](https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables#build-variables) on which the build was initiated.
- `updateBuildNumber` writes a [logging command](https://github.com/Microsoft/azure-pipelines-tasks/blob/master/docs/authoring/commands.md#build-logging-commands) to standard output to update the build number to a given version.

We'll need to use `tryGetSourceBranch` at the point where we try to read the branch from Git in `getVersionWithPrerelease`. The updated code to get the branch looks like this:

```f#
let branch = 
    match Git.Information.getBranchName repo with
    | "NoBranch" -> AzureDevOps.tryGetSourceBranch ()
    | branch -> Some branch
```

You can call `updateBuildNumber` at any point, for example just before doing the build:

```f#
let version = Version.getVersionWithPrerelease ()
AzureDevOps.updateBuildNumber version
DotNet.build 
    (fun o -> { o with MSBuildParams = o.MSBuildParams |> withDefaults version })
    projectOrSolutionPath
```

Now we have a fully reproducible version numbering system that will automatically assign a version to every commit, and the versions are directly used as the build number in Azure DevOps. If you'd like to see the complete code, it's available in [this Gist](https://gist.github.com/arthurrump/5cc9005e4d817f7d36c2c4ad736fd894).

