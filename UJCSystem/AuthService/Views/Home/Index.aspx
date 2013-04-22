<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AuthService.Models.AuthModel>" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page - My ASP.NET MVC Application
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <hgroup class="title">
        <h1>Přihlášení do admin sekce</h1>
    </hgroup>

    <section id="loginForm">
        <% using (Html.BeginForm(new { Action = "Login" }))
           { %>
        <%: Html.AntiForgeryToken() %>
        <%: Html.ValidationMessageFor(m => m) %>

        <fieldset>
            <legend>Přihlášení do admin sekce</legend>
            <ol>
                <li>
                    <%: Html.LabelFor(m => m.Email)%>
                    <%: Html.TextBoxFor(m => m.Email, new { @id = "Email" } ) %>
                    <%: Html.ValidationMessageFor(m => m.Email) %>
                </li>
                <li>
                    <%: Html.LabelFor(m => m.Password) %>
                    <%: Html.PasswordFor(m => m.Password) %>
                    <%: Html.ValidationMessageFor(m => m.Password) %>
                </li>

            </ol>
            <input type="submit" value="Přihlásit" />
        </fieldset>
        <% } %>
    </section>


</asp:Content>
