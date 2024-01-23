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
                            "PoolId": "eu-west-1:6e03ae81-c695-45de-82f1-6e1398f6ad30",
                            "Region": "eu-west-1"
                        }
                    }
                },
                "CognitoUserPool": {
                    "Default": {
                        "PoolId": "eu-west-1_eJ59ibbay",
                        "AppClientId": "60sev9r9l9mrbo9iv5qt6902b2",
                        "Region": "eu-west-1"
                    }
                },
                "Auth": {
                    "Default": {
                        "OAuth": {
                            "WebDomain": "nabzwdf7t9ey-staging.auth.eu-west-1.amazoncognito.com",
                            "AppClientId": "60sev9r9l9mrbo9iv5qt6902b2",
                            "SignInRedirectURI": "flyingdartscontrol://login,https://staging.flyingdarts.net/login",
                            "SignOutRedirectURI": "flyingdartscontrol://logout,https://staging.flyingdarts.net/logout",
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
                            "GOOGLE"
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
