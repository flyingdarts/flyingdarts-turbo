.PHONY: format format-only lint lint-fix setup-turbo setup-beachball

restore:
	dotnet restore && \
	npm install 