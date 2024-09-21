import { createFeatureSelector } from '@ngrx/store';
import { Role } from '@services/user.service';
import { DataSourceState } from './data-source.state';

export type RolesState = DataSourceState<Role>;

export const selectRolesFeature = createFeatureSelector<RolesState>('roles');
