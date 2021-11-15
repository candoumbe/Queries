# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
- Added `DateOnly` support ([#41](https://github.com/candoumbe/queries/issues/41))

## [0.3.0] / 2021-01-31
- Changed `SkipVariableDeclaration` from `bool` to a [`ParametrizationSettings`](src/Queries.Core/Renderers/ParametrizationSettings.cs) enum `[BREAKING]`
- Fixes incorrect rendering of [`CasesColumn`].
- Fixes `�rgumentOutOfRangeException` thrown when using `ClauseOperator.In` with `SelectQuery`
- Fixes `�rgumentOutOfRangeException` thrown when using `ClauseOperator.NotIn` with `SelectQuery`

## [0.2.0] / 2021-01-30
- Adds `Queries.EntityFrameworkCore.Extensions` to provide additional `MigrationOperation` implementations.
- Adds [`CreateViewMigrationOperation`](src/Queries.EntityFrameworkCore.Extensions/Operations/CreateViewMigrationOperation.cs) to create a SQL view.
- Adds [`DeleteMigrationOperation`](src/Queries.EntityFrameworkCore.Extensions/Operations/DeleteMigrationOperation.cs) to perform a DELETE statement.
- Fixes `ArgumentOutOfRangeException` thrown when using `ClauseOperator.In` with string values 

## [0.1.0] / 2021-01-08
- Initial release


