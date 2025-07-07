# 🛠️ FlyingDarts Workspace Schema Validation

This folder contains the JSON Schema for validating your multilingual monorepo configuration YAML (`fd-v2.yml`).

## 📄 What is this?

- `fd-v2.schema.json` defines the structure your YAML config should follow.
- The `types/` folder contains modular schema definitions for packages, apps, tools, and scripts.

## ✅ How to validate your YAML

You can use several tools to validate your YAML file against this schema. Here are the recommended approaches:

### 1. Using Node.js (`js-yaml` + `ajv`) 🚀

1. **Install dependencies:**
   ```bash
   npm install js-yaml ajv
   ```
2. **Run the validator script:**
   ```bash
   node validate.js ../../../../fd-v2.yml fd-v2.schema.json
   ```
   _(Adjust the path to your YAML file as needed)_

### 2. Using Python (`pyyaml` + `jsonschema`)

1. **Install dependencies:**
   ```bash
   pip install pyyaml jsonschema
   ```
2. **Run validation:**
   ```bash
   python validate_yaml.py fd-v2.yml fd-v2.schema.json
   ```
   _(You will need a small script. See below!)_

#### Example Python validation script (`validate_yaml.py`):

```python
import sys, yaml, json, jsonschema
with open(sys.argv[1]) as f: data = yaml.safe_load(f)
with open(sys.argv[2]) as f: schema = json.load(f)
jsonschema.validate(instance=data, schema=schema)
print('✅ YAML is valid!')
```

---

## ℹ️ Tips

- Make sure to update the schema if you change your YAML structure.
- Modular types make it easy to extend and maintain your schema.

Happy validating! 🎯
