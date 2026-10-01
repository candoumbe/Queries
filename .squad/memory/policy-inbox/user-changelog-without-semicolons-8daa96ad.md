---
id: 8daa96ad-27de-4c1f-a4aa-2ad67820c30a
class: POLICY
loadGuidance: [ALWAYS]
title: "Changelog without semicolons"
author: "user"
createdAt: 2026-10-01T11:16:29.001Z
metadata: {}
---

### 2026-10-01: User directive
**By:** user (via Copilot)
**Rule:** Never use the `;` character when modifying `CHANGELOG.md`, because it is not properly escaped by `./build.sh` commands.
**Why:** Build commands in this repository may interpret this character incorrectly.