# C# and VB Samples Guide

This guide explains how to run the non-Selenium samples from:

- `DBC_Examples/` (C#)
- `DBC_Examples_VB/` (VB)

For Selenium, use the dedicated document: `SELENIUM_TESTS.md`.

## Prerequisites

- Set credentials in environment variables:

```bash
export DBC_USERNAME='your_username'
export DBC_PASSWORD='your_password'
```

- Use a supported framework target (`net6.0`, `net8.0`, `net10.0`).
- Replace placeholder CAPTCHA params inside each sample as needed.

## Generic Run Command

C# samples:

```bash
dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 -p:ExamplesStartupObject=DeathByCaptcha.<StartupClass>
```

VB samples:

```bash
dotnet run --project DBC_Examples_VB/DBC_Examples_VB.vbproj -c Release -f net10.0 -p:ExamplesStartupObject=DBC_Examples_VB.<StartupClass>
```

If you need a clean rebuild first:

```bash
dotnet build DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 -t:Rebuild -p:ExamplesStartupObject=DeathByCaptcha.<StartupClass>
dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 --no-build -p:ExamplesStartupObject=DeathByCaptcha.<StartupClass>
```

## What Is `captcha type`?

`captcha type` is the numeric code sent in the payload (usually `{"type", N}`) that tells DeathByCaptcha which solver flow to use.

- Example: `type = 4` means token-based reCAPTCHA v2.
- Example: `type = 12` means Cloudflare Turnstile token.
- Some classic image flows do not require an explicit type (normal image decode).

If `type` is wrong for the target challenge, solving will fail or return invalid results.

## C# Samples Map

Use startup object prefix `DeathByCaptcha.`

| Sample | File | StartupObject suffix | Captcha type |
|---|---|---|---|
| Basic image decode | `DBC_Examples/Examples.cs` | `ExampleSimple` | `none` (normal image) |
| Balance check only | `DBC_Examples/Examples.cs` | `GetBalance` | `n/a` |
| Generic token sample | `DBC_Examples/Examples.cs` | `ExampleToken` | `4` |
| Full upload/poll flow | `DBC_Examples/Examples.cs` | `ExampleFull` | `none` (normal image) |
| Async decode flow | `DBC_Examples/Examples.cs` | `ExampleAsync` | `none` (normal image) |
| Normal captcha | `DBC_Examples/NormalCaptchaExample.cs` | `NormalCaptchaExample` | `none` (normal image) |
| Audio captcha | `DBC_Examples/AudioExample.cs` | `AudioExample` | `13` |
| reCAPTCHA v2 token | `DBC_Examples/RecaptchaV2Example.cs` | `RecaptchaV2Example` | `4` |
| reCAPTCHA v3 token | `DBC_Examples/RecaptchaV3Example.cs` | `RecaptchaV3Example` | `5` |
| reCAPTCHA v2 Enterprise | `DBC_Examples/RecaptchaV2EnterpriseExample.cs` | `RecaptchaV2EnterpriseExample` | `25` |
| reCAPTCHA coordinates | `DBC_Examples/RecaptchaCoordinatesExample.cs` | `RecaptchaCoordinatesExample` | `2` |
| reCAPTCHA image group | `DBC_Examples/RecaptchaImageGroupExample.cs` | `RecaptchaImageGroupExample` | `3` |
| DataDome | `DBC_Examples/DatadomeExample.cs` | `DatadomeExample` | `21` |
| FriendlyCaptcha | `DBC_Examples/FriendlycaptchaExample.cs` | `FriendlycaptchaExample` | `20` |
| AtbCaptcha | `DBC_Examples/AtbExample.cs` | `AtbExample` | `24` |
| Capy puzzle | `DBC_Examples/CapyExample.cs` | `CapyExample` | `15` |
| Lemin | `DBC_Examples/LeminExample.cs` | `LeminExample` | `14` |
| MTCaptcha | `DBC_Examples/MtcaptchaExample.cs` | `MtcaptchaExample` | `18` |
| Tencent | `DBC_Examples/TencentExample.cs` | `TencentExample` | `23` |
| Cloudflare Turnstile | `DBC_Examples/TurnstileExample.cs` | `TurnstileExample` | `12` |
| Siara | `DBC_Examples/SiaraExample.cs` | `SiaraExample` | `17` |
| Amazon WAF | `DBC_Examples/AmazonWafExample.cs` | `AmazonWafExample` | `16` |
| CutCaptcha | `DBC_Examples/CutcaptchaExample.cs` | `CutcaptchaExample` | `19` |
| GeeTest v3 | `DBC_Examples/GeetestV3Example.cs` | `GeetestV3Example` | `8` |
| GeeTest v4 | `DBC_Examples/GeetestV4Example.cs` | `GeetestV4Example` | `9` |
| Text captcha | `DBC_Examples/TextcaptchaExample.cs` | `TextcaptchaExample` | `11` |

## VB Samples Map

Use startup object prefix `DBC_Examples_VB.`

| Sample | File | StartupObject suffix | Captcha type |
|---|---|---|---|
| Basic image decode | `DBC_Examples_VB/Examples.vb` | `ExampleSimple` | `none` (normal image) |
| Balance check only | `DBC_Examples_VB/Examples.vb` | `GetBalance` | `n/a` |
| Generic token sample | `DBC_Examples_VB/Examples.vb` | `ExampleToken` | `4` |
| Full upload/poll flow | `DBC_Examples_VB/Examples.vb` | `ExampleFull` | `none` (normal image) |
| Normal captcha | `DBC_Examples_VB/Normal_Captcha.vb` | `NormalCaptcha` | `none` (normal image) |
| Audio captcha | `DBC_Examples_VB/Audio.vb` | `Audio` | `13` |
| reCAPTCHA v2 token | `DBC_Examples_VB/RecaptchaV2.vb` | `RecaptchaV2` | `4` |
| reCAPTCHA v3 token | `DBC_Examples_VB/RecaptchaV3.vb` | `RecaptchaV3` | `5` |
| reCAPTCHA v2 Enterprise | `DBC_Examples_VB/RecaptchaV2Enterprise.vb` | `RecaptchaV2Enterprise` | `25` |
| reCAPTCHA coordinates | `DBC_Examples_VB/RecaptchaCoordinates.vb` | `RecaptchaCoordinates` | `2` |
| reCAPTCHA image group | `DBC_Examples_VB/RecaptchaImageGroup.vb` | `RecaptchaImageGroup` | `3` |
| DataDome | `DBC_Examples_VB/Datadome.vb` | `Datadome` | `21` |
| FriendlyCaptcha | `DBC_Examples_VB/Friendlycaptcha.vb` | `Friendlycaptcha` | `20` |
| AtbCaptcha | `DBC_Examples_VB/Atb.vb` | `Atb` | `24` |
| Capy puzzle | `DBC_Examples_VB/Capy.vb` | `Capy` | `15` |
| Lemin | `DBC_Examples_VB/Lemin.vb` | `Lemin` | `14` |
| MTCaptcha | `DBC_Examples_VB/Mtcaptcha.vb` | `Mtcaptcha` | `18` |
| Tencent | `DBC_Examples_VB/Tencent.vb` | `Tencent` | `23` |
| Cloudflare Turnstile | `DBC_Examples_VB/Turnstile.vb` | `Turnstile` | `12` |
| Siara | `DBC_Examples_VB/Siara.vb` | `Siara` | `17` |
| Amazon WAF | `DBC_Examples_VB/AmazonWaf.vb` | `AmazonWaf` | `16` |
| CutCaptcha | `DBC_Examples_VB/Cutcaptcha.vb` | `Cutcaptcha` | `19` |
| GeeTest v3 | `DBC_Examples_VB/GeetestV3.vb` | `GeetestV3` | `8` |
| GeeTest v4 | `DBC_Examples_VB/GeetestV4.vb` | `GeetestV4` | `9` |
| Text captcha | `DBC_Examples_VB/Textcaptcha.vb` | `Textcaptcha` | `11` |

## Quick Examples

Run C# Turnstile:

```bash
dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 -p:ExamplesStartupObject=DeathByCaptcha.TurnstileExample
```

Run VB Turnstile:

```bash
dotnet run --project DBC_Examples_VB/DBC_Examples_VB.vbproj -c Release -f net10.0 -p:ExamplesStartupObject=DBC_Examples_VB.Turnstile
```

## Notes

- Do not pass `/t:Rebuild` to `dotnet run`.
- Use `-p:ExamplesStartupObject=...` to select the sample entry point.
- Many samples in `DBC_Examples/` and `DBC_Examples_VB/` use hardcoded placeholders for credentials and challenge parameters.
- Most non-test samples do not load `.env` automatically and do not read environment variables by default.
- If a sample requires values (username/password, site key, page URL, proxy, etc.), edit placeholders in that sample file or add your own env-loading logic.
