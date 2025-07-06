.PHONY: format format-only lint lint-fix

build:
	turbo run build

clean:
	sh scripts/clean-all.sh

restore:
	sh scripts/restore.sh



