# Testing

## Projects

| Project | Scope |
| --- | --- |
| `Domain.UnitTests` | Aggregate and value object rules |
| `Application.UnitTests` | Handlers, validators, fakes (no mock framework) |
| `ArchitectureTests` | Layer dependency rules (NetArchTest) |
| `Api.IntegrationTests` | HTTP + Testcontainers SQL Server |

## Application Test Helpers

- `FakeCurrentUser`
- `FakeTimeProvider`
- `FakeCacheService`
- `FakeUnitOfWork`

## Integration Tests

- Shared `DatabaseFixture` (one container per test run)
- `IntegrationTestBase` starts a transaction per test and rolls back on dispose
- `MaxRetryCount = 0` in test environment (retry + manual transaction conflict)

## Frontend

```bash
cd frontend
npm run test
npm run test:e2e
```

MSW mocks API in unit tests; Playwright covers login -> product -> language switch -> F5 session persistence.
