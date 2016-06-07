namespace sample

open System
open FSharpx
open FSharpx.Option

module Int32 =
  let tryParse s =
    match Int32.TryParse(s) with
    | true, result -> Some result
    | false, _     -> None


module Example1 =
  let findCustomers count = []

  let doWebRequest param =
    param
    |> Int32.tryParse
    |> Option.getOrElse -1
    |> findCustomers

module Example2 =
  type Result<'TData> =
    | Success of 'TData
    | Error of string

  type Request() =
    member this.tryGetParam str = None

  let queryCustomers count country city = []

  let invalidRequest = "Boooo! Parameters are not present!"

  let doWebRequest (req:Request) =
    maybe {
      let! strCount = req.tryGetParam "count"
      let! city     = req.tryGetParam "city"
      let! country  = req.tryGetParam "country"
      let! count    = Int32.tryParse strCount
      return queryCustomers count country city
    }
    |> Option.map Success
    |> Option.getOrElse (invalidRequest |> Error)

module Example3 =
  open Example2

  let findCustomers count city country = []

  let doWebRequest (req:Request) =
    let city    = req.tryGetParam "city"
    let country = req.tryGetParam "country"
    let count   = req.tryGetParam "count" >>= Int32.tryParse

    findCustomers <!> count <*> city <*> country
    |> Option.map Success
    |> Option.getOrElse (invalidRequest |> Error)
