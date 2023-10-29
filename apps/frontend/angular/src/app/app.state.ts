import { UserProfileDetails } from "./shared/models/user-profile-details.model";

export const initialApplicationState: AppState = {
    loading: false,
    profile: null
}
export interface AppState { 
    loading: boolean;
    profile: UserProfileDetails | null;
}

