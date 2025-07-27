# Flying Darts CDK Infrastructure

## Overview

The Flying Darts CDK Infrastructure is a .NET 8 application that provides Infrastructure as Code (IaC) for the Flying Darts Turbo platform using AWS CDK. This tool manages the complete cloud infrastructure deployment, including Lambda functions, API Gateway, DynamoDB tables, and supporting AWS services.

This tool is responsible for:
- Defining and managing AWS infrastructure as code
- Deploying Lambda functions for backend services
- Setting up API Gateway for REST and WebSocket APIs
- Configuring DynamoDB tables for data persistence
- Managing IAM roles and permissions
- Setting up monitoring and logging infrastructure
- Providing environment-specific deployments
- Automating infrastructure updates and rollbacks

## Features

- **Infrastructure as Code**: Complete AWS infrastructure defined in C#
- **Multi-environment Support**: Development, staging, and production environments
- **Lambda Function Deployment**: Automated deployment of all backend services
- **API Gateway Configuration**: REST and WebSocket API setup
- **Database Management**: DynamoDB table creation and configuration
- **Security Management**: IAM roles, policies, and security groups
- **Monitoring Setup**: CloudWatch logs, metrics, and alarms
- **CI/CD Integration**: Automated deployment pipelines
- **Cost Optimization**: Resource tagging and cost allocation
- **Disaster Recovery**: Backup and recovery procedures

## Prerequisites

- .NET 8 SDK
- AWS CLI configured with appropriate permissions
- AWS CDK CLI installed globally
- Visual Studio 2022 or VS Code with C# extensions
- Docker (for containerized deployment)

## Installation

1. Clone the monorepo and navigate to the CDK project:
```bash
cd apps/tools/dotnet/cdk
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the project:
```bash
dotnet build
```

4. Install AWS CDK globally (if not already installed):
```bash
npm install -g aws-cdk
```

## Usage

### Development Environment

Deploy to development environment:

```bash
# Bootstrap CDK (first time only)
cdk bootstrap

# Deploy development stack
cdk deploy --profile dev
```

### Production Environment

Deploy to production environment:

```bash
# Deploy production stack
cdk deploy --profile prod --require-approval never
```

### Destroy Infrastructure

Remove deployed infrastructure:

```bash
# Destroy development stack
cdk destroy --profile dev

# Destroy production stack
cdk destroy --profile prod
```

### Synthesize CloudFormation

Generate CloudFormation templates without deploying:

```bash
cdk synth --profile dev
```

## Infrastructure Components

### Core Infrastructure

#### Lambda Functions

- **X01 Game API**: Game logic and scoring service
- **Authentication Service**: JWT token validation
- **Friends API**: Social features and friend management
- **Signalling Service**: Real-time WebSocket communication
- **Rust Authorizer**: Custom API Gateway authorization

#### API Gateway

- **REST API**: HTTP endpoints for game and user management
- **WebSocket API**: Real-time communication endpoints
- **Custom Authorizers**: JWT token validation
- **CORS Configuration**: Cross-origin resource sharing
- **Rate Limiting**: API usage throttling

#### Database Layer

- **DynamoDB Tables**: NoSQL data storage
  - Game data tables
  - User profile tables
  - Friend relationship tables
  - Connection state tables
- **Global Secondary Indexes**: Optimized query performance
- **Auto-scaling**: Automatic capacity management

#### Security Infrastructure

- **IAM Roles**: Service-specific permissions
- **Security Groups**: Network access control
- **KMS Keys**: Data encryption
- **VPC Configuration**: Network isolation
- **WAF Rules**: Web application firewall

### Monitoring and Logging

#### CloudWatch

- **Log Groups**: Centralized logging for all services
- **Metrics**: Performance and business metrics
- **Alarms**: Automated alerting for issues
- **Dashboards**: Operational visibility

#### X-Ray

- **Distributed Tracing**: Request flow tracking
- **Performance Analysis**: Bottleneck identification
- **Service Maps**: Architecture visualization

### Supporting Services

#### Systems Manager

- **Parameter Store**: Configuration management
- **Secrets Manager**: Secure credential storage
- **Session Manager**: Secure server access

#### S3

- **Static Assets**: Web application hosting
- **Backup Storage**: Data backup and recovery
- **Log Archives**: Long-term log storage

## Configuration

### Environment Configuration

The CDK application supports multiple environments:

#### Development Environment

```csharp
var devConfig = new EnvironmentConfig
{
    Environment = "dev",
    Account = "123456789012",
    Region = "us-east-1",
    DomainName = "dev.flyingdarts.com",
    CertificateArn = "arn:aws:acm:us-east-1:123456789012:certificate/...",
    LambdaMemorySize = 512,
    DynamoDbBillingMode = BillingMode.PAY_PER_REQUEST
};
```

#### Production Environment

```csharp
var prodConfig = new EnvironmentConfig
{
    Environment = "prod",
    Account = "123456789012",
    Region = "us-east-1",
    DomainName = "flyingdarts.com",
    CertificateArn = "arn:aws:acm:us-east-1:123456789012:certificate/...",
    LambdaMemorySize = 1024,
    DynamoDbBillingMode = BillingMode.PROVISIONED
};
```

### CDK Configuration (`cdk.json`)

```json
{
  "app": "dotnet run --project Flyingdarts.CDK.csproj",
  "watch": {
    "include": [
      "**"
    ],
    "exclude": [
      "README.md",
      "cdk*.json",
      "**/*.d.ts",
      "**/*.js",
      "tsconfig.json",
      "package*.json",
      "yarn.lock",
      "node_modules",
      "test"
    ]
  },
  "context": {
    "@aws-cdk/aws-lambda:recognizeLayerVersion": true,
    "@aws-cdk/core:checkSecretUsage": true,
    "@aws-cdk/core:target-partitions": [
      "aws",
      "aws-cn"
    ],
    "@aws-cdk-containers/ecs-service-extensions:enableDefaultLogDriver": true,
    "@aws-cdk/aws-ec2:uniqueImdsv2TemplateName": true,
    "@aws-cdk/aws-ecs:arnFormatIncludesClusterName": true,
    "@aws-cdk/aws-iam:minimizePolicies": true,
    "@aws-cdk/core:validateSnapshotRemovalPolicy": true,
    "@aws-cdk/aws-codepipeline:crossAccountKeyAliasStackSafeResourceName": true,
    "@aws-cdk/aws-s3:createDefaultLoggingPolicy": true,
    "@aws-cdk/aws-sns-subscriptions:restrictSqsDescryption": true,
    "@aws-cdk/aws-apigateway:disableCloudWatchRole": true,
    "@aws-cdk/core:enablePartitionLiterals": true,
    "@aws-cdk/aws-events:eventsTargetQueueSameAccount": true,
    "@aws-cdk/aws-iam:standardizedServicePrincipals": true,
    "@aws-cdk/aws-ecs:disableExplicitDeploymentControllerForCircuitBreaker": true,
    "@aws-cdk/aws-iam:importedRoleStackSafeDefaultPolicyName": true,
    "@aws-cdk/aws-s3:serverAccessLogsUseBucketPolicy": true,
    "@aws-cdk/aws-route53-patters:useCertificate": true,
    "@aws-cdk/customresources:installLatestAwsSdkDefault": false,
    "@aws-cdk/aws-rds:databaseProxyUniqueResourceName": true,
    "@aws-cdk/aws-codedeploy:removeAlarmsFromDeploymentGroup": true,
    "@aws-cdk/aws-apigateway:authorizerChangeDeploymentLogicalId": true,
    "@aws-cdk/aws-ec2:launchTemplateDefaultUserData": true,
    "@aws-cdk/aws-secretsmanager:useAttachedSecretResourcePolicyForSecretTargetAttachments": true,
    "@aws-cdk/aws-redshift:columnId": true,
    "@aws-cdk/aws-stepfunctions-tasks:enableLoggingConfiguration": true,
    "@aws-cdk/aws-ec2:restrictDefaultSecurityGroup": true,
    "@aws-cdk/aws-apigateway:requestValidatorUniqueId": true,
    "@aws-cdk/aws-kms:aliasNameRef": true,
    "@aws-cdk/aws-autoscaling:generateLaunchTemplateInsteadOfLaunchConfig": true,
    "@aws-cdk/core:includePrefixInUniqueNameGeneration": true,
    "@aws-cdk/aws-efs:denyAnonymousAccess": true,
    "@aws-cdk/aws-opensearchservice:enableOpensearchMultiAzWithStandby": true,
    "@aws-cdk/aws-lambda-nodejs:useLatestRuntimeVersion": true,
    "@aws-cdk/aws-efs:mountTargetOrderInsensitiveLogicalId": true,
    "@aws-cdk/aws-rds:auroraClusterChangeScopeOfInstanceParameterGroupWithEachParameters": true,
    "@aws-cdk/aws-appsync:useArnForSourceApiAssociationIdentifier": true,
    "@aws-cdk/aws-rds:preventRenderingDeprecatedCredentials": true,
    "@aws-cdk/aws-codepipeline-actions:useNewDefaultBranchForSourceAction": true,
    "@aws-cdk/aws-ec2:securityGroupChangeScopeOfDescription": true,
    "@aws-cdk/aws-iam:managedPolicyChangeScopeOfDescription": true,
    "@aws-cdk/aws-ecs:arnFormatIncludesClusterName": true,
    "@aws-cdk/aws-iam:importedRoleStackSafeDefaultPolicyName": true,
    "@aws-cdk/aws-s3:serverAccessLogsUseBucketPolicy": true,
    "@aws-cdk/aws-route53-patters:useCertificate": true,
    "@aws-cdk/customresources:installLatestAwsSdkDefault": false,
    "@aws-cdk/aws-rds:databaseProxyUniqueResourceName": true,
    "@aws-cdk/aws-codedeploy:removeAlarmsFromDeploymentGroup": true,
    "@aws-cdk/aws-apigateway:authorizerChangeDeploymentLogicalId": true,
    "@aws-cdk/aws-ec2:launchTemplateDefaultUserData": true,
    "@aws-cdk/aws-secretsmanager:useAttachedSecretResourcePolicyForSecretTargetAttachments": true,
    "@aws-cdk/aws-redshift:columnId": true,
    "@aws-cdk/aws-stepfunctions-tasks:enableLoggingConfiguration": true,
    "@aws-cdk/aws-ec2:restrictDefaultSecurityGroup": true,
    "@aws-cdk/aws-apigateway:requestValidatorUniqueId": true,
    "@aws-cdk/aws-kms:aliasNameRef": true,
    "@aws-cdk/aws-autoscaling:generateLaunchTemplateInsteadOfLaunchConfig": true,
    "@aws-cdk/core:includePrefixInUniqueNameGeneration": true,
    "@aws-cdk/aws-efs:denyAnonymousAccess": true,
    "@aws-cdk/aws-opensearchservice:enableOpensearchMultiAzWithStandby": true,
    "@aws-cdk/aws-lambda-nodejs:useLatestRuntimeVersion": true,
    "@aws-cdk/aws-efs:mountTargetOrderInsensitiveLogicalId": true,
    "@aws-cdk/aws-rds:auroraClusterChangeScopeOfInstanceParameterGroupWithEachParameters": true,
    "@aws-cdk/aws-appsync:useArnForSourceApiAssociationIdentifier": true,
    "@aws-cdk/aws-rds:preventRenderingDeprecatedCredentials": true,
    "@aws-cdk/aws-codepipeline-actions:useNewDefaultBranchForSourceAction": true,
    "@aws-cdk/aws-ec2:securityGroupChangeScopeOfDescription": true,
    "@aws-cdk/aws-iam:managedPolicyChangeScopeOfDescription": true
  }
}
```

## Dependencies

### Core Dependencies

- **Amazon.CDK.Lib**: AWS CDK core library
- **Constructs**: CDK construct library
- **Amazon.CDK.AWS.Apigatewayv2.Alpha**: API Gateway v2 constructs
- **Amazon.CDK.AWS.Apigatewayv2.Integrations.Alpha**: API Gateway integrations
- **AWSSDK.SimpleSystemsManagement**: AWS Systems Manager SDK
- **Microsoft.Extensions.Configuration**: Configuration management
- **Microsoft.Extensions.DependencyInjection.Abstractions**: DI abstractions
- **Microsoft.Extensions.Options**: Options pattern

### Internal Dependencies

- **Flyingdarts.CDK.Constructs**: Custom CDK constructs for Flying Darts

## Development

### Project Structure

```
cdk/
├── Program.cs                   # Main entry point
├── GlobalSuppressions.cs        # Code analysis suppressions
├── Flyingdarts.CDK.csproj       # Project file
├── cdk.json                    # CDK configuration
├── cdk.context.json            # CDK context
├── package.json                # Node.js dependencies
├── lambda.zip                  # Lambda deployment package
└── scripts/                    # Deployment scripts
```

### Main Components

#### `Program.cs`

The main entry point for the CDK application.

**Key Methods:**

##### `Main(string[] args)`

Main entry point that initializes and runs the CDK app.

**Parameters:**
- `args` (string[]): Command line arguments

##### `CreateApp()`

Creates the CDK application with all stacks.

**Returns:**
- `App`: Configured CDK application

#### Stack Definitions

##### `FlyingDartsStack`

Main infrastructure stack containing all resources.

**Resources:**
- Lambda functions
- API Gateway
- DynamoDB tables
- IAM roles and policies
- CloudWatch resources

##### `DatabaseStack`

Database-specific resources.

**Resources:**
- DynamoDB tables
- Global secondary indexes
- Auto-scaling policies
- Backup configurations

##### `MonitoringStack`

Monitoring and logging resources.

**Resources:**
- CloudWatch log groups
- Metrics and alarms
- Dashboards
- X-Ray tracing

## Deployment Process

### Pre-deployment Steps

1. **Environment Setup**:
   ```bash
   # Configure AWS credentials
   aws configure --profile dev
   aws configure --profile prod
   ```

2. **CDK Bootstrap**:
   ```bash
   # Bootstrap CDK for first-time deployment
   cdk bootstrap --profile dev
   cdk bootstrap --profile prod
   ```

3. **Build Lambda Functions**:
   ```bash
   # Build all Lambda functions
   dotnet build ../backend/dotnet/api
   dotnet build ../backend/dotnet/auth
   dotnet build ../backend/dotnet/friends
   dotnet build ../backend/dotnet/signalling
   ```

### Deployment Commands

#### Development Deployment

```bash
# Deploy to development
cdk deploy --profile dev --require-approval never

# Deploy specific stack
cdk deploy FlyingDartsStack --profile dev
```

#### Production Deployment

```bash
# Deploy to production
cdk deploy --profile prod --require-approval never

# Deploy with approval
cdk deploy --profile prod
```

#### Staging Deployment

```bash
# Deploy to staging
cdk deploy --profile staging --require-approval never
```

### Post-deployment Verification

1. **Check Stack Status**:
   ```bash
   cdk list --profile dev
   cdk diff --profile dev
   ```

2. **Verify Resources**:
   ```bash
   # Check Lambda functions
   aws lambda list-functions --profile dev

   # Check API Gateway
   aws apigateway get-rest-apis --profile dev

   # Check DynamoDB tables
   aws dynamodb list-tables --profile dev
   ```

3. **Test Endpoints**:
   ```bash
   # Test API endpoints
   curl -X GET https://api-dev.flyingdarts.com/health
   ```

## Security Considerations

### IAM Best Practices

- **Principle of Least Privilege**: Minimum required permissions
- **Role-based Access**: Service-specific IAM roles
- **Cross-account Access**: Secure cross-account resource access
- **Temporary Credentials**: Use temporary credentials where possible

### Network Security

- **VPC Configuration**: Network isolation and segmentation
- **Security Groups**: Restrict network access
- **WAF Rules**: Web application firewall protection
- **DDoS Protection**: Shield and GuardDuty integration

### Data Protection

- **Encryption at Rest**: KMS encryption for all data
- **Encryption in Transit**: TLS/SSL for all communications
- **Secrets Management**: Secure credential storage
- **Backup Encryption**: Encrypted backup storage

## Monitoring and Alerting

### CloudWatch Monitoring

#### Metrics

- **Lambda Metrics**: Invocation count, duration, errors
- **API Gateway Metrics**: Request count, latency, 4xx/5xx errors
- **DynamoDB Metrics**: Read/write capacity, throttling
- **Custom Metrics**: Business-specific metrics

#### Alarms

- **High Error Rate**: Alert on increased error rates
- **High Latency**: Alert on performance degradation
- **Resource Utilization**: Alert on resource constraints
- **Cost Monitoring**: Alert on cost thresholds

### Logging

#### Log Groups

- **Application Logs**: Lambda function logs
- **Access Logs**: API Gateway access logs
- **Audit Logs**: Security and compliance logs
- **Performance Logs**: Performance monitoring logs

#### Log Analysis

- **CloudWatch Insights**: Real-time log analysis
- **Log Aggregation**: Centralized log collection
- **Log Retention**: Configurable retention policies
- **Log Encryption**: Encrypted log storage

## Cost Optimization

### Resource Optimization

- **Lambda Optimization**: Memory and timeout optimization
- **DynamoDB Optimization**: Auto-scaling and capacity planning
- **API Gateway Optimization**: Caching and throttling
- **Storage Optimization**: Lifecycle policies and compression

### Cost Monitoring

- **Cost Allocation**: Resource tagging for cost tracking
- **Budget Alerts**: Cost threshold monitoring
- **Resource Rightsizing**: Optimize resource allocation
- **Reserved Capacity**: Reserved instance planning

## Disaster Recovery

### Backup Strategy

- **DynamoDB Backups**: Point-in-time recovery
- **Lambda Code**: Version control and deployment packages
- **Configuration**: Infrastructure as code backup
- **Data Replication**: Cross-region replication

### Recovery Procedures

- **RTO/RPO**: Recovery time and point objectives
- **Failover Procedures**: Automated failover processes
- **Data Restoration**: Backup restoration procedures
- **Service Recovery**: Service-specific recovery steps

## Troubleshooting

### Common Issues

1. **Deployment Failures**: Check CloudFormation stack events
2. **Permission Errors**: Verify IAM roles and policies
3. **Resource Limits**: Check AWS service limits
4. **Configuration Errors**: Validate CDK configuration

### Debugging Commands

```bash
# Check stack events
aws cloudformation describe-stack-events --stack-name FlyingDartsStack --profile dev

# Check Lambda logs
aws logs tail /aws/lambda/flyingdarts-x01-api --profile dev

# Check API Gateway logs
aws logs tail /aws/apigateway/flyingdarts-api --profile dev

# Validate CDK app
cdk synth --profile dev
```

### Support Resources

- **AWS Documentation**: Official AWS CDK documentation
- **CloudFormation**: AWS CloudFormation documentation
- **CDK Constructs**: AWS CDK constructs library
- **Community Support**: CDK community forums

## Related Projects

- **flyingdarts-backend**: Backend services deployed by this CDK
- **flyingdarts-frontend**: Frontend applications
- **flyingdarts-cdk-constructs**: Custom CDK constructs
- **flyingdarts-config**: Shared configuration

## Contributing

1. Follow CDK best practices and patterns
2. Add tests for new infrastructure components
3. Update documentation for infrastructure changes
4. Ensure security compliance for all resources
5. Follow the monorepo coding standards

## License

Part of the Flying Darts Turbo monorepo. See root LICENSE file for details.