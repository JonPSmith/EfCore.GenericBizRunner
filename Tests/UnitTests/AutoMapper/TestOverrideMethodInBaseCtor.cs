// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.AutoMapper
{
    public class TestOverrideMethodInBaseCtor
    {
        private class BaseClass
        {
            public string MyString { get; protected set; }

            protected virtual void MyMethod()
            {
                MyString = "Base";
            }

            public BaseClass()
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                MyMethod();
            }
        }

        private class DerivedOverride : BaseClass
        {
            protected override void MyMethod()
            {
                MyString = "Derived";
            }
        }

        // See https://www.codeproject.com/tips/641610/be-careful-with-virtual-method
        [Fact]
        public void TestThatOverridingMethodInDerivedClassIsCalledInBaseCtor()
        {
            //SETUP

            //ATTEMPT
            var baseClass = new BaseClass();
            var derivedClass = new DerivedOverride();

            //VERIFY
            baseClass.MyString.ShouldEqual("Base");
            derivedClass.MyString.ShouldEqual("Derived");
        }
    }
}