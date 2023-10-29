import { CognitoAttributes } from "./cognito-attributes.model";


export interface CognitoUser {
  id: string;
  username: string;
  attributes: CognitoAttributes;
}
