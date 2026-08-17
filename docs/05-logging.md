# Logging

## Technical Logs

- Serilog sink: SQL Server table `log.Logs`
- Async batching via `Serilog.Sinks.Async`
- Custom columns: TraceId, UserId, Culture, RequestPath, StatusCode, ElapsedMs, etc.
- Rolling file fallback when SQL is unavailable

## Audit Logs

Separate table `audit.AuditLogs` written by `AuditLogInterceptor`.

## Rules

- Structured logging only — no string interpolation in log messages
- Never log secrets (password, token, card number)
- Health check paths filtered from request logging noise

## Retention

`LogRetentionService` deletes rows older than `LogOptions.RetentionDays`.

## Correlation

`CorrelationIdMiddleware` sets `X-Correlation-Id` and enriches Serilog context.
