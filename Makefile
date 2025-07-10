.PHONY: format format-only lint lint-fix

validate-workspace:
	node scripts/generator/workspace/validate.js fd-v2.yml scripts/generator/workspace/fd-v2.schema.json
