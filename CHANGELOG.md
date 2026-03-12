# Changelog

All notable changes to this project will be documented in this file.

## 4.7.0 - 2026-03-12

### Added

- Added a dedicated test project with unit tests for the core library.
- Added real integration tests for both HTTP and socket API clients.
- Added `.env.sample` for local integration test credentials.
- Added GitHub Actions workflows for unit tests, basic integration tests, and coverage reporting.
- Added separate CI badges for unit tests on `net6.0`, `net8.0`, and `net10.0`.
- Added a published Shields-compatible coverage badge endpoint payload.

### Changed

- Modernized the main library and examples to SDK-style .NET projects.
- Updated supported target frameworks to `net6.0`, `net8.0`, and `net10.0`.
- Centralized shared build and version properties in `Directory.Build.props`.
- Set the repository version to `4.7.0`.
- Updated the public client version string to `DBC/.NET v4.7.0`.
- Made `dotnet` CLI the canonical build and test workflow.
- Consolidated .NET documentation into the main `README.md`.

### Fixed

- Fixed Linux HTTP response decoding when the API returns the non-standard `charset=utf8` token.
- Fixed multiple example project compilation issues during the modernization work.
- Improved testability of transport code without breaking public signatures.

### Security

- Removed the global certificate validation bypass pattern.
- Added per-instance control for accepting any server certificate in the HTTP client.

### CI and Quality

- Increased automated coverage of the core library beyond the requested threshold.
- Included both HTTP and socket transports in test coverage instead of excluding critical paths.
- Added optional gating for expensive full integration workflows through `DBC_INTEGRATION_FULL=true`.

### Documentation

- Added a structured documentation index to `README.md`.
- Documented local and CI test execution flows.
- Clarified that the old Mono build path is deprecated and removed.
