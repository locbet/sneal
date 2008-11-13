using System;
using System.Web.UI;

namespace Sneal.AspNetWindsorIntegration.WebSample
{
    public partial class CustomerDetail : Page
    {
        public ICustomerRepository CustomerRepository { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            customerGrid.DataSource = CustomerRepository.GetAll();
            customerGrid.DataBind();
        }
    }
}
