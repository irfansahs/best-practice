# ADR-001: Custom CQRS Dispatcher Instead of MediatR

## Status

Accepted

## Context

MediatR v13+ requires a commercial license for many teams.

## Decision

Implement a lightweight `IDispatcher` with `IPipelineBehavior` chain (~120 lines) and Scrutor-based handler registration.

## Consequences

- Zero licensing risk
- Full control over behavior order
- Handlers remain standard interfaces, easy to test
