.PHONY: format format-only lint lint-fix setup-turbo setup-beachball

build:
	turbo run build

restore:
	dotnet restore && \
	flutter pub get && \
	cargo fetch && \
	npm install  

build:
	dotnet build && \
	turbo run generate && \
	cargo build --workspace && \
	npm run build:flutter:mobile && \
	npm run build:angular:fd-app

run-flutter-app:
	flutter run --flavor dev --target apps/frontend/flutter/flyingdarts_mobile/lib/main_dev.dart

upgrade-flutter-packages:
	sh scripts/flutter/workspace/upgrade-flutter-packages.sh

clean-caches:
	npm cache clean --force && \
	pod cache clean --all && \
	dart pub cache clean

clean: 

	sh scripts/clean-build-folders.sh 

ci:
	make clean-caches && \
	make clean && \
	make ci-node && \
	make ci-dotnet && \
	make ci-flutter && \
	make ci-rust

ci-node:
	rm -rf node_modules && \
	npm install

ci-dotnet:
	dotnet clean && \
	dotnet restore && \
	dotnet build

ci-flutter:
	flutter clean && \
	flutter pub get && \
	turbo run generate && \
	npm run build:flutter:mobile

ci-rust:
	cargo clean && \
	cargo clippy --workspace -- -D warnings && \
	cargo fmt --all -- --check && \
	cargo build --workspace --release

run-angular-app:
	npm run start:angular:fd-app