using System.Net;
using System.Net.Mail;

namespace API.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetLink)
    {
        var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST");
        var smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
        var smtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME");
        var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
        var smtpFromEmail = Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL") ?? smtpUsername;
        var smtpFromName = Environment.GetEnvironmentVariable("SMTP_FROM_NAME") ?? "School Management System";

        if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
        {
            return;
        }

        var message = new MailMessage
        {
            From = new MailAddress(smtpFromEmail, smtpFromName),
            Subject = "Đặt lại mật khẩu",
            Body = $@"
                <html>
                <body>
                    <h2>Yêu cầu đặt lại mật khẩu</h2>
                    <p>Bạn đã yêu cầu đặt lại mật khẩu cho tài khoản của mình.</p>
                    <p>Vui lòng nhấp vào liên kết bên dưới để đặt lại mật khẩu:</p>
                    <p><a href='{resetLink}'>Đặt lại mật khẩu</a></p>
                    <p>Liên kết này sẽ hết hạn sau 1 giờ.</p>
                    <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
                </body>
                </html>
            ",
            IsBodyHtml = true
        };

        message.To.Add(email);

        using var smtpClient = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(smtpUsername, smtpPassword)
        };

        await smtpClient.SendMailAsync(message);
    }

    public async Task SendAccountSetupEmailAsync(string email, string setupLink)
    {
        var message = await CreateEmailMessage(
            email,
            "Thiết lập tài khoản của bạn",
            $@"
                <html>
                <body>
                    <h2>Chào mừng đến với Hệ thống Quản lý Trường học</h2>
                    <p>Một tài khoản mới đã được tạo cho bạn.</p>
                    <p>Vui lòng nhấp vào liên kết bên dưới để thiết lập mật khẩu của bạn:</p>
                    <p><a href='{setupLink}' style='display: inline-block; padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px;'>Thiết lập mật khẩu</a></p>
                    <p>Liên kết này sẽ hết hạn sau 7 ngày.</p>
                    <p>Nếu bạn không yêu cầu tài khoản này, vui lòng liên hệ với quản trị viên.</p>
                </body>
                </html>
            "
        );

        await SendEmailAsync(message);
    }

    public async Task SendAccountDeletionScheduledEmailAsync(string email, DateTime deletionDate)
    {
        var message = await CreateEmailMessage(
            email,
            "Tài khoản của bạn đã được lên lịch xóa",
            $@"
                <html>
                <body>
                    <h2>Thông báo xóa tài khoản</h2>
                    <p>Tài khoản của bạn đã được lên lịch xóa.</p>
                    <p><strong>Ngày xóa:</strong> {deletionDate:dd/MM/yyyy HH:mm}</p>
                    <p>Tài khoản của bạn sẽ bị khóa ngay lập tức và sẽ bị xóa vĩnh viễn sau 30 ngày.</p>
                    <p>Nếu đây là một sai lầm, vui lòng liên hệ với quản trị viên để hủy xóa trước ngày {deletionDate:dd/MM/yyyy}.</p>
                    <p style='color: red;'><strong>Cảnh báo:</strong> Sau ngày này, dữ liệu của bạn sẽ bị xóa vĩnh viễn và không thể khôi phục.</p>
                </body>
                </html>
            "
        );

        await SendEmailAsync(message);
    }

    public async Task SendAccountDeletionCancelledEmailAsync(string email)
    {
        var message = await CreateEmailMessage(
            email,
            "Xóa tài khoản đã được hủy",
            $@"
                <html>
                <body>
                    <h2>Thông báo hủy xóa tài khoản</h2>
                    <p>Xóa tài khoản của bạn đã được hủy.</p>
                    <p>Tài khoản của bạn đã được mở khóa và bạn có thể tiếp tục sử dụng như bình thường.</p>
                    <p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với quản trị viên.</p>
                </body>
                </html>
            "
        );

        await SendEmailAsync(message);
    }

    public async Task SendAccountLockedEmailAsync(string email)
    {
        var message = await CreateEmailMessage(
            email,
            "Tài khoản của bạn đã bị khóa",
            $@"
                <html>
                <body>
                    <h2>Thông báo khóa tài khoản</h2>
                    <p>Tài khoản của bạn đã bị khóa bởi quản trị viên.</p>
                    <p>Bạn sẽ không thể đăng nhập cho đến khi tài khoản được mở khóa.</p>
                    <p>Nếu bạn tin rằng đây là một sai lầm, vui lòng liên hệ với quản trị viên.</p>
                </body>
                </html>
            "
        );

        await SendEmailAsync(message);
    }

    public async Task SendAccountUnlockedEmailAsync(string email)
    {
        var message = await CreateEmailMessage(
            email,
            "Tài khoản của bạn đã được mở khóa",
            $@"
                <html>
                <body>
                    <h2>Thông báo mở khóa tài khoản</h2>
                    <p>Tài khoản của bạn đã được mở khóa.</p>
                    <p>Bạn có thể đăng nhập và sử dụng hệ thống như bình thường.</p>
                    <p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với quản trị viên.</p>
                </body>
                </html>
            "
        );

        await SendEmailAsync(message);
    }

    private async Task<MailMessage> CreateEmailMessage(string toEmail, string subject, string body)
    {
        var smtpFromEmail = Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL") ?? Environment.GetEnvironmentVariable("SMTP_USERNAME");
        var smtpFromName = Environment.GetEnvironmentVariable("SMTP_FROM_NAME") ?? "School Management System";

        var message = new MailMessage
        {
            From = new MailAddress(smtpFromEmail!, smtpFromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(toEmail);
        return message;
    }

    private async Task SendEmailAsync(MailMessage message)
    {
        var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST");
        var smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
        var smtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME");
        var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

        if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
        {
            return;
        }

        using var smtpClient = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(smtpUsername, smtpPassword)
        };

        await smtpClient.SendMailAsync(message);
    }
}
