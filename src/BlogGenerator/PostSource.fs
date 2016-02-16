module PostSource

open Extensions.Path

let getPostSource fileExists dirExists getLastWriteTime loadJsonAsync basePath postName  =
    let metaPath = basePath +/ "posts-source" +/ postName +/ "meta.json"
    let contentPath = basePath +/ "posts-source" +/ postName +/ "post.md"
    let resourcesBasePath = 
        let r = basePath +/ "posts-source" +/ postName +/ "resources"
        in if dirExists r then Some r else None

    let getLastWritten () =
        [ Some metaPath; Some contentPath; resourcesBasePath ]
        |> List.choose id
        |> List.map getLastWriteTime
        |> List.max

    async {
        match fileExists metaPath && fileExists contentPath with
        | false -> 
            return None
        | _ ->
            let! meta = loadJsonAsync metaPath
            return Some {
                PostName = postName;
                Meta = meta;
                ContentPath = contentPath;
                ResourcesBasePath = resourcesBasePath;
                SourceLastWriteTime = getLastWritten ()
            }
    }
