using System;

namespace Utility
{
    public static class Clock
    {
        static Clock()
        {
            Reset();
        }

        public static DateTime Now
        {
            get { return currentTimeProvider(); }
        }

        public static DateTime Today
        {
            get { return Now.Date; }
        }

        public static int Year
        {
            get { return Now.Year; }
        }

        public static void ChangeTimeProviderTo(Func<DateTime> newTimeProvider)
        {
            currentTimeProvider = newTimeProvider;
        }

        public static void Reset()
        {
            currentTimeProvider = DefaultTimeProvider;
        }

        private static Func<DateTime> currentTimeProvider;
        private static readonly Func<DateTime> DefaultTimeProvider = () => DateTime.Now;
    }
}