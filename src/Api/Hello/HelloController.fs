namespace Hello

open Saturn
open Giraffe.ResponseWriters
open Giraffe

module Controller =

    let authFailedHandler: HttpHandler =
        // You'd probably want to respond with more structured message, perhaps JSON.
        setStatusCode 401 >=> text "Unauthorized"
    
    let mustHaveApiScope: HttpHandler =
        // The `authorizeByPolicyName` function takes two parameters - the name of the policy and
        // the handler which should be invoked when the policy check fails.
        authorizeByPolicyName "ApiScope" authFailedHandler 

    let helloProtectedPipeline: HttpHandler = pipeline {
        plug mustHaveApiScope
    }
    
    // Private handler, you're supposed to be authenticated and authorized to get the response.
    let indexProtectedAction (name: string): HttpHandler = 
        json ("Hello, " + name + "!")
    
    // Public handler, you can access it without being authenticated.
    let indexAction: HttpHandler = 
        json "Hello!"

    let helloProtectedRouter: HttpHandler = router {
        pipe_through helloProtectedPipeline
        
        getf "/%s" indexProtectedAction
    }

    let helloRouter: HttpHandler = router {
        get "/" indexAction // Requests to /hello/ go through the "public" handler.
        forward "" helloProtectedRouter // All other requests (/hello/*) go through the "protected" handler.
    }