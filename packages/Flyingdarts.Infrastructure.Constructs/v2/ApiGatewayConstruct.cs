using Amazon.CDK.AWS.APIGateway;
using Amazon.JSII.JsonModel.Spec;
using Stage = Amazon.CDK.AWS.APIGateway.Stage;
using StageProps = Amazon.CDK.AWS.APIGateway.StageProps;

namespace Flyingdarts.Infrastructure.Constructs.v2;

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

    public ApiGatewayConstruct(Construct scope, string id, string environment, LambdaConstruct lambdaConstruct) : base(scope, id)
    {
        #region Users
        UsersApi = new RestApi(this, $"Flyingdarts-Backend-Users-RestApi-{environment}", new RestApiProps
        {
            RestApiName = $"Flyingdarts.Backend.Users.RestApi.{environment}",
            ApiKeySourceType = ApiKeySourceType.HEADER,
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

        profile.AddMethod("GET", new LambdaIntegration(lambdaConstruct.ProfileCreate, new LambdaIntegrationOptions { Proxy = true }));
        profile.AddMethod("POST", new LambdaIntegration(lambdaConstruct.ProfileCreate, new LambdaIntegrationOptions { Proxy = true }));
        profile.AddMethod("PUT", new LambdaIntegration(lambdaConstruct.ProfileCreate, new LambdaIntegrationOptions { Proxy = true }));

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
            tournaments.AddMethod("POST", new LambdaIntegration(lambdaConstruct.TournamentsCreate, new LambdaIntegrationOptions { Proxy = true }));
            tournaments.AddMethod("PUT", new LambdaIntegration(lambdaConstruct.TournamentsStart, new LambdaIntegrationOptions { Proxy = true }));

        var particpants = tournaments.AddResource("participants");
            particpants.AddMethod("POST", new LambdaIntegration(lambdaConstruct.TournamentsParticipantsCreate, new LambdaIntegrationOptions { Proxy = true }));

        var matches = tournaments.AddResource("matches");
            matches.AddMethod("PUT", new LambdaIntegration(lambdaConstruct.TournamentsMatchesUpdate, new LambdaIntegrationOptions { Proxy = true }));

        #endregion
        WebSocketApi = new WebSocketApi(this, $"Flyingdarts-Backend-Api-{environment}", new WebSocketApiProps
        {
            ApiName = $"Flyingdarts.Backend.Api.{environment}",
            ConnectRouteOptions = new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration($"Flyingdarts-Backend-Api-OnConnect-Integration-{environment}", lambdaConstruct.SignallingOnConnect)
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

        // For chat functionality 
        WebSocketStage.GrantManagementApiAccess(lambdaConstruct.SignallingOnDefault);
        lambdaConstruct.SignallingOnDefault.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);

        // For the x01 queue to notify players
        WebSocketStage.GrantManagementApiAccess(lambdaConstruct.GamesX01Queue);
        lambdaConstruct.GamesX01Queue.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);

        // create game (to play with friends)
        WebSocketApi.AddRoute("games/x01/create", new WebSocketRouteOptions
        {
            Integration = new WebSocketLambdaIntegration($"Games-Create-Integration-{environment}", lambdaConstruct.GamesX01Create),
            ReturnResponse = true
        });
        WebSocketStage.GrantManagementApiAccess(lambdaConstruct.GamesX01Create);
        lambdaConstruct.GamesX01Create.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);

        // join game (called when game is opened on client)
        WebSocketApi.AddRoute("games/x01/join", new WebSocketRouteOptions
        {
            Integration = new WebSocketLambdaIntegration($"Games-Join-Integration-{environment}", lambdaConstruct.GamesX01Join),
            ReturnResponse = true
        });
        WebSocketStage.GrantManagementApiAccess(lambdaConstruct.GamesX01Join);
        lambdaConstruct.GamesX01Join.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);

        // join queue (players join an actual fifo queue)
        WebSocketApi.AddRoute("games/x01/joinqueue", new WebSocketRouteOptions
        {
            Integration = new WebSocketLambdaIntegration($"Games-JoinQueue-Integration-{environment}", lambdaConstruct.GamesX01JoinQueue),
            ReturnResponse = true
        });

        WebSocketStage.GrantManagementApiAccess(lambdaConstruct.GamesX01JoinQueue);
        lambdaConstruct.GamesX01JoinQueue.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);

        WebSocketApi.AddRoute("games/x01/score", new WebSocketRouteOptions
        {
            Integration = new WebSocketLambdaIntegration($"Games-Score-Integration-{environment}", lambdaConstruct.GamesX01Score),
            ReturnResponse = true
        });
        WebSocketStage.GrantManagementApiAccess(lambdaConstruct.GamesX01Score);
        lambdaConstruct.GamesX01Score.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);

        // User routes
        WebSocketApi.AddRoute("user/profile/create", new WebSocketRouteOptions
        {
            Integration = new WebSocketLambdaIntegration($"User-Profile-Create-{environment}", lambdaConstruct.ProfileCreate),
            ReturnResponse = true
        });
        WebSocketStage.GrantManagementApiAccess(lambdaConstruct.ProfileCreate);
        lambdaConstruct.ProfileCreate.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);

        WebSocketApi.AddRoute("user/profile/update", new WebSocketRouteOptions
        {
            Integration = new WebSocketLambdaIntegration($"User-Profile-Update-{environment}", lambdaConstruct.ProfileUpdate),
            ReturnResponse = true
        });
        WebSocketStage.GrantManagementApiAccess(lambdaConstruct.ProfileUpdate);
        lambdaConstruct.ProfileUpdate.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);

        WebSocketApi.AddRoute("user/profile/get", new WebSocketRouteOptions
        {
            Integration = new WebSocketLambdaIntegration($"User-Profile-Get-{environment}", lambdaConstruct.ProfileGet),
            ReturnResponse = true
        });
        WebSocketStage.GrantManagementApiAccess(lambdaConstruct.ProfileGet);
        lambdaConstruct.ProfileGet.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);
    }
}