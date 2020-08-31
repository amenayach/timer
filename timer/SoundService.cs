namespace timer
{
    public static class SoundService
    {
        public static void PlayAlarm()
        {
            using (var sp = new System.Media.SoundPlayer(Properties.Resources.alarm))
            {
                sp.PlaySync();
            }
        }
    }
}
