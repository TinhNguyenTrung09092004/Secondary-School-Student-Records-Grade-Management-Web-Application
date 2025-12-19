import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BackupService } from '../../../services/backup.service';
import { BackupSchedule, BackupFile } from '../../../models/backup.model';

@Component({
  selector: 'app-backup',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './backup.component.html',
  styleUrl: './backup.component.css'
})
export class BackupComponent implements OnInit {
  private backupService = inject(BackupService);

  tables: string[] = [];
  selectedTables: string[] = [];
  backupFiles: BackupFile[] = [];
  schedule: BackupSchedule = {
    backupTime: '00:00',
    isEnabled: false
  };
  loading = false;
  errorMessage = '';
  successMessage = '';
  showRestoreConfirm = false;
  restoreFileName = '';
  confirmText = '';
  createBackupBeforeRestore = true;

  ngOnInit() {
    this.loadTables();
    this.loadBackupFiles();
    this.loadSchedule();
  }

  loadTables() {
    this.backupService.getTables().subscribe({
      next: (data) => {
        this.tables = data;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load tables';
        console.error(error);
      }
    });
  }

  loadBackupFiles() {
    this.backupService.getBackupFiles().subscribe({
      next: (data) => {
        this.backupFiles = data;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load backup files';
        console.error(error);
      }
    });
  }

  loadSchedule() {
    this.backupService.getSchedule().subscribe({
      next: (data) => {
        this.schedule = data;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load schedule';
        console.error(error);
      }
    });
  }

  toggleTable(table: string) {
    const index = this.selectedTables.indexOf(table);
    if (index > -1) {
      this.selectedTables.splice(index, 1);
    } else {
      this.selectedTables.push(table);
    }
  }

  selectAll() {
    this.selectedTables = [...this.tables];
  }

  deselectAll() {
    this.selectedTables = [];
  }

  createBackup() {
    if (this.selectedTables.length === 0) {
      this.errorMessage = 'Please select at least one table';
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.backupService.createBackup({ tables: this.selectedTables }).subscribe({
      next: (response) => {
        this.loading = false;
        this.successMessage = response.message;
        this.loadBackupFiles();
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = error.error?.message || 'Backup failed';
        console.error(error);
      }
    });
  }

  updateSchedule() {
    this.errorMessage = '';
    this.successMessage = '';

    this.backupService.updateSchedule(this.schedule).subscribe({
      next: (response) => {
        this.successMessage = response.message;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to update schedule';
        console.error(error);
      }
    });
  }

  downloadBackup(fileName: string) {
    this.backupService.downloadBackup(fileName).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = fileName;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        this.errorMessage = 'Failed to download backup file';
        console.error(error);
      }
    });
  }

  formatFileSize(bytes: number): string {
    if (bytes < 1024) return bytes + ' B';
    if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(2) + ' KB';
    return (bytes / (1024 * 1024)).toFixed(2) + ' MB';
  }

  confirmRestore(fileName: string) {
    this.restoreFileName = fileName;
    this.showRestoreConfirm = true;
    this.confirmText = '';
    this.createBackupBeforeRestore = true;
    this.errorMessage = '';
    this.successMessage = '';
  }

  cancelRestore() {
    this.showRestoreConfirm = false;
    this.restoreFileName = '';
    this.confirmText = '';
  }

  restoreBackup() {
    if (this.confirmText !== 'CONFIRM') {
      this.errorMessage = 'Please type CONFIRM to proceed';
      return;
    }

    this.showRestoreConfirm = false;
    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    // If backup before restore is enabled, create backup first
    if (this.createBackupBeforeRestore) {
      this.successMessage = 'Creating safety backup before restore...';

      this.backupService.createBackup({ tables: this.tables }).subscribe({
        next: (backupResponse) => {
          this.successMessage = `Safety backup created: ${backupResponse.fileName}. Now restoring...`;
          this.performRestore();
        },
        error: (error) => {
          this.loading = false;
          this.errorMessage = 'Failed to create safety backup. Restore cancelled: ' + (error.error?.message || 'Unknown error');
          console.error(error);
        }
      });
    } else {
      this.performRestore();
    }
  }

  private performRestore() {
    this.backupService.restoreBackup(this.restoreFileName).subscribe({
      next: (response) => {
        this.loading = false;
        this.successMessage = response.message + ' - Page will reload in 3 seconds...';
        setTimeout(() => {
          window.location.reload();
        }, 3000);
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = error.error?.message || 'Restore failed';
        console.error(error);
      }
    });
  }
}
