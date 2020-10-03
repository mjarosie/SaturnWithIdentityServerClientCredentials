module Server

open Saturn
open Microsoft.IdentityModel.Tokens
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.Extensions.DependencyInjection;

let endpointPipe = pipeline {
    plug head
    plug requestId
}

let app = application {
    use_router Router.appRouter
    url "https://0.0.0.0:8085/"
    use_jwt_authentication_with_config (fun (opt: JwtBearerOptions) ->
        opt.Authority <- "https://localhost:5001"

        let tvp = TokenValidationParameters()
        tvp.ValidateAudience <- false

        opt.TokenValidationParameters <- tvp
    )
    use_policy "ApiScope" (fun context ->
        context.User <> null && 
        context.User.HasClaim(fun claim ->
            (claim.Type = "scope") &&
            (claim.Value = "api1") && 
            (claim.Issuer = "https://localhost:5001"))
    )
}

[<EntryPoint>]
let main _ =
    printfn "Working directory - %s" (System.IO.Directory.GetCurrentDirectory())
    run app
    0 // return an integer exit code