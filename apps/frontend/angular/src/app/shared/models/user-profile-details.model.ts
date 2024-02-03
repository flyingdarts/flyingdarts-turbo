
export interface UserProfileDetails {
    UserId?: string;
    UserName: string;
    Country: string;
    Email: string;

    isRegistered?: boolean;
    cameraPermissionGranted?: boolean;
    
    AuthProviderUserId?: string;
}
