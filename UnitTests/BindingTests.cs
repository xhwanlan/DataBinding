using NUnit.Framework;
using System;

namespace DataBinding.UnitTests
{
    internal class BindingTests
    {
        [Test]
        public void BindingModeOneWay()
        {
            var person = new Person();
            var school = new School();

            person.Binding(nameof(person.Name), school, nameof(school.HeadmasterName));

            person.Name = "John";
            Assert.IsNotNull(person.Name);
            Assert.IsTrue(String.CompareOrdinal(school.HeadmasterName, person.Name) == 0);

            school.HeadmasterName = "Jane";
            Assert.IsFalse(String.CompareOrdinal(school.HeadmasterName, person.Name) == 0);
        }

        [Test]
        public void BindingModeTwoWay()
        {
            var person = new Person();
            var school = new School();

            person.Binding(nameof(person.Age), school, nameof(school.MaxTeacherAge), BindingMode.TwoWay);

            person.Age = 22;
            Assert.IsTrue(person.Age == 22);

            Assert.IsTrue(school.MaxTeacherAge == person.Age);

            school.MaxTeacherAge += 1;
            Assert.IsTrue(school.MaxTeacherAge == 23);

            Assert.IsTrue(school.MaxTeacherAge == person.Age);
        }

        [Test]
        public void BindingModeOneWayToSource()
        {
            var person = new Person();
            var school = new School();

            person.Binding(nameof(person.Salary), school, nameof(school.TeacherSalary), BindingMode.OneWayToSource);

            school.TeacherSalary = 1000;
            Assert.IsTrue(school.TeacherSalary == 1000);

            Assert.IsTrue(school.TeacherSalary == person.Salary);

            person.Salary = 2000;
            Assert.IsFalse(school.TeacherSalary == person.Salary);
        }

        [Test]
        public void ClearBinding()
        {
            var person = new Person();
            var school = new School();

            school.Binding(nameof(school.TeacherSalary), person, nameof(person.Salary));

            school.TeacherSalary = 1000;
            Assert.IsTrue(school.TeacherSalary == person.Salary);

            school.ClearBinding(nameof(school.TeacherSalary));
            school.TeacherSalary = 2000;
            Assert.IsFalse(school.TeacherSalary == person.Salary);
        }

        [Test]
        public void ClearAllBindings()
        {
            var person = new Person();
            var school = new School();

            person.Binding(nameof(person.Name), school, nameof(school.HeadmasterName));
            person.Binding(nameof(person.Age), school, nameof(school.MaxTeacherAge), BindingMode.TwoWay);
            person.Binding(nameof(person.Salary), school, nameof(school.TeacherSalary), BindingMode.OneWayToSource);

            person.Name = "John";
            person.Age = 18;
            school.TeacherSalary = 1000;

            Assert.IsNotNull(person.Name);
            Assert.IsTrue(String.CompareOrdinal(person.Name, school.HeadmasterName) == 0);
            Assert.IsTrue(person.Age == school.MaxTeacherAge);
            Assert.IsTrue(person.Salary == school.TeacherSalary);

            person.ClearAllBinding();
            person.Name = "Jane";
            person.Age = 19;
            school.TeacherSalary = 1200;

            Assert.IsFalse(String.CompareOrdinal(person.Name, school.HeadmasterName) == 0);
            Assert.IsFalse(person.Age == school.MaxTeacherAge);
            Assert.IsFalse(person.Salary == school.TeacherSalary);
        }

        private class Person : IDataBindingObject
        {
            // One Way
            private String m_name;
            public String Name
            {
                get => m_name;
                set => ((IDataBindingObject)this).SetPropertyValue(ref m_name, value, nameof(Name));
            }

            // Two Way
            private Byte m_age;
            public Byte Age
            {
                get => m_age;
                set => ((IDataBindingObject)this).SetPropertyValue(ref m_age, value, nameof(Age));
            }

            // One Way To Source
            private UInt16 m_salary;
            public UInt16 Salary
            {
                get => m_salary;
                set => m_salary = value;
            }
        }

        private class School : IDataBindingObject
        {
            // One Way
            private String m_headmasterName;
            public String HeadmasterName
            {
                get => m_headmasterName;
                set => m_headmasterName = value;
            }

            // Two Way
            private Byte m_maxTeacherAge;
            public Byte MaxTeacherAge
            {
                get => m_maxTeacherAge;
                set => ((IDataBindingObject)this).SetPropertyValue(ref m_maxTeacherAge, value, nameof(MaxTeacherAge));
            }

            // One Way To Source
            private UInt16 m_teacherSalary;
            public UInt16 TeacherSalary
            {
                get => m_teacherSalary;
                set => ((IDataBindingObject)this).SetPropertyValue(ref m_teacherSalary, value, nameof(TeacherSalary));
            }
        }
    }
}
