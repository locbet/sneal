using System;

namespace Stormwind.Models
{
    public class User
    {
        public virtual Guid Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string FullName
        {
            get { return FirstName + " " + LastName; }
        }
        public virtual string EmailAddress { get; set; }
    }
}