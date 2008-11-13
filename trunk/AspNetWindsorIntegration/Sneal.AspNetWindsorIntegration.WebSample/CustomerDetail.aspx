<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomerDetail.aspx.cs" Inherits="Sneal.AspNetWindsorIntegration.WebSample.CustomerDetail" %>
<%@ Register src="CustomerFooter.ascx" tagname="CustomerFooter" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h2>Customers:</h2>
        <asp:GridView ID="customerGrid" runat="server" />
    </div>
    <uc1:CustomerFooter ID="customerFooter" runat="server" />
    </form>
</body>
</html>
