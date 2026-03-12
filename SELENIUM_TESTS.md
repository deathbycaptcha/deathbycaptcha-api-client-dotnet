# Selenium Test Sample (C#)

This document explains how to run the Selenium reCAPTCHA v2 token sample migrated to the main C# samples project.

## Sample Location

- Code file: `DBC_Examples/SeleniumRecaptchaV2Example.cs`
- Project: `DBC_Examples/DBC_Examples.csproj`
- Startup object: `DeathByCaptcha.SeleniumRecaptchaV2Example`

## What It Does

The sample:

1. Opens a page with reCAPTCHA v2 (default: Google demo page).
2. Reads the site key (`data-sitekey`) from the page.
3. Sends a token request (`type = 4`) to DeathByCaptcha.
4. Injects the solved token into `g-recaptcha-response`.
5. Submits the form and checks for a success message.

## Prerequisites

- A DeathByCaptcha account with balance.
- Firefox installed.
- Geckodriver available in `PATH` (or default Selenium driver resolution working in your environment).
- .NET SDK that supports this repo targets (`net6.0`, `net8.0`, `net10.0`).

## Credentials

Set credentials with environment variables:

```bash
export DBC_USERNAME='your_username'
export DBC_PASSWORD='your_password'
```

You can also keep placeholders in code, but environment variables are recommended.

## Headless Mode

Headless mode is enabled by default.

- Environment variable: `DBC_SELENIUM_HEADLESS=true|false`
- CLI flags:
	- `--headless` to force headless mode
	- `--headed` to force visible browser mode

CLI flags take precedence over the environment variable.

## Run

Default page URL:

```bash
dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 /p:StartupObject=DeathByCaptcha.SeleniumRecaptchaV2Example /t:Rebuild
```

Custom page URL:

```bash
dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 /p:StartupObject=DeathByCaptcha.SeleniumRecaptchaV2Example /t:Rebuild -- https://www.google.com/recaptcha/api2/demo
```

Headed mode example:

```bash
dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 /p:StartupObject=DeathByCaptcha.SeleniumRecaptchaV2Example /t:Rebuild -- --headed https://www.google.com/recaptcha/api2/demo
```

## CI Integration Test

A dedicated GitHub Actions workflow runs Selenium integration tests:

- `.github/workflows/integration-selenium.yml`

It runs the test in headless mode and uses repository secrets `DBC_USERNAME` and `DBC_PASSWORD`.

## Notes

- This is a functional sample intended for test and integration scenarios.
- Real target pages may require additional anti-bot handling beyond this minimal flow.
- If browser startup fails, verify Firefox/geckodriver versions and `PATH`.
- Use `/p:StartupObject=...` with `dotnet run`. Do not use `-p:...` because `-p` is also used by `--project`.
- When switching between startup classes, use `/t:Rebuild` (or run `dotnet clean`) to avoid stale outputs.
