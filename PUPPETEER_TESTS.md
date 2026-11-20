# Puppeteer Test Sample (C#)

This document explains how to run the PuppeteerSharp reCAPTCHA v2 sample.

## Sample location

- Code: `DBC_Examples/PuppeteerRecaptchaV2Example.cs`
- Startup object: `DeathByCaptcha.PuppeteerRecaptchaV2Example`
- Integration test: `DeathByCaptcha/DeathByCaptcha.Tests/PuppeteerIntegrationTests.cs`

## What it does

1. Opens a page with reCAPTCHA v2 (default: Google demo).
2. Reads the site key from `data-sitekey`.
3. Requests a `type = 4` token from DeathByCaptcha.
4. Injects it into `g-recaptcha-response` and calls the widget callback.
5. Submits the form and checks for the success message.

The sample never prints the token.

## Prerequisites

- .NET SDK for the repository targets (`net6.0`, `net8.0`, `net10.0`).
- Chrome: `CHROME_BIN` to reuse an installed browser, or let PuppeteerSharp
  download Chrome for Testing on the first run.
- A DeathByCaptcha account with balance.

## Credentials

Set the variables in the environment or in a `.env` file at the repository
root:

```dotenv
DBC_USERNAME=your_username
DBC_PASSWORD=your_password
```

## Run

```bash
dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 \
  -p:ExamplesStartupObject=DeathByCaptcha.PuppeteerRecaptchaV2Example
```

`--headed` shows the browser; `--headless` is the default. A URL argument
replaces the demo page.

## Test

```bash
dotnet test DeathByCaptcha/DeathByCaptcha.Tests/DeathByCaptcha.Tests.csproj \
  -c Release -f net10.0 --filter "Category=Puppeteer"
```

The test is skipped when `DBC_USERNAME`/`DBC_PASSWORD` are missing.
