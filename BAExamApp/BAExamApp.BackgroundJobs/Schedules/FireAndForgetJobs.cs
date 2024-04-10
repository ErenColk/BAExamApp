using BAExamApp.BackgroundJobs.Managers.FireAndForgetJobs;

namespace BAExamApp.BackgroundJobs.Schedules;
public static class FireAndForgetJobs
{
    public static void FireSendMailJob()
    {
        Hangfire.BackgroundJob.Enqueue<SendMailJobManager>
                (
                    job => job.Process()
                );
    }
}
