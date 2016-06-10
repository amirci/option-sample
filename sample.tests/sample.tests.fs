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
  // matching function taken from http://blog.nikosbaxevanis.com/2015/09/25/regex-constrained-strings-with-fscheck/
  let matching pattern =
    Gen.sized (fun size ->
      let xeger = Xeger pattern
      let count = if size < 1 then 1 else size
      [ for i in 1..count -> xeger.Generate() ]
      |> Gen.elements
      |> Gen.resize count)

  let genPattern pattern fn =
    matching pattern
    |> Gen.map fn
    |> Arb.fromGen

  type NotParseable = NotParseable of string with
    member x.Get = match x with NotParseable r -> r
    override x.ToString() = x.Get

  type Generator =
    static member NotParseable() = 
      genPattern "[a-zA-Z]*" NotParseable


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
  let ``applies the function when is Some`` (actual: NonEmptyString) =
    actual.Get |> Some |> Option.map ((+) "Extra-") |> Option.get |> (=) ("Extra-" + actual.Get)

  [<Fact>]
  let ``returns None when is None`` () =
    None |> Option.map ((+) "Extra-") |> Option.isNone


module ``Option bind`` = 
  [<Property(Arbitrary=[|typeof<Generator>|])>]
  let ``When is Some applies the fn and returns an Option`` (num: int) =
    num.ToString() |> Some |> Option.bind Int32.parse |> Option.get |> (=) num

  [<Property(Arbitrary=[|typeof<Generator>|])>]
  let ``When is None returns None`` (actual: NotParseable) =
    actual.Get |> Some |> Option.bind Int32.parse |> Option.isNone


module ``Using maybe builder`` =
  open sample.Requests
  open sample.Example2

  let isInvalid = mkRequest >> doWebRequest >> Result.errorMsg >> (=) invalidRequest

  [<Fact>]
  let ``a request without "count" returns invalid`` () =
    <@ [ "city", "Madrid";"country", "Spain" ] |> isInvalid @>

  [<Fact>]
  let ``a request without "city" returns invalid`` () =
    <@ [ "count", "3";"country", "Spain" ] |> isInvalid @>

  [<Fact>]
  let ``a request without "country" returns invalid`` () =
    <@ [ "count", "3"; "city" , "Madrid" ] |> isInvalid @>


  [<Fact>]
  let ``a request that can not parse the count returns invalid`` () =
    <@ [ "count", "a3"; "country", "Spain";"city" , "Madrid" ] |> isInvalid @>


  [<Fact>]
  let ``a request with valid parameters returns Succes for two customers`` () =
    <@ 
      [ "count", "3"; "country", "Spain";"city" , "Madrid" ] 
      |> mkRequest
      |> doWebRequest
      |> Result.isSuccessful
    @>
    
module ``Using applicative style`` =
  open sample.Requests
  open sample.Example3

  let isInvalid = mkRequest >> doWebRequest >> Result.errorMsg >> (=) invalidRequest

  [<Fact>]
  let ``a request without "count" returns invalid`` () =
    <@ [ "city", "Madrid";"country", "Spain" ] |> isInvalid @>

  [<Fact>]
  let ``a request without "city" returns invalid`` () =
    <@ [ "count", "3";"country", "Spain" ] |> isInvalid @>

  [<Fact>]
  let ``a request without "country" returns invalid`` () =
    <@ [ "count", "3"; "city" , "Madrid" ] |> isInvalid @>


  [<Fact>]
  let ``a request that can not parse the count returns invalid`` () =
    <@ [ "count", "a3"; "country", "Spain";"city" , "Madrid" ] |> isInvalid @>


  [<Fact>]
  let ``a request with valid parameters returns Succes for two customers`` () =
    <@ 
      [ "count", "3"; "country", "Spain";"city" , "Madrid" ] 
      |> mkRequest
      |> doWebRequest
      |> Result.isSuccessful
    @>
    
    
