<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AuthService.Models.EditModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    AdminPage
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Upravit uživatele</h2>
    <% using (Html.BeginForm(new { Action = "Edit" }))
       { %>
    <%: Html.AntiForgeryToken() %>

    <fieldset>
        <legend>Upravit uživatele</legend>
        <%:  Html.HiddenFor(m => m.Id)%>
        <ol>
            <li>
                <%: Html.LabelFor(m => m.FirstName) %>
                <%: Html.TextBoxFor(m => m.FirstName) %>
                <%: Html.ValidationMessageFor(m => m.FirstName) %>
            </li>
            <li>
                <%: Html.LabelFor(m => m.LastName) %>
                <%: Html.TextBoxFor(m => m.LastName) %>
                <%: Html.ValidationMessageFor(m => m.LastName) %>
            </li>
            <li>
                <%: Html.LabelFor(m => m.Role) %>
                <%: Html.DropDownListFor(m => m.Role, new SelectList(Model.Roles)) %>
                <%: Html.ValidationMessageFor(m => m.Role) %>
            </li>
            <li>
                <%: Html.LabelFor(m => m.Password) %>
                <%: Html.PasswordFor(m => m.Password) %>
                <%: Html.ValidationMessageFor(m => m.Password) %>
            </li>
        </ol>
        <input type="submit" value="Edit" />
    </fieldset>
    <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsSection" runat="server">
</asp:Content>
