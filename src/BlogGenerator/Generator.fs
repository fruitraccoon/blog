namespace BlogGenerator

open FSharp.Literate
open System.IO
open Extensions
open Extensions.Path

module Generator = 
    open System.Text.RegularExpressions

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

        let generatePost (source, destination) = 
            let templatePath = basePath +/ "templates" +/ "blog-post.html"
            let { Meta = meta; ContentPath = contentPath; ResourcesBasePath = sourceResourcesPath } = source
            let { HtmlPath = htmlPath; ResourcesBasePath = destResourcesPath } = destination
            let replacements = [ "page-title", sprintf "%s - Jono's Blog" source.Meta.Title ]
            Directory.CreateDirectory(Path.GetDirectoryName htmlPath) |> ignore
            Literate.ProcessMarkdown(contentPath, templateFile = templatePath, output = htmlPath, replacements = replacements)

        let generateIndex posts =
            let templatePath = basePath +/ "templates" +/ "blog-index.html"
            let outputPath = basePath +/ "index.html"
            let postsSorted =
                posts
                |> Seq.sortByDescending (fun ({ Meta = m },_) -> m.Date)
                |> Seq.toList

            let getPostHtml ps pd = 
                let tags = 
                    ps.Meta.Tags 
                    |> Array.map (fun t -> sprintf "<li>%s</li>" t) 
                    |> String.concat "" 
                    |> sprintf "<ul>%s</ul>"
                let date = ps.Meta.Date.ToString("dd MMMM yyyy")

                sprintf """
                    <div class="post-summary">
                        <div class="title"><a href="%s">%s</a></div>
                        <div class="date">%s</div>
                        <div class="tags">%s</div>
                    </div>""" pd.Url ps.Meta.Title date tags

            let postsHtml = 
                postsSorted
                |> List.map (fun (s,d) -> getPostHtml s d |> sprintf "<li>%s</li>")
                |> String.concat ""
                |> sprintf "<ul>%s</ul>"

            let replacements = 
              [ "page-title", "Jono's Blog"
                "posts", postsHtml ]

            let templateContents = File.ReadAllText(templatePath);
            let indexPage = 
                replacements
                |> List.fold (fun s (k,v) -> Regex.Replace(s, (sprintf "{%s}" k), v)) templateContents
            File.WriteAllText(outputPath, indexPage)

        // Generate posts that require it
        allPosts 
        |> Seq.filter postUpdated
        |> Seq.map generatePost
        |> Seq.toList
        |> ignore

        // Generate main blog page
        generateIndex allPosts