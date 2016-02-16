module PostSourceSpec

open System
open Xunit
open FsUnit.Xunit
open BlogGenerator

[<Fact>]
let ``Should return PostSource`` () =
    // -- Arrange --
    let testJson = """ { "title": "Testing", "date": "2016-02-15T13:00:00", "tags": ["tag1", "tag2"] } """
    let testWriteTime = new DateTime(2000, 1, 2)
    let fileExists (s:string) = true
    let dirExists (s:string) = true
    let getLastWriteTime (s:string) = testWriteTime
    let loadJsonAsync (s:string) = async { return Meta.Parse testJson }

    // -- Act --
    let result = PostSource.getPostSource fileExists dirExists getLastWriteTime loadJsonAsync "C:\\temp\\" "test-post" |> Async.RunSynchronously

    // -- Assert --
    result.IsSome |> should be True
    let value = result.Value
    value.PostName |> should equal "test-post"
    value.ContentPath |> should equal """C:\temp\posts-source\test-post\post.md"""
    value.ResourcesBasePath |> should equal (Some """C:\temp\posts-source\test-post\resources""")
    value.SourceLastWriteTime |> should equal testWriteTime
    let meta = result.Value.Meta
    meta.Title |> should equal "Testing"
    meta.Date.ToString("yyyy-MM-dd HH:mm:ss") |> should equal "2016-02-15 13:00:00"
    meta.Tags |> should haveLength 2
    meta.Tags |> should contain "tag1"
    meta.Tags |> should contain "tag2"

[<Fact>]
let ``Should parse when Meta.Tags is missing`` () =
    // -- Arrange --
    let testJson = """ { "title": "Testing", "date": "2016-02-15T13:00:00" } """
    let fileExists (s:string) = true
    let dirExists (s:string) = true
    let getLastWriteTime (s:string) = DateTime.Now
    let loadJsonAsync (s:string) = async { return Meta.Parse testJson }

    // -- Act --
    let result = PostSource.getPostSource fileExists dirExists getLastWriteTime loadJsonAsync "C:\\temp\\" "test-post" |> Async.RunSynchronously

    // -- Assert --
    result.IsSome |> should be True
    let meta = result.Value.Meta
    meta.Title |> should equal "Testing"
    meta.Tags |> should haveLength 0

[<Fact>]
let ``Should throw when Meta.Date is missing`` () =
    // -- Arrange --
    let testJson = """ { "title": "Testing" } """
    let fileExists (s:string) = true
    let dirExists (s:string) = true
    let getLastWriteTime (s:string) = DateTime.Now
    let loadJsonAsync (s:string) = async { return Meta.Parse testJson }

    // -- Act --
    let result = PostSource.getPostSource fileExists dirExists getLastWriteTime loadJsonAsync "C:\\temp\\" "test-post" |> Async.RunSynchronously

    // -- Assert --
    result.IsSome |> should be True
    let meta = result.Value.Meta
    meta.Title |> should equal "Testing"
    (fun () -> meta.Date |> ignore) |> should throw typeof<System.Exception>