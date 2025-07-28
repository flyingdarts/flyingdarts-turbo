namespace Flyingdarts.CDK.Constructs.v2;

public class ApiGatewayConstruct : Construct
{
    public RestApi FriendsApi { get; }
    public Deployment FriendsDeployment { get; }
    public Stage FriendsStage { get; }

    public WebSocketApi WebSocketApi { get; }
    public WebSocketStage WebSocketStage { get; }

    public ApiGatewayConstruct(
        Construct scope,
        string id,
        string environment,
        LambdaConstruct lambdaConstruct,
        AuthorizersConstruct authorizersConstruct
    )
        : base(scope, id)
    {
        #region Friends
        FriendsApi = new RestApi(
            this,
            $"Flyingdarts-Backend-Friends-Api-{environment}",
            new RestApiProps
            {
                RestApiName = $"Flyingdarts.Backend.Friends.Api.{environment}",
                ApiKeySourceType = ApiKeySourceType.HEADER,
                DefaultCorsPreflightOptions = new CorsOptions
                {
                    AllowOrigins = Cors.ALL_ORIGINS,
                    AllowMethods = Cors.ALL_METHODS,
                    AllowHeaders = new[] { "Content-Type", "Authorization" },
                },
            }
        );

        FriendsDeployment = new Deployment(
            this,
            $"Flyingdarts-Backend-Friends-Api-Deployment-{environment}",
            new DeploymentProps { Api = FriendsApi, Description = $"A restfull {environment} Friends api for flyingdarts" }
        );

        FriendsStage = new Stage(
            this,
            $"Flyingdarts-Backend-Friends-Api-Stage-{environment}",
            new StageProps
            {
                Deployment = FriendsDeployment,
                Description = $"deployment for the restull {environment} Friends api",
                StageName = environment,
            }
        );

        FriendsApi.AddUsagePlan(
            $"Flyingdarts-Backend-Friends-Api-UsagePlan-{environment}",
            new UsagePlanProps
            {
                ApiStages = new[]
                {
                    new UsagePlanPerApiStage { Api = FriendsApi, Stage = FriendsStage },
                },
            }
        );

        var friends = FriendsApi.Root.AddResource(
            "friends",
            new ResourceOptions
            {
                DefaultMethodOptions = new MethodOptions
                {
                    AuthorizationType = AuthorizationType.CUSTOM,
                    Authorizer = authorizersConstruct.UsersAuthorizer,
                },
            }
        );
        // GET /friends - Get user's friends list
        friends.AddMethod("GET", new LambdaIntegration(lambdaConstruct.FriendsApi));

        // GET /user - Get user dto
        var user = friends.AddResource("user");
        user.AddMethod("GET", new LambdaIntegration(lambdaConstruct.FriendsApi));

        // POST /friends/search - Search for users to add as friends
        var friendsSearch = friends.AddResource("search");
        friendsSearch.AddMethod("POST", new LambdaIntegration(lambdaConstruct.FriendsApi));

        // POST /friends/request - Send friend request
        var friendsRequest = friends.AddResource("request");
        friendsRequest.AddMethod("POST", new LambdaIntegration(lambdaConstruct.FriendsApi));

        // PUT /friends/request/{requestId} - Accept/decline friend request
        var friendsRequestWithId = friendsRequest.AddResource("{requestId}");
        friendsRequestWithId.AddMethod("PUT", new LambdaIntegration(lambdaConstruct.FriendsApi));

        // DELETE /friends/{friendId} - Remove friend
        var friendsWithId = friends.AddResource("{friendId}");
        friendsWithId.AddMethod("DELETE", new LambdaIntegration(lambdaConstruct.FriendsApi));

        // GET /friends/requests - Get pending friend requests
        var friendsRequests = friends.AddResource("requests");
        friendsRequests.AddMethod("GET", new LambdaIntegration(lambdaConstruct.FriendsApi));

        // POST /friends/invite - Invite friend to game
        var friendsInvite = friends.AddResource("invite");
        friendsInvite.AddMethod("POST", new LambdaIntegration(lambdaConstruct.FriendsApi));
        #endregion

        #region WebSocketApi

        WebSocketApi = new WebSocketApi(
            this,
            $"Flyingdarts-Backend-Api-{environment}",
            new WebSocketApiProps
            {
                ApiName = $"Flyingdarts.Backend.Api.{environment}",
                ConnectRouteOptions = new WebSocketRouteOptions
                {
                    Integration = new WebSocketLambdaIntegration(
                        $"Flyingdarts-Backend-Api-OnConnect-Integration-{environment}",
                        lambdaConstruct.SignallingApi
                    ),
                    Authorizer = authorizersConstruct.WebSocketApiConnectAuthorizer,
                },
                DefaultRouteOptions = new WebSocketRouteOptions
                {
                    Integration = new WebSocketLambdaIntegration(
                        $"Flyingdarts-Backend-Api-OnDefault-Integration-{environment}",
                        lambdaConstruct.SignallingApi
                    ),
                },
                DisconnectRouteOptions = new WebSocketRouteOptions
                {
                    Integration = new WebSocketLambdaIntegration(
                        $"Flyingdarts-Backend-Api-OnDisconnect-Integration-{environment}",
                        lambdaConstruct.SignallingApi
                    ),
                },
            }
        );

        WebSocketStage = new WebSocketStage(
            this,
            $"Flyingdarts-Backend-Api-Stage-{environment}",
            new WebSocketStageProps
            {
                WebSocketApi = WebSocketApi,
                StageName = environment,
                AutoDeploy = true,
            }
        );

        // for authentication
        lambdaConstruct.AuthLambda.AddEnvironment(
            "ConnectMethodArn",
            $"arn:aws:execute-api:{System.Environment.GetEnvironmentVariable("AWS_REGION")!}:{System.Environment.GetEnvironmentVariable("AWS_ACCOUNT")!}:{WebSocketApi.ApiId}/Development/$connect"
        );

        // For chat functionality
        lambdaConstruct.SignallingApi.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);

        // Grant SignallingApi permission to invoke WebSocket API
        lambdaConstruct.SignallingApi.AddToRolePolicy(
            new PolicyStatement(
                new PolicyStatementProps
                {
                    Actions = new[] { "execute-api:ManageConnections" },
                    Resources = new[] { $"arn:aws:execute-api:*:*:{WebSocketApi.ApiId}/*" },
                }
            )
        );

        // create game (to play with friends)
        WebSocketApi.AddRoute(
            "games/x01/create",
            new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration($"Games-Create-Integration-{environment}", lambdaConstruct.GamesX01Api),
                ReturnResponse = true,
            }
        );

        // join game (called when game is opened on client)
        WebSocketApi.AddRoute(
            "games/x01/join",
            new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration($"Games-Join-Integration-{environment}", lambdaConstruct.GamesX01Api),
                ReturnResponse = true,
            }
        );

        // send score to this endpoint
        WebSocketApi.AddRoute(
            "games/x01/score",
            new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration($"Games-Score-Integration-{environment}", lambdaConstruct.GamesX01Api),
                ReturnResponse = true,
            }
        );

        lambdaConstruct.GamesX01Api.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);

        // Grant GamesX01Api permission to invoke WebSocket API
        lambdaConstruct.GamesX01Api.AddToRolePolicy(
            new PolicyStatement(
                new PolicyStatementProps
                {
                    Actions = new[] { "execute-api:ManageConnections" },
                    Resources = new[] { $"arn:aws:execute-api:*:*:{WebSocketApi.ApiId}/*" },
                }
            )
        );

        #endregion

        lambdaConstruct.FriendsApi.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);

        // Grant FriendsApi permission to invoke WebSocket API
        lambdaConstruct.FriendsApi.AddToRolePolicy(
            new PolicyStatement(
                new PolicyStatementProps
                {
                    Actions = new[] { "execute-api:ManageConnections" },
                    Resources = new[] { $"arn:aws:execute-api:*:*:{WebSocketApi.ApiId}/*" },
                }
            )
        );
    }
}
