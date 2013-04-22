<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AuthService.Models.RegisterModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    AdminPage
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Registrovat uživatele</h2>
    <% using (Html.BeginForm(new { Action = "Register" }))
       { %>
    <%: Html.AntiForgeryToken() %>

    <fieldset>
        <legend>Registrovat uživatele</legend>
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
                <%: Html.LabelFor(m => m.Email) %>
                <%: Html.TextBoxFor(m => m.Email) %>
                <%: Html.ValidationMessageFor(m => m.Email) %>
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
            <li>
                <%: Html.LabelFor(m => m.PasswordAgain) %>
                <%: Html.PasswordFor(m => m.PasswordAgain) %>
                <%: Html.ValidationMessageFor(m => m.PasswordAgain) %>
            </li>
        </ol>
        <input type="submit" value="Registrovat" />
    </fieldset>
    <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsSection" runat="server">
</asp:Content>
