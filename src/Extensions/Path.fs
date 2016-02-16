namespace Extensions

open System
open System.IO

module Path = 
    /// Gets the names of the immediate child directories of the path, without any path details
    let getChildDirectoryNames path = 
        Directory.EnumerateDirectories(path) 
        |> Seq.map (fun p -> (new DirectoryInfo(p)).Name)

    /// Custom operator for combining paths
    let (+/) path1 path2 = Path.Combine(path1, path2)
