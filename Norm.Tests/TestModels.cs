using System;

namespace Norm.Tests
{
    internal class Person
    {
        // mapped directly to db fields
        public int Id { get; set; }
        public string FirstName { get; set; }
        public char MiddleInitial { get; set; }
        public string LastName { get; set; }
        public char Gender { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public static Person Random()
        {
            var person = new Person();
            person.FirstName = TestHelpers.GetRandomString(20);
            person.MiddleInitial = Convert.ToChar(TestHelpers.GetRandomString(1));
            person.LastName = TestHelpers.GetRandomString(20);
            person.Gender = 'M';

            return person;
        }

        #region Equality

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return Equals((Person)obj);
        }

        protected bool Equals(Person other)
        {
            return Id == other.Id &&
                   string.Equals(FirstName, other.FirstName) &&
                   MiddleInitial == other.MiddleInitial &&
                   string.Equals(LastName, other.LastName) &&
                   string.Equals(Gender, other.Gender);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id;
                hashCode = (hashCode * 397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ MiddleInitial.GetHashCode();
                hashCode = (hashCode * 397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Gender.GetHashCode();
                return hashCode;
            }
        }

        #endregion // Equality
    }

    internal class PersonAltPk
    {
        // uses alternate primary key name

        public int PersonAltPkId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    internal class PersonNoPk
    {
        // has no primary key

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
