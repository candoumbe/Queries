📌 Imported from squad.-export on 2026-09-18T22:49:35.147Z. Portable knowledge carried over; project learnings from previous project preserved below.

# Project Context

- **Owner:** Cyrille NDOUMBE
- **Project:** MedEasy management platform
- **Stack:** .NET, Aspire, microservices, web frontend
- **Created:** 2026-07-14T01:20:55+0000

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

📌 Team update (2026-09-15T10:58:34+0000): Reviewed Trinity's centralized RFC 7807 Problem Details implementation for Physiotrack and issued APPROVE. Added test-only assertions for the exact `about:blank` type, query strings in `instance`, and the `Allow` header; all reported validations passed.
📌 Team update (2026-09-28T00:00:00Z): CI-as-code migration from Nuke to Fallout planned in issue #402 — decided by Morpheus. The `UnitTests` and `IntegrationTests` targets (invoked via `./build.sh unit-tests integration-tests`) will be impacted: the move to `Candoumbe.Pipelines 3.x` implies a .NET 10 SDK jump and a rewrite of the build entry points.
