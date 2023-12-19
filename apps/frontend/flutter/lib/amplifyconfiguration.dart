const amplifyconfig = '''{
    "UserAgent": "aws-amplify-cli/2.0",
    "Version": "1.0",
    "auth": {
        "plugins": {
            "awsCognitoAuthPlugin": {
                "UserAgent": "aws-amplify-cli/0.1.0",
                "Version": "0.1.0",
                "IdentityManager": {
                    "Default": {}
                },
                "CredentialsProvider": {
                    "CognitoIdentity": {
                        "Default": {
                            "PoolId": "eu-west-1:e2168b52-d7c1-452b-bc05-0e4d06bcba71",
                            "Region": "eu-west-1"
                        }
                    }
                },
                "CognitoUserPool": {
                    "Default": {
                        "PoolId": "eu-west-1_gInTJENXg",
                        "AppClientId": "7duge0a35ki8fvq8ip8v4miop6",
                        "Region": "eu-west-1"
                    }
                },
                "Auth": {
                    "Default": {
                        "OAuth": {
                            "WebDomain": "nabzwdf7t9ey-production.auth.eu-west-1.amazoncognito.com",
                            "AppClientId": "7duge0a35ki8fvq8ip8v4miop6",
                            "SignInRedirectURI": "flyingdartscontrol://login,https://flyingdarts.net/login",
                            "SignOutRedirectURI": "flyingdartscontrol://logout,https://flyingdarts.net/logout",
                            "Scopes": [
                                "aws.cognito.signin.user.admin",
                                "email",
                                "openid",
                                "phone",
                                "profile"
                            ]
                        },
                        "authenticationFlowType": "USER_SRP_AUTH",
                        "socialProviders": [
                            "FACEBOOK"
                        ],
                        "usernameAttributes": [
                            "EMAIL"
                        ],
                        "signupAttributes": [],
                        "passwordProtectionSettings": {
                            "passwordPolicyMinLength": 8,
                            "passwordPolicyCharacters": [
                                "REQUIRES_LOWERCASE",
                                "REQUIRES_UPPERCASE",
                                "REQUIRES_NUMBERS",
                                "REQUIRES_SYMBOLS"
                            ]
                        },
                        "mfaConfiguration": "OFF",
                        "mfaTypes": [],
                        "verificationMechanisms": [
                            "EMAIL"
                        ]
                    }
                }
            }
        }
    }
}''';
