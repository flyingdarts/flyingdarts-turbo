#!/usr/bin/env node

/**
 * Flyingdarts Documentation Agent - Example Implementation
 *
 * This is a sample implementation demonstrating how the documentation agent
 * would work in practice. It shows the core functionality for:
 * - Project discovery
 * - README generation
 * - Class documentation extraction
 * - Cross-reference management
 */

const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

class DocumentationAgent {
    constructor(configPath = 'docs/docs-agent.config.json') {
        this.config = this.loadConfig(configPath);
        this.projects = [];
        this.classes = [];
        this.dependencies = [];
    }

    /**
     * Load configuration from JSON file
     */
    loadConfig(configPath) {
        try {
            const configContent = fs.readFileSync(configPath, 'utf8');
            return JSON.parse(configContent);
        } catch (error) {
            console.error(`Failed to load config from ${configPath}:`, error.message);
            process.exit(1);
        }
    }

    /**
     * Main entry point for documentation generation
     */
    async generateDocumentation() {
        console.log('🚀 Starting Flyingdarts Documentation Agent...');

        try {
            // Phase 1: Discovery
            await this.discoverProjects();

            // Phase 2: Analysis
            await this.analyzeProjects();

            // Phase 3: Documentation Generation
            await this.generateDocumentation();

            // Phase 4: Validation
            await this.validateDocumentation();

            console.log('✅ Documentation generation completed successfully!');

        } catch (error) {
            console.error('❌ Documentation generation failed:', error.message);
            process.exit(1);
        }
    }

    /**
     * Phase 1: Discover all projects in the monorepo
     */
    async discoverProjects() {
        console.log('🔍 Discovering projects...');

        const monorepoRoot = this.config.monorepo.root;

        // Discover applications
        await this.scanDirectory(path.join(monorepoRoot, this.config.monorepo.appsPath), 'application');

        // Discover packages
        await this.scanDirectory(path.join(monorepoRoot, this.config.monorepo.packagesPath), 'library');

        // Discover tools
        await this.scanDirectory(path.join(monorepoRoot, this.config.monorepo.scriptsPath), 'tool');

        console.log(`📁 Discovered ${this.projects.length} projects`);
    }

    /**
     * Scan directory for projects
     */
    async scanDirectory(dirPath, projectType) {
        if (!fs.existsSync(dirPath)) return;

        const entries = fs.readdirSync(dirPath, { withFileTypes: true });

        for (const entry of entries) {
            if (entry.isDirectory()) {
                const projectPath = path.join(dirPath, entry.name);
                const project = await this.analyzeProject(projectPath, entry.name, projectType);

                if (project) {
                    this.projects.push(project);
                }
            }
        }
    }

    /**
     * Analyze individual project
     */
    async analyzeProject(projectPath, projectName, projectType) {
        const configFiles = this.findConfigFiles(projectPath);

        if (configFiles.length === 0) {
            return null; // Not a valid project
        }

        const language = this.detectLanguage(configFiles);
        const metadata = await this.extractProjectMetadata(projectPath, configFiles, language);

        return {
            name: projectName,
            path: projectPath,
            type: projectType,
            language: language,
            configFiles: configFiles,
            metadata: metadata,
            classes: [],
            dependencies: []
        };
    }

    /**
     * Find configuration files for the project
     */
    findConfigFiles(projectPath) {
        const configFiles = [];

        for (const [language, config] of Object.entries(this.config.languages)) {
            for (const configFile of config.configFiles) {
                const configPath = path.join(projectPath, configFile);
                if (fs.existsSync(configPath)) {
                    configFiles.push({
                        language: language,
                        file: configFile,
                        path: configPath
                    });
                }
            }
        }

        return configFiles;
    }

    /**
     * Detect programming language based on config files
     */
    detectLanguage(configFiles) {
        if (configFiles.length === 0) return 'unknown';

        // Return the first detected language
        return configFiles[0].language;
    }

    /**
     * Extract project metadata from configuration files
     */
    async extractProjectMetadata(projectPath, configFiles, language) {
        const metadata = {
            name: path.basename(projectPath),
            description: '',
            version: '1.0.0',
            dependencies: [],
            features: []
        };

        for (const configFile of configFiles) {
            try {
                const content = fs.readFileSync(configFile.path, 'utf8');

                switch (configFile.language) {
                    case 'rust':
                        metadata.description = this.extractRustDescription(content);
                        metadata.dependencies = this.extractRustDependencies(content);
                        break;
                    case 'csharp':
                        metadata.description = this.extractCSharpDescription(content);
                        metadata.dependencies = this.extractCSharpDependencies(content);
                        break;
                    case 'dart':
                        metadata.description = this.extractDartDescription(content);
                        metadata.dependencies = this.extractDartDependencies(content);
                        break;
                    case 'typescript':
                        metadata.description = this.extractTypeScriptDescription(content);
                        metadata.dependencies = this.extractTypeScriptDependencies(content);
                        break;
                }
            } catch (error) {
                console.warn(`Failed to parse ${configFile.path}:`, error.message);
            }
        }

        return metadata;
    }

    /**
     * Phase 2: Analyze projects for classes and dependencies
     */
    async analyzeProjects() {
        console.log('🔬 Analyzing projects...');

        for (const project of this.projects) {
            console.log(`  📂 Analyzing ${project.name}...`);

            // Extract classes from source files
            project.classes = await this.extractClasses(project);

            // Extract dependencies
            project.dependencies = await this.extractDependencies(project);

            // Build cross-references
            await this.buildCrossReferences(project);
        }
    }

    /**
     * Extract classes from project source files
     */
    async extractClasses(project) {
        const classes = [];
        const languageConfig = this.config.languages[project.language];

        if (!languageConfig) {
            console.warn(`No language config found for ${project.language}`);
            return classes;
        }

        // Find all source files
        const sourceFiles = this.findSourceFiles(project.path, languageConfig.extensions);

        for (const sourceFile of sourceFiles) {
            const fileClasses = await this.parseSourceFile(sourceFile, project.language);
            classes.push(...fileClasses);
        }

        return classes;
    }

    /**
     * Find source files in project directory
     */
    findSourceFiles(projectPath, extensions) {
        const sourceFiles = [];

        const scanDirectory = (dir) => {
            const entries = fs.readdirSync(dir, { withFileTypes: true });

            for (const entry of entries) {
                const fullPath = path.join(dir, entry.name);

                if (entry.isDirectory()) {
                    // Skip common directories that shouldn't contain source code
                    if (!['node_modules', 'bin', 'obj', 'target', '.git'].includes(entry.name)) {
                        scanDirectory(fullPath);
                    }
                } else if (entry.isFile()) {
                    const ext = path.extname(entry.name);
                    if (extensions.includes(ext)) {
                        sourceFiles.push(fullPath);
                    }
                }
            }
        };

        scanDirectory(projectPath);
        return sourceFiles;
    }

    /**
     * Parse source file to extract class information
     */
    async parseSourceFile(filePath, language) {
        const classes = [];

        try {
            const content = fs.readFileSync(filePath, 'utf8');

            switch (language) {
                case 'rust':
                    classes.push(...this.parseRustFile(content, filePath));
                    break;
                case 'csharp':
                    classes.push(...this.parseCSharpFile(content, filePath));
                    break;
                case 'dart':
                    classes.push(...this.parseDartFile(content, filePath));
                    break;
                case 'typescript':
                    classes.push(...this.parseTypeScriptFile(content, filePath));
                    break;
            }
        } catch (error) {
            console.warn(`Failed to parse ${filePath}:`, error.message);
        }

        return classes;
    }

    /**
     * Parse Rust source file
     */
    parseRustFile(content, filePath) {
        const classes = [];

        // Simple regex-based parsing for demonstration
        const structRegex = /pub\s+struct\s+(\w+)\s*\{([^}]+)\}/g;
        const implRegex = /impl\s+(\w+)\s*\{([^}]+)\}/g;

        let match;

        // Extract structs
        while ((match = structRegex.exec(content)) !== null) {
            const className = match[1];
            const fields = this.extractRustFields(match[2]);

            classes.push({
                name: className,
                type: 'struct',
                file: filePath,
                fields: fields,
                methods: [],
                documentation: this.extractDocumentation(content, match.index)
            });
        }

        // Extract implementations
        while ((match = implRegex.exec(content)) !== null) {
            const className = match[1];
            const methods = this.extractRustMethods(match[2]);

            // Find existing class or create new one
            let classObj = classes.find(c => c.name === className);
            if (!classObj) {
                classObj = {
                    name: className,
                    type: 'impl',
                    file: filePath,
                    fields: [],
                    methods: []
                };
                classes.push(classObj);
            }

            classObj.methods.push(...methods);
        }

        return classes;
    }

    /**
     * Extract Rust struct fields
     */
    extractRustFields(fieldsContent) {
        const fields = [];
        const fieldRegex = /pub\s+(\w+):\s+([^,\n]+)/g;

        let match;
        while ((match = fieldRegex.exec(fieldsContent)) !== null) {
            fields.push({
                name: match[1],
                type: match[2].trim(),
                access: 'public'
            });
        }

        return fields;
    }

    /**
     * Extract Rust methods
     */
    extractRustMethods(methodsContent) {
        const methods = [];
        const methodRegex = /pub\s+fn\s+(\w+)\s*\(([^)]*)\)\s*(?:->\s*([^{]+))?/g;

        let match;
        while ((match = methodRegex.exec(methodsContent)) !== null) {
            const methodName = match[1];
            const parameters = this.parseRustParameters(match[2]);
            const returnType = match[3] ? match[3].trim() : '()';

            methods.push({
                name: methodName,
                parameters: parameters,
                returnType: returnType,
                access: 'public'
            });
        }

        return methods;
    }

    /**
     * Parse Rust method parameters
     */
    parseRustParameters(paramsString) {
        if (!paramsString.trim()) return [];

        return paramsString.split(',').map(param => {
            const parts = param.trim().split(':');
            return {
                name: parts[0].trim(),
                type: parts[1] ? parts[1].trim() : 'unknown'
            };
        });
    }

    /**
     * Extract documentation comments
     */
    extractDocumentation(content, position) {
        const lines = content.split('\n');
        const lineNumber = content.substring(0, position).split('\n').length - 1;

        const documentation = [];

        // Look for documentation comments before the position
        for (let i = lineNumber - 1; i >= 0; i--) {
            const line = lines[i].trim();

            if (line.startsWith('///') || line.startsWith('//!')) {
                documentation.unshift(line.substring(3).trim());
            } else if (line.startsWith('//') || line.startsWith('/*')) {
                break; // Stop at regular comments
            } else if (line === '') {
                continue; // Skip empty lines
            } else {
                break; // Stop at code
            }
        }

        return documentation.join('\n');
    }

    /**
     * Parse C# source file (simplified)
     */
    parseCSharpFile(content, filePath) {
        const classes = [];

        // Simple regex-based parsing for demonstration
        const classRegex = /public\s+class\s+(\w+)(?:\s*:\s*([^{]+))?\s*\{/g;

        let match;
        while ((match = classRegex.exec(content)) !== null) {
            const className = match[1];
            const inheritance = match[2] ? match[2].trim() : '';

            classes.push({
                name: className,
                type: 'class',
                file: filePath,
                inheritance: inheritance,
                properties: [],
                methods: [],
                documentation: this.extractDocumentation(content, match.index)
            });
        }

        return classes;
    }

    /**
     * Parse Dart source file (simplified)
     */
    parseDartFile(content, filePath) {
        const classes = [];

        // Simple regex-based parsing for demonstration
        const classRegex = /class\s+(\w+)(?:\s+extends\s+([^{]+))?\s*\{/g;

        let match;
        while ((match = classRegex.exec(content)) !== null) {
            const className = match[1];
            const inheritance = match[2] ? match[2].trim() : '';

            classes.push({
                name: className,
                type: 'class',
                file: filePath,
                inheritance: inheritance,
                fields: [],
                methods: [],
                documentation: this.extractDocumentation(content, match.index)
            });
        }

        return classes;
    }

    /**
     * Parse TypeScript source file (simplified)
     */
    parseTypeScriptFile(content, filePath) {
        const classes = [];

        // Simple regex-based parsing for demonstration
        const classRegex = /(?:export\s+)?class\s+(\w+)(?:\s+extends\s+([^{]+))?\s*\{/g;

        let match;
        while ((match = classRegex.exec(content)) !== null) {
            const className = match[1];
            const inheritance = match[2] ? match[2].trim() : '';

            classes.push({
                name: className,
                type: 'class',
                file: filePath,
                inheritance: inheritance,
                properties: [],
                methods: [],
                documentation: this.extractDocumentation(content, match.index)
            });
        }

        return classes;
    }

    /**
     * Extract dependencies from project
     */
    async extractDependencies(project) {
        const dependencies = [];

        // Extract from metadata
        if (project.metadata.dependencies) {
            dependencies.push(...project.metadata.dependencies);
        }

        // Extract from source files (imports/using statements)
        const sourceFiles = this.findSourceFiles(project.path, this.config.languages[project.language].extensions);

        for (const sourceFile of sourceFiles) {
            const fileDeps = this.extractFileDependencies(sourceFile, project.language);
            dependencies.push(...fileDeps);
        }

        return dependencies;
    }

    /**
     * Extract dependencies from source file
     */
    extractFileDependencies(filePath, language) {
        const dependencies = [];

        try {
            const content = fs.readFileSync(filePath, 'utf8');

            switch (language) {
                case 'rust':
                    // Extract use statements
                    const useRegex = /use\s+([^;]+);/g;
                    let match;
                    while ((match = useRegex.exec(content)) !== null) {
                        dependencies.push({
                            type: 'import',
                            name: match[1].trim(),
                            file: filePath
                        });
                    }
                    break;

                case 'csharp':
                    // Extract using statements
                    const usingRegex = /using\s+([^;]+);/g;
                    while ((match = usingRegex.exec(content)) !== null) {
                        dependencies.push({
                            type: 'import',
                            name: match[1].trim(),
                            file: filePath
                        });
                    }
                    break;

                case 'dart':
                    // Extract import statements
                    const importRegex = /import\s+['"]([^'"]+)['"]/g;
                    while ((match = importRegex.exec(content)) !== null) {
                        dependencies.push({
                            type: 'import',
                            name: match[1].trim(),
                            file: filePath
                        });
                    }
                    break;

                case 'typescript':
                    // Extract import statements
                    const tsImportRegex = /import\s+(?:.*\s+from\s+)?['"]([^'"]+)['"]/g;
                    while ((match = tsImportRegex.exec(content)) !== null) {
                        dependencies.push({
                            type: 'import',
                            name: match[1].trim(),
                            file: filePath
                        });
                    }
                    break;
            }
        } catch (error) {
            console.warn(`Failed to extract dependencies from ${filePath}:`, error.message);
        }

        return dependencies;
    }

    /**
     * Build cross-references between projects
     */
    async buildCrossReferences(project) {
        // This would implement logic to link classes and dependencies
        // between different projects in the monorepo
        console.log(`  🔗 Building cross-references for ${project.name}...`);
    }

    /**
     * Phase 3: Generate documentation
     */
    async generateDocumentation() {
        console.log('📝 Generating documentation...');

        for (const project of this.projects) {
            console.log(`  📄 Generating docs for ${project.name}...`);

            // Generate README.md
            await this.generateReadme(project);

            // Generate API documentation
            await this.generateApiDocs(project);

            // Generate class documentation
            await this.generateClassDocs(project);
        }

        // Generate main index
        await this.generateMainIndex();
    }

    /**
     * Generate README.md for project
     */
    async generateReadme(project) {
        const template = this.loadTemplate('readme');
        const data = this.prepareReadmeData(project);

        const readmeContent = this.renderTemplate(template, data);
        const readmePath = path.join(project.path, 'README.md');

        fs.writeFileSync(readmePath, readmeContent);
        console.log(`    ✅ Generated README.md for ${project.name}`);
    }

    /**
     * Generate API documentation for project
     */
    async generateApiDocs(project) {
        const template = this.loadTemplate('api');
        const data = this.prepareApiData(project);

        const apiContent = this.renderTemplate(template, data);
        const apiPath = path.join(project.path, 'docs', 'api', 'index.md');

        // Ensure directory exists
        fs.mkdirSync(path.dirname(apiPath), { recursive: true });
        fs.writeFileSync(apiPath, apiContent);

        console.log(`    ✅ Generated API docs for ${project.name}`);
    }

    /**
     * Generate class documentation for project
     */
    async generateClassDocs(project) {
        const template = this.loadTemplate('class');

        for (const classObj of project.classes) {
            const data = this.prepareClassData(classObj, project);
            const classContent = this.renderTemplate(template, data);

            const classPath = path.join(project.path, 'docs', 'api', 'classes', `${classObj.name}.md`);

            // Ensure directory exists
            fs.mkdirSync(path.dirname(classPath), { recursive: true });
            fs.writeFileSync(classPath, classContent);
        }

        console.log(`    ✅ Generated class docs for ${project.name} (${project.classes.length} classes)`);
    }

    /**
     * Generate main documentation index
     */
    async generateMainIndex() {
        const template = this.loadTemplate('index');
        const data = this.prepareIndexData();

        const indexContent = this.renderTemplate(template, data);
        const indexPath = path.join(this.config.monorepo.root, 'docs', 'index.md');

        // Ensure directory exists
        fs.mkdirSync(path.dirname(indexPath), { recursive: true });
        fs.writeFileSync(indexPath, indexContent);

        console.log('    ✅ Generated main documentation index');
    }

    /**
     * Load template from file
     */
    loadTemplate(templateName) {
        const templatePath = path.join(this.config.monorepo.root, this.config.templates[templateName]);

        try {
            return fs.readFileSync(templatePath, 'utf8');
        } catch (error) {
            console.error(`Failed to load template ${templateName}:`, error.message);
            return '';
        }
    }

    /**
     * Prepare data for README template
     */
    prepareReadmeData(project) {
        return {
            projectName: project.name,
            projectDescription: project.metadata.description,
            overview: project.metadata.description || `Documentation for ${project.name}`,
            features: project.metadata.features || [],
            dependencies: {
                internalDependencies: project.dependencies.filter(d => d.type === 'internal'),
                externalDependencies: project.dependencies.filter(d => d.type === 'external')
            },
            installationCommand: this.getInstallationCommand(project),
            usageDescription: `See the [API Reference](docs/api/) for detailed usage information.`,
            apiReference: 'docs/api/',
            generatedDate: new Date().toISOString()
        };
    }

    /**
     * Prepare data for API template
     */
    prepareApiData(project) {
        return {
            projectName: project.name,
            overview: project.metadata.description || `API documentation for ${project.name}`,
            classes: project.classes.map(c => ({
                name: c.name,
                description: c.documentation || '',
                inheritance: c.inheritance || '',
                summary: `${c.fields?.length || 0} properties, ${c.methods?.length || 0} methods`,
                properties: c.fields?.length || 0,
                methods: c.methods?.length || 0,
                link: `classes/${c.name}.md`
            })),
            generatedDate: new Date().toISOString(),
            version: project.metadata.version || '1.0.0',
            lastUpdated: new Date().toISOString()
        };
    }

    /**
     * Prepare data for class template
     */
    prepareClassData(classObj, project) {
        return {
            className: classObj.name,
            overview: classObj.documentation || `Documentation for ${classObj.name}`,
            inheritance: {
                extends: classObj.inheritance || null,
                implements: classObj.implements || []
            },
            properties: classObj.fields?.map(f => ({
                name: f.name,
                type: f.type,
                access: f.access,
                description: f.documentation || ''
            })) || [],
            methods: classObj.methods?.map(m => ({
                name: m.name,
                parameters: m.parameters || [],
                returnType: m.returnType || 'void',
                description: m.documentation || '',
                access: m.access
            })) || [],
            generatedDate: new Date().toISOString()
        };
    }

    /**
     * Prepare data for index template
     */
    prepareIndexData() {
        const backendApps = this.projects.filter(p => p.type === 'application' && p.path.includes('backend'));
        const frontendApps = this.projects.filter(p => p.type === 'application' && p.path.includes('frontend'));
        const backendLibs = this.projects.filter(p => p.type === 'library' && p.path.includes('backend'));
        const frontendLibs = this.projects.filter(p => p.type === 'library' && p.path.includes('frontend'));

        return {
            backendApps: backendApps.map(p => ({
                name: p.name,
                path: p.path,
                description: p.metadata.description || '',
                technology: p.language,
                type: p.type,
                status: 'active',
                features: p.metadata.features || [],
                docsLink: path.join(p.path, 'README.md'),
                apiLink: path.join(p.path, 'docs/api/')
            })),
            frontendApps: frontendApps.map(p => ({
                name: p.name,
                path: p.path,
                description: p.metadata.description || '',
                technology: p.language,
                platform: 'web/mobile',
                status: 'active',
                features: p.metadata.features || [],
                docsLink: path.join(p.path, 'README.md'),
                apiLink: path.join(p.path, 'docs/api/')
            })),
            backendLibs: backendLibs.map(p => ({
                name: p.name,
                path: p.path,
                description: p.metadata.description || '',
                technology: p.language,
                purpose: 'shared library',
                version: p.metadata.version || '1.0.0',
                features: p.metadata.features || [],
                docsLink: path.join(p.path, 'README.md'),
                apiLink: path.join(p.path, 'docs/api/')
            })),
            frontendLibs: frontendLibs.map(p => ({
                name: p.name,
                path: p.path,
                description: p.metadata.description || '',
                technology: p.language,
                purpose: 'shared library',
                version: p.metadata.version || '1.0.0',
                features: p.metadata.features || [],
                docsLink: path.join(p.path, 'README.md'),
                apiLink: path.join(p.path, 'docs/api/')
            })),
            lastUpdated: new Date().toISOString(),
            totalProjects: this.projects.length,
            coverage: this.calculateCoverage(),
            version: this.config.version
        };
    }

    /**
     * Get installation command for project
     */
    getInstallationCommand(project) {
        switch (project.language) {
            case 'rust':
                return 'cargo build';
            case 'csharp':
                return 'dotnet build';
            case 'dart':
                return 'flutter pub get';
            case 'typescript':
                return 'npm install';
            default:
                return 'See project README for installation instructions';
        }
    }

    /**
     * Calculate documentation coverage
     */
    calculateCoverage() {
        let totalClasses = 0;
        let documentedClasses = 0;

        for (const project of this.projects) {
            totalClasses += project.classes.length;
            documentedClasses += project.classes.filter(c => c.documentation).length;
        }

        return totalClasses > 0 ? Math.round((documentedClasses / totalClasses) * 100) : 100;
    }

    /**
     * Render template with data
     */
    renderTemplate(template, data) {
        // Simple template rendering for demonstration
        // In a real implementation, you'd use a proper templating engine like Handlebars

        let result = template;

        // Replace simple variables
        for (const [key, value] of Object.entries(data)) {
            const placeholder = `{{${key}}}`;
            if (typeof value === 'string') {
                result = result.replace(new RegExp(placeholder, 'g'), value);
            }
        }

        // Handle conditional blocks
        result = result.replace(/\{\{#if\s+(\w+)\}\}([\s\S]*?)\{\{\/if\}\}/g, (match, condition, content) => {
            return data[condition] ? content : '';
        });

        // Handle loops
        result = result.replace(/\{\{#each\s+(\w+)\}\}([\s\S]*?)\{\{\/each\}\}/g, (match, arrayName, content) => {
            const array = data[arrayName];
            if (!Array.isArray(array)) return '';

            return array.map(item => {
                let itemContent = content;
                for (const [key, value] of Object.entries(item)) {
                    const placeholder = `{{${key}}}`;
                    if (typeof value === 'string') {
                        itemContent = itemContent.replace(new RegExp(placeholder, 'g'), value);
                    }
                }
                return itemContent;
            }).join('');
        });

        return result;
    }

    /**
     * Phase 4: Validate generated documentation
     */
    async validateDocumentation() {
        console.log('🔍 Validating documentation...');

        let errors = 0;
        let warnings = 0;

        for (const project of this.projects) {
            // Check README exists
            const readmePath = path.join(project.path, 'README.md');
            if (!fs.existsSync(readmePath)) {
                console.error(`  ❌ Missing README.md for ${project.name}`);
                errors++;
            }

            // Check API docs exist
            const apiPath = path.join(project.path, 'docs', 'api', 'index.md');
            if (!fs.existsSync(apiPath)) {
                console.warn(`  ⚠️  Missing API docs for ${project.name}`);
                warnings++;
            }

            // Check class documentation coverage
            const documentedClasses = project.classes.filter(c => c.documentation).length;
            const coverage = project.classes.length > 0 ? (documentedClasses / project.classes.length) * 100 : 100;

            if (coverage < this.config.quality.coverageThreshold * 100) {
                console.warn(`  ⚠️  Low documentation coverage for ${project.name}: ${coverage.toFixed(1)}%`);
                warnings++;
            }
        }

        if (errors > 0) {
            console.error(`❌ Validation failed with ${errors} errors and ${warnings} warnings`);
        } else if (warnings > 0) {
            console.warn(`⚠️  Validation completed with ${warnings} warnings`);
        } else {
            console.log('✅ Documentation validation passed');
        }
    }

    // Helper methods for extracting metadata from different languages
    extractRustDescription(content) {
        // Extract description from Cargo.toml
        const descriptionMatch = content.match(/description\s*=\s*["']([^"']+)["']/);
        return descriptionMatch ? descriptionMatch[1] : '';
    }

    extractRustDependencies(content) {
        const dependencies = [];
        const depsMatch = content.match(/\[dependencies\]([\s\S]*?)(?=\[|$)/);
        if (depsMatch) {
            const depsContent = depsMatch[1];
            const depRegex = /(\w+)\s*=\s*["']([^"']+)["']/g;
            let match;
            while ((match = depRegex.exec(depsContent)) !== null) {
                dependencies.push({
                    name: match[1],
                    version: match[2],
                    type: 'external'
                });
            }
        }
        return dependencies;
    }

    extractCSharpDescription(content) {
        // Extract description from .csproj
        const descriptionMatch = content.match(/<Description>([^<]+)<\/Description>/);
        return descriptionMatch ? descriptionMatch[1] : '';
    }

    extractCSharpDependencies(content) {
        const dependencies = [];
        const packageRefRegex = /<PackageReference\s+Include="([^"]+)"\s+Version="([^"]+)"/g;
        let match;
        while ((match = packageRefRegex.exec(content)) !== null) {
            dependencies.push({
                name: match[1],
                version: match[2],
                type: 'external'
            });
        }
        return dependencies;
    }

    extractDartDescription(content) {
        // Extract description from pubspec.yaml
        const descriptionMatch = content.match(/description:\s*(.+)/);
        return descriptionMatch ? descriptionMatch[1].trim() : '';
    }

    extractDartDependencies(content) {
        const dependencies = [];
        const depsMatch = content.match(/dependencies:([\s\S]*?)(?=\n\w|$)/);
        if (depsMatch) {
            const depsContent = depsMatch[1];
            const depRegex = /(\w+):\s*([^\n]+)/g;
            let match;
            while ((match = depRegex.exec(depsContent)) !== null) {
                dependencies.push({
                    name: match[1],
                    version: match[2].trim(),
                    type: 'external'
                });
            }
        }
        return dependencies;
    }

    extractTypeScriptDescription(content) {
        // Extract description from package.json
        const descriptionMatch = content.match(/"description":\s*"([^"]+)"/);
        return descriptionMatch ? descriptionMatch[1] : '';
    }

    extractTypeScriptDependencies(content) {
        const dependencies = [];
        const depsMatch = content.match(/"dependencies":\s*\{([\s\S]*?)\}/);
        if (depsMatch) {
            const depsContent = depsMatch[1];
            const depRegex = /"([^"]+)":\s*"([^"]+)"/g;
            let match;
            while ((match = depRegex.exec(depsContent)) !== null) {
                dependencies.push({
                    name: match[1],
                    version: match[2],
                    type: 'external'
                });
            }
        }
        return dependencies;
    }
}

// Main execution
if (require.main === module) {
    const agent = new DocumentationAgent();
    agent.generateDocumentation().catch(console.error);
}

module.exports = DocumentationAgent;
