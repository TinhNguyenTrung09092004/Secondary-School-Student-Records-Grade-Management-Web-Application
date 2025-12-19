export interface BackupSchedule {
  backupTime: string;
  isEnabled: boolean;
  lastBackupDate?: Date;
}

export interface BackupFile {
  fileName: string;
  createdDate: Date;
  fileSizeBytes: number;
}

export interface CreateBackupRequest {
  tables: string[];
}

export interface BackupResponse {
  message: string;
  fileName?: string;
}
