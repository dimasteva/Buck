namespace Buck
{
    public partial class App : Application
{
    private readonly EmailBackgroundTaskManager _emailBackgroundTaskManager;

    public App(EmailBackgroundTaskManager emailBackgroundTaskManager)
    {
        InitializeComponent();

        MainPage = new NavigationPage(new LoginPage());

        _emailBackgroundTaskManager = emailBackgroundTaskManager;
        _emailBackgroundTaskManager.StartBackgroundTask();
    }

    protected override void OnStart()
    {
        base.OnStart();
        _emailBackgroundTaskManager.StartBackgroundTask();
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        _emailBackgroundTaskManager.StopBackgroundTask();
    }

    protected override void OnResume()
    {
        base.OnResume();
        _emailBackgroundTaskManager.StartBackgroundTask();
    }
}
}
