# ZennoPoster Test Sample (C#)

How to build and test the ZennoPoster snippets.

## What it covers

- The snippets live in `DBC_Examples/ZennoPoster/ZennoPosterSnippets.cs`
  and compile with the examples project on every build.
- `DeathByCaptcha/DeathByCaptcha.Tests/ZennoPosterSnippetTests.cs` reads the
  sitekey from Google's reCAPTCHA demo page and solves it through the same
  snippet the cubes use. It runs against the live API and consumes balance.

## Build

```bash
dotnet build dbc_api_net.sln -c Release
```

The snippets are part of `DBC_Examples`, so a normal build compiles them
for `net6.0`, `net8.0` and `net10.0`.

## Run the live test

```bash
dotnet test DeathByCaptcha/DeathByCaptcha.Tests/DeathByCaptcha.Tests.csproj \
  -c Release --filter "Category=ZennoPoster"
```

Credentials come from the environment or a `.env` file at the repository
root:

```dotenv
DBC_USERNAME=your_username
DBC_PASSWORD=your_password
```

The test is skipped when the credentials are missing, so a pull request
without secrets still passes.

## Notes

- The test spends one reCAPTCHA v2 solve per run, like the other browser
  integration tests.
- It asserts that a non-empty token comes back; it does not submit the
  token to a third-party site.
