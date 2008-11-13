using System;
using System.Web.UI;

namespace Sneal.AspNetWindsorIntegration.WebSample
{
    public partial class CustomerFooter : UserControl
    {
        public ICustomerRepository CustomerRepository { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Customer customer = CustomerRepository.Get(2);
            lastEditedBy.Text = "Last edited by: " + customer.FirstName + " " + customer.LastName;
        }
    }
}