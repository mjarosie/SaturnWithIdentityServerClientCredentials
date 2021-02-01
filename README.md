### How to build this application

1. Make sure you have installed the version of .Net SDK defined in `global.json`
1. Trust the ASP.NET Core HTTPS development certificate by invoking `dotnet dev-certs https --trust` (see [the documentation](https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl#trust-the-aspnet-core-https-development-certificate-on-windows-and-macos), you might have to do something else if you're not running Windows/macOS)
1. Run `dotnet tool restore` to restore all necessary tools
1. Run `dotnet fake build -t Run` to start the application in watch mode (automatic recompilation and restart at file save)

### How to test the Client Credentials flow

You can do it from your command line by running `cURL` commands:

```powershell
# Send a request to the public, unprotected endpoint:
curl --request GET --url 'https://localhost:8085/api/hello/'

# Send a request to the protected endpoint without attaching the access token (in Authorization header).
# You should get 401 Unauthorized:
curl --request GET --url 'https://localhost:8085/api/hello/test'

# Retrieve an access token:
curl --request POST --url 'https://localhost:5001/connect/token' `
--header 'content-type: application/x-www-form-urlencoded' `
--data 'grant_type=client_credentials&client_id=client&client_secret=secret&scope=api1'

# Send a request to the protected endpoint with the access token attached:
curl --request GET --url 'https://localhost:8085/api/hello/test' `
--header 'Authorization: Bearer <paste the access token here>'
```

You can make your life easier by extracting the access token with [jq](https://stedolan.github.io/jq/) into a variable:

Powershell:

```powershell
# Retrieve an access token and store it into a variable:
$access_token = curl --request POST --url 'https://localhost:5001/connect/token' `
--header 'content-type: application/x-www-form-urlencoded' `
--data 'grant_type=client_credentials&client_id=client&client_secret=secret&scope=api1' `
| jq .access_token

# Send a request to the protected endpoint with the access token attached:
curl --request GET --url 'https://localhost:8085/api/hello/test' `
--header "Authorization: Bearer $access_token"
```

Bash:

```bash
# Retrieve an access token and store it into a variable:
access_token=$(curl --request POST --url 'https://localhost:5001/connect/token' \
--header 'content-type: application/x-www-form-urlencoded' \
--data 'grant_type=client_credentials&client_id=client&client_secret=secret&scope=api1' \
| jq .access_token)

# Send a request to the protected endpoint with the access token attached:
curl --request GET --url 'https://localhost:8085/api/hello/test' \
--header "Authorization: Bearer ${access_token}"
```