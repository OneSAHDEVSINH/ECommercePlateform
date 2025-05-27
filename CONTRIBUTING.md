# Contributing to ECommercePlatform

Thank you for your interest in contributing to our E-Commerce Platform! This document provides guidelines and instructions for contributing to this project.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Environment Setup](#development-environment-setup)
- [Coding Standards](#coding-standards)
- [Pull Request Process](#pull-request-process)
- [Branch Naming Convention](#branch-naming-convention)
- [Commit Message Guidelines](#commit-message-guidelines)
- [Testing](#testing)
- [Documentation](#documentation)

## Code of Conduct

By participating in this project, you agree to abide by our [Code of Conduct](CODE_OF_CONDUCT.md). Please ensure all interactions are respectful, inclusive, and professional.

## Getting Started

1. Fork the repository
2. Clone your fork: `git clone https://github.com/your-username/ECommercePlatform.git`
3. Add the upstream repository: `git remote add upstream https://github.com/OneSAHDEVSINH/ECommercePlatform.git`
4. Create a new branch for your contribution (see [branch naming convention](#branch-naming-convention))
5. Make your changes
6. Submit a pull request

## Development Environment Setup

### Prerequisites
- .NET SDK (latest stable version)
- Node.js (LTS version)
- Visual Studio 2022 or Visual Studio Code
- SQL Server (or SQL Server Express)

### Setup Steps
1. Restore .NET dependencies: `dotnet restore`
2. Install frontend dependencies: `npm install` (from the client-side directory)
3. Set up the database:
   - Update connection string in `appsettings.json`
   - Run migrations: `dotnet ef database update`
4. Start the application:
   - Backend: `dotnet run`
   - Frontend: `npm start` (from the client-side directory)

## Coding Standards

### C# Code Style
- Follow Microsoft's [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use nullable reference types
- Prefer async/await over direct Task manipulation
- Write unit tests for business logic
- Use dependency injection for services

### TypeScript/JavaScript
- Use TypeScript for new frontend features
- Follow the existing project structure
- Use ES6+ features
- Adhere to the ESLint configuration
- Write component tests where applicable

### CSS/SCSS
- Follow BEM naming convention
- Organize styles according to component structure
- Minimize global styles

## Pull Request Process

1. Ensure your code adheres to the project's coding standards
2. Update documentation as necessary
3. Include relevant tests for your changes
4. Ensure all tests pass
5. Update the README.md with details of changes if applicable
6. The PR should target the `develop` branch (not `main`)
7. Request review from at least one maintainer
8. PRs require approval from at least one maintainer before merging

## Branch Naming Convention

Use the following format for branch names:
- `feature/short-description` - for new features
- `bugfix/short-description` - for bug fixes
- `hotfix/short-description` - for critical fixes to production
- `docs/short-description` - for documentation updates
- `refactor/short-description` - for code refactoring
- `test/short-description` - for adding or modifying tests

## Commit Message Guidelines

Follow the [Conventional Commits](https://www.conventionalcommits.org/) specification:
