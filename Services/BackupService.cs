using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Diagnostics;

namespace API.Services;

public class BackupService : IBackupService
{
    private readonly ApplicationDbContext _context;
    private readonly string _backupDirectory;
    private readonly IConfiguration _configuration;

    public BackupService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _backupDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Backups");

        if (!Directory.Exists(_backupDirectory))
        {
            Directory.CreateDirectory(_backupDirectory);
        }
    }

    public async Task<List<string>> GetAllTablesAsync()
    {
        var tables = new List<string>
        {
            "ETHNICITY", "RELIGION", "SCHOOL_YEAR", "SEMESTER", "GRADE_LEVEL", "SUBJECT",
            "ACADEMIC_PERFORMANCE", "CONDUCT", "RESULT", "OCCUPATION", "TEACHER",
            "STUDENT", "CLASS", "CLASS_ASSIGNMENT", "TEACHING_ASSIGNMENT", "GRADE_TYPE", "GRADE",
            "STUDENT_SUBJECT_RESULT", "STUDENT_YEAR_RESULT", "CLASS_SUBJECT_RESULT",
            "CLASS_SEMESTER_RESULT", "REGULATION", "AspNetUsers", "AspNetRoles",
            "AspNetUserRoles", "AspNetUserClaims", "AspNetUserLogins",
            "AspNetRoleClaims", "AspNetUserTokens"
        };

        return await Task.FromResult(tables);
    }

    public async Task<string> CreateBackupAsync(List<string> tables)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var backupFileName = $"backup_{timestamp}.sql";
        var backupFilePath = Path.Combine(_backupDirectory, backupFileName);

        var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
        var dbName = Environment.GetEnvironmentVariable("DB_NAME");
        var dbUser = Environment.GetEnvironmentVariable("DB_USER");
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

        var tableArgs = string.Join(" ", tables.Select(t => $"-t public.\\\"{t}\\\""));
        var arguments = $"-h {dbHost} -p {dbPort} -U {dbUser} -d {dbName} {tableArgs} -f \"{backupFilePath}\" --data-only --inserts --column-inserts";

        var startInfo = new ProcessStartInfo
        {
            FileName = "pg_dump",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        startInfo.Environment["PGPASSWORD"] = dbPassword!;

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new Exception("Failed to start pg_dump process");
        }

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync();
            throw new Exception($"Backup failed: {error}");
        }

        return backupFileName;
    }

    public async Task<List<BackupFileDto>> GetBackupFilesAsync()
    {
        var backupFiles = new List<BackupFileDto>();

        if (Directory.Exists(_backupDirectory))
        {
            var files = Directory.GetFiles(_backupDirectory, "*.sql");

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                backupFiles.Add(new BackupFileDto
                {
                    FileName = fileInfo.Name,
                    CreatedDate = fileInfo.CreationTime,
                    FileSizeBytes = fileInfo.Length,
                    FilePath = file
                });
            }
        }

        return await Task.FromResult(backupFiles.OrderByDescending(f => f.CreatedDate).ToList());
    }

    public async Task<BackupScheduleDto> GetBackupScheduleAsync()
    {
        var schedule = await _context.Set<BackupSchedule>().FirstOrDefaultAsync();

        if (schedule == null)
        {
            return new BackupScheduleDto
            {
                BackupTime = "00:00",
                IsEnabled = false,
                LastBackupDate = null
            };
        }

        return new BackupScheduleDto
        {
            BackupTime = schedule.BackupTime,
            IsEnabled = schedule.IsEnabled,
            LastBackupDate = schedule.LastBackupDate
        };
    }

    public async Task UpdateBackupScheduleAsync(BackupScheduleDto dto)
    {
        var schedule = await _context.Set<BackupSchedule>().FirstOrDefaultAsync();

        if (schedule == null)
        {
            schedule = new BackupSchedule
            {
                BackupTime = dto.BackupTime,
                IsEnabled = dto.IsEnabled
            };
            _context.Set<BackupSchedule>().Add(schedule);
        }
        else
        {
            schedule.BackupTime = dto.BackupTime;
            schedule.IsEnabled = dto.IsEnabled;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<string> GetBackupFilePathAsync(string fileName)
    {
        var filePath = Path.Combine(_backupDirectory, fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Backup file not found");
        }

        return await Task.FromResult(filePath);
    }

    public async Task RestoreBackupAsync(string fileName)
    {
        var filePath = Path.Combine(_backupDirectory, fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Backup file not found");
        }

        var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
        var dbName = Environment.GetEnvironmentVariable("DB_NAME");
        var dbUser = Environment.GetEnvironmentVariable("DB_USER");
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

        var backupContent = await File.ReadAllTextAsync(filePath);
        var tablesToTruncate = new List<string>();

        var insertMatches = System.Text.RegularExpressions.Regex.Matches(backupContent, @"INSERT INTO public\.""([^""]+)""");
        foreach (System.Text.RegularExpressions.Match match in insertMatches)
        {
            var tableName = match.Groups[1].Value;
            if (!tablesToTruncate.Contains(tableName))
            {
                tablesToTruncate.Add(tableName);
            }
        }

        if (tablesToTruncate.Count > 0)
        {
            var truncateCommands = string.Join("; ", tablesToTruncate.Select(t => $"TRUNCATE TABLE \\\"{t}\\\" RESTART IDENTITY CASCADE"));
            var truncateArgs = $"-h {dbHost} -p {dbPort} -U {dbUser} -d {dbName} -c \"{truncateCommands}\"";

            var truncateStartInfo = new ProcessStartInfo
            {
                FileName = "psql",
                Arguments = truncateArgs,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            truncateStartInfo.Environment["PGPASSWORD"] = dbPassword!;

            using (var truncateProcess = Process.Start(truncateStartInfo))
            {
                if (truncateProcess != null)
                {
                    truncateProcess.StandardInput.Close();
                    await truncateProcess.WaitForExitAsync();

                    var truncateError = await truncateProcess.StandardError.ReadToEndAsync();
                    if (truncateProcess.ExitCode != 0)
                    {
                        throw new Exception($"Failed to truncate tables: {truncateError}");
                    }
                }
            }
        }

        var arguments = $"-h {dbHost} -p {dbPort} -U {dbUser} -d {dbName} -f \"{filePath}\" -v ON_ERROR_STOP=1";

        var startInfo = new ProcessStartInfo
        {
            FileName = "psql",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        startInfo.Environment["PGPASSWORD"] = dbPassword!;

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new Exception("Failed to start psql process");
        }

        process.StandardInput.Close();

        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

        try
        {
            await process.WaitForExitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            process.Kill();
            throw new Exception("Restore operation timed out after 5 minutes");
        }

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        if (process.ExitCode != 0)
        {
            throw new Exception($"Restore failed: {error}");
        }
    }
}
