public class ApiGatewayConstruct : Construct
{
    public RestApi UsersApi { get; }
    public Deployment UsersDeployment { get; }
    public Stage UsersStage { get; }

    public RestApi TournamentsApi { get; }
    public Deployment TournamentsDeployment { get; }
    public Stage TournamentsStage { get; }

    public WebSocketApi WebSocketApi { get; }
    public WebSocketStage WebSocketStage { get; }

    public ApiGatewayConstruct(Construct scope, string id, string environment, LambdaConstruct lambdaConstruct, AuthorizersConstruct authorizersConstruct) : base(scope, id)
    {
        
        #region Users
        UsersApi = new RestApi(this, $"Flyingdarts-Backend-Users-RestApi-{environment}", new RestApiProps
        {
            RestApiName = $"Flyingdarts.Backend.Users.RestApi.{environment}",
            ApiKeySourceType = ApiKeySourceType.HEADER,
            DefaultCorsPreflightOptions = new CorsOptions
            {
                AllowOrigins = Cors.ALL_ORIGINS,
                AllowMethods = Cors.ALL_METHODS,
                AllowHeaders = new []{"Content-Type", "Authorization"}
            }
        });

        UsersDeployment = new Deployment(this, $"Flyingdarts-Backend-Users-RestApi-Deployment-{environment}", new DeploymentProps
        {
            Api = UsersApi,
            Description = $"A restfull {environment} Users api for flyingdarts",
        });

        UsersStage = new Stage(this, $"Flyingdarts-Backend-Users-RestApi-Stage-{environment}", new StageProps
        {
            Deployment = UsersDeployment,
            Description = $"deployment for the restull {environment} Users api",
            StageName = environment,
        });


        UsersApi.AddApiKey($"Flyingdarts-Angular-Frontend-Users-ApiKey-{environment}", new ApiKeyOptions
        {
            ApiKeyName = $"AngularFrontend{environment}UsersApiKey",
            Value = environment == "Development" ? "6de6d6d76ce00be4d0b143b2d9694eb9" : "80031499b8fd6270417763a1d978bd41"
        });

        UsersApi.AddUsagePlan($"Flyingdarts-Backend-Users-RestApi-UsagePlan-{environment}", new UsagePlanProps
        {
            ApiStages = new[]
            {
                new UsagePlanPerApiStage
                {
                    Api = UsersApi,
                    Stage = UsersStage,
                }
            }
        });

        var users = UsersApi.Root.AddResource("users");
        var profile = users.AddResource("profile");
        
        profile.AddMethod("GET", new LambdaIntegration(lambdaConstruct.ProfileApi, new LambdaIntegrationOptions { Proxy = true }), new MethodOptions
        {
            AuthorizationType = AuthorizationType.CUSTOM,
            Authorizer = authorizersConstruct.UsersAuthorizer
        });
        profile.AddMethod("POST", new LambdaIntegration(lambdaConstruct.ProfileApi, new LambdaIntegrationOptions { Proxy = true }), new MethodOptions
        {
            AuthorizationType = AuthorizationType.CUSTOM,
            Authorizer = authorizersConstruct.UsersAuthorizer
        });
        profile.AddMethod("PUT", new LambdaIntegration(lambdaConstruct.ProfileApi, new LambdaIntegrationOptions { Proxy = true }), new MethodOptions
        {
            AuthorizationType = AuthorizationType.CUSTOM,
            Authorizer = authorizersConstruct.UsersAuthorizer
        });

        #endregion

        #region Tournaments
        
        TournamentsApi = new RestApi(this, $"Flyingdarts-Backend-Tournaments-RestApi-{environment}", new RestApiProps
        {
            RestApiName = $"Flyingdarts.Backend.Tournaments.RestApi.{environment}",
            ApiKeySourceType = ApiKeySourceType.HEADER,
        });

        TournamentsDeployment = new Deployment(this, $"Flyingdarts-Backend-Tournaments-RestApi-Deployment-{environment}", new DeploymentProps
        {
            Api = TournamentsApi,
            Description = $"A restfull {environment} api for flyingdarts",
        });

        TournamentsStage = new Stage(this, $"Flyingdarts-Backend-Tournaments-RestApi-Stage-{environment}", new StageProps
        {
            Deployment = TournamentsDeployment,
            Description = $"deployment for the restull {environment} api",
            StageName = environment,
        });

       
        TournamentsApi.AddApiKey($"Flyingdarts-Angular-Frontend-Tournaments-ApiKey-{environment}", new ApiKeyOptions {
            ApiKeyName = $"AngularFrontend{environment}TournamentsApiKey",
            Value = environment == "Development" ? "efcf5cf6027bdddaca4864354b85c12c" : "f2115380d9cd08d4246b11f1e027ddbd"
        });

        TournamentsApi.AddUsagePlan($"Flyingdarts-Backend-Tournaments-RestApi-UsagePlan-{environment}", new UsagePlanProps
        {
            ApiStages = new[]
            {
                new UsagePlanPerApiStage
                {
                    Api = TournamentsApi,
                    Stage = TournamentsStage,
                }
            } 
        });
        var tournaments = TournamentsApi.Root.AddResource("tournaments");
            tournaments.AddMethod("POST", new LambdaIntegration(lambdaConstruct.TournamentsApi, new LambdaIntegrationOptions { Proxy = true }), new MethodOptions
        {
            AuthorizationType = AuthorizationType.CUSTOM,
            Authorizer = authorizersConstruct.TournamentsAuthorizer
        });
            tournaments.AddMethod("PUT", new LambdaIntegration(lambdaConstruct.TournamentsApi, new LambdaIntegrationOptions { Proxy = true }), new MethodOptions
        {
            AuthorizationType = AuthorizationType.CUSTOM,
            Authorizer = authorizersConstruct.TournamentsAuthorizer
        });

        var particpants = tournaments.AddResource("participants");
            particpants.AddMethod("POST", new LambdaIntegration(lambdaConstruct.TournamentsApi, new LambdaIntegrationOptions { Proxy = true }), new MethodOptions
        {
            AuthorizationType = AuthorizationType.CUSTOM,
            Authorizer = authorizersConstruct.TournamentsAuthorizer
        });

        var matches = tournaments.AddResource("matches");
            matches.AddMethod("PUT", new LambdaIntegration(lambdaConstruct.TournamentsApi, new LambdaIntegrationOptions { Proxy = true }), new MethodOptions
        {
            AuthorizationType = AuthorizationType.CUSTOM,
            Authorizer = authorizersConstruct.TournamentsAuthorizer
        });

        #endregion

        #region WebSocketApi

        WebSocketApi = new WebSocketApi(this, $"Flyingdarts-Backend-Api-{environment}", new WebSocketApiProps
        {
            ApiName = $"Flyingdarts.Backend.Api.{environment}",
            ConnectRouteOptions = new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration($"Flyingdarts-Backend-Api-OnConnect-Integration-{environment}", lambdaConstruct.SignallingOnConnect),
                Authorizer = authorizersConstruct.WebSocketApiConnectAuthorizer
            },
            DefaultRouteOptions = new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration($"Flyingdarts-Backend-Api-OnDefault-Integration-{environment}", lambdaConstruct.SignallingOnDefault)
            },
            DisconnectRouteOptions = new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration($"Flyingdarts-Backend-Api-OnDisconnect-Integration-{environment}", lambdaConstruct.SignallingOnDisconnect)
            }
        });
        
        WebSocketStage = new WebSocketStage(this, $"Flyingdarts-Backend-Api-Stage-{environment}", new WebSocketStageProps
        {
            WebSocketApi = WebSocketApi,
            StageName = environment,
            AutoDeploy = true
        });
        
        // for authentication
        lambdaConstruct.AuthLambda.AddEnvironment("ConnectMethodArn",
            $"arn:aws:execute-api:{System.Environment.GetEnvironmentVariable("AWS_REGION")!}:{System.Environment.GetEnvironmentVariable("AWS_ACCOUNT")!}:{WebSocketApi.ApiId}/Development/$connect");


        // For chat functionality 
        WebSocketStage.GrantManagementApiAccess(lambdaConstruct.SignallingOnDefault);
        lambdaConstruct.SignallingOnDefault.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);

        // For the x01 queue to notify players
        WebSocketStage.GrantManagementApiAccess(lambdaConstruct.GamesX01Queue);
        lambdaConstruct.GamesX01Queue.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);

        // create game (to play with friends)
        WebSocketApi.AddRoute("games/x01/create", new WebSocketRouteOptions
        {
            Integration = new WebSocketLambdaIntegration($"Games-Create-Integration-{environment}", lambdaConstruct.GamesX01Api),
            ReturnResponse = true
        });

        // join game (called when game is opened on client)
        WebSocketApi.AddRoute("games/x01/join", new WebSocketRouteOptions
        {
            Integration = new WebSocketLambdaIntegration($"Games-Join-Integration-{environment}", lambdaConstruct.GamesX01Api),
            ReturnResponse = true
        });
      
        // join queue (players join an actual fifo queue)
        WebSocketApi.AddRoute("games/x01/joinqueue", new WebSocketRouteOptions
        {
            Integration = new WebSocketLambdaIntegration($"Games-JoinQueue-Integration-{environment}", lambdaConstruct.GamesX01Api),
            ReturnResponse = true
        });

         // send score to this endpoint
        WebSocketApi.AddRoute("games/x01/score", new WebSocketRouteOptions
        {
            Integration = new WebSocketLambdaIntegration($"Games-Score-Integration-{environment}", lambdaConstruct.GamesX01Api),
            ReturnResponse = true
        });
        
        WebSocketStage.GrantManagementApiAccess(lambdaConstruct.GamesX01Api);
        lambdaConstruct.GamesX01Api.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);
        
        #endregion
    }
}