import { createAction, props } from '@ngrx/store';
import { Profile, UpdateProfile } from '@services/profile.service';

export const loadProfile = createAction('[Profile] Load Profile');
export const loadProfileSuccess = createAction(
    '[Profile] Load Profile Success',
    props<{ profile: Profile }>(),
);
export const loadProfileFailure = createAction('[Profile] Load Profile Failure');

export const updateProfile = createAction(
    '[Profile] Update Profile',
    props<{ profile: UpdateProfile }>(),
);
export const updateProfileSuccess = createAction('[Profile] Update Profile Success');
export const updateProfileFailure = createAction('[Profile] Update Profile Failure');
