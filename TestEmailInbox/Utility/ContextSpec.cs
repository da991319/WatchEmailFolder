using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using NUnit.Framework;
using System.Linq;
using Utility;

namespace TestEmailInbox.Utility
{
    [TestFixture]
    public abstract class ContextSpec<TSystemUnderTest> where TSystemUnderTest : class
    {
        private const BindingFlags SpecPropertyBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        protected TSystemUnderTest sut { get; set; }

        protected abstract void UnderTheseConditions();
        protected abstract void BecauseOf();

        [TestFixtureSetUp]
        public virtual void BeforeAllTests(){}

        [TestFixtureTearDown]
        public virtual void AfterAllTests()
        {
            Clock.Reset();
        }

        [SetUp]
        public virtual void BeforeEachTest()
        {
            SetUpTestClasses();
            CreateSUT();
            UnderTheseConditions();
            try
            {
                BecauseOf();
            }
            catch (BailoutException)
            {
                // Squash intentional bailouts
            }
        }

        [TearDown]
        public virtual void AfterEachTest(){}

        private void SetUpTestClasses()
        {
            Bail.GoIntoRecordMode();
            BailRecorder.ClearRecordings();
        }

        protected virtual void CreateSUT()
        {
            var constructor = typeof (TSystemUnderTest).GetConstructors().First();
            var parameters = constructor.GetParameters();
            var dependencies = parameters.Select(GetDependency).ToList();
            sut = MakeSutUsing(constructor, dependencies);
            SetDependencyProperties(dependencies);
        }

        private object GetDependency(ParameterInfo parameterInfo)
        {
            return FindPreSetUpDependency(parameterInfo) ?? GenerateDependency(parameterInfo);
        }

        private object FindPreSetUpDependency(ParameterInfo parameterInfo)
        {
            var property = GetDependantPropertyOfType(parameterInfo.ParameterType);
            if (property == null)
                return null;

            var propertyValue = property.GetValue(this);
            return propertyValue;
        }

        private object GenerateDependency(ParameterInfo parameterInfo)
        {
            var parameterType = parameterInfo.ParameterType;
            const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

            var methodInfo = GetType().GetMethod("MakeMock", bindingFlags);
            var makeDependencyMethod = methodInfo.MakeGenericMethod(parameterType);
            return makeDependencyMethod.Invoke(this, null);
        }

        private TSystemUnderTest MakeSutUsing(ConstructorInfo constructor, IList<object> dependencies)
        {
            return (TSystemUnderTest) constructor.Invoke(dependencies.ToArray());
        }

        private void SetDependencyProperties(IList<object> dependencies)
        {
            foreach (var dependency in dependencies)
                SetProperty(dependency);
        }

        private void SetProperty(object dependency)
        {
            var dependantProperty = GetDependantPropertyOfType(dependency.GetType());
            
            if (dependantProperty != null)
                dependantProperty.SetValue(this, dependency, SpecPropertyBindingFlags, null, null);
        }

        private FieldInfo GetDependantPropertyOfType(Type parameterType)
        {
            var dependantProperty = GetType()
                .GetFields(SpecPropertyBindingFlags)
                .SingleOrDefault(x => x.FieldType.IsAssignableFrom(parameterType));
            return dependantProperty;
        }

        protected T MakeMock<T>() where T : class
        {
            return new Mock<T>().Object;
        }
    }
}