# Playwright Test Sample (C#)

This document explains how to run the Playwright reCAPTCHA v2 token sample.

## Sample Location

- Code file: [DBC_Examples/PlaywrightRecaptchaV2Example.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/PlaywrightRecaptchaV2Example.cs)
- Project: [DBC_Examples/DBC_Examples.csproj](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/DBC_Examples.csproj)
- Startup object: `DeathByCaptcha.PlaywrightRecaptchaV2Example`

## What It Does

The sample:

1. Opens a page with reCAPTCHA v2 (default: Google demo page).
2. Reads the site key (`data-sitekey`) from the page.
3. Sends a token request (`type = 4`) to DeathByCaptcha.
4. Injects the solved token into `g-recaptcha-response`.
5. Submits the form and checks for a success message.

## Prerequisites

- A DeathByCaptcha account with balance.
- .NET SDK that supports this repo targets (`net6.0`, `net8.0`, `net10.0`).
- Playwright browsers installed:
  ```bash
  pwsh -Command "& {dotnet tool install --global Microsoft.Playwright.CLI; playwright install}"
  ```

## Credentials

Set credentials with environment variables:

```bash
export DBC_USERNAME='your_username'
export DBC_PASSWORD='your_password'
```

## Headless Mode

Headless mode is enabled by default.

- Environment variable: `DBC_PLAYWRIGHT_HEADLESS=true|false`
- CLI flags:
	- `--headless` to force headless mode
	- `--headed` to force visible browser mode

CLI flags take precedence over the environment variable.

## Run

Default page URL:

```bash
dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 -p:ExamplesStartupObject=DeathByCaptcha.PlaywrightRecaptchaV2Example
```

Custom page URL:

```bash
dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 -p:ExamplesStartupObject=DeathByCaptcha.PlaywrightRecaptchaV2Example -- https://www.google.com/recaptcha/api2/demo
```

Headed mode example:

```bash
dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 -p:ExamplesStartupObject=DeathByCaptcha.PlaywrightRecaptchaV2Example -- --headed https://www.google.com/recaptcha/api2/demo
```

## Notes

- This is a functional sample intended for test and integration scenarios.
- Real target pages may require additional anti-bot handling beyond this minimal flow.
- Use `-p:ExamplesStartupObject=...` with `dotnet run` to select a sample entry point.
