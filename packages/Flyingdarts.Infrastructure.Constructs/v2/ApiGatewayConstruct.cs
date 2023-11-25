namespace Flyingdarts.Infrastructure.Constructs.v2;

public class ApiGatewayConstruct : Construct
{
    public WebSocketApi WebSocketApi { get; }
    public WebSocketStage WebSocketStage { get; }
    
    public ApiGatewayConstruct(Construct scope, string id, string environment,  LambdaConstruct lambdaConstruct) : base(scope, id)
    {
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
        
        // Game routes
        WebSocketApi.AddRoute("games/x01/join", new WebSocketRouteOptions
        {
            Integration = new WebSocketLambdaIntegration($"Games-Join-Integration-{environment}", lambdaConstruct.GamesX01Join),
            ReturnResponse = true
        });
        WebSocketStage.GrantManagementApiAccess(lambdaConstruct.GamesX01Join);
        lambdaConstruct.GamesX01Join.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);
        
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