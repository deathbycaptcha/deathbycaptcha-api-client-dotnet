# [DeathByCaptcha](https://deathbycaptcha.com/)


<p align="center">
  <a href="https://github.com/deathbycaptcha/deathbycaptcha-api-client-python"><img alt="Python" src="https://img.shields.io/badge/Python-3776AB?style=for-the-badge&logo=python&logoColor=white"></a>
  <a href="https://github.com/deathbycaptcha/deathbycaptcha-api-client-nodejs"><img alt="Node.js" src="https://img.shields.io/badge/Node.js-339933?style=for-the-badge&logo=nodedotjs&logoColor=white"></a>
  <a href="https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet"><img alt=".NET" src="https://img.shields.io/badge/%E2%80%BA-.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white&labelColor=555555"></a>
  <a href="https://github.com/deathbycaptcha/deathbycaptcha-api-client-java"><img alt="Java" src="https://img.shields.io/badge/Java-ED8B00?style=for-the-badge&logo=openjdk&logoColor=white"></a>
  <a href="https://github.com/deathbycaptcha/deathbycaptcha-api-client-php"><img alt="PHP" src="https://img.shields.io/badge/PHP-777BB4?style=for-the-badge&logo=php&logoColor=white"></a>
  <a href="https://github.com/deathbycaptcha/deathbycaptcha-api-client-perl"><img alt="Perl" src="https://img.shields.io/badge/Perl-39457E?style=for-the-badge&logo=perl&logoColor=white"></a>
  <a href="https://github.com/deathbycaptcha/deathbycaptcha-api-client-c11"><img alt="C" src="https://img.shields.io/badge/C-A8B9CC?style=for-the-badge&logo=c&logoColor=black"></a>
  <a href="https://github.com/deathbycaptcha/dbc_api_autoit"><img alt="AutoIt" src="https://img.shields.io/badge/AutoIt-1C3552?style=for-the-badge"></a>
</p>


## 📖 Introduction

The [DeathByCaptcha](https://deathbycaptcha.com) .NET client is a library for **automated captcha solving** in C# and VB.NET applications. It provides a simple, well-documented interface to the [DeathByCaptcha](https://deathbycaptcha.com) solving service — covering everything from basic image CAPTCHAs to modern token-based challenges. A common use case is **captcha solving for data extraction** pipelines, where CAPTCHAs block access to the pages you need to scrape. It supports both the HTTPS API (encrypted transport — recommended when security is a priority) and the socket-based API (faster and lower latency, recommended for high-throughput production workloads).

Key features:

- 🧩 Send image, audio and modern token-based CAPTCHA types (reCAPTCHA v2/v3, Turnstile, GeeTest, etc.).
- 🔄 Unified client API across HTTP and socket transports — switching implementations is straightforward.
- 🔐 Built-in support for proxies, timeouts and advanced token parameters for modern CAPTCHA flows.

Quick start example (HTTP):

```csharp
using DeathByCaptcha;

Client client = new DeathByCaptcha.HttpClient("your_username", "your_password");
Captcha captcha = client.Decode("path/to/captcha.jpg", 120);
if (captcha != null)
    Console.WriteLine(captcha.Text);
```

> **🚌 Transport options:** Use `DeathByCaptcha.HttpClient` for encrypted HTTPS communication — credentials and data travel over TLS. Use `DeathByCaptcha.SocketClient` for lower latency and higher throughput — it is faster but communicates over a plain TCP connection to `api.dbcapi.me` on ports `8123–8130`.

---

### Tests Status

[![Unit Tests net10](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/unit-tests-net10.yml/badge.svg?branch=master)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/unit-tests-net10.yml)
[![Integration Tests Basic](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/integration-basic.yml/badge.svg?branch=master)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/integration-basic.yml)
[![Coverage net10](https://img.shields.io/endpoint?url=https://raw.githubusercontent.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/badges/.github/badges/coverage-net10.json)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/coverage.yml)

---

## 🗂️ Index

- [Installation](#installation)
    - [From NuGet (Recommended)](#from-nuget-recommended)
    - [From GitHub Repository](#from-github-repository)
- [How to Use DBC API Clients](#how-to-use-dbc-api-clients)
    - [Common Clients' Interface](#common-clients-interface)
    - [Available Methods](#available-methods)
- [Credentials & Configuration](#credentials--configuration)
    - [Quick Setup](#quick-setup)
- [CAPTCHA Types Quick Reference & Examples](#captcha-types-quick-reference--examples)
    - [Quick Start](#quick-start)
    - [Type Reference](#type-reference)
    - [Per-Type Code Snippets](#per-type-code-snippets)
- [CAPTCHA Types Extended Reference](#captcha-types-extended-reference)
    - [Featured Sample: Selenium reCAPTCHA v2](#-featured-sample-selenium-recaptcha-v2)
    - [reCAPTCHA Image-Based API — Deprecated (Types 2 & 3)](#recaptcha-image-based-api--deprecated-types-2--3)
    - [reCAPTCHA Token API (v2 & v3)](#recaptcha-token-api-v2--v3)
    - [reCAPTCHA v2 API FAQ](#recaptcha-v2-api-faq)
    - [What is reCAPTCHA v3?](#what-is-recaptcha-v3)
    - [reCAPTCHA v3 API FAQ](#recaptcha-v3-api-faq)
    - [Amazon WAF API (Type 16)](#amazon-waf-api-type-16)
    - [Cloudflare Turnstile API (Type 12)](#cloudflare-turnstile-api-type-12)
- [Running Tests](#running-tests)
- [CI Status Badges](#ci-status-badges)
- [Changelog](CHANGELOG.md)
- [Selenium Test Sample](SELENIUM_TESTS.md)
- [C# and VB Samples Guide](EXAMPLES_CSHARP_VB.md)

<a id="installation"></a>
## 🛠️ Installation

<a id="from-nuget-recommended"></a>
### 📦 From NuGet (Recommended)

```bash
dotnet add package DeathByCaptcha --version 4.7.1
```

Or add it directly in your project file:

```xml
<ItemGroup>
    <PackageReference Include="DeathByCaptcha" Version="4.7.1" />
</ItemGroup>
```

<a id="from-github-repository"></a>
### 🐙 From GitHub Repository

For the latest development version or to contribute:

```bash
git clone https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet.git
cd deathbycaptcha-api-client-dotnet
dotnet restore dbc_api_net.sln
```

<a id="how-to-use-dbc-api-clients"></a>
## 🚀 How to Use DBC API Clients

<a id="common-clients-interface"></a>
### 🔌 Common Clients' Interface

All clients must be instantiated with your DeathByCaptcha credentials — either *username* and *password*, or an *authtoken* (available in the DBC user panel). Replace `DeathByCaptcha.HttpClient` with `DeathByCaptcha.SocketClient` to use the socket transport instead.

```csharp
using DeathByCaptcha;

// Username + password (HTTPS transport — encrypted, recommended when security matters)
Client client = new DeathByCaptcha.HttpClient(username, password);

// Username + password (socket transport — faster, lower latency, recommended for high throughput)
// Client client = new DeathByCaptcha.SocketClient(username, password);

// Authtoken — pass the literal string "authtoken" as username and the token as password
// Client client = new DeathByCaptcha.HttpClient("authtoken", token_from_panel);
```

> **💡 Note:** If your project also uses `System.Net.Http`, qualify the class as `DeathByCaptcha.HttpClient` to avoid type ambiguity.

| Transport | Class | Best for |
|---|---|---|
| HTTPS | `DeathByCaptcha.HttpClient` | Encrypted TLS transport — safer for credential handling and network-sensitive environments |
| Socket | `DeathByCaptcha.SocketClient` | Plain TCP — faster and lower latency, recommended for high-throughput production workloads |

The .NET clients are **thread-safe** — it is perfectly fine to share a single client between multiple threads (although in heavily multithreaded applications a pool of clients is preferable).

All clients share the same interface. Below is a summary of every available method and its signature.

<a id="available-methods"></a>
### 🔧 Available Methods

| Method | Signature | Returns | Description |
|---|---|---|---|
| `Upload()` | `Upload(string imageFileName)` | `Captcha` or `null` | Upload a CAPTCHA for solving without waiting. |
| `Decode()` | `Decode(string imageFileName)` | `Captcha` or `null` | Upload and poll until solved using the default timeout (`Client.DefaultTimeout`). |
| `Decode()` | `Decode(string imageFileName, int timeout)` | `Captcha` or `null` | Upload and poll until solved or timed out. Preferred method for most integrations. |
| `Decode()` | `Decode(int timeout, Hashtable ext_data)` | `Captcha` or `null` | Upload token/vendor CAPTCHA types using the `ext_data` hashtable. |
| `GetCaptcha()` | `GetCaptcha(int captchaId)` | `Captcha` or `null` | Fetch status and result of a previously uploaded CAPTCHA by its numeric ID. |
| `Report()` | `Report(Captcha captcha)` | `bool` | Report a CAPTCHA as incorrectly solved to request a refund. Only report genuine errors. |
| `GetBalance()` | `GetBalance()` | `double` | Return the current account balance in US cents. |

### 📬 CAPTCHA Result Object

All methods that return a solved CAPTCHA return a `DeathByCaptcha.Captcha` object exposing the following properties:

| Property | Type | Description |
|---|---|---|
| `Id` | `int` | Numeric CAPTCHA ID assigned by DBC |
| `Text` | `string` | Solved text or token (the value you inject into the page) |
| `Correct` | `bool` | Whether DBC considers the solution correct |
| `Solved` | `bool` | Whether the CAPTCHA has been solved |
| `Uploaded` | `bool` | Whether the CAPTCHA was successfully uploaded |

### 💡 Full Usage Example

```csharp
using System;
using DeathByCaptcha;

string username = Environment.GetEnvironmentVariable("DBC_USERNAME") ?? "your_username";
string password = Environment.GetEnvironmentVariable("DBC_PASSWORD") ?? "your_password";

Client client = new DeathByCaptcha.HttpClient(username, password);

try
{
    Console.WriteLine("Balance: " + client.GetBalance() + " US cents");

    Captcha captcha = client.Decode("path/to/captcha.jpg", 120);
    if (captcha != null)
    {
        Console.WriteLine($"Solved CAPTCHA {captcha.Id}: {captcha.Text}");

        // Report only if you are certain the solution is wrong:
        // client.Report(captcha);
    }
}
catch (AccessDeniedException)
{
    Console.WriteLine("Access denied — check your credentials and/or balance");
}
```

<a id="credentials--configuration"></a>
## 🔑 Credentials & Configuration

Keep credentials in environment variables instead of hardcoding them:

```bash
export DBC_USERNAME="your_username"
export DBC_PASSWORD="your_password"
```

For CI environments set repository secrets `DBC_USERNAME` and `DBC_PASSWORD`. Integration tests read these variables automatically and skip gracefully if they are absent.

<a id="quick-setup"></a>
### ⚡ Quick Setup

```bash
# ① Clone and restore
git clone https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet.git
cd deathbycaptcha-api-client-dotnet
dotnet restore dbc_api_net.sln

# ② Copy credentials template and add credentials
cp .env.sample .env
# Edit .env with your DBC_USERNAME and DBC_PASSWORD

# ③ Run unit tests locally
dotnet test DeathByCaptcha/DeathByCaptcha.Tests/DeathByCaptcha.Tests.csproj -c Release --filter "Category!=Integration"

# ④ Run a C# example (replace startup class as needed)
dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 \
  -p:ExamplesStartupObject=DeathByCaptcha.ExampleSimple \
  -- "$DBC_USERNAME" "$DBC_PASSWORD" DBC_Examples/images/normal.jpg
```

See [EXAMPLES_CSHARP_VB.md](EXAMPLES_CSHARP_VB.md) for a complete startup class map and run commands for all C# and VB samples.

### Modern .NET Workspace

This repository already includes SDK-style .NET projects for the library and examples. You do not need to create a new console project to use the included samples.

**Projects:**

- Main library: `DeathByCaptcha/DeathByCaptcha/DeathByCaptcha.csproj`
- Internal JSON dependency: `DeathByCaptcha/SimpleJson/SimpleJson.csproj`
- C# examples: `DBC_Examples/DBC_Examples.csproj`
- VB examples: `DBC_Examples_VB/DBC_Examples_VB.vbproj`

**Target frameworks:** `net6.0`, `net8.0`, `net10.0` — prefer `net10.0` for regular development.

**Canonical dotnet CLI commands:**

```bash
dotnet restore dbc_api_net.sln
dotnet build dbc_api_net.sln -c Release
dotnet test DeathByCaptcha/DeathByCaptcha.Tests/DeathByCaptcha.Tests.csproj -c Release

# Run C# example with explicit startup class:
dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 \
  -p:ExamplesStartupObject=DeathByCaptcha.ExampleSimple \
  -- "$DBC_USERNAME" "$DBC_PASSWORD" DBC_Examples/images/normal.jpg

# Run Selenium sample:
dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 \
  -p:ExamplesStartupObject=DeathByCaptcha.SeleniumRecaptchaV2Example
```

**Optional Make wrapper:**

```bash
make -f Makefile-dotnet build
make -f Makefile-dotnet test
make -f Makefile-dotnet run-cs CS_STARTUP=DeathByCaptcha.RecaptchaV2Example
make -f Makefile-dotnet run-vb VB_STARTUP=DBC_Examples_VB.RecaptchaV2
```

<a id="captcha-types-quick-reference--examples"></a>
## 🧩 CAPTCHA Types Quick Reference & Examples

This section covers every supported CAPTCHA type, how to run the corresponding example files, and ready-to-copy code snippets. Start with the Quick Start below, then use the Type Reference to find the type you need.

<a id="quick-start"></a>
### 🏁 Quick Start

1. **📦 Install or restore the library** (see [Installation](#installation))
2. **📂 Navigate to the project** and run the example for the type you need using `dotnet run`:

```bash
dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 \
  -p:ExamplesStartupObject=DeathByCaptcha.NormalCaptchaExample \
  -- "$DBC_USERNAME" "$DBC_PASSWORD" DBC_Examples/images/normal.jpg

# Balance check:
dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 \
  -p:ExamplesStartupObject=DeathByCaptcha.ExampleSimple \
  -- "$DBC_USERNAME" "$DBC_PASSWORD"
```

Before running any example, set `DBC_USERNAME` and `DBC_PASSWORD` in your environment or `.env` file.

<a id="type-reference"></a>
### 📋 Type Reference

The table below maps every supported type to its use case, a code snippet, and the corresponding example files in `DBC_Examples/` and `DBC_Examples_VB/`.

| Type ID | CAPTCHA Type | Use Case | Quick Use | C# Sample | VB Sample |
| --- | --- | --- | --- | --- | --- |
| 0 | Standard Image | Basic image CAPTCHA | [snippet](#sample-type-0-standard-image) | [NormalCaptchaExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/NormalCaptchaExample.cs) | [Normal_Captcha.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Normal_Captcha.vb) |
| 2 | ~~reCAPTCHA Coordinates~~ | Deprecated — do not use for new integrations | — | — | — |
| 3 | ~~reCAPTCHA Image Group~~ | Deprecated — do not use for new integrations | — | — | — |
| 4 | reCAPTCHA v2 Token | reCAPTCHA v2 token solving | [snippet](#sample-type-4-recaptcha-v2-token) | [RecaptchaV2Example.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/RecaptchaV2Example.cs) | [RecaptchaV2.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/RecaptchaV2.vb) |
| 5 | reCAPTCHA v3 Token | reCAPTCHA v3 with risk scoring | [snippet](#sample-type-5-recaptcha-v3-token) | [RecaptchaV3Example.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/RecaptchaV3Example.cs) | [RecaptchaV3.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/RecaptchaV3.vb) |
| 25 | reCAPTCHA v2 Enterprise | reCAPTCHA v2 Enterprise tokens | [snippet](#sample-type-25-recaptcha-v2-enterprise) | [RecaptchaV2EnterpriseExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/RecaptchaV2EnterpriseExample.cs) | [RecaptchaV2Enterprise.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/RecaptchaV2Enterprise.vb) |
| 8 | GeeTest v3 | Geetest v3 verification | [snippet](#sample-type-8-geetest-v3) | [GeetestV3Example.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/GeetestV3Example.cs) | [GeetestV3.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/GeetestV3.vb) |
| 9 | GeeTest v4 | Geetest v4 verification | [snippet](#sample-type-9-geetest-v4) | [GeetestV4Example.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/GeetestV4Example.cs) | [GeetestV4.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/GeetestV4.vb) |
| 11 | Text CAPTCHA | Text-based question solving | [snippet](#sample-type-11-text-captcha) | [TextcaptchaExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/TextcaptchaExample.cs) | [Textcaptcha.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Textcaptcha.vb) |
| 12 | Cloudflare Turnstile | Cloudflare Turnstile token | [snippet](#sample-type-12-cloudflare-turnstile) | [TurnstileExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/TurnstileExample.cs) | [Turnstile.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Turnstile.vb) |
| 13 | Audio CAPTCHA | Audio CAPTCHA solving | [snippet](#sample-type-13-audio-captcha) | [AudioExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/AudioExample.cs) | [Audio.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Audio.vb) |
| 14 | Lemin | Lemin CAPTCHA | [snippet](#sample-type-14-lemin) | [LeminExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/LeminExample.cs) | [Lemin.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Lemin.vb) |
| 15 | Capy | Capy CAPTCHA | [snippet](#sample-type-15-capy) | [CapyExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/CapyExample.cs) | [Capy.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Capy.vb) |
| 16 | Amazon WAF | Amazon WAF verification | [snippet](#sample-type-16-amazon-waf) | [AmazonWafExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/AmazonWafExample.cs) | [AmazonWaf.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/AmazonWaf.vb) |
| 17 | Siara | Siara CAPTCHA | [snippet](#sample-type-17-siara) | [SiaraExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/SiaraExample.cs) | [Siara.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Siara.vb) |
| 18 | MTCaptcha | Mtcaptcha CAPTCHA | [snippet](#sample-type-18-mtcaptcha) | [MtcaptchaExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/MtcaptchaExample.cs) | [Mtcaptcha.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Mtcaptcha.vb) |
| 19 | Cutcaptcha | Cutcaptcha CAPTCHA | [snippet](#sample-type-19-cutcaptcha) | [CutcaptchaExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/CutcaptchaExample.cs) | [Cutcaptcha.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Cutcaptcha.vb) |
| 20 | Friendly Captcha | Friendly Captcha | [snippet](#sample-type-20-friendly-captcha) | [FriendlycaptchaExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/FriendlycaptchaExample.cs) | [Friendlycaptcha.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Friendlycaptcha.vb) |
| 21 | DataDome | Datadome verification | [snippet](#sample-type-21-datadome) | [DatadomeExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/DatadomeExample.cs) | [Datadome.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Datadome.vb) |
| 23 | Tencent | Tencent CAPTCHA | [snippet](#sample-type-23-tencent) | [TencentExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/TencentExample.cs) | [Tencent.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Tencent.vb) |
| 24 | ATB | ATB CAPTCHA | [snippet](#sample-type-24-atb) | [AtbExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/AtbExample.cs) | [Atb.vb](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples_VB/Atb.vb) |

<a id="per-type-code-snippets"></a>
### 📝 Per-Type Code Snippets

Minimal usage snippet for each supported type. Use these as a starting point and refer to the full example files in `DBC_Examples/` and `DBC_Examples_VB/` for complete implementations.

<a id="sample-type-0-standard-image"></a>
#### 🖼️ Sample Type 0: Standard Image
Official description: [Supported CAPTCHAs](https://deathbycaptcha.com/api#supported_captchas)
Full C# sample: [NormalCaptchaExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/NormalCaptchaExample.cs)

```csharp
Captcha captcha = client.Decode("images/normal.jpg", 120);
```

---

<a id="sample-type-4-recaptcha-v2-token"></a>
#### 🤖 Sample Type 4: reCAPTCHA v2 Token
Official description: [reCAPTCHA Token API (v2)](https://deathbycaptcha.com/api/newtokenrecaptcha#token-v2)
Full C# sample: [RecaptchaV2Example.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/RecaptchaV2Example.cs)

```csharp
string tokenParams = "{\"proxy\":\"http://user:pass@127.0.0.1:1234\",\"proxytype\":\"HTTP\",\"googlekey\":\"sitekey\",\"pageurl\":\"https://target\"}";
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 4}, {"token_params", tokenParams} });
```

---

<a id="sample-type-5-recaptcha-v3-token"></a>
#### 🤖 Sample Type 5: reCAPTCHA v3 Token
Official description: [reCAPTCHA v3](https://deathbycaptcha.com/api/newtokenrecaptcha#reCAPTCHAv3)
Full C# sample: [RecaptchaV3Example.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/RecaptchaV3Example.cs)

```csharp
string tokenParams = "{\"proxy\":\"http://user:pass@127.0.0.1:1234\",\"proxytype\":\"HTTP\",\"googlekey\":\"sitekey\",\"pageurl\":\"https://target\",\"action\":\"verify\",\"min_score\":0.3}";
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 5}, {"token_params", tokenParams} });
```

---

<a id="sample-type-25-recaptcha-v2-enterprise"></a>
#### 🏢 Sample Type 25: reCAPTCHA v2 Enterprise
Official description: [reCAPTCHA v2 Enterprise](https://deathbycaptcha.com/api/newtokenrecaptcha#reCAPTCHAv2Enterprise)
Full C# sample: [RecaptchaV2EnterpriseExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/RecaptchaV2EnterpriseExample.cs)

```csharp
string tokenParams = "{\"proxy\":\"http://user:pass@127.0.0.1:1234\",\"proxytype\":\"HTTP\",\"googlekey\":\"sitekey\",\"pageurl\":\"https://target\"}";
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 25}, {"token_enterprise_params", tokenParams} });
```

---

<a id="sample-type-8-geetest-v3"></a>
#### 🧩 Sample Type 8: GeeTest v3
Official description: [GeeTest](https://deathbycaptcha.com/api/geetest)
Full C# sample: [GeetestV3Example.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/GeetestV3Example.cs)

```csharp
string geetestParams = "{\"proxy\":\"http://user:pass@127.0.0.1:1234\",\"proxytype\":\"HTTP\",\"gt\":\"gt_value\",\"challenge\":\"challenge_value\",\"pageurl\":\"https://target\"}";
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 8}, {"geetest_params", geetestParams} });
```

---

<a id="sample-type-9-geetest-v4"></a>
#### 🧩 Sample Type 9: GeeTest v4
Official description: [GeeTest](https://deathbycaptcha.com/api/geetest)
Full C# sample: [GeetestV4Example.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/GeetestV4Example.cs)

```csharp
string geetestParams = "{\"proxy\":\"http://user:pass@127.0.0.1:1234\",\"proxytype\":\"HTTP\",\"captcha_id\":\"captcha_id\",\"pageurl\":\"https://target\"}";
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 9}, {"geetest_params", geetestParams} });
```

---

<a id="sample-type-11-text-captcha"></a>
#### 💬 Sample Type 11: Text CAPTCHA
Official description: [Text CAPTCHA](https://deathbycaptcha.com/api/textcaptcha)
Full C# sample: [TextcaptchaExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/TextcaptchaExample.cs)

```csharp
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 11}, {"textcaptcha", "What is two plus two?"} });
```

---

<a id="sample-type-12-cloudflare-turnstile"></a>
#### ☁️ Sample Type 12: Cloudflare Turnstile
Official description: [Cloudflare Turnstile](https://deathbycaptcha.com/api/turnstile)
Full C# sample: [TurnstileExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/TurnstileExample.cs)

```csharp
string turnstileParams = "{\"proxy\":\"http://user:pass@127.0.0.1:1234\",\"proxytype\":\"HTTP\",\"sitekey\":\"sitekey\",\"pageurl\":\"https://target\"}";
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 12}, {"turnstile_params", turnstileParams} });
```

---

<a id="sample-type-13-audio-captcha"></a>
#### 🔊 Sample Type 13: Audio CAPTCHA
Official description: [Audio CAPTCHA](https://deathbycaptcha.com/api/audio)
Full C# sample: [AudioExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/AudioExample.cs)

```csharp
string audioBase64 = Convert.ToBase64String(File.ReadAllBytes("images/audio.mp3"));
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 13}, {"audio", audioBase64}, {"language", "en"} });
```

---

<a id="sample-type-14-lemin"></a>
#### 🔵 Sample Type 14: Lemin
Official description: [Lemin](https://deathbycaptcha.com/api/lemin)
Full C# sample: [LeminExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/LeminExample.cs)

```csharp
string leminParams = "{\"proxy\":\"http://user:pass@127.0.0.1:1234\",\"proxytype\":\"HTTP\",\"captchaid\":\"CROPPED_xxx\",\"pageurl\":\"https://target\"}";
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 14}, {"lemin_params", leminParams} });
```

---

<a id="sample-type-15-capy"></a>
#### 🏴 Sample Type 15: Capy
Official description: [Capy](https://deathbycaptcha.com/api/capy)
Full C# sample: [CapyExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/CapyExample.cs)

```csharp
string capyParams = "{\"proxy\":\"http://user:pass@127.0.0.1:1234\",\"proxytype\":\"HTTP\",\"captchakey\":\"PUZZLE_xxx\",\"api_server\":\"https://www.capy.me/\",\"pageurl\":\"https://target\"}";
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 15}, {"capy_params", capyParams} });
```

---

<a id="sample-type-16-amazon-waf"></a>
#### 🛡️ Sample Type 16: Amazon WAF
Official description: [Amazon WAF](https://deathbycaptcha.com/api/amazonwaf)
Full C# sample: [AmazonWafExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/AmazonWafExample.cs)

```csharp
string wafParams = "{\"proxy\":\"http://user:pass@127.0.0.1:1234\",\"proxytype\":\"HTTP\",\"sitekey\":\"sitekey\",\"pageurl\":\"https://target\",\"iv\":\"iv_value\",\"context\":\"context_value\"}";
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 16}, {"waf_params", wafParams} });
```

---

<a id="sample-type-17-siara"></a>
#### 🔍 Sample Type 17: Siara
Official description: [Siara](https://deathbycaptcha.com/api/siara)
Full C# sample: [SiaraExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/SiaraExample.cs)

```csharp
string siaraParams = "{\"proxy\":\"http://user:pass@127.0.0.1:1234\",\"proxytype\":\"HTTP\",\"slideurlid\":\"slide_master_url_id\",\"pageurl\":\"https://target\",\"useragent\":\"Mozilla/5.0\"}";
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 17}, {"siara_params", siaraParams} });
```

---

<a id="sample-type-18-mtcaptcha"></a>
#### 🔒 Sample Type 18: MTCaptcha
Official description: [MTCaptcha](https://deathbycaptcha.com/api/mtcaptcha)
Full C# sample: [MtcaptchaExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/MtcaptchaExample.cs)

```csharp
string mtcaptchaParams = "{\"proxy\":\"http://user:pass@127.0.0.1:1234\",\"proxytype\":\"HTTP\",\"sitekey\":\"MTPublic-xxx\",\"pageurl\":\"https://target\"}";
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 18}, {"mtcaptcha_params", mtcaptchaParams} });
```

---

<a id="sample-type-19-cutcaptcha"></a>
#### ✂️ Sample Type 19: Cutcaptcha
Official description: [Cutcaptcha](https://deathbycaptcha.com/api/cutcaptcha)
Full C# sample: [CutcaptchaExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/CutcaptchaExample.cs)

```csharp
string cutcaptchaParams = "{\"proxy\":\"http://user:pass@127.0.0.1:1234\",\"proxytype\":\"HTTP\",\"apikey\":\"api_key\",\"miserykey\":\"misery_key\",\"pageurl\":\"https://target\"}";
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 19}, {"cutcaptcha_params", cutcaptchaParams} });
```

---

<a id="sample-type-20-friendly-captcha"></a>
#### 💚 Sample Type 20: Friendly Captcha
Official description: [Friendly Captcha](https://deathbycaptcha.com/api/friendly)
Full C# sample: [FriendlycaptchaExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/FriendlycaptchaExample.cs)

```csharp
string friendlyParams = "{\"proxy\":\"http://user:pass@127.0.0.1:1234\",\"proxytype\":\"HTTP\",\"sitekey\":\"FCMG...\",\"pageurl\":\"https://target\"}";
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 20}, {"friendly_params", friendlyParams} });
```

---

<a id="sample-type-21-datadome"></a>
#### 🛡️ Sample Type 21: DataDome
Official description: [DataDome](https://deathbycaptcha.com/api/datadome)
Full C# sample: [DatadomeExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/DatadomeExample.cs)

```csharp
string datadomeParams = "{\"proxy\":\"http://user:pass@127.0.0.1:1234\",\"proxytype\":\"HTTP\",\"pageurl\":\"https://target\",\"captcha_url\":\"https://target/captcha\"}";
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 21}, {"datadome_params", datadomeParams} });
```

---

<a id="sample-type-23-tencent"></a>
#### 🧧 Sample Type 23: Tencent
Official description: [Tencent](https://deathbycaptcha.com/api/tencent)
Full C# sample: [TencentExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/TencentExample.cs)

```csharp
string tencentParams = "{\"proxy\":\"http://user:pass@127.0.0.1:1234\",\"proxytype\":\"HTTP\",\"appid\":\"appid\",\"pageurl\":\"https://target\"}";
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 23}, {"tencent_params", tencentParams} });
```

---

<a id="sample-type-24-atb"></a>
#### 🏷️ Sample Type 24: ATB
Official description: [ATB](https://deathbycaptcha.com/api/atb)
Full C# sample: [AtbExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/AtbExample.cs)

```csharp
string atbParams = "{\"proxy\":\"http://user:pass@127.0.0.1:1234\",\"proxytype\":\"HTTP\",\"appid\":\"appid\",\"apiserver\":\"https://cap.aisecurius.com\",\"pageurl\":\"https://target\"}";
Captcha captcha = client.Decode(Client.DefaultTimeout, new Hashtable { {"type", 24}, {"atb_params", atbParams} });
```

---

<a id="captcha-types-extended-reference"></a>
## 📚 CAPTCHA Types Extended Reference

Full API-level documentation for selected CAPTCHA types: parameter references, payload schemas, request/response formats, token lifespans, and integration notes.

<a id="-featured-sample-selenium-recaptcha-v2"></a>
### ⭐ Featured Sample: Selenium reCAPTCHA v2

This repository includes an integrated Selenium sample in `DBC_Examples/`.

Use this sample when you need a browser-automation flow that extracts `sitekey`, requests a token using DeathByCaptcha (type 4), injects it into the page, and submits the form.

Quick run:

```bash
dotnet run --project DBC_Examples/DBC_Examples.csproj -c Release -f net10.0 \
  -p:ExamplesStartupObject=DeathByCaptcha.SeleniumRecaptchaV2Example
```

See detailed usage in [SELENIUM_TESTS.md](SELENIUM_TESTS.md).

---

<a id="recaptcha-image-based-api--deprecated-types-2--3"></a>
### ⛔ reCAPTCHA Image-Based API — Deprecated (Types 2 & 3)

> ⚠️ **Deprecated.** Types 2 (Coordinates) and 3 (Image Group) are legacy image-based reCAPTCHA challenge methods that are no longer used at captcha solving. Do not use them for new integrations — use the [reCAPTCHA Token API (v2 & v3)](#recaptcha-token-api-v2--v3) instead.

---

<a id="recaptcha-token-api-v2--v3"></a>
### 🔐 reCAPTCHA Token API (v2 & v3)

The Token-based API solves reCAPTCHA challenges by returning a token you inject directly into the page form, rather than clicking images. Given a site URL and site key, DBC solves the challenge on its side and returns a token valid for one submission.

- **Token Image API**: Provided a site URL and site key, the API returns a token that you use to submit the form on the page with the reCAPTCHA challenge.

---

<a id="recaptcha-v2-api-faq"></a>
### ❓ reCAPTCHA v2 API FAQ

**What's the Token Image API URL?**
To use the Token Image API you will have to send a HTTP POST Request to <http://api.dbcapi.me/api/captcha>

**What are the POST parameters for the Token image API?**
-   **`username`**: Your DBC account username
-   **`password`**: Your DBC account password
-   **`type`=4**: Type 4 specifies this is the reCAPTCHA v2 Token API
-   **`token_params`=json(payload)**: the data to access the recaptcha challenge
json payload structure:
    -   **`proxy`**: your proxy url and credentials (if any). Examples:
        -   <http://127.0.0.1:3128>
        -   <http://user:password@127.0.0.1:3128>

    -   **`proxytype`**: your proxy connection protocol. Example:
        -   HTTP

    -   **`googlekey`**: the google recaptcha site key of the website with the recaptcha. Example:
        -   6Le-wvkSAAAAAPBMRTvw0Q4Muexq9bi0DJwx_mJ-

    -   **`pageurl`**: the url of the page with the recaptcha challenges. Example: if the recaptcha you want to solve is in <http://test.com/path1>, pageurl has to be <http://test.com/path1> and not <http://test.com>.
    -   **`data-s`**: Only required for google search tokens. Use the data-s value inside the google search response html. Do not use for regular tokens.

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

**What's the response from the Token image API?**
The token will come in the `Text` property of the `Captcha` object. It's valid for one use and has a 2 minute lifespan.
```
"03AOPBWq_RPO2vLzyk0h8gH0cA2X4v3tpYCPZR6Y4yxKy1s3Eo7CHZRQntxrdsaD..."
```

---

<a id="what-is-recaptcha-v3"></a>
### 🔎 What is reCAPTCHA v3?

This API extends the reCAPTCHA v2 Token API with two additional parameters: `action` and **minimal score (`min_score`)**.

reCAPTCHA v3 returns a score from each user, evaluating if user is a bot or human. The website uses the score value (0 to 1) to decide whether to accept the requests. Lower scores near 0 are identified as bot.

---

<a id="recaptcha-v3-api-faq"></a>
### ❓ reCAPTCHA v3 API FAQ

**What is `action` in reCAPTCHA v3?**
Is a new parameter that allows processing user actions on the website differently.
To find this we need to inspect the javascript code of the website looking for call of `grecaptcha.execute` function. Example:
```javascript
grecaptcha.execute('6Lc2fhwTAAAAAGatXTzFYfvlQMI2T7B6ji8UVV_f', {action: something})
```
The API will use `"verify"` as default value if we don't provide `action` in our request.

**What is `min_score` in reCAPTCHA v3 API?**
The minimal score needed for the captcha resolution. We recommend using the 0.3 min-score value.

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
The solution comes in the `Text` property. Refer to [Polling for uploaded CAPTCHA status](https://deathbycaptcha.com/api#polling-captcha) for details. It's valid for one use and has a 1 minute lifespan.

---

<a id="amazon-waf-api-type-16"></a>
### 🛡️ Amazon WAF API (Type 16)

Amazon WAF Captcha is part of the Intelligent Threat Mitigation system within Amazon AWS. It presents image-alignment challenges that DBC solves by returning a token you set as the `aws-waf-token` cookie on the target page.

- **Official documentation:** [deathbycaptcha.com/api/amazonwaf](https://deathbycaptcha.com/api/amazonwaf)
- **C# sample:** [AmazonWafExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/AmazonWafExample.cs)

**`waf_params` payload fields:**

| Parameter | Required | Description |
|---|---|---|
| `proxy` | Optional\* | Proxy URL with credentials. E.g. `http://user:password@127.0.0.1:3128` |
| `proxytype` | Required if proxy set | Proxy protocol. Currently only `HTTP` is supported. |
| `sitekey` | Required | Amazon WAF site key found in the page's captcha script |
| `pageurl` | Required | Full URL of the page showing the Amazon WAF challenge |
| `iv` | Required | Value of the `iv` parameter found in the captcha script |
| `context` | Required | Value of the `context` parameter found in the captcha script |
| `challengejs` | Optional | URL of the `challenge.js` script referenced on the page |
| `captchajs` | Optional | URL of the `captcha.js` script referenced on the page |

Full example of `waf_params`:
```json
{
  "proxy": "http://user:password@127.0.0.1:1234",
  "proxytype": "HTTP",
  "sitekey": "AQIDAHjcYu/GjX+QlghicBgQ/7bFaQZ+m5FKCMDnO+vTbNg96AHDh0IR5vgzHNceHYqZR+GO...",
  "pageurl": "https://efw47fpad9.execute-api.us-east-1.amazonaws.com/latest",
  "iv": "CgAFRjIw2vAAABSM",
  "context": "zPT0jOl1rQlUNaldX6LUpn4D6Tl9bJ8VUQ/NrWFxPii..."
}
```

**Response:** The API returns a token valid for one use with a 1-minute lifespan. Set it as the `aws-waf-token` cookie on the target page before submitting the form.

---

<a id="cloudflare-turnstile-api-type-12"></a>
### 🌐 Cloudflare Turnstile API (Type 12)

Cloudflare Turnstile is a CAPTCHA alternative that protects pages without requiring user interaction in most cases. DBC solves it by returning a token you inject into the target form.

- **Official documentation:** [deathbycaptcha.com/api/turnstile](https://deathbycaptcha.com/api/turnstile)
- **C# sample:** [TurnstileExample.cs](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/blob/master/DBC_Examples/TurnstileExample.cs)

**`turnstile_params` payload fields:**

| Field | Required | Description |
|---|---|---|
| `proxy` | Optional | Proxy URL with optional credentials |
| `proxytype` | Required if `proxy` set | Proxy connection protocol. Currently only `HTTP` is supported. |
| `sitekey` | Required | The Turnstile site key found in `data-sitekey` attribute or the `turnstile.render` call |
| `pageurl` | Required | Full URL of the page hosting the Turnstile challenge |
| `action` | Optional | Value of the `data-action` attribute or `action` option passed to `turnstile.render` |

**Example `turnstile_params`:**
```json
{
    "proxy": "http://user:password@127.0.0.1:1234",
    "proxytype": "HTTP",
    "sitekey": "0x4AAAAAAAGlwMzq_9z6S9Mh",
    "pageurl": "https://testsite.com/xxx-test"
}
```

**Response:** The API returns a token valid for one use with a 2-minute lifespan. Submit it via the `input[name="cf-turnstile-response"]` field.

---

<a id="running-tests"></a>
## 🧪 Running Tests

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

### GitLab CI / gitlab-ci-local

A `.gitlab-ci.yml` is included and mirrors all GitHub Actions jobs.
To run the full pipeline locally with [gitlab-ci-local](https://github.com/firecow/gitlab-ci-local):

1. Create `.gitlab-ci-local-variables.yml` (auto-detected, never commit it):

```yaml
DBC_USERNAME: "your_username"
DBC_PASSWORD: "your_password"
```

2. Run all tests:

```bash
gitlab-ci-local --file .gitlab-ci.yml
```

Integration and Selenium jobs skip gracefully if the variables file is absent.

### CI (GitHub Actions)

Available workflows:

- `.github/workflows/unit-tests-net6.yml`
- `.github/workflows/unit-tests-net8.yml`
- `.github/workflows/unit-tests-net10.yml`
- `.github/workflows/integration-basic.yml`
- `.github/workflows/integration-selenium.yml`
- `.github/workflows/coverage.yml`
- `.github/workflows/nuget-publish.yml`

Set repository secrets `DBC_USERNAME` and `DBC_PASSWORD` before running the integration workflow.

The coverage badge is published from workflow `.github/workflows/coverage.yml` to the `badges` branch as a Shields endpoint payload.

<a id="ci-status-badges"></a>
## 🔖 CI Status Badges

| Workflow | Status |
|---|---|
| Unit Tests net6 | [![Unit Tests net6](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/unit-tests-net6.yml/badge.svg?branch=master)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/unit-tests-net6.yml) |
| Unit Tests net8 | [![Unit Tests net8](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/unit-tests-net8.yml/badge.svg?branch=master)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/unit-tests-net8.yml) |
| Unit Tests net10 | [![Unit Tests net10](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/unit-tests-net10.yml/badge.svg?branch=master)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/unit-tests-net10.yml) |
| Integration Tests Basic | [![Integration Tests Basic](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/integration-basic.yml/badge.svg?branch=master)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/integration-basic.yml) |
| Integration Tests Selenium | [![Integration Tests Selenium](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/integration-selenium.yml/badge.svg?branch=master)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/integration-selenium.yml) |
| Coverage | [![Coverage net10](https://img.shields.io/endpoint?url=https://raw.githubusercontent.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/badges/.github/badges/coverage-net10.json)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/coverage.yml) |
| NuGet Publish | [![NuGet Publish](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/nuget-publish.yml/badge.svg)](https://github.com/deathbycaptcha/deathbycaptcha-api-client-dotnet/actions/workflows/nuget-publish.yml) |

## ⚖️ Responsible Use

See [Responsible Use Agreement](RESPONSIBLE_USE.md).
