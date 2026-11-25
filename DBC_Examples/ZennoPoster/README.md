# ZennoPoster snippets (C#)

Reusable C# snippets that solve CAPTCHAs with DeathByCaptcha from inside a
ZennoPoster "Own code" cube.

They use only the official DeathByCaptcha client and the standard library,
so they compile anywhere the client compiles. The ZennoPoster side — reading
the page, saving the screenshot, filling the field — stays in your cube.

## Files

- `ZennoPosterSnippets.cs` — the snippets.
- The test `DeathByCaptcha.Tests/ZennoPosterSnippetTests.cs` solves a live
  reCAPTCHA v2 through the same code path.

## What the snippets do

| Method | DeathByCaptcha type | Needs |
| --- | --- | --- |
| `SolveRecaptchaV2(sitekey, pageurl, timeout)` | 4 | sitekey and page URL |
| `SolveRecaptchaV3(sitekey, pageurl, action, minScore, timeout)` | 5 | sitekey and page URL |
| `SolveTurnstile(sitekey, pageurl, timeout)` | 12 | sitekey and page URL |
| `SolveAmazonWaf(sitekey, pageurl, iv, context, timeout)` | 16 | the values from the page |
| `SolveImage(imagePath, timeout)` | 0 | a screenshot on disk |
| `GetBalance()` | — | nothing |

Each method returns the token or the text, and throws when there is no
solution. `timeout` is in seconds; leave it at zero for the client's
defaults (sixty for images, a hundred twenty for tokens).

## Using them in ZennoPoster

1. Build the client (`dotnet build DeathByCaptcha/DeathByCaptcha.csproj -c Release`)
   or take the assembly from the NuGet package, and reference the DLL from
   your ZennoPoster project.
2. Set the credentials as environment variables on the machine that runs
   ZennoPoster: `DBC_USERNAME` and `DBC_PASSWORD`, or `DBC_AUTHTOKEN`.
   Never paste them into a cube or a project file.
3. Solve from the cube:

```csharp
// Own code cube
string token = DeathByCaptcha.ZennoPoster.Snippets.SolveRecaptchaV2(
    sitekey: "6Le...",
    pageurl: "https://example.test/");
// then write the token into the page with your ZennoPoster scripting
```

For an image challenge, save the screenshot first:

```csharp
string shot = @"C:\temp\challenge.png";
// ZennoPoster: capture the element into $shot
string answer = DeathByCaptcha.ZennoPoster.Snippets.SolveImage(shot);
```

## Why the IP is already right

Turnstile, Amazon WAF and DataDome tokens are valid only from the exit IP
that solved them. Because the snippet runs on the machine that runs the
browser, the solve and the target request leave from the same IP, and no
proxy configuration is needed. A proxy is only needed if the browser
leaves through a different one.

## Limits

- The snippets do not know how ZennoPoster renders a page; the wiring is
  yours.
- reCAPTCHA v3 needs an `action` and a `min_score` that match the site.
- The library targets `net6.0`, `net8.0` and `net10.0`. If your
  ZennoPoster runs on .NET Framework, check that it can load assemblies
  for one of those targets (**verify** the runtime your version uses).

## Tests

See [ZENNOPOSTER_TESTS.md](../ZENNOPOSTER_TESTS.md).
