// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing.XUnit2

// Directories
let buildDir  = "./build/"

// Filesets
let appReferences  =
    !! "/**/*.csproj"
      ++ "/**/*.fsproj"

// version info
let version = "0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir]
)

Target "Build" (fun _ ->
    // compile all projects below
    appReferences
    |> MSBuildDebug buildDir "Build"
    |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
  let testDir = buildDir
  !! (testDir @@ "*tests*.dll")
    |> xUnit2 (fun p -> { p with HtmlOutputPath = Some (testDir @@ "xunit.html") })
)



// Build order
"Clean"
  ==> "Build"
  ==> "Test"

// start build
RunTargetOrDefault "Test"
