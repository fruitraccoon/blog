module PostDestination

open System
open Extensions
open Extensions.Path

let getPostDestination getLastWriteTime basePath source  =
    let { PostName = name; Meta = m; } = source
    let postRoot = basePath +/ "posts" +/ m.Date.Year.ToString("0000") +/ m.Date.Month.ToString("00") +/ name
    let htmlPath = postRoot +/ "index.html"
    let resourcesBasePath = postRoot +/ "resources"
    let getLastWritten () =
        [ htmlPath; resourcesBasePath ]
        |> List.map getLastWriteTime
        |> List.max
        |> fun d -> if d > DateTime.minFileTimeUtc then Some d else None
    {
        HtmlPath = htmlPath;
        ResourcesBasePath = resourcesBasePath;
        DestinationLastWriteTime = getLastWritten ();
    }