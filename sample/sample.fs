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

module Requests =
  type Result<'TData> =
    | Success of 'TData
    | Error of string

  type Request = Map<string, string>

  module Result =
    let errorMsg = 
      function
      | Success _ -> failwith "Can't get error from Success"
      | Error s -> s

    let isSuccessful =
      function
      | Success _ -> true
      | _ -> false

  let mkRequest = Map.ofList

  let tryParam = Map.tryFind

  let invalidRequest = "Boooo! Parameters are not present!"


module Example2 =
  open Requests

  let queryCustomers count country city = ["Customer1"; "Customer2"]

  let doWebRequest (req:Request) =
    maybe {
      let! strCount = req |> tryParam "count"
      let! city     = req |> tryParam "city"
      let! country  = req |> tryParam "country"
      let! count    = Int32.tryParse strCount
      return queryCustomers count country city
    }
    |> Option.map Success
    |> Option.getOrElse (invalidRequest |> Error)

module Example3 =
  open Requests
  open Example2

  let findCustomers count city country = ["Customer1"; "Customer2"]

  let doWebRequest (req:Request) =
    let city    = req |> tryParam "city"
    let country = req |> tryParam "country"
    let count   = req |> tryParam "count" >>= Int32.tryParse

    findCustomers <!> count <*> city <*> country
    |> Option.map Success
    |> Option.getOrElse (invalidRequest |> Error)
