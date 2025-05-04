# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### 💥 Breaking changes
- Changed the output of `UniqueIdentifierValue` column when rendering Postgres queries from `uuid_generate_v4()` to `gen_random_uuid()`

### ✨ Improvements
- Improved `PrettyPrint` support
- Enhance column type conversion and implicit casting documentation and support

### 🧹 Housekeeping

- Updated `Candoumbe.Pipelines` to 0.7.0
- Added unit tests for `SelectIntoQuery` class
- Added documentation for `QueryWriter` class
- Removed direct dependency to `xunit` NuGet package in favor of transitive dependency brought by `FsCheck.Xunit`
- Replaced `Codecov.Tool` with `CodecovUploader` NuGet package

### 🖹 Documentation
- Added more C# documentation.
- Added specific documentation for parameterized queries usage with `CollectVariableVisitor`.

## [0.4.0] / 2021-11-16
### 🚀 New features
- Added `DateOnly` support ([#41](https://github.com/candoumbe/queries/issues/41))
- Added `TimeOnly` support ([#41](https://github.com/candoumbe/queries/issues/42))

## [0.3.0] / 2021-01-31
### ⚠️ Breaking changes

- Changed `SkipVariableDeclaration` from `bool` to a [`ParametrizationSettings`](src/Queries.Core/Renderers/ParametrizationSettings.cs) enum `[BREAKING]`

### 🛠️ Fixes
- Fixes incorrect rendering of [`CasesColumn`].
- Fixes `ArgumentOutOfRangeException` thrown when using `ClauseOperator.In` with `SelectQuery`
- Fixes `ArgumentOutOfRangeException` thrown when using `ClauseOperator.NotIn` with `SelectQuery`

## [0.2.0] / 2021-01-30
### 🚀 New features
- Adds `Queries.EntityFrameworkCore.Extensions` to provide additional `MigrationOperation` implementations.
- Adds [`CreateViewMigrationOperation`](src/Queries.EntityFrameworkCore.Extensions/Operations/CreateViewMigrationOperation.cs) to create a SQL view.
- Adds [`DeleteMigrationOperation`](src/Queries.EntityFrameworkCore.Extensions/Operations/DeleteMigrationOperation.cs) to perform a DELETE statement.

### 🛠️ Fixes
- Fixes `ArgumentOutOfRangeException` thrown when using `ClauseOperator.In` with string values 

## [0.1.0] / 2021-01-08
- Initial release

[Unreleased]: https://github.com/candoumbe/Queries/compare/0.4.0...HEAD
[0.4.0]: https://github.com/candoumbe/Queries/compare/0.3.0...0.4.0
[0.3.0]: https://github.com/candoumbe/Queries/compare/0.2.0...0.3.0
[0.2.0]: https://github.com/candoumbe/Queries/compare/0.1.0...0.2.0
[0.1.0]: https://github.com/candoumbe/Queries/tree/0.1.0