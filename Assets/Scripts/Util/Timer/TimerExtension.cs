namespace Util.Timer
{
    public static class TimerExtension
    {
        public static void Release(this Timer timer)
        {
            TimerCollection.RemoveTimer(timer);
        }
    }
}