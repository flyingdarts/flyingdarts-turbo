
export interface CreateUserProfileCommand {
  CognitoUserId: string;
  CognitoUserName: string;
  UserName: string;
  Email: string;
  Country: string;
}