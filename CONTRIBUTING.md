# Contributing to AiryPay

### 🐛 Reporting Issues
- Use the [GitHub Issues](../../issues) page to report bugs or request features.
- Provide clear steps to reproduce bugs.
- Include environment details (OS, .NET version, DB, etc.).

### 🔧 Submitting Changes

1. **Fork** the repository and create your branch from `main`.
```bash
git checkout -b feature/your-feature-name
```
2. Make your changes, following the coding standards (see below).
3. Run tests locally to ensure nothing breaks:
```bash
dotnet test
```
4. Commit with a clear message.

### 🧑‍💻 Code Style

- Use C# 12 / .NET 8 features where appropriate.
- Follow existing conventions in the project.
- Keep methods small and focused.
- Use meaningful names (no DoStuff()).
- Format your code with dotnet format before committing.

### ✅ Before submitting a PR, make sure:

- Code compiles without errors.
- All tests pass.
- New/modified code is covered by tests where possible.
- You’ve updated docs or comments if behavior has changed.

### 📦 Dependencies

Avoid introducing new dependencies unless necessary.
If a new dependency is required, explain why in your PR.

### Questions?

Feel free to open a discussion in GitHub Discussions or reach out via issues.
