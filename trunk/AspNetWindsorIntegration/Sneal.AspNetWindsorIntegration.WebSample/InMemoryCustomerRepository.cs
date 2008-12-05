#region license
// Copyright 2008 Shawn Neal (sneal@sneal.net)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
