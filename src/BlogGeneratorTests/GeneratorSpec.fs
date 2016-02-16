module GeneratorSpec

open Xunit
open FsUnit.Xunit
open BlogGenerator

// Generates the blog
[<Fact>]
let testGenerate () =
    let basePath = System.IO.Path.Combine(__SOURCE_DIRECTORY__, @"..\..\");
    Generator.generate basePath
    true |> should be True
    