
export interface UserProfileDetails {
    UserName: string;
    Country: string;
    Email: string;
    UserId?: string;

    isRegistered?: boolean;
    cameraPermissionGranted?: boolean;
    
    AuthProviderUserId?: string;
}
