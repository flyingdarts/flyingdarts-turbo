namespace Flyingdarts.CDK.Constructs;

public class ApiGatewayConstruct : Construct
{
    public RestApi FriendsApi { get; }
    public Deployment FriendsDeployment { get; }
    public Stage FriendsStage { get; }

    public WebSocketApi WebSocketApi { get; }
    public WebSocketStage WebSocketStage { get; }
    public string WebSocketApiUrl => WebSocketStage.Url;
    public string FriendsApiUrl => FriendsApi.Url;

    public ApiGatewayConstruct(Construct scope, string id, ApiGatewayConstructProps props)
        : base(scope, id)
    {
        #region Friends
        FriendsApi = new RestApi(
            this,
            props.GetResourceIdentifier(nameof(FriendsApi)),
            new RestApiProps
            {
                RestApiName = $"Flyingdarts.Backend.Friends.Api.{props.DeploymentEnvironment.Name}",
                ApiKeySourceType = ApiKeySourceType.HEADER,
                DefaultCorsPreflightOptions = new CorsOptions
                {
                    AllowOrigins = Cors.ALL_ORIGINS,
                    AllowMethods = Cors.ALL_METHODS,
                    AllowHeaders = ["Content-Type", "Authorization"],
                },
            }
        );

        FriendsDeployment = new Deployment(
            this,
            props.GetResourceIdentifier(nameof(FriendsDeployment)),
            new DeploymentProps
            {
                Api = FriendsApi,
                Description = $"A restfull Friends api for flyingdarts",
            }
        );

        FriendsStage = new Stage(
            this,
            props.GetResourceIdentifier(nameof(FriendsStage)),
            new StageProps
            {
                Deployment = FriendsDeployment,
                Description =
                    $"Deployment for the restful {props.DeploymentEnvironment.Name} Friends api",
                StageName = props.DeploymentEnvironment.Name,
            }
        );

        FriendsApi.AddUsagePlan(
            props.GetResourceIdentifier($"{nameof(FriendsApi)}-{nameof(UsagePlan)}"),
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
                    Authorizer = props.AuthorizersConstruct.RestApiAuthorizer,
                },
            }
        );
        // GET /friends - Get user's friends list
        friends.AddMethod("GET", new LambdaIntegration(props.LambdaConstruct.Friends));

        // GET /user - Get user dto
        var user = friends.AddResource("user");
        user.AddMethod("GET", new LambdaIntegration(props.LambdaConstruct.Friends));

        // POST /friends/search - Search for users to add as friends
        var friendsSearch = friends.AddResource("search");
        friendsSearch.AddMethod("POST", new LambdaIntegration(props.LambdaConstruct.Friends));

        // POST /friends/request - Send friend request
        var friendsRequest = friends.AddResource("request");
        friendsRequest.AddMethod("POST", new LambdaIntegration(props.LambdaConstruct.Friends));

        // PUT /friends/request/{requestId} - Accept/decline friend request
        var friendsRequestWithId = friendsRequest.AddResource("{requestId}");
        friendsRequestWithId.AddMethod("PUT", new LambdaIntegration(props.LambdaConstruct.Friends));

        // DELETE /friends/{friendId} - Remove friend
        var friendsWithId = friends.AddResource("{friendId}");
        friendsWithId.AddMethod("DELETE", new LambdaIntegration(props.LambdaConstruct.Friends));

        // GET /friends/requests - Get pending friend requests
        var friendsRequests = friends.AddResource("requests");
        friendsRequests.AddMethod("GET", new LambdaIntegration(props.LambdaConstruct.Friends));

        // POST /friends/invite - Invite friend to game
        var friendsInvite = friends.AddResource("invite");
        friendsInvite.AddMethod("POST", new LambdaIntegration(props.LambdaConstruct.Friends));
        #endregion

        #region WebSocketApi

        WebSocketApi = new WebSocketApi(
            this,
            props.GetResourceIdentifier(nameof(WebSocketApi)),
            new WebSocketApiProps
            {
                ApiName = $"Flyingdarts.Backend.Api.{props.DeploymentEnvironment.Name}",
                ConnectRouteOptions = new WebSocketRouteOptions
                {
                    Integration = new WebSocketLambdaIntegration(
                        props.GetResourceIdentifier(
                            $"ConnectRouteOptions-{nameof(WebSocketLambdaIntegration)}"
                        ),
                        props.LambdaConstruct.Signalling
                    ),
                    Authorizer = props.AuthorizersConstruct.WebSocketApiConnectAuthorizer,
                },
                DefaultRouteOptions = new WebSocketRouteOptions
                {
                    Integration = new WebSocketLambdaIntegration(
                        props.GetResourceIdentifier(
                            $"DefaultRouteOptions-{nameof(WebSocketLambdaIntegration)}"
                        ),
                        props.LambdaConstruct.Signalling
                    ),
                },
                DisconnectRouteOptions = new WebSocketRouteOptions
                {
                    Integration = new WebSocketLambdaIntegration(
                        props.GetResourceIdentifier(
                            $"DisconnectRouteOptions-{nameof(WebSocketLambdaIntegration)}"
                        ),
                        props.LambdaConstruct.Signalling
                    ),
                },
            }
        );

        WebSocketStage = new WebSocketStage(
            this,
            props.GetResourceIdentifier(nameof(WebSocketStage)),
            new WebSocketStageProps
            {
                WebSocketApi = WebSocketApi,
                StageName = props.DeploymentEnvironment.Name,
                AutoDeploy = true,
            }
        );

        // for authentication
        props.LambdaConstruct.Authorizer.AddEnvironment(
            "ConnectMethodArn",
            $"arn:aws:execute-api:{System.Environment.GetEnvironmentVariable("AWS_REGION")!}:{System.Environment.GetEnvironmentVariable("AWS_ACCOUNT")!}:{WebSocketApi.ApiId}/Development/$connect"
        );

        props.LambdaConstruct.Signalling.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);

        // Allow signalling api to send messages to clients
        props.LambdaConstruct.Signalling.AddToRolePolicy(
            new PolicyStatement(
                new PolicyStatementProps
                {
                    Actions = ["execute-api:ManageConnections"],
                    Resources = [$"arn:aws:execute-api:*:*:{WebSocketApi.ApiId}/*"],
                }
            )
        );

        // create game (to play with friends)
        WebSocketApi.AddRoute(
            "games/x01/create",
            new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration(
                    props.GetResourceIdentifier(
                        $"Games-X01-Create-{nameof(WebSocketLambdaIntegration)}"
                    ),
                    props.LambdaConstruct.Backend
                ),
                ReturnResponse = true,
            }
        );

        // join game (called when game is opened on client)
        WebSocketApi.AddRoute(
            "games/x01/join",
            new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration(
                    props.GetResourceIdentifier(
                        $"Games-X01-Join-{nameof(WebSocketLambdaIntegration)}"
                    ),
                    props.LambdaConstruct.Backend
                ),
                ReturnResponse = true,
            }
        );

        // send score to this endpoint
        WebSocketApi.AddRoute(
            "games/x01/score",
            new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration(
                    props.GetResourceIdentifier(
                        $"Games-X01-Score-{nameof(WebSocketLambdaIntegration)}"
                    ),
                    props.LambdaConstruct.Backend
                ),
                ReturnResponse = true,
            }
        );

        props.LambdaConstruct.Backend.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);

        // Grant GamesX01Api permission to invoke WebSocket API
        props.LambdaConstruct.Backend.AddToRolePolicy(
            new PolicyStatement(
                new PolicyStatementProps
                {
                    Actions = ["execute-api:ManageConnections"],
                    Resources = [$"arn:aws:execute-api:*:*:{WebSocketApi.ApiId}/*"],
                }
            )
        );

        #endregion

        props.LambdaConstruct.Friends.AddEnvironment("WebSocketApiUrl", WebSocketStage.Url);

        // Grant FriendsApi permission to invoke WebSocket API
        props.LambdaConstruct.Friends.AddToRolePolicy(
            new PolicyStatement(
                new PolicyStatementProps
                {
                    Actions = ["execute-api:ManageConnections"],
                    Resources = [$"arn:aws:execute-api:*:*:{WebSocketApi.ApiId}/*"],
                }
            )
        );
    }
}
