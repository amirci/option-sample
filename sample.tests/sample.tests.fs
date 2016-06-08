namespace sample.tests

open System
open Xunit
open Xunit.Extensions
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit

open Fare
open FSharpx
open FSharpx.Functional

[<AutoOpen>]
module Regex =
  let matching pattern =
    Gen.sized (fun size ->
      let xeger = Xeger pattern
      let count = if size < 1 then 1 else size
      [ for i in 1..count -> xeger.Generate() ]
      |> Gen.elements
      |> Gen.resize count)

  type NotParseable = NotParseable of string with
    member x.Get = match x with NotParseable r -> r
    override x.ToString() = x.Get

  type Generator =
    static member NotParseable() =
      matching "[a-zA-Z]*"
      |> Gen.map NotParseable
      |> Arb.fromGen


module ``Option get`` =
  [<Property>]
  let ``Does not throw an exception with Some`` (actual: NonEmptyString) =
    actual.Get |> Some |> Option.get |> (=) actual.Get

  [<Property(Arbitrary=[|typeof<Generator>|])>]
  let ``Throws an exception with None`` (actual:NotParseable) =
    raises<exn> <@ actual.Get |> Int32.parse |> Option.get @>


module ``Option getOrElse`` =
  [<Property>]
  let ``Returns the actual value with Some`` (actual: int) =
    actual |> Some |> Option.getOrElse -1 |> (=) actual

  [<Property(Arbitrary=[|typeof<Generator>|])>]
  let ``Returns the default value with None`` (actual: NotParseable) =
    actual.Get |> Int32.parse |> Option.getOrElse -1 |> (=) -1


module ``Option map`` =

  [<Property>]
  let ``maps the function when is Some`` (actual: NonEmptyString) =
    actual.Get |> Some |> Option.map ((+) "Extra-") |> Option.get |> (=) ("Extra-" + actual.Get)
