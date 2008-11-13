using System.Collections.Generic;

namespace Sneal.AspNetWindsorIntegration.WebSample
{
    public interface ICustomerRepository
    {
        void Save(Customer customer);
        Customer Get(int id);
        IList<Customer> GetAll();
    }
}