module Router

open Saturn

let apiRouter = router {
    forward "/hello" Hello.Controller.helloRouter
}

let appRouter = router {
    forward "/api" apiRouter
}