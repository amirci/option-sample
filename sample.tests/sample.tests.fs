namespace sample.tests

open System
open Xunit
open Xunit.Extensions
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit

open FSharpx
open FSharpx.Functional

module ``Option get`` =

  [<Fact>]
  let ``Does not throw an exception with Some`` () =
    <@ "abc"|> Some |> Option.get |> (=) "abc" @>

  [<Fact>]
  let ``Throws an exception with None`` () =
    raises<exn> <@ "abc" |> Int32.parse |> Option.get @>


module ``Option getOrElse`` =

  [<Fact>]
  let ``Returns the actual value with Some`` () =
    <@
      "22"
      |> Int32.parse
      |> Option.getOrElse -1
      |> (=) 22
    @>

  [<Fact>]
  let ``Returns the default value with None`` () =
    <@
      "abc"
      |> Int32.parse
      |> Option.getOrElse -1
      |> (=) -1
    @>
