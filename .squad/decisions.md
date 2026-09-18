# Squad Decisions

## Active Decisions

### 2026-09-14T00:00:00Z: API pagination convention — all collection endpoints must be paginated
**By:** Cyrille NDOUMBE (via Copilot)
**What:** Regardless of language/stack, any API endpoint returning a list/collection of resources must return paginated content, unless the user explicitly asks for no pagination. Applies to all sub-squads (agenda, document, physiotrack) and the root project.
- Query params: `page` (1-based) and `pageSize`.
- Server enforces a default page size (50) when the client doesn't specify one, and a configurable max page size (200) the client cannot exceed — page size is always the backend's responsibility, never fully client-controlled.
- Response body shape: `{ "items": [...], "links": { "prev": {"href", "rel"}, "next": {...}, "first": {...}, "last": {...} }, "total": <int> }`.
- Response headers: `Link` (prev/next, RFC 5988 style), `X-Count` (items in current page), `X-Total-Count` (total items across all pages).
- HTTP status: `200 OK` when the result fits in a single page, `206 Partial Content` when the result spans multiple pages.
- Standard HTTP status families otherwise apply: 2XX/3XX success, 4XX client error (correctable/resubmittable), 5XX server error.
- When an agent designs a collection endpoint and pagination parameter names/index start aren't already fixed by this record or an existing project convention, the agent must ask the user before proceeding rather than guessing.
**Why:** User directive to standardize collection API responses across every language/stack used in the monorepo.

### 2026-08-14T17:05:45+0000: Squad repository documentation language
**By:** Cyrille NDOUMBE (via Copilot)
**What:** All documents related to Squad's operation that are stored in the repository must be written in English.
**Why:** User directive to keep Squad's repository-based operational documentation consistent and accessible to the team.

### 2026-08-16T00:00:00Z: agenda-api startup failure — root cause is GssEncryptionMode.Prefer on the Brighter path (consolidated)
**By:** Trinity, Morpheus, Fact Checker, Scribe
**What:** Supersedes the 2026-08-13 dual-root-cause hypothesis (image/runtime mismatch plus apphost environment interpolation). The confirmed cause is narrower: Paramore.Brighter's Postgres outbox (API only) opens an `NpgsqlConnection` on the raw connection string; the default `GssEncryptionMode=Prefer` triggers `dlopen` of `libgssapi_krb5.so.2`, which is absent from the image, producing a native `abort()`. The "Azure Linux base image" angle is refuted — both the API and worker images are Ubuntu 24.04 and neither ships the library. The Migrator survives because it normalizes the connection string via `configureSettings` (`Program.cs` L18) and is published self-contained (`Build.cs` L372); the API only normalizes the EF data source (`Program.cs` L46), leaving the Brighter path (`ServiceCollectionExtensions.cs` L143) uncovered.
**Why:** The original hypothesis correctly pointed at a build-vs-container behavioral difference and at connection-string handling, but attributed it to the base image. Morpheus validated the actual mechanism empirically from container logs and an image/environment diff; Trinity confirmed there is no Kerberos or Negotiate authentication anywhere in the code, so GSSAPI is pulled in purely by the Npgsql default.

### 2026-08-16T00:00:00Z: Remediation — normalize ConnectionStrings:postgres via a shared helper in Agenda.DataStores.Postgres (consolidated)
**By:** Trinity, Morpheus, Fact Checker, Scribe
**What:** Replaces the 2026-08-13 two-step sequencing (publish/runtime settings first, connection string second). Now a single step: a shared helper normalizes `ConnectionStrings:postgres` by forcing `GSS Encryption Mode=disable`, applied at all three entry points — API/EF (`Agenda.API/Program.cs`, via `configureSettings`), API/Brighter (`Agenda.API/ServiceCollectionExtensions.cs`, `AddCustomBrighter` — the blocking call site), and Migrator (`Agenda.Migrator/Program.cs`, replacing the string concatenation). The helper landed in `Agenda.DataStores.Postgres` (`NpgsqlConnectionStringExtensions.cs`, `WithGssDisabled()`), **not** in `Agenda.ServiceDefaults` as originally sketched; `Npgsql` is now an explicit `PackageReference` there, pinned centrally at `10.0.3` in `Directory.Packages.props`. The now-redundant `ConfigureDataSource` blocks were removed from both API and Migrator. Installing krb5 in the container image was considered and rejected. Deployment constraint: `apphost.mts` pins pre-built images (tag `0.2-scalar-fails-to-start-in-azurelinux-image.fe0fa8d`), so a rebuild and tag bump are mandatory — a code change alone will not reach the running containers.
**Why:** The earlier sequencing existed to de-risk an unknown runtime layer; with the cause now pinned to a single Npgsql default, isolating the execution layer first is unnecessary. Normalizing at one shared point removes the whole class of failure instead of patching one call site and keeps the three consumers consistent. Adding krb5 to the image would grow it and mask the real issue, since the application never uses Kerberos authentication. On placement: `Agenda.ServiceDefaults` is the Aspire shared project — deliberately storage-agnostic and still targeting `net9.0` while its consumers target `net10.0` — so adding a PostgreSQL driver there would force every future service, Postgres-backed or not, to carry Npgsql. `Agenda.DataStores.Postgres` is already referenced by both `Agenda.API` and `Agenda.Migrator` and already pulled Npgsql transitively via `Paramore.Brighter.Outbox.PostgreSql`, so the helper adds no new dependency surface. Removing `ConfigureDataSource` keeps a single source of truth — the normalized connection string, which is what Aspire uses to build the `NpgsqlDataSource`; keeping both would apply the same setting twice at two different layers.

### 2026-08-16T00:00:00Z: Side bugs logged for separate follow-up
**By:** Squad (Coordinator), from Morpheus and Trinity
**What:** Three unrelated defects surfaced during the investigation and are tracked separately, not as part of the GSSAPI fix:
1. `apphost.mts` L63 — `AGENDA_AUTH_AUTHORITY` is serialized as `"[object Object]"`.
2. `Agenda.API` `appsettings.json` — key `ApiOptions:Messaging` while the binder expects `ApiOptions:MessagingOptions`.
3. `TimedOutboxSweeper` throws on the timer thread; the exception is never observed.
**Why:** Keeping the GSSAPI fix scoped makes it reviewable and independently verifiable; the side bugs have their own risk profiles and test surfaces.

### 2026-07-14T01:20:55+0000: User directive
**By:** Cyrille NDOUMBE (via Copilot)
**What:**
- All commits performed by Squad agents must follow the Conventional Commits specification.
- All commit titles and commit descriptions must be written in English.
- All developer-facing documentation must be written in English because the repository is public.
**Why:** Team operating rule provided by the user.

### 2026-08-16T21:28:55+0000: Document build migrated from Nuke to Fallout
**By:** Morpheus (requested by Cyrille NDOUMBE)
**What:** Ported the active build surface in `document` to Fallout on `feature/migrate-nuke-to-fallout`: wrappers, pipeline project and API, central build dependencies, dotnet tools, generated `.fallout` configuration, and GitHub Actions cache/configuration references.
**Why:** Align `document` with the completed migration in `agenda` while preserving document-specific migration targets and the existing `NUKE_ENTERPRISE_TOKEN` feed option. Agenda-only frontend, container-image, and GHCR publishing behavior was not copied because the corresponding projects are absent from `document`.
**Validation:** `./build.sh --help`; `./build.sh Compile --skip Format`; `dotnet build build/Documents.Pipelines.csproj --no-restore` all succeeded. Remaining Nuke references are intentionally limited to the historical `NUKE_ENTERPRISE_TOKEN` and `nuke-enterprise` feed identifiers.

### 2026-08-16T21:28:55+0000: Port Agenda Scalar and Serilog fixes to Documents
**By:** Trinity (requested by Cyrille NDOUMBE)
**What:** Ported the Agenda API fixes to `document`: configure Serilog from `builder.Configuration` and registered services, add the Scalar trailing-slash asset redirect, and cover Scalar, OpenAPI JSON, and the absence of Swagger UI with integration tests. `Scalar.AspNetCore` was already at 2.16.20 in Documents, so no dependency change was needed.
**Why:** Keep Documents API behavior aligned with the verified Agenda implementation while preserving the existing user migration changes in the working tree.

### 2026-08-25T00:00:00Z: User directive — atomic commits with mandatory description
**By:** Cyrille NDOUMBE (via Copilot)
**What:** Reinforces the 2026-07-14 Conventional Commits directive. Whenever asked to commit/save current changes, Squad agents must:
- Split changes into atomic commits (one logical change per commit).
- Follow the Conventional Commits format for every commit title.
- Always include a short body/description explaining what changed in each commit.
**Why:** Team operating rule provided by the user to keep commit history clean, reviewable, and self-explanatory.

### 2026-08-26T18:02:32+0000: Agenda.Frontend startup diagnostic — runtime image and orchestration gaps
**By:** Morpheus, Switch
**What:** The pre-built `ghcr.io/candoumbe/agenda.frontend:0.3-alpha` image starts nginx as user `nginx`, but nginx cannot create `/var/cache/nginx/client_temp`; it exits with code 1 before opening HTTP. The AppHost additionally maps target port `3000` while nginx listens on `8080`. Endpoint-object interpolation serializes as `[object Object]` in `API_HTTP` and `AGENDA_AUTH_AUTHORITY`; the frontend entrypoint emits literal authentication placeholders because its heredoc is quoted; and nginx has no API proxy configuration. This was a read-only diagnosis; no product files were changed.
**Why:** Morpheus and Switch independently inspected the Agenda.Frontend startup path and the container runtime to establish the blocking cause and the configuration gaps that must be resolved before the frontend can become reachable.

### 2026-09-12T11-10-29: pyproject.toml as the single source of truth for version in physiotrack/Makefile
**By:** Morpheus
**What:** pyproject.toml as the single source of truth for version in physiotrack/Makefile
**Why:** Version retrieval in `physiotrack/Makefile` was updated to use `pyproject.toml` (`[project].version`) as the single source of truth. The command uses `python3` with the standard `tomllib` module (Python 3.11+), with an automatic fallback to a more flexible `sed` expression.

### 2026-09-12T11-26-00: Dynamic CHANNEL computation and Docker tags in physiotrack/Makefile
**By:** Morpheus
**What:** Dynamic CHANNEL computation and Docker tags in physiotrack/Makefile
**Why:** `CHANNEL` derivation and Docker tag formatting in `physiotrack/Makefile` were updated based on the current Git branch:
- `main`: `CHANNEL` empty; tags `major.minor.patch`, `major.minor`, `major`
- `develop`: `CHANNEL=alpha`; tags `major.minor.patch-alpha`, `major.minor-alpha`, `major-alpha`
- `release/*`: `CHANNEL=rc`; tags `major.minor.patch-rc`, `major.minor-rc`, `major-rc`
- Other branch: `CHANNEL` in lower-kebab-case; tags `major.minor.patch-{channel}`, etc.

### 2026-09-12T11-30-01: Exclusive use of the Makefile in ci.yml for version and Docker tags
**By:** Morpheus
**What:** Exclusive use of the Makefile in ci.yml for version and Docker tags
**Why:** Updated the GitHub Actions workflow `physiotrack/.github/workflows/ci.yml` to call `Makefile` targets directly (`make check-version`, `make docker-build`, `make docker-tags`, `make docker-push`) passing `BRANCH`, removing the manual bash `Resolve version and channel` step. The Makefile is now the single source of truth for version and Docker tag computation.

### 2026-09-12T00:00:00Z: All team-member exchanges must be logged in English
**By:** Cyrille NDOUMBE (via Copilot)
**What:** All exchanges between squad members (agent-to-agent communication, logged decisions, orchestration-log entries, session logs, history entries) must be recorded in English, since this is a public repository.
**Why:** User directive — extends the 2026-08-14 documentation-language rule to cover all inter-agent communication artifacts, not just repository documentation.

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
