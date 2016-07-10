namespace Extensions

module OptionBuilder =

    type OptionBuilder() =
        member this.Bind(x, f) = Option.bind f x
        member this.Return(i) = Option.Some i

    let option = new OptionBuilder()
