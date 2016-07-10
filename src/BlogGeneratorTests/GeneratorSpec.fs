module GeneratorSpec

open Xunit
open FsUnit.Xunit
open BlogGenerator
open System.IO

// Generates the blog
[<Fact>]
let testGenerate () =
    let basePath = new DirectoryInfo(System.IO.Path.Combine(__SOURCE_DIRECTORY__, @"..\..\"));
    Generator.generate basePath.FullName
    true |> should be True
    