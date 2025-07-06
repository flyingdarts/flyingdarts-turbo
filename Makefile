.PHONY: format format-only lint lint-fix

build:
	turbo run build

clean:
	sh scripts/clean-build-folders.sh

restore:
	sh scripts/restore.sh

clean-beachball:
	sh scripts/clean-beachball.sh

setup-beachball:
	sh scripts/setup-beachball.sh



