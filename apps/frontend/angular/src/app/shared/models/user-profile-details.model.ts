
export interface UserProfileDetails {
    UserName: string;
    Country: string;
    Email: string;
    UserId?: string;

    isRegistered?: boolean;
    cameraPermissionGranted?: boolean;
    cognitoUserId?: string;
    cognitoUserName?: string;
}
