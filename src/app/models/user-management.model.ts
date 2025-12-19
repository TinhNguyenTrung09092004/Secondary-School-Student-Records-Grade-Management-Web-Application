export interface UserDto {
  id: string;
  email: string;
  userName: string;
  emailConfirmed: boolean;
  roles: string[];
  lockoutEnd?: Date;
  isLockedOut: boolean;
  isAccountSetupComplete: boolean;
  scheduledDeletionDate?: Date;
}

export interface CreateUserDto {
  email: string;
  roles: string[];
}

export interface UpdateUserDto {
  email: string;
  roles: string[];
}

export interface UpdateUserPasswordDto {
  newPassword: string;
}

export interface RoleDto {
  id: string;
  name: string;
  userCount: number;
}

export interface UserRoleUpdateDto {
  roles: string[];
}
