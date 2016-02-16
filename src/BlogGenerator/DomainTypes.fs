[<AutoOpen>]
module DomainTypes

open System
open FSharp.Data

let [<Literal>] private MetaSample = """
[
{ 
    "title": "Sample", 
    "date": "1970-01-01T00:00:00",
    "tags": ["one", "two"]
},
{ 
    "title": "Sample", 
    "date": "1970-01-01T00:00:00"
}
]"""

// Use "SampleIsList" to collapse multiple samples into one to show optional values
type Meta = JsonProvider<MetaSample, SampleIsList=true>

type PostSource = {
    PostName: string;
    Meta: Meta.Root;
    ContentPath: string;
    ResourcesBasePath: string option;
    SourceLastWriteTime: DateTime;
}

type PostDestination = {
    HtmlPath: string;
    ResourcesBasePath: string;
    DestinationLastWriteTime: DateTime option;
}
