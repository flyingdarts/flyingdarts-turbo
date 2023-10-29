import { CognitoIdentity } from "./cognito-identity.model";


export interface CognitoAttributes {
  sub: string;
  identities: CognitoIdentity[];
}
