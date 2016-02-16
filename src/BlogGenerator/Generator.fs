namespace BlogGenerator

open FSharp.Literate
open System.IO
open Extensions
open Extensions.Path

module Generator = 
    let private generatePost (source, destination) = 
        let { Meta = meta; ContentPath = contentPath; ResourcesBasePath = sourceResourcesPath } = source
        let { HtmlPath = htmlPath; ResourcesBasePath = destResourcesPath } = destination
        Directory.CreateDirectory(Path.GetDirectoryName htmlPath) |> ignore
        Literate.ProcessMarkdown(contentPath, output = htmlPath)

    let generate basePath = 
        let getPostSource = PostSource.getPostSource File.Exists Directory.Exists File.GetLastWriteTimeUtc Meta.AsyncLoad basePath
        let getPostDestination = PostDestination.getPostDestination File.GetLastWriteTimeUtc basePath
        let postUpdated ({ SourceLastWriteTime = s }, { DestinationLastWriteTime = d }) = if d.IsSome then s > d.Value else true
        
        let allPosts = 
            async { 
                let! sources = 
                    Path.getChildDirectoryNames (basePath +/ "posts-source")
                    |> Seq.map getPostSource
                    |> Async.Parallel
                return
                    sources
                    |> Seq.choose id
                    |> Seq.map (fun source -> source, getPostDestination source)
            }
            |> Async.RunSynchronously

        // Generate posts that require it
        allPosts 
        |> Seq.filter postUpdated
        |> Seq.map generatePost
        |> Seq.toList
        |> ignore
