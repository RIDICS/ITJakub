<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AuthService.Models.RegisterModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    AdminPage
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Admin sekce</h2>
    Vítejte <%: ViewBag.UserName %>! 

        <table>
            <tr>
                <th>E-mail</th>
                <th>Jméno</th>
                <th>Příjmení</th>
                <th>Role</th>
            </tr>
            <% foreach (var item in ViewBag.Users)
               { %>
            <tr>
                <td><%: item.email %></td>
                <td><%: item.firstname %></td>
                <td><%: item.lastname %></td>
                <td><%: item.role %></td>                
                <td><%: Html.ActionLink("Upravit", "EditPage", "Home", new { id = item.Id }, null) %></td>
                <td><%: Html.ActionLink("Smazat", "Delete", "Home", new { id = item.Id }, null) %></td>
            </tr>
            <% } %>
        </table>

    <%: Html.ActionLink("Přidat uživatele", "RegisterPage", "Home")%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsSection" runat="server">
</asp:Content>
