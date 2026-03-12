# [DeathByCaptcha](https://deathbycaptcha.com/)

[![Unit Tests net10](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/unit-tests-net10.yml/badge.svg?branch=master)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/unit-tests-net10.yml)
[![Coverage net10](https://img.shields.io/endpoint?url=https://raw.githubusercontent.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/badges/.github/badges/coverage-net10.json)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/coverage.yml)

## Documentation Index

1. [Introduction](#introduction)
2. [Modern .NET Workspace](#modern-net-workspace)
3. [How to Use DBC API Clients](#how-to-use-dbc-api-clients)
4. [New Recaptcha API support](#new-recaptcha-api-support)
5. [New Recaptcha by Token API support (reCAPTCHA v2 and reCAPTCHA v3)](#new-recaptcha-by-token-api-support-recaptcha-v2-and-recaptcha-v3)
6. [Running Tests](#running-tests)
7. [CI Status Badges](#ci-status-badges)
8. [Changelog](CHANGELOG.md)
9. [Selenium Test Sample](SELENIUM_TESTS.md)

## Introduction

DeathByCaptcha offers APIs of two types — HTTP and socket-based, with the latter being recommended for having faster responses and overall better performance. Switching between different APIs is usually as easy as changing the client class and/or package name, the interface stays the same.

Current release: `4.7.0`. See [CHANGELOG.md](CHANGELOG.md) for release details.

When using the socket API, please make sure that outgoing TCP traffic to *api.dbcapi.me* to the ports range *8123–8130* is not blocked on your side.

## Modern .NET Workspace

This repository already includes SDK-style .NET projects for the library and examples.
You do not need to create a new console project to use the included samples.

### Projects

- Main library: `DeathByCaptcha/DeathByCaptcha/DeathByCaptcha.csproj`
- Internal JSON dependency: `DeathByCaptcha/SimpleJson/SimpleJson.csproj`
- C# examples: `DBC_Examples/DBC_Examples.csproj`
- VB examples: `DBC_Examples_VB/DBC_Examples_VB.vbproj`

### Target frameworks

- `net6.0`
- `net8.0`
- `net10.0`

### Recommended target

Prefer `net10.0` for regular development.
Use `net8.0` when you need broader environment compatibility.
Use `net6.0` only when compatibility with existing consumers is required.

### Notes

- The examples projects already contain a valid `StartupObject`.
- The examples use `ProjectReference`, so the correct version of the library is resolved automatically.
- Before running any example, replace placeholder credentials and CAPTCHA parameters with valid values.
- Convention: use `dotnet` CLI commands directly as the primary workflow.
- Make is optional and only provided as a convenience wrapper (`Makefile-dotnet`).

### Typical workflow

1. Build the main library project.
2. Build the examples project you want to use.
3. Update the `StartupObject` if you want to launch a different example class.
4. Run the selected project with the framework you need.

### Canonical dotnet CLI commands

- Restore: `dotnet restore dbc_api_net.sln`
- Build all: `dotnet build dbc_api_net.sln -c Release`
- Build library only: `dotnet build DeathByCaptcha/DeathByCaptcha/DeathByCaptcha.csproj -c Release -f net10.0`
- Run C# examples with explicit startup class:
    `dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 /p:StartupObject=DeathByCaptcha.ExampleSimple /t:Rebuild`
- Run VB examples with explicit startup class:
    `dotnet run --project DBC_Examples_VB/DBC_Examples_VB.vbproj -c Release -f net10.0 /p:StartupObject=DBC_Examples_VB.ExampleSimple /t:Rebuild`
- Run tests: `dotnet test DeathByCaptcha/DeathByCaptcha.Tests/DeathByCaptcha.Tests.csproj -c Release`
- Run Selenium sample:
    `dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 /p:StartupObject=DeathByCaptcha.SeleniumRecaptchaV2Example /t:Rebuild`

Use `/p:StartupObject=...` with `dotnet run` (not `-p:...`) to avoid conflict with `-p/--project`.
If you are switching between different startup classes, include `/t:Rebuild` (or run `dotnet clean`) to avoid stale up-to-date outputs.

### Optional Make wrapper

- `make -f Makefile-dotnet build`
- `make -f Makefile-dotnet test`
- `make -f Makefile-dotnet run-cs CS_STARTUP=DeathByCaptcha.RecaptchaV2Example`
- `make -f Makefile-dotnet run-vb VB_STARTUP=DBC_Examples_VB.RecaptchaV2`

## How to Use DBC API Clients

### Thread-safety notes

*.NET* client  are thread-safe, means it is perfectly fine to share a client between multiple threads (although in a heavily multithreaded applications it is a better idea to keep a pool of clients).

### Common Clients' Interface

All the clients have to be instantiated with two string arguments: your DeathByCaptcha account's *username* and *password*.

All the clients provide a few methods to handle your CAPTCHAs and your DBC account. Below you will find those methods' short summary summary and signatures in pseudo-code. Check the example scripts and the clients' source code for more details.

#### Upload()

Uploads a CAPTCHA to the DBC service for solving, returns uploaded CAPTCHA details on success, `NULL` otherwise. Here are the signatures in pseudo-code:

```c#
DeathByCaptcha.Captcha DeathByCaptcha.Client.Upload(byte[] imageData)

DeathByCaptcha.Captcha DeathByCaptcha.Client.Upload(Stream imageStream)

DeathByCaptcha.Captcha DeathByCaptcha.Client.Upload(string imageFileName)
```

#### GetCaptcha()

Fetches uploaded CAPTCHA details, returns `NULL` on failures.

```c#  
DeathByCaptcha.Captcha DeathByCaptcha.Client.GetCaptcha(int captchaId)

DeathByCaptcha.Captcha DeathByCaptcha.Client.GetCaptcha(DeathByCaptcha.Captcha captcha)
```

#### Report()

Reports incorrectly solved CAPTCHA for refund, returns `true` on success, `false` otherwise.

Please make sure the CAPTCHA you're reporting was in fact incorrectly solved, do not just report them thoughtlessly, or else you'll be flagged as abuser and banned.
  
```c#
bool DeathByCaptcha.Client.Report(int captchaId)

bool DeathByCaptcha.Client.Report(DeathByCaptcha.Captcha captcha)
```

#### Decode()

This method uploads a CAPTCHA, then polls for its status until it's solved or times out; returns solved CAPTCHA details on success, `NULL` otherwise.

```.c#
DeathByCaptcha.Captcha DeathByCaptcha.Client.Decode(byte[] imageData, int timeout)

DeathByCaptcha.Captcha DeathByCaptcha.Client.Decode(Stream imageStream, int timeout)

DeathByCaptcha.Captcha DeathByCaptcha.Client.Decode(string imageFileName, int timeout)

DeathByCaptcha.Captcha DeathByCaptcha.Client.Decode(int timeout, Hashtable ext_data)
```  

#### GetBalance()

Fetches your current DBC credit balance (in US cents).
  
```c#
double DeathByCaptcha.Client.GetBalance()
```


### CAPTCHA objects/details hashes

*.NET* client wrap CAPTCHA details in `DeathByCaptcha.Captcha`, exposing CAPTCHA details through the following properties and methods:

-   CAPTCHA numeric ID as integer `Id` property.
-   CAPTCHA text as string `Text` property.
-   a flag showing whether the CAPTCHA was uploaded, as boolean `Uploaded` property.
-   a flag showing whether the CAPTCHA was solved, as boolean `Solved` property.
-   a flag showing whether the CAPTCHA was solved correctly, as boolean `Correct` property.

### Examples

Below you can find a DBC API client usage example.

```c#
using DeathByCaptcha;

/* Put your DeathByCaptcha account username and password here.
   Use HttpClient for HTTP API. */
Client client = (Client)new SocketClient(username, password);
try {
    double balance = client.GetBalance();

    /* Put your CAPTCHA file name, or file object, or arbitrary stream,
       or an array of bytes, and optional solving timeout (in seconds) here: */
    Captcha captcha = client.Decode(captchaFileName, timeout);
    if (null != captcha) {
        /* The CAPTCHA was solved; captcha.Id property holds its numeric ID,
           and captcha.Text holds its text. */
        Console.WriteLine("CAPTCHA {0} solved: {1}", captcha.Id, captcha.Text);

        if (/* check if the CAPTCHA was incorrectly solved */) {
            client.Report(captcha);
        }
    }
} catch (AccessDeniedException e) {
    /* Access to DBC API denied, check your credentials and/or balance */
}
```

# New Recaptcha API support

## What's "new reCAPTCHA/noCAPTCHA"?

They're new reCAPTCHA challenges that typically require the user to identify and click on certain images. They're not to be confused with traditional word/number reCAPTCHAs (those have no images).

For your convinience, we implemented support for New Recaptcha API. If your software works with it, and supports minimal configuration, you should be able to decode captchas using New Recaptcha API in no time.

We provide two different types of New Recaptcha API:

-   **Coordinates API**: Provided a screenshot, the API returns a group of coordinates to click.
-   **Image Group API**: Provided a group of (base64-encoded) images, the API returns the indexes of the images to click.

## Coordinates API FAQ:

**What's the Coordinates API URL?**  
To use the **Coordinates API** you will have to send a HTTP POST Request to <http://api.dbcapi.me/api/captcha>

What are the POST parameters for the **Coordinates API**?  

-   **`username`**: Your DBC account username
-   **`password`**: Your DBC account password
-   **`captchafile`**: a Base64 encoded or Multipart file contents with a valid New Recaptcha screenshot
-   **`type`=2**: Type 2 specifies this is a New Recaptcha **Coordinates API**

**What's the response from the Coordinates API?**  
-   **`captcha`**: id of the provided captcha, if the **text** field is null, you will have to pool the url <http://api.dbcapi.me/api/captcha/captcha_id> until it becomes available

-   **`is_correct`**: (0 or 1) specifying if the captcha was marked as incorrect or unreadable

-   **`text`**: a json-like nested list, with all the coordinates (x, y) to click relative to the image, for example:

                  [[23.21, 82.11]]
              
    where the X coordinate is 23.21 and the Y coordinate is 82.11

****

## Image Group API FAQ:

**What's the Image Group API URL?**  
To use the **Image Group API** you will have to send a HTTP POST Request to <http://api.dbcapi.me/api/captcha>

**What are the POST parameters for the Image Group API?** 

-   **`username`**: Your DBC account username
-   **`password`**: Your DBC account password
-   **`captchafile`**: the Base64 encoded file contents with a valid New Recaptcha screenshot. You must send each image in a single "captchafile" parameter. The order you send them matters
-   **`banner`**: the Base64 encoded banner image (the example image that appears on the upper right)
-   **`banner_text`**: the banner text (the text that appears on the upper left)
-   **`type`=3**: Type 3 specifies this is a New Recaptcha **Image Group API**
-   **`grid`**: Optional grid parameter specifies what grid individual images in captcha are aligned to (string, width+"x"+height, Ex.: "2x4", if images aligned to 4 rows with 2 images in each. If not supplied, dbc will attempt to autodetect the grid.

**What's the response from the Image Group API?**  
-   **`captcha`**: id of the provided captcha, if the **`text`** field is null, you will have to pool the url <http://api.dbcapi.me/api/captcha/captcha_id> until it becomes available

-   **`is_correct`**: (0 or 1) specifying if the captcha was marked as incorrect or unreadable

-   **`text`**: a json-like list of the index for each image that should be clicked. for example:

                  [1, 4, 6]
              
    where the images that should be clicked are the first, the fourth and the six, counting from left to right and up to bottom


# New Recaptcha by Token API support (reCAPTCHA v2 and reCAPTCHA v3)


## What's "new reCAPTCHA by Token"?


They're new reCAPTCHA challenges that typically require the user to identify and click on certain images. They're not to be confused with traditional word/number reCAPTCHAs (those have no images).

For your convenience, we implemented support for New Recaptcha by Token API. If your software works with it, and supports minimal configuration, you should be able to decode captchas using Death By Captcha in no time.

-   **Token Image API**: Provided a site url and site key, the API returns a token that you will use to submit the form in the page with the reCaptcha challenge.

## reCAPTCHA v2 API FAQ:

**What's the Token Image API URL?**   
To use the Token Image API you will have to send a HTTP POST Request to <http://api.dbcapi.me/api/captcha>

**What are the POST parameters for the Token image API?**

-   **`username`**: Your DBC account username
-   **`password`**: Your DBC account password
-   **`type`=4**: Type 4 specifies this is a New Recaptcha Token Image API
-   **`token_params`=json(payload)**: the data to access the recaptcha challenge
json payload structure:
    -   **`proxy`**: your proxy url and credentials (if any). Examples:
        -   <http://127.0.0.1:3128>
        -   <http://user:password@127.0.0.1:3128>
    
    -   **`proxytype`**: your proxy connection protocol. For supported proxy types refer to Which proxy types are supported?. Example:
        -   HTTP
    
    -   **`googlekey`**: the google recaptcha site key of the website with the recaptcha. For more details about the site key refer to What is a recaptcha site key?. Example:
        -   6Le-wvkSAAAAAPBMRTvw0Q4Muexq9bi0DJwx_mJ-
    
    -   **`pageurl`**: the url of the page with the recaptcha challenges. This url has to include the path in which the recaptcha is loaded. Example: if the recaptcha you want to solve is in <http://test.com/path1>, pageurl has to be <http://test.com/path1> and not <http://test.com>.

    -   **`data-s`**: This parameter is only required for solve the google search tokens, the ones visible, while google search trigger the robot protection. Use the data-s value inside the google search response html. For regulars tokens don't use this parameter.
    
The **`proxy`** parameter is optional, but we strongly recommend to use one to prevent token rejection by the provided page due to inconsistencies between the IP that solved the captcha (ours if no proxy is provided) and the IP that submitted the token for verification (yours).

**Note**: If **`proxy`** is provided, **`proxytype`** is a required parameter.

Full example of **`token_params`**:
```json
{
  "proxy": "http://127.0.0.1:3128",
  "proxytype": "HTTP",
  "googlekey": "6Le-wvkSAAAAAPBMRTvw0Q4Muexq9bi0DJwx_mJ-",
  "pageurl": "http://test.com/path_with_recaptcha"
}
```

Example of **`token_params`** for google search captchas:
```json
{
  "googlekey": "6Le-wvkSA...",
  "pageurl": "...",
  "data-s": "IUdfh4rh0sd..."
}
```

**What's the response from the Token image API?**  
The token image API response has the same structure as regular captchas' response. Refer to Polling for uploaded CAPTCHA status for details about the response. The token will come in the text key of the response. It's valid for one use and has a 2 minute lifespan. It will be a string like the following:
```bash
"03AOPBWq_RPO2vLzyk0h8gH0cA2X4v3tpYCPZR6Y4yxKy1s3Eo7CHZRQntxrdsaD2H0e6S3547xi1FlqJB4rob46J0-wfZMj6YpyVa0WGCfpWzBWcLn7tO_EYsvEC_3kfLNINWa5LnKrnJTDXTOz-JuCKvEXx0EQqzb0OU4z2np4uyu79lc_NdvL0IRFc3Cslu6UFV04CIfqXJBWCE5MY0Ag918r14b43ZdpwHSaVVrUqzCQMCybcGq0yxLQf9eSexFiAWmcWLI5nVNA81meTXhQlyCn5bbbI2IMSEErDqceZjf1mX3M67BhIb4"
```

## What's "new reCAPTCHA v3"?

This API is quite similar to the tokens(reCAPTCHA v2) API. Only 2 new parameters were added, one for the `action` and other for the **minimal score(`min-score`)**

reCAPTCHA v3 returns a score from each user, that evaluate if user is a bot or human. Then the website uses the score value that could range from 0 to 1 to decide if will accept or not the requests. Lower scores near to 0 are identified as bot.

The `action` parameter at reCAPTCHA v3 is an additional data used to separate different captcha validations like for example **login**, **register**, **sales**, **etc**.

## reCAPTCHA v3 API FAQ:

**What is `action` in reCAPTCHA v3?**  
Is a new parameter that allows processing user actions on the website differently.

To find this we need to inspect the javascript code of the website looking for call of grecaptcha.execute function. Example: 
```javascript
grecaptcha.execute('6Lc2fhwTAAAAAGatXTzFYfvlQMI2T7B6ji8UVV_f', {action: something})
```
Sometimes it's really hard to find it and we need to look through all javascript files. We may also try to find the value of action parameter inside ___grecaptcha_cfg configuration object. Also we can call grecaptcha.execute and inspect javascript code. The API will use "verify" default value it if we won't provide action in our request.

**What is `min-score` in reCAPTCHA v3 API?**  
The minimal score needed for the captcha resolution. We recommend using the 0.3 min-score value, scores highers than 0.3 are hard to get.

**What are the POST parameters for the reCAPTCHA v3 API?**

-   **`username`**: Your DBC account username
-   **`password`**: Your DBC account password
-   **`type`=5**: Type 5 specifies this is reCAPTCHA v3 API
-   **`token_params`**=json(payload): the data to access the recaptcha challenge
json payload structure:
    -   **`proxy`**: your proxy url and credentials (if any).Examples:
        -   <http://127.0.0.1:3128>
        -   <http://user:password@127.0.0.1:3128>
    
    -   **`proxytype`**: your proxy connection protocol. For supported proxy types refer to Which proxy types are supported?. Example: 
        -   HTTP
    
    -   **`googlekey`**: the google recaptcha site key of the website with the recaptcha. For more details about the site key refer to What is a recaptcha site key?. Example:
        -   6Le-wvkSAAAAAPBMRTvw0Q4Muexq9bi0DJwx_mJ-
    
    -   **`pageurl`**: the url of the page with the recaptcha challenges. This url has to include the path in which the recaptcha is loaded. Example: if the recaptcha you want to solve is in <http://test.com/path1>, pageurl has to be <http://test.com/path1> and not <http://test.com>.
    
    -   **`action`**: The action name.
    
    -   **`min_score`**: The minimal score, usually 0.3
    
The **`proxy`** parameter is optional, but we strongly recommend to use one to prevent rejection by the provided page due to inconsistencies between the IP that solved the captcha (ours if no proxy is provided) and the IP that submitted the solution for verification (yours).

**Note**: If **`proxy`** is provided, **`proxytype`** is a required parameter.

Full example of **`token_params`**:
```json
{
  "proxy": "http://127.0.0.1:3128",
  "proxytype": "HTTP",
  "googlekey": "6Le-wvkSAAAAAPBMRTvw0Q4Muexq9bi0DJwx_mJ-",
  "pageurl": "http://test.com/path_with_recaptcha",
  "action": "example/action",
  "min_score": 0.3
}
```

**What's the response from reCAPTCHA v3 API?**  
The response has the same structure as regular captcha. Refer to [Polling for uploaded CAPTCHA status](https://deathbycaptcha.com/api#polling-captcha) for details about the response. The solution will come in the **text** key of the response. It's valid for one use and has a 1 minute lifespan.

## Running Tests

### Unit tests

Run only unit tests:

```bash
dotnet test DeathByCaptcha/DeathByCaptcha.Tests/DeathByCaptcha.Tests.csproj -c Release --filter "Category!=Integration"
```

Run the whole local test suite:

```bash
dotnet test DeathByCaptcha/DeathByCaptcha.Tests/DeathByCaptcha.Tests.csproj -c Release
```

### Integration tests (real DBC API)

Integration tests call the live HTTP and Socket APIs and consume account balance.

1. Create a local `.env` file from the sample:

```bash
cp .env.sample .env
```

2. Set credentials in `.env`:

```dotenv
DBC_USERNAME='your_username'
DBC_PASSWORD='your_password'
DBC_INTEGRATION_FULL=false
```

3. Load variables and run integration tests:

```bash
set -a && source .env && set +a
dotnet test DeathByCaptcha/DeathByCaptcha.Tests/DeathByCaptcha.Tests.csproj -c Release --filter "Category=Integration"
```

If `DBC_USERNAME` and `DBC_PASSWORD` are not present, integration tests are skipped.

### Full API-surface integration tests

The two "full workflow" tests are disabled by default (slower and more costly).
Enable them only when needed:

```bash
set -a && source .env && set +a
export DBC_INTEGRATION_FULL=true
dotnet test DeathByCaptcha/DeathByCaptcha.Tests/DeathByCaptcha.Tests.csproj -c Release --filter "Http_FullApiSurface_BasicWorkflow|Socket_FullApiSurface_BasicWorkflow"
```

### CI (GitHub Actions)

Available workflows:

- `.github/workflows/unit-tests-net6.yml`
- `.github/workflows/unit-tests-net8.yml`
- `.github/workflows/unit-tests-net10.yml`
- `.github/workflows/integration-basic.yml`
- `.github/workflows/integration-selenium.yml`
- `.github/workflows/coverage.yml`

Set repository secrets `DBC_USERNAME` and `DBC_PASSWORD` before running the integration workflow.

The coverage badge is published from workflow `.github/workflows/coverage.yml` to the `badges` branch as a Shields endpoint payload.

## CI Status Badges

| Workflow | Status |
|---|---|
| Unit Tests net6 | [![Unit Tests net6](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/unit-tests-net6.yml/badge.svg?branch=master)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/unit-tests-net6.yml) |
| Unit Tests net8 | [![Unit Tests net8](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/unit-tests-net8.yml/badge.svg?branch=master)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/unit-tests-net8.yml) |
| Unit Tests net10 | [![Unit Tests net10](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/unit-tests-net10.yml/badge.svg?branch=master)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/unit-tests-net10.yml) |
| Integration Tests Basic | [![Integration Tests Basic](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/integration-basic.yml/badge.svg?branch=master)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/integration-basic.yml) |
| Integration Tests Selenium | [![Integration Tests Selenium](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/integration-selenium.yml/badge.svg?branch=master)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/integration-selenium.yml) |
| Coverage | [![Coverage net10](https://img.shields.io/endpoint?url=https://raw.githubusercontent.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/badges/.github/badges/coverage-net10.json)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/coverage.yml) |
