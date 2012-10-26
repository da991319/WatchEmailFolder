using System;
using System.Collections.Generic;

namespace Utility
{
    public static class Bail
    {
        private static bool recordModeIsOn;

        public static Bailer If(bool condition)
        {
            return recordModeIsOn
                ? new BailRecorder(condition)
                : new Bailer(condition);
        }

        public static void GoIntoRecordMode()
        {
            recordModeIsOn = true;
        }

        public static void ReturnToExceptionThrowingMode()
        {
            recordModeIsOn = false;
        }
    }

    public class Bailer
    {
        protected readonly bool shouldBail;

        public Bailer(bool shouldBail)
        {
            this.shouldBail = shouldBail;
        }

        public virtual void Because(string message, params object[] args)
        {
            if (shouldBail)
                throw new BailoutException(string.Format(message, args));
        }
    }

    public class BailRecorder : Bailer
    {
        private static Dictionary<string, bool> recordedBails = new Dictionary<string, bool>();

        public BailRecorder(bool shouldBail) : base(shouldBail)
        {
        }

        public override void Because(string message, params object[] args)
        {
            recordedBails[string.Format(message, args)] = shouldBail;
            base.Because(message, args);
        }

        public static void ShouldHaveRecorded(string message, bool shouldBail)
        {
            if (!recordedBails.ContainsKey(message))
                throw new Exception(string.Format(
                    "Condition was not tested:\n{0}\nTested Conditions were:\n{1}",
                    message,
                    string.Join("\n", recordedBails.Keys)));
            if (recordedBails[message] != shouldBail)
                throw new Exception(string.Format("Expected to assert {0}, but was actually {1} for message: {2}", shouldBail, !shouldBail, message));
        }

        public static void ClearRecordings()
        {
            recordedBails.Clear();
        }
    }

    public class BailoutException : Exception
    {
        public BailoutException(string message) : base(message)
        {
        }
    }
}