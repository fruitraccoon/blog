module PostDestination

open System
open Extensions
open Extensions.Path

let getPostDestination getLastWriteTime basePath source  =
    let { PostName = name; Meta = m; } = source
    let postPath = "posts" +/ m.Date.Year.ToString("0000") +/ m.Date.Month.ToString("00") +/ name
    let postRoot = basePath +/ postPath
    let htmlPath = postRoot +/ "index.html"
    let resourcesBasePath = postRoot +/ "resources"
    let lastWritten =
        [ htmlPath; resourcesBasePath ]
        |> List.map getLastWriteTime
        |> List.max
        |> fun d -> if d > DateTime.minFileTimeUtc then Some d else None
    let url = new Uri("." +/ postPath +/ "index.html", UriKind.Relative)
    {
        Url = url.ToString()
        HtmlPath = htmlPath
        ResourcesBasePath = resourcesBasePath
        DestinationLastWriteTime = lastWritten
    }