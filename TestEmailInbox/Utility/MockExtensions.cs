using Moq;
using System;
using System.Linq.Expressions;

namespace TestEmailInbox.Utility
{
    public static class MockExtensions
    {
        public static void ShouldHaveBeenToldTo<T>(this T subject, Expression<Action<T>> action) where T : class
        {
            Mock.Get(subject).Verify(action);
        }

        public static void ShouldNotHaveBeenToldTo<T>(this T subject, Expression<Action<T>> action) where T : class
        {
            Mock.Get(subject).Verify(action, Times.Never());
        }

        public static T Setup<T,TResult>(this T subject, Expression<Func<T,TResult>> func,TResult value) where T : class
        {
            Mock.Get(subject).Setup(func).Returns(value);
            return subject;
        }

        public static T Throws<T>(this T subject, Expression<Action<T>> func, Exception value) where T : class
        {
            Mock.Get(subject).Setup(func).Throws(value);
            return subject;
        }
    }
}