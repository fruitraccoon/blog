module PostDestinationSpec

open System
open Xunit
open FsUnit.Xunit
open BlogGenerator

[<Fact>]
let ``Should return PostDestination`` () =
    // -- Arrange --
    let testWriteTime = new DateTime(2000, 1, 2)
    let getLastWriteTime (s:string) = testWriteTime
    let postSource = {
        PostName = "test-post";
        Meta = Meta.Parse("""{ "title": "Test", "date": "2016-02-15T12:00:00" }""");
        ContentPath = """C:\content\file.md""";
        ResourcesBasePath = None;
        SourceLastWriteTime = DateTime.Now;
    }

    // -- Act --
    let result = PostDestination.getPostDestination getLastWriteTime "C:\\site\\" postSource

    // -- Assert --
    result.HtmlPath |> should equal """C:\site\posts\2016\02\test-post\index.html"""
    result.ResourcesBasePath |> should equal """C:\site\posts\2016\02\test-post\resources"""
    result.DestinationLastWriteTime.IsSome |> should be True
    result.DestinationLastWriteTime.Value |> should equal testWriteTime
