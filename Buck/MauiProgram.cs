using Microsoft.Extensions.Logging;

namespace Buck
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<EmailService>(sp =>
            {
                return new EmailService(
                    smtpHost: "smtp.gmail.com",
                    smtpPort: 587,
                    smtpUser: "buckappnoreply@gmail.com",
                    smtpPass: "IdeGas692007");
            });

            builder.Services.AddSingleton<EmailBackgroundTaskManager>();

            return builder.Build();
        }
    }
}
