# C# and VB Samples Guide

This guide explains how to run the non-Selenium samples from:

- `DBC_Examples/` (C#)
- `DBC_Examples_VB/` (VB)

For Selenium, use the dedicated document: `SELENIUM_TESTS.md`.

## Prerequisites

- Use a supported framework target (`net6.0`, `net8.0`, `net10.0`).
- Understand input style differences:
	- Selenium sample (`SeleniumRecaptchaV2Example`) reads `DBC_USERNAME`/`DBC_PASSWORD` from environment variables.
	- Most legacy C#/VB samples use positional CLI args (`args` / `argv`) and/or hardcoded placeholders in code.
- Replace placeholder CAPTCHA params inside each sample as needed.

If you run the default `ExampleSimple` entrypoint, pass args explicitly:

```bash
export DBC_USERNAME='your_username'
export DBC_PASSWORD='your_password'

dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 \
	-p:ExamplesStartupObject=DeathByCaptcha.ExampleSimple -- \
	"$DBC_USERNAME" "$DBC_PASSWORD" DBC_Examples/images/normal.jpg

dotnet run --project DBC_Examples_VB/DBC_Examples_VB.vbproj -c Release -f net10.0 \
	-p:ExamplesStartupObject=DBC_Examples_VB.ExampleSimple -- \
	"$DBC_USERNAME" "$DBC_PASSWORD" DBC_Examples_VB/images/normal.jpg
```

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

To pass sample arguments, append `-- <args...>` to `dotnet run`.

## StartupObject and Selenium

- `ExamplesStartupObject` only chooses which `Main` class runs.
- Selenium does not auto-start unless startup object is `DeathByCaptcha.SeleniumRecaptchaV2Example`.
- `DBC_Examples` references Selenium packages for that one sample, but non-Selenium startup classes do not launch Firefox.

## What Is `captcha type`?

`captcha type` is the numeric code sent in the payload (usually `{"type", N}`) that tells DeathByCaptcha which solver flow to use.

- Example: `type = 4` means token-based reCAPTCHA v2.
- Example: `type = 12` means Cloudflare Turnstile token.
- For normal image decode, omitting type is equivalent to `type = 0` (basic image).

If `type` is wrong for the target challenge, solving will fail or return invalid results.

## C# Samples Map

Use startup object prefix `DeathByCaptcha.`

| Sample | File | StartupObject suffix | Captcha type |
|---|---|---|---|
| Basic image decode | [Examples.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/Examples.cs) | `ExampleSimple` | `0` (omitted in code) |
| Balance check only | [Examples.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/Examples.cs) | `GetBalance` | `n/a` |
| Generic token sample | [Examples.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/Examples.cs) | `ExampleToken` | `4` |
| Full upload/poll flow | [Examples.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/Examples.cs) | `ExampleFull` | `0` (omitted in code) |
| Async decode flow | [Examples.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/Examples.cs) | `ExampleAsync` | `0` (omitted in code) |
| Normal captcha | [NormalCaptchaExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/NormalCaptchaExample.cs) | `NormalCaptchaExample` | `0` (omitted in code) |
| Audio captcha | [AudioExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/AudioExample.cs) | `AudioExample` | `13` |
| reCAPTCHA v2 token | [RecaptchaV2Example.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/RecaptchaV2Example.cs) | `RecaptchaV2Example` | `4` |
| reCAPTCHA v3 token | [RecaptchaV3Example.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/RecaptchaV3Example.cs) | `RecaptchaV3Example` | `5` |
| reCAPTCHA v2 Enterprise | [RecaptchaV2EnterpriseExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/RecaptchaV2EnterpriseExample.cs) | `RecaptchaV2EnterpriseExample` | `25` |
| reCAPTCHA coordinates | [RecaptchaCoordinatesExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/RecaptchaCoordinatesExample.cs) | `RecaptchaCoordinatesExample` | `2` |
| reCAPTCHA image group | [RecaptchaImageGroupExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/RecaptchaImageGroupExample.cs) | `RecaptchaImageGroupExample` | `3` |
| DataDome | [DatadomeExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/DatadomeExample.cs) | `DatadomeExample` | `21` |
| FriendlyCaptcha | [FriendlycaptchaExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/FriendlycaptchaExample.cs) | `FriendlycaptchaExample` | `20` |
| AtbCaptcha | [AtbExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/AtbExample.cs) | `AtbExample` | `24` |
| Capy puzzle | [CapyExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/CapyExample.cs) | `CapyExample` | `15` |
| Lemin | [LeminExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/LeminExample.cs) | `LeminExample` | `14` |
| MTCaptcha | [MtcaptchaExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/MtcaptchaExample.cs) | `MtcaptchaExample` | `18` |
| Tencent | [TencentExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/TencentExample.cs) | `TencentExample` | `23` |
| Cloudflare Turnstile | [TurnstileExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/TurnstileExample.cs) | `TurnstileExample` | `12` |
| Siara | [SiaraExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/SiaraExample.cs) | `SiaraExample` | `17` |
| Amazon WAF | [AmazonWafExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/AmazonWafExample.cs) | `AmazonWafExample` | `16` |
| CutCaptcha | [CutcaptchaExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/CutcaptchaExample.cs) | `CutcaptchaExample` | `19` |
| GeeTest v3 | [GeetestV3Example.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/GeetestV3Example.cs) | `GeetestV3Example` | `8` |
| GeeTest v4 | [GeetestV4Example.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/GeetestV4Example.cs) | `GeetestV4Example` | `9` |
| Text captcha | [TextcaptchaExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/TextcaptchaExample.cs) | `TextcaptchaExample` | `11` |

## VB Samples Map

Use startup object prefix `DBC_Examples_VB.`

| Sample | File | StartupObject suffix | Captcha type |
|---|---|---|---|
| Basic image decode | [Examples.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Examples.vb) | `ExampleSimple` | `0` (omitted in code) |
| Balance check only | [Examples.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Examples.vb) | `GetBalance` | `n/a` |
| Generic token sample | [Examples.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Examples.vb) | `ExampleToken` | `4` |
| Full upload/poll flow | [Examples.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Examples.vb) | `ExampleFull` | `0` (omitted in code) |
| Normal captcha | [Normal_Captcha.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Normal_Captcha.vb) | `NormalCaptcha` | `0` (omitted in code) |
| Audio captcha | [Audio.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Audio.vb) | `Audio` | `13` |
| reCAPTCHA v2 token | [RecaptchaV2.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/RecaptchaV2.vb) | `RecaptchaV2` | `4` |
| reCAPTCHA v3 token | [RecaptchaV3.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/RecaptchaV3.vb) | `RecaptchaV3` | `5` |
| reCAPTCHA v2 Enterprise | [RecaptchaV2Enterprise.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/RecaptchaV2Enterprise.vb) | `RecaptchaV2Enterprise` | `25` |
| reCAPTCHA coordinates | [RecaptchaCoordinates.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/RecaptchaCoordinates.vb) | `RecaptchaCoordinates` | `2` |
| reCAPTCHA image group | [RecaptchaImageGroup.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/RecaptchaImageGroup.vb) | `RecaptchaImageGroup` | `3` |
| DataDome | [Datadome.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Datadome.vb) | `Datadome` | `21` |
| FriendlyCaptcha | [Friendlycaptcha.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Friendlycaptcha.vb) | `Friendlycaptcha` | `20` |
| AtbCaptcha | [Atb.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Atb.vb) | `Atb` | `24` |
| Capy puzzle | [Capy.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Capy.vb) | `Capy` | `15` |
| Lemin | [Lemin.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Lemin.vb) | `Lemin` | `14` |
| MTCaptcha | [Mtcaptcha.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Mtcaptcha.vb) | `Mtcaptcha` | `18` |
| Tencent | [Tencent.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Tencent.vb) | `Tencent` | `23` |
| Cloudflare Turnstile | [Turnstile.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Turnstile.vb) | `Turnstile` | `12` |
| Siara | [Siara.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Siara.vb) | `Siara` | `17` |
| Amazon WAF | [AmazonWaf.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/AmazonWaf.vb) | `AmazonWaf` | `16` |
| CutCaptcha | [Cutcaptcha.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Cutcaptcha.vb) | `Cutcaptcha` | `19` |
| GeeTest v3 | [GeetestV3.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/GeetestV3.vb) | `GeetestV3` | `8` |
| GeeTest v4 | [GeetestV4.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/GeetestV4.vb) | `GeetestV4` | `9` |
| Text captcha | [Textcaptcha.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Textcaptcha.vb) | `Textcaptcha` | `11` |

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
- `ExampleSimple`, `ExampleToken`, `ExampleFull`, and `ExampleAsync` expect positional arguments.
- `DBC_Examples_VB.ExampleSimple` validates missing args and prints `Incorrect number of arguments`.
- Most non-Selenium samples do not load `.env` automatically and do not read environment variables by default.
- If a sample requires values (username/password, site key, page URL, proxy, etc.), edit placeholders in that sample file or add your own env-loading logic.
