import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserManagementService } from '../../../services/user-management.service';
import { AuthService } from '../../../services/auth.service';
import { UserDto, CreateUserDto, UpdateUserDto, RoleDto } from '../../../models/user-management.model';
import { getRoleDisplayName } from '../../../core/constants';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-management.component.html'
})
export class UserManagementComponent implements OnInit {
  private userManagementService = inject(UserManagementService);
  private authService = inject(AuthService);

  currentUserId: string | null = null;

  users: UserDto[] = [];
  roles: RoleDto[] = [];
  selectedUser: UserDto | null = null;
  showCreateModal = false;
  showEditModal = false;
  showRoleModal = false;
  loading = false;
  errorMessage = '';
  successMessage = '';

  newUser: CreateUserDto = {
    email: '',
    roles: []
  };

  editUser: UpdateUserDto = {
    email: '',
    roles: []
  };

  ngOnInit(): void {
    // Get current user ID
    this.authService.currentUser$.subscribe(user => {
      this.currentUserId = user?.id || null;
    });

    this.loadUsers();
    this.loadRoles();
  }

  loadUsers(): void {
    this.loading = true;
    this.userManagementService.getAllUsers().subscribe({
      next: (users) => {
        this.users = users;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách người dùng');
        this.loading = false;
      }
    });
  }

  loadRoles(): void {
    this.userManagementService.getAllRoles().subscribe({
      next: (roles) => {
        this.roles = roles;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách quyền');
      }
    });
  }

  openCreateModal(): void {
    this.newUser = { email: '', roles: [] };
    this.showCreateModal = true;
    this.clearMessages();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createUser(): void {
    this.loading = true;
    this.userManagementService.createUser(this.newUser).subscribe({
      next: (user) => {
        this.showSuccess('Người dùng đã được tạo thành công');
        this.loadUsers();
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tạo người dùng');
        this.loading = false;
      }
    });
  }

  openEditModal(user: UserDto): void {
    this.selectedUser = user;
    this.editUser = {
      email: user.email,
      roles: [...user.roles]
    };
    this.showEditModal = true;
    this.clearMessages();
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.selectedUser = null;
  }

  updateUser(): void {
    if (!this.selectedUser) return;

    this.loading = true;
    this.userManagementService.updateUser(this.selectedUser.id, this.editUser).subscribe({
      next: (user) => {
        this.showSuccess('Người dùng đã được cập nhật thành công');
        this.loadUsers();
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể cập nhật người dùng');
        this.loading = false;
      }
    });
  }


  openRoleModal(user: UserDto): void {
    this.selectedUser = user;
    this.showRoleModal = true;
    this.clearMessages();
  }

  closeRoleModal(): void {
    this.showRoleModal = false;
    this.selectedUser = null;
  }

  toggleRole(roleName: string): void {
    if (!this.selectedUser) return;

    const index = this.selectedUser.roles.indexOf(roleName);
    if (index > -1) {
      // Check if this is the last admin
      if (roleName === 'Admin' && !this.canRemoveAdminRole()) {
        this.showError('Không thể xóa quyền Admin. Hệ thống phải có ít nhất 1 Admin.');
        return;
      }
      this.selectedUser.roles.splice(index, 1);
    } else {
      this.selectedUser.roles.push(roleName);
    }
  }

  canRemoveAdminRole(): boolean {
    // Count total admins in the system (excluding those scheduled for deletion)
    const adminCount = this.users.filter(user =>
      user.roles.includes('Admin') && !user.scheduledDeletionDate
    ).length;
    return adminCount > 1;
  }

  updateRoles(): void {
    if (!this.selectedUser) return;

    this.loading = true;
    this.userManagementService.updateUserRoles(this.selectedUser.id, { roles: this.selectedUser.roles }).subscribe({
      next: () => {
        this.showSuccess('Quyền đã được cập nhật thành công');
        this.loadUsers();
        this.closeRoleModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể cập nhật quyền');
        this.loading = false;
      }
    });
  }

  deleteUser(user: UserDto): void {
    // Check if admin is trying to delete themselves
    if (user.id === this.currentUserId) {
      this.showError('Bạn không thể xóa chính mình');
      return;
    }

    // Check if this is the last admin (excluding those already scheduled for deletion)
    if (user.roles.includes('Admin')) {
      const adminCount = this.users.filter(u =>
        u.roles.includes('Admin') && !u.scheduledDeletionDate
      ).length;
      if (adminCount <= 1) {
        this.showError('Không thể xóa Admin cuối cùng. Hệ thống phải có ít nhất 1 Admin.');
        return;
      }
    }

    if (!confirm(`Bạn có chắc chắn muốn xóa người dùng ${user.email}? Tài khoản sẽ bị xóa sau 30 ngày.`)) {
      return;
    }

    this.loading = true;
    this.userManagementService.deleteUser(user.id).subscribe({
      next: () => {
        this.showSuccess('Người dùng đã được lên lịch xóa. Sẽ bị xóa vĩnh viễn sau 30 ngày.');
        this.loadUsers();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể xóa người dùng');
        this.loading = false;
      }
    });
  }

  cancelDeletion(user: UserDto): void {
    if (!confirm(`Bạn có muốn hủy xóa người dùng ${user.email}?`)) {
      return;
    }

    this.loading = true;
    this.userManagementService.cancelDeletion(user.id).subscribe({
      next: () => {
        this.showSuccess('Đã hủy xóa người dùng thành công');
        this.loadUsers();
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể hủy xóa người dùng');
        this.loading = false;
      }
    });
  }

  toggleLockout(user: UserDto): void {
    this.loading = true;
    this.userManagementService.toggleUserLockout(user.id).subscribe({
      next: () => {
        this.showSuccess('Trạng thái khóa đã được thay đổi');
        this.loadUsers();
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể thay đổi trạng thái khóa');
        this.loading = false;
      }
    });
  }

  toggleRoleForNewUser(roleName: string): void {
    const index = this.newUser.roles.indexOf(roleName);
    if (index > -1) {
      this.newUser.roles.splice(index, 1);
    } else {
      this.newUser.roles.push(roleName);
    }
  }

  toggleRoleForEditUser(roleName: string): void {
    const index = this.editUser.roles.indexOf(roleName);
    if (index > -1) {
      // Check if this is the last admin
      if (roleName === 'Admin' && !this.canRemoveAdminRole()) {
        this.showError('Không thể xóa quyền Admin. Hệ thống phải có ít nhất 1 Admin.');
        return;
      }
      this.editUser.roles.splice(index, 1);
    } else {
      this.editUser.roles.push(roleName);
    }
  }

  hasRole(roleName: string): boolean {
    return this.selectedUser?.roles.includes(roleName) || false;
  }

  showError(message: string): void {
    this.errorMessage = message;
    this.successMessage = '';
    setTimeout(() => this.errorMessage = '', 5000);
  }

  showSuccess(message: string): void {
    this.successMessage = message;
    this.errorMessage = '';
    setTimeout(() => this.successMessage = '', 5000);
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  getRoleDisplay(roleKey: string): string {
    return getRoleDisplayName(roleKey);
  }
}
