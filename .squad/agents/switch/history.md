📌 Imported from squad.-export on 2026-09-18T22:49:35.147Z. Portable knowledge carried over; project learnings from previous project preserved below.

# Project Context

- **Owner:** Cyrille NDOUMBE
- **Project:** MedEasy management platform
- **Stack:** .NET, Aspire, microservices, web frontend
- **Created:** 2026-07-14T01:20:55+0000

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

📌 Team update (2026-08-26T18:02:32+0000): Agenda.Frontend's pre-built image exits before HTTP because nginx, running as `nginx`, cannot create `/var/cache/nginx/client_temp`; nginx listens on `8080` while AppHost targets `3000`. Endpoint interpolation, quoted auth heredoc placeholders, and missing nginx API proxy are additional confirmed configuration gaps. Read-only diagnosis; no product files changed. — decided by Morpheus and Switch
📌 Team update (2026-08-26T18:02:32+0000): Corrected Agenda.Frontend's rootless nginx startup configuration. `nginx.conf` uses `/tmp` for PID and temp paths and stdout/stderr for logs; the Dockerfile creates and assigns `/tmp/nginx` while retaining `USER nginx`. Validation with `nginx:1.31-alpine` passed `nginx -t`, kept the container alive, and returned `OK` from `/health`. A full image build remains blocked earlier at `npm ci` with `ERR_SSL_CIPHER_OPERATION_FAILED`, unrelated to this change. — completed by Switch
