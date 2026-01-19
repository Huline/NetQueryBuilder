# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- ASP.NET Core Razor Pages integration (`NetQueryBuilder.AspNetCore`)
  - Tag Helpers for entity selection, property selection, and condition building
  - View Components for query results, pagination, and expression display
  - `NetQueryPageModelBase` base class for simplified page implementation
  - Session-based state management with automatic cleanup
  - Embedded CSS served from static web assets
- WPF UI components (`NetQueryBuilder.WPF`)
  - Material Design-inspired query editor
  - Condition builder with visual interface
  - Result display with pagination support
- Comprehensive NuGet package metadata across all packages

### Changed
- Improved Blazor component architecture with better re-rendering optimization
- Enhanced expression building with caching for improved performance
- Refactored QueryBuilder and related components for better UI/UX

### Fixed
- Condition handling improvements
- ASP.NET Core design fixes
- WPF component fixes

## [1.0.0] - 2025-01-01

### Added
- Initial release of NetQueryBuilder ecosystem
- **NetQueryBuilder** (Core)
  - Type-safe expression tree-based query building
  - Extensible operator system (Equals, NotEquals, Contains, Like, GreaterThan, LessThan, Between, In, IsNull)
  - Property path navigation for complex object graphs
  - `IQuery` interface for query execution
  - `IQueryConfigurator` factory pattern
  - `BlockCondition` and `SimpleCondition` for hierarchical condition structures
- **NetQueryBuilder.EntityFramework**
  - Entity Framework Core integration
  - `EfQueryConfigurator<TContext>` for DbContext-based queries
  - Support for navigation properties and relationships
  - Efficient SQL query translation
- **NetQueryBuilder.Blazor**
  - MudBlazor-based UI components
  - `QueryBuilderContainer` root component
  - `QueryBuilder<T>` generic query builder
  - `QueryResultTable` with pagination
  - Event system for condition changes and query execution
- Multi-framework support (.NET 6.0, 8.0, 9.0, .NET Standard 2.1, .NET Framework 4.8)
- Dual licensing (MIT for non-commercial, Commercial license available)

[Unreleased]: https://github.com/remihenache/NetQueryBuilder/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/remihenache/NetQueryBuilder/releases/tag/v1.0.0
