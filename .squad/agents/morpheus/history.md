📌 Imported from squad.-export on 2026-09-18T22:49:35.147Z. Portable knowledge carried over; project learnings from previous project preserved below.

# Project Context

- **Owner:** Cyrille NDOUMBE
- **Project:** MedEasy management platform
- **Stack:** .NET, Aspire, microservices, web frontend
- **Created:** 2026-07-14T01:20:55+0000

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

📌 Team update (2026-08-16T00:00:00Z): agenda-api startup abort is caused by Npgsql's default `GssEncryptionMode=Prefer` on the Paramore.Brighter outbox path (`dlopen` of missing `libgssapi_krb5.so.2`), not by the base image — both API and worker images are Ubuntu 24.04. Fix is a shared `Agenda.ServiceDefaults` helper forcing `GSS Encryption Mode=disable` at all three entry points; krb5 in the image was rejected. `apphost.mts` pins pre-built image tags, so a rebuild and tag bump are mandatory. — decided by Squad (Coordinator), from Morpheus and Trinity

📌 Team update (2026-08-16T00:00:00Z): Trinity landed the fix — `WithGssDisabled()` in `Agenda.DataStores.Postgres` (not `Agenda.ServiceDefaults`), applied to `AddCustomBrighter`, API `Program.cs` and Migrator `Program.cs`; `ConfigureDataSource` blocks removed; `Npgsql` pinned to `10.0.3`. API + Migrator build clean. The remaining work is operational and on your side: rebuild the images and bump the pinned tag in `apphost.mts`, then verify `agenda-api` actually starts. — decided by Trinity

📌 Team update (2026-08-26T18:02:32+0000): Agenda.Frontend's pre-built image exits before HTTP because nginx, running as `nginx`, cannot create `/var/cache/nginx/client_temp`; nginx listens on `8080` while AppHost targets `3000`. Endpoint interpolation, quoted auth heredoc placeholders, and missing nginx API proxy are additional confirmed configuration gaps. Read-only diagnosis; no product files changed. — decided by Morpheus and Switch