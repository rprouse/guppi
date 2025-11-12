# Guppi Constitution

## Core Principles

### I. Modular Internal Skills & DI
Every skill and feature is implemented as an internal class, registered via dependency injection (DI) and exposed through interfaces. All modules must be independently testable, documented, and have a clear purpose. New features are added by extending DI registration and implementing the required interfaces.

### II. CLI Interface (System.CommandLine & Spectre.Console)
All features are exposed via a command-line interface using System.CommandLine. Structured, rich output and logging are provided via Spectre.Console. Input is accepted via CLI arguments and options; output is delivered in both human-readable and JSON formats for interoperability and debugging.

### III. Test-First (NON-NEGOTIABLE)
Test-driven development is mandatory. All skills and services must be independently testable via interfaces and mocks. Tests must be written and approved before implementation. The Red-Green-Refactor cycle is strictly enforced for all code changes.

### IV. Integration Testing
Integration and contract tests are required for new skills/services, contract changes, inter-skill communication, and shared schemas. All critical interactions must be covered by automated tests. DI and interface-driven design must enable isolated integration tests.

### V. Observability & Simplicity
Structured, rich CLI output and logging are required for all features using Spectre.Console. Simplicity is prioritized: avoid unnecessary complexity (YAGNI). Versioning follows semantic rules (MAJOR.MINOR.PATCH). Text I/O and JSON output ensure debuggability and traceability.

## Additional Constraints
C# 12+ and .NET 9.0+ are required. All API keys and credentials must be managed securely and loaded via configuration. The project must remain cross-platform and support Windows, Linux, and macOS. All features must use async/await for asynchronous operations. Security and compliance standards must be followed for all integrations and dependencies. All output must use Spectre.Console for consistency.

## Development Workflow
All code changes require peer review and must pass CI test gates before merging. All new features/skills must be registered via DI and implement the required interfaces. Deployment is managed via NuGet packages. All features must be independently testable and deliver value incrementally. Documentation must be updated with every feature addition or change.

## Governance
This constitution supersedes all other practices. Amendments require documentation, approval, and a migration plan. All pull requests and reviews must verify compliance with these principles. Any complexity must be justified and documented. Runtime development guidance is provided in the README and supporting documentation.

**Version**: 1.1.0 | **Ratified**: 2025-11-12 | **Last Amended**: 2025-11-12

<!--
Sync Impact Report
Version change: 1.0.0 → 1.1.0
Modified principles: Library-First → Modular Internal Skills & DI, CLI Interface → CLI Interface (System.CommandLine & Spectre.Console), Observability & Simplicity updated
Added sections: C# 12+, async/await, DI registration workflow
Removed sections: None
Templates requiring updates: ✅ plan-template.md, ✅ spec-template.md, ✅ tasks-template.md
Follow-up TODOs: None
-->
