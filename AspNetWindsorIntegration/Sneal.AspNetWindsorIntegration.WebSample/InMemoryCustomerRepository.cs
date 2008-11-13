using System.Collections.Generic;

namespace Sneal.AspNetWindsorIntegration.WebSample
{
    public class InMemoryCustomerRepository : ICustomerRepository
    {
        private static readonly Dictionary<int, Customer> customers = new Dictionary<int, Customer>();

        static InMemoryCustomerRepository()
        {
            // hard coding a couple of customers
            customers.Add(1, new Customer { FirstName = "John", Id = 1, LastName = "Smith", PhoneNumber = "353 555-1234"});
            customers.Add(2, new Customer { FirstName = "Jo", Id = 2, LastName = "Dixon", PhoneNumber = "353 555-1231" });
        }

        public virtual void Save(Customer customer)
        {
            customers[customer.Id] = customer;
        }

        public virtual Customer Get(int id)
        {
            Customer customer;
            customers.TryGetValue(id, out customer);
            return customer;
        }

        public virtual IList<Customer> GetAll()
        {
            return new List<Customer>(customers.Values);
        }
    }
}
