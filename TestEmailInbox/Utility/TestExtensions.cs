using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

namespace TestEmailInbox.Utility
{
    public static class TestExtensions
    {
        public static void ShouldNotBeNull<T>(this T subject)
        {
            Assert.NotNull(subject);
        }

        public static void ShouldBeNull<T>(this T subject)
        {
            Assert.Null(subject);
        }

        public static void ShouldBeEqualTo<T>(this T subject,T item)
        {
            Assert.AreEqual(item,subject);
        }

        public static void ShouldNotBeEqualTo<T>(this T subject, T item)
        {
            Assert.AreNotEqual(item, subject);
        }

        public static void ShouldBeTrue(this bool subject)
        {
            Assert.True(subject);
        }

        public static void ShouldBeFalse(this bool subject)
        {
            Assert.False(subject);
        }

        public static void ShouldBeNullOrEmpty(this string subject)
        {
            Assert.IsNullOrEmpty(subject);
        }

        public static void ShouldBeGreaterThan(this long subject, long item)
        {
            Assert.Greater(subject, item);
        }

        public static void ShouldContain<T>(this IEnumerable<T> subject, T item)
        {
            if (subject == null)
                Assert.Fail("Expected to find {0} in collection, but the collection was null", item);
            Assert.IsTrue(subject.Contains(item), "Expected to find {0} in {1}", item, subject);
        }

        public static void ShouldContain<T>(this IEnumerable<T> subject, Func<T, bool> predicate)
        {
            if (subject == null)
                Assert.Fail("Expected to find an item in the collection, but the collection was null");
            Assert.IsTrue(subject.Any(predicate), "Expected to find matching element in {0}", subject);
        }

        public static void ShouldContain(this string haystack, string needle)
        {
            StringAssert.Contains(needle, haystack);
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> subject, T item)
        {
            if (subject == null)
                Assert.Fail("Expected to find an item in the collection, but the collection was null");
            Assert.IsFalse(subject.Contains(item), "Did not expect to find {0} in {1}", item, subject);
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> subject, Func<T, bool> predicate)
        {
            if (subject == null)
                Assert.Fail("Expected to find an item in the collection, but the collection was null");
            Assert.IsFalse(subject.Any(predicate), "Did not expect matching element in {0}", subject);
        }

        public static void ShouldContainOnly<T>(this IEnumerable<T> actualItems, params T[] expectedItems)
        {
            var actualList = actualItems.ToList();

            Assert.AreEqual(
                actualList.Count, expectedItems.Length,
                "Found {0} items in collection, expected {1}", actualList.Count, expectedItems.Length);

            if (expectedItems.Any(x => !actualList.Contains(x)))
                Assert.Fail("Missing item in {0}", actualItems);
        }

        public static void ShouldThrow<TException>(this Action action) where TException : Exception
        {
            Assert.Throws<TException>(() => action());
        }

        public static void ShouldBeInstanceOf<T>(this T subject, Type instance)
        {
            Assert.IsInstanceOf(instance,subject, "Object not the expecting instance");
        }
    }
}