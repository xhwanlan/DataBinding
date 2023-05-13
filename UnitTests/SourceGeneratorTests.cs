using NUnit.Framework;
using System;

namespace DataBinding.UnitTests
{
    internal class SourceGeneratorTests
    {
        [Test]
        public void BindingModeOneWay()
        {
            var person = new Person();
            var school = new School();

            person.Binding(nameof(person.Name), school, nameof(school.HeadmasterName));

            person.Name = "Alice";
            Assert.IsNotNull(person.Name);
            Assert.IsTrue(String.CompareOrdinal(school.HeadmasterName, person.Name) == 0);

            school.HeadmasterName = "Bob";
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

            person.Name = "Alice";
            person.Age = 18;
            school.TeacherSalary = 1000;

            Assert.IsNotNull(person.Name);
            Assert.IsTrue(String.CompareOrdinal(person.Name, school.HeadmasterName) == 0);
            Assert.IsTrue(person.Age == school.MaxTeacherAge);
            Assert.IsTrue(person.Salary == school.TeacherSalary);

            person.ClearAllBinding();
            person.Name = "Bob";
            person.Age = 19;
            school.TeacherSalary = 1200;

            Assert.IsFalse(String.CompareOrdinal(person.Name, school.HeadmasterName) == 0);
            Assert.IsFalse(person.Age == school.MaxTeacherAge);
            Assert.IsFalse(person.Salary == school.TeacherSalary);
        }
    }

    internal partial class Person : IDataBindingObject
    {
        // One Way
        [DataBindingProperty("Name", BindingMode.OneWay, true)]
        private String m_name;


        // Two Way
        [DataBindingProperty("Age", BindingMode.TwoWay, true)]
        private Byte m_age;

        // One Way To Source
        [DataBindingProperty("Salary", BindingMode.OneWayToSource, true)]
        private UInt16 m_salary;
    }

    internal partial class School
    {
        // One Way
        [DataBindingProperty("HeadmasterName", BindingMode.OneWay, false)]
        private String m_headmasterName;

        // Two Way
        [DataBindingProperty("MaxTeacherAge", BindingMode.TwoWay, true)]
        private Byte m_maxTeacherAge;

        // One Way To Source
        [DataBindingProperty("TeacherSalary", BindingMode.OneWayToSource, false)]
        private UInt16 m_teacherSalary;
    }
}
