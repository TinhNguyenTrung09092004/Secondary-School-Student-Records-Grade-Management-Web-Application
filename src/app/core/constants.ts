export const APP_CONSTANTS = {
  TOKEN_KEY: 'token',
  API_TIMEOUT: 30000,
  DATE_FORMAT: 'dd/MM/yyyy'
};

export const ROLES = {
  ADMIN: 'Admin',
  PRINCIPAL: 'Principal',
  HOME_ROOM_TEACHER: 'HomeRoomTeacher',
  ACADEMIC_AFFAIRS: 'AcademicAffairs',
  DEPARTMENT_HEAD: 'DepartmentHead',
  SUBJECT_TEACHER: 'SubjectTeacher',
  STUDENT: 'Student'
};

// Role name translations for display (English backend -> Vietnamese frontend)
export const ROLE_DISPLAY_NAMES: { [key: string]: string } = {
  'Admin': 'Quản trị viên',
  'Principal': 'Ban giám hiệu',
  'HomeRoomTeacher': 'Giáo viên chủ nhiệm',
  'AcademicAffairs': 'Giáo vụ',
  'DepartmentHead': 'Trưởng bộ môn',
  'SubjectTeacher': 'Giáo viên bộ môn',
  'Student': 'Học sinh'
};

// Helper function to get display name
export function getRoleDisplayName(roleKey: string): string {
  return ROLE_DISPLAY_NAMES[roleKey] || roleKey;
}

// Helper function to get all roles for dropdown
export function getAllRolesForDisplay(): { key: string; display: string }[] {
  return Object.keys(ROLE_DISPLAY_NAMES).map(key => ({
    key: key,
    display: ROLE_DISPLAY_NAMES[key]
  }));
}
