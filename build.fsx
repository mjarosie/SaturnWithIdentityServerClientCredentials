#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open System.Threading

let apiPath = "./src/Api/" |> Path.getFullName
let apiProjectPath = Path.combine apiPath "Api.fsproj"

let identityServerPath = "./src/IdentityServer/" |> Path.getFullName
let identityServerProjectPath = Path.combine identityServerPath "IdentityServer.csproj"


Target.create "Clean" ignore

Target.create "Restore" (fun _ ->
    DotNet.restore id apiProjectPath
    DotNet.restore id identityServerProjectPath
)

Target.create "Build" (fun _ ->
    DotNet.build id apiProjectPath
    DotNet.build id identityServerProjectPath
)


Target.create "Run" (fun _ ->
  let apiServer = async {
    DotNet.exec (fun p -> { p with WorkingDirectory = apiPath } ) "watch" "run" |> ignore
  }
  let identityServer = async {
    DotNet.exec (fun p -> { p with WorkingDirectory = identityServerPath } ) "watch" "run" |> ignore
  }

  [ apiServer; identityServer]
  |> Async.Parallel
  |> Async.RunSynchronously
  |> ignore
)

"Clean"
  ==> "Restore"
  ==> "Build"

"Clean"
  ==> "Restore"
  ==> "Run"

Target.runOrDefault "Build"