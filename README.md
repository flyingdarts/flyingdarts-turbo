# Flying Darts Turbo Monorepo

## Recommended VS Code Extensions

To ensure a smooth and productive development experience, we recommend the following VS Code extensions. When you open this repository, VS Code will prompt you to install these if you don't have them already.

### Rust
- **[rust-lang.rust-analyzer](https://marketplace.visualstudio.com/items?itemName=rust-lang.rust-analyzer)** — Advanced Rust language support (syntax, completion, go-to-definition, refactoring, and more).
- **[serayuzgur.crates](https://marketplace.visualstudio.com/items?itemName=serayuzgur.crates)** — Crate version management and dependency insights for Cargo.toml.
- **[vadimcn.vscode-lldb](https://marketplace.visualstudio.com/items?itemName=vadimcn.vscode-lldb)** — Powerful Rust/C debugging support.
- **[tamasfe.even-better-toml](https://marketplace.visualstudio.com/items?itemName=tamasfe.even-better-toml)** — Syntax highlighting and validation for TOML files (Cargo.toml, rustfmt.toml, etc.).

### Dart & Flutter
- **[Dart](https://marketplace.visualstudio.com/items?itemName=Dart-Code.dart-code)** — Dart language support.
- **[Flutter](https://marketplace.visualstudio.com/items?itemName=Dart-Code.flutter)** — Flutter development tools and UI support.

### Angular
- **[Angular Language Service](https://marketplace.visualstudio.com/items?itemName=Angular.ng-template)** — Angular template and TypeScript support.

### General / Monorepo
- **[EditorConfig for VS Code](https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig)** — Consistent coding styles across editors and IDEs.
- **[ESLint](https://marketplace.visualstudio.com/items?itemName=dbaeumer.vscode-eslint)** — Linting for JavaScript/TypeScript projects.

---

For more details, see `.vscode/extensions.json` in the repository.

## Rust Development Tools

This project uses several Rust development tools to ensure code quality and consistency across all developers and environments.

### rust-toolchain.toml

This file specifies the Rust toolchain version and required components for the project. It ensures all developers and CI environments use the same Rust version and tools, leading to consistent builds and a smooth onboarding experience.

#### Contents
- **channel**: Sets the Rust version (e.g., `stable`).
- **components**: Ensures tools like `rustfmt`, `clippy`, `rust-analyzer`, and `rust-src` are installed.
- **targets**: Pre-installs support for building on Linux and macOS.

#### Why use it?
- Guarantees everyone uses the same Rust version and tools
- Simplifies onboarding and CI setup
- Ensures IDE features work out of the box

#### Usage
When you enter the project directory, `rustup` will automatically use the specified toolchain. To install all components and targets:

```sh
rustup show
rustup component add rustfmt clippy rust-analyzer rust-src
rustup target add x86_64-unknown-linux-gnu x86_64-apple-darwin
```

### .clippy.toml

This file configures [Clippy](https://github.com/rust-lang/rust-clippy), Rust's linter, to enforce code quality and style standards across the project.

#### Key Settings
- **Performance**: Warns on unsafe code, missing docs, and missing trait implementations.
- **Complexity**: Sets thresholds for cyclomatic and cognitive complexity.
- **Style**: Enforces naming, documentation, and argument count rules.
- **Correctness**: Disallows certain methods and types for safety.
- **Suspicious**: Warns about potentially dangerous type casts.

#### Why use it?
- Maintains high code quality
- Enforces consistent documentation and style
- Prevents common mistakes and unsafe code

#### Usage
Clippy runs automatically on save in VS Code (with rust-analyzer) or manually:

```sh
cargo clippy --all-targets --all-features -- -D warnings
```

### rustfmt.toml

This file configures [rustfmt](https://github.com/rust-lang/rustfmt), Rust's official code formatter, to enforce a consistent code style across the project.

#### Key Settings
- **Line width**: 100 characters
- **Indentation**: 4 spaces
- **Braces and blocks**: Consistent placement and style
- **Imports**: Sorted and grouped
- **Trailing commas**: Used in multiline lists for cleaner diffs

#### Why use it?
- Ensures code looks the same for everyone
- Improves readability and maintainability
- Reduces noise in code reviews

#### Usage
Format your codebase automatically with:

```sh
cargo fmt
```

This will apply the rules in `rustfmt.toml` to all Rust files in the project.
