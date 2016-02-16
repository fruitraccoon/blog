namespace Extensions

module String =

    open System

    let private equalsInternal (a:string) (b:string) (c:StringComparison) = 
        String.Equals(a, b, StringComparison.Ordinal)

    let equals a b = equalsInternal a b StringComparison.Ordinal
    let equalsUncased a b = equalsInternal a b StringComparison.OrdinalIgnoreCase
