using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buck
{
    public class EmailBackgroundTaskManager
    {
        private readonly EmailService _emailService;
        private CancellationTokenSource _cancellationTokenSource;

        public EmailBackgroundTaskManager(EmailService emailService)
        {
            _emailService = emailService;
        }

        public void StartBackgroundTask()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(async () => await DoWorkAsync(_cancellationTokenSource.Token));
        }

        public void StopBackgroundTask()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task DoWorkAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // Simulacija nekog posla u pozadini
                await Task.Delay(10000, token); // Čekanje 10 sekundi

                // Primer slanja e-maila
                await _emailService.SendEmailAsync("example@example.com", "Subject", "Body content");
            }
        }

        public async Task SendEmailVerificationAsync(string userEmail)
        {
            string subject = "Email Verification";
            string body = "Please verify your email by clicking this link.";
            await _emailService.SendEmailAsync(userEmail, subject, body);
        }
    }
}
