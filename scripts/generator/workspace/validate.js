#!/usr/bin/env node
// validate.js - Validate a YAML file against a JSON Schema using js-yaml and ajv

const fs = require("fs");
const path = require("path");
const yaml = require("js-yaml");
const Ajv = require("ajv");

if (process.argv.length < 4) {
  console.error("Usage: node validate.js <yaml-file> <schema-file>");
  process.exit(1);
}

const yamlFile = process.argv[2];
const schemaFile = process.argv[3];
const schemaDir = path.dirname(schemaFile);
const typesDir = path.join(schemaDir, "types");
const languagesDir = path.join(schemaDir, "languages");

try {
  const data = yaml.load(fs.readFileSync(yamlFile, "utf8"));
  const schema = JSON.parse(fs.readFileSync(schemaFile, "utf8"));
  const ajv = new Ajv({ allErrors: true, strict: false });

  // Load all schemas in types/ and add to Ajv with 'types/<filename>' as the key
  if (fs.existsSync(typesDir)) {
    const typeFiles = fs
      .readdirSync(typesDir)
      .filter((f) => f.endsWith(".json"));
    for (const file of typeFiles) {
      const typeSchema = JSON.parse(
        fs.readFileSync(path.join(typesDir, file), "utf8")
      );
      ajv.addSchema(typeSchema, "types/" + file);
    }
  }

  // Load all schemas in languages/ and add to Ajv with 'languages/<filename>' as the key
  if (fs.existsSync(languagesDir)) {
    const langFiles = fs
      .readdirSync(languagesDir)
      .filter((f) => f.endsWith(".json"));
    for (const file of langFiles) {
      const langSchema = JSON.parse(
        fs.readFileSync(path.join(languagesDir, file), "utf8")
      );
      ajv.addSchema(langSchema, "languages/" + file);
    }
  }

  // Add the main schema with its $id
  ajv.addSchema(schema, schema.$id || "main");
  const validate = ajv.getSchema(schema.$id || "main");
  if (!validate) {
    throw new Error("Could not get validator for main schema");
  }
  const valid = validate(data);
  if (valid) {
    console.log("✅ YAML is valid!");
  } else {
    console.error("❌ Validation errors:");
    for (const err of validate.errors) {
      console.error(`- ${err.instancePath} ${err.message}`);
    }
    process.exit(1);
  }
} catch (err) {
  console.error("❌ Error:", err.message);
  process.exit(1);
}
