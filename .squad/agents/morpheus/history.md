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
📌 Learning (2026-09-28T00:00:00Z): Le build "CI as code" de Queries ne référence aucun paquet `Nuke.*` directement — le moteur (12 paquets `Nuke.* 9.0.4`) arrive uniquement en transitif via `Candoumbe.Pipelines 2.0.1`. Conséquence : toute migration Nuke → Fallout est pilotée par la montée de `Candoumbe.Pipelines`, dont la `3.0.0` (première release Fallout, `Fallout.Common 10.3.49`) cible `net10.0` exclusivement — alors que `global.json` pinne le SDK `9.0.317` et que `build/Queries.ContinuousIntegration.csproj` cible `net8.0`. Seul le générateur `[GitHubActions]` est utilisé (3 workflows générés : integration, delivery, nightly-manual) ; `build-release.yaml` est un pipeline Azure DevOps écrit à la main en tâches `DotNetCoreCLI@2`, sans lien avec Nuke. 16 targets exposés (`Clean`, `Restore`, `Compile` par défaut, `UnitTests`, `IntegrationTests`, `MutationTests`, `ReportUnitTestCoverage`, `ReportIntegrationTestCoverage`, `Pack`, `Publish`, `Changelog`, `Feature`, `Chore`, `Coldfix`, `Hotfix`, `Release`). Tâche produite : issue #402. Aucun fichier de build modifié. — Morpheus

## 2026-09-28T00:00:00Z — Issue #402 translated to English

**Requested by:** Cyrille NDOUMBE

**Standing directive (new):** Every GitHub issue in `candoumbe/Queries` — title AND body — must always be written in English, because the repository is public. Recorded in `.squad/decisions/inbox/copilot-directive-issues-in-english.md`.

**Action:** Issue #402, which I had originally authored in French, was fully translated.
- Title: `Migrer le CI as code de Nuke vers Fallout` -> `Migrate CI-as-code from Nuke to Fallout`
- Body: translated to natural technical English via `gh issue edit --body-file`.
- Preserved verbatim: all Markdown structure, URLs, shell commands, the ASCII dependency graph, target names (`UnitTests`, `IntegrationTests`, `MutationTests`, `Pack`, `Publish`, ...), package names, file paths, CLI parameters, C# type names (`EnhancedNukeBuild`, `IGitFlowWithPullRequest`, ...), secret names, version numbers and the `FALLOUT004` warning code.
- Integrity check before/after: 6 `##` headings, 16 `###` headings, 30 table rows, 23 checkboxes, 5 `---` separators, 4 code fences, 2 block quotes, 7 URLs (identical set) — all counts match.
- No label, assignee or milestone touched. No commit, no push.

**Takeaway:** default to English for all public-repo GitHub artifacts (issues, PR titles/descriptions, commit messages) from now on.

## 2026-09-28T00:00:00Z — Issue #402 reformatted to the repo `task`/`chore` convention

**Requested by:** Cyrille NDOUMBE

**Investigation:** No `.github/ISSUE_TEMPLATE/` and no `.github/issue_template.md` exist (`.github/` holds only `agents/`, `skills/`, `workflows/`). `CONTRIBUTING.md` documents a bug-report prose template only — nothing for tasks. GraphQL `issueType` is `null` on all 17 issues, so GitHub native issue types are NOT enabled on this repo. The convention therefore lives in the issue corpus itself.

**Convention found (evidence):**
- Title: emoji prefix `💪🏾 ` + imperative verb, no Conventional-Commits prefix — #400, #327, #279, #182, #178, #162, #44. The older `[Task] 💪🏾` form (#165, #9) is deprecated.
- Body H2 (bold inside the heading): `## **Describe the work that need to be done**` then `## **Acceptance criteria**` — #400, #279. Exactly two H2 levels; all detail lives under `###` sub-headings.
- Acceptance criteria are `- [ ]` checkboxes, last section, with `CHANGELOG.md updated` as a recurring item — #400.
- Labels: `task` + `type:chore` — #400 (most recent and closest analogue); `technical-debt🛠️` / `housekeeping 🧹` are optional extras (#327, #162).

**Action on #402:**
- Title: `Migrate CI-as-code from Nuke to Fallout` -> `💪🏾 Migrate CI-as-code from Nuke to Fallout`.
- Body restructured from 6 H2 to 2 H2 (`Describe the work that need to be done` / `Acceptance criteria`); former sections Context, Scope, Migration plan, Risks, Rollback demoted to `###`; the 6 numbered migration steps demoted from `###` to bold paragraph leads.
- Labels added: `task`, `type:chore` (both already existed). No label/type/milestone created, assignee untouched, nothing committed or pushed.

**Integrity check:** 30 table rows identical, 4 code fences identical, 6 unique URLs identical (byte-for-byte set diff), 225 distinct backticked technical tokens before == 225 after with an empty `comm` diff both ways (zero identifier lost). Checkboxes 23 -> 24: the single addition is `CHANGELOG.md updated`, adopted from #400 convention. Published body matches the source file exactly except one trailing blank line added by GitHub.

**Takeaway:** when a repo has no issue template, derive the format from the most recent well-formed issue of the same type rather than inventing one — and cite the issue numbers as evidence.
