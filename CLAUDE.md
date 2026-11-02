# Borrow Ease – Loan App API

.NET 6 backend for Borrow Ease loans: customers, loan requests/approvals, repayments, reports, and third-party integrations. Also hosts the email-sending endpoints used by the user-management service.

## Projects (`TrivisterLoanApplication.sln`)

| Project | Role |
|---|---|
| `Trivista.LoanApp.Api` | Host: `Program.cs`, DI setup in `ServiceCollection/` (auth, CORS, Swagger, logging, options), email templates. Prod: `https://loan.borrow-ease.net/`. |
| `Trivista.LoanApp.ApplicationCore` | Everything else: features, EF Core 7 `TrivistaDbContext` (`Data/`), entities, integrations, background jobs, webhooks. |
| `Trivista.LoanApp.ApplicationCore.UnitTest` / `.IntegrationTest` | xUnit + Moq. |

## Conventions

- Endpoints are **Carter** modules, one file per feature in `ApplicationCore/Features/<Area>/`: an `ICarterModule` class mapping the route, the command/query `record`, FluentValidation validator and MediatR 12 handler.
- Handlers return LanguageExt `Result<T>`; endpoints convert with `result.ToOk(...)`.
- `Controllers/` only contains the template `WeatherForecastController` — don't add MVC controllers.
- Enums serialize as strings (`JsonStringEnumConverter`).
- Database migrations are applied automatically at startup (`app.MigrateDatabase()`).
- Background work uses Coravel (`SendMailHostedService` every 30 seconds).
- MassTransit/RabbitMQ code exists (`Infrastructure/MessageBroker`, `Extensions/ConfigureServiceBus.cs`, `RabbitMqSettings`) but is **not used**. Don't build on it; services talk over HTTP.

## Integrations (`ApplicationCore/Infrastructure/Http`, `Webhook/`)

Remita (`RemittaService`), Recova (mandates/repayment webhooks), Indicina and MyBankStatement (statement analysis), Smile ID (identity), Paystack (payments/BVN), Elastic Email / SendGrid / FluentEmail (mail). Base URLs and keys are in `appsettings.<env>.json`.

## Auth & the user-management service

- JWT bearer auth (`ServiceCollection/AuthenticatinService.cs`): authority = `IdentityOptions:IdentityUrl`, audience `travisterApiLoanApi`, roles from the `role` claim.
- Tokens come from the Duende IdentityServer in `C:\Projects\TrivistaApps\borrow-ease-usermanagement` (`https://idp.borrow-ease.net/`).
- That service calls this one: `POST /addCustomer` during user registration (and `addRole`). Changing the `AddCustomerCommand` shape breaks registration there.
- `addCustomer` is delivered from user management's outbox and may be retried, so it must stay idempotent: an existing customer Id returns success.
- Production domain is `borrow-ease.net` (hyphenated). Old `borrowease.net` / `*.trivistang.com` hosts are dead.

## Commands

```bash
dotnet build TrivisterLoanApplication.sln
dotnet test Trivista.LoanApp.ApplicationCore.UnitTest
dotnet run --project Trivista.LoanApp.Api      # or: tye run (port 12345)
dotnet ef migrations add <Name> --project Trivista.LoanApp.ApplicationCore --startup-project Trivista.LoanApp.Api
```

Environments: `Local`, `Development`, `Staging`, `Production`. Swagger UI is enabled in all of them at `/swagger`. Deployed as a Docker image (`Trivista.LoanApp.Api/Dockerfile`).

## Git workflow

- Every fix goes on its own new branch — one fix per branch, never several unrelated fixes together.
- Always branch off an up-to-date `main` (`git fetch` then fast-forward `main` to `origin/main` first). Never branch off another feature/fix branch.

## Cautions

- appsettings files contain real production secrets (DB, payment/KYC provider keys). Don't copy them into code, logs or messages.
- Integrations hit real providers (Paystack, Remita, Recova) — don't call them with production config while testing.
