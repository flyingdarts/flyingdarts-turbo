import { UserProfileDetails } from "./shared/models/user-profile-details.model";

export const initialApplicationState: AppState = {
    loading: false,
    profile: null,
    preferedSettings: {
        x01Sets: 1,
        x01Legs: 3
    }
}
export interface AppState { 
    loading: boolean;
    profile: UserProfileDetails | null;
    preferedSettings: PreferedSettings
}

export interface PreferedSettings {
    x01Sets: number;
    x01Legs: number;
}

