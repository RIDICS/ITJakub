<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DocProjectStorageWeb.WebModels.LoginModel>" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Document storage
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <hgroup class="title">
        <h1>Přihlášení</h1>
    </hgroup>

    <section id="loginForm">
        <% using (Html.BeginForm(new { Action = "Login" }))
           { %>
        <%: Html.AntiForgeryToken() %>
        <%: Html.ValidationMessageFor(m => m) %>

        <fieldset>
            <legend>Přihlášení</legend>
            <ol>
                <li>
                    <%: Html.LabelFor(m => m.Email)%>
                    <%: Html.TextBoxFor(m => m.Email, new { @id = "email" } ) %>
                    <%: Html.ValidationMessageFor(m => m.Email) %>
                </li>
                <li>
                    <%: Html.LabelFor(m => m.Password) %>
                    <%: Html.PasswordFor(m => m.Password) %>
                    <%: Html.ValidationMessageFor(m => m.Password) %>
                </li>
                <li>
                    <%: Html.CheckBoxFor(m => m.RememberMe) %>
                    Zapamatuj si mě?
                </li>
            </ol>
            <input type="submit" value="Přihlásit" />
        </fieldset>
        <% } %>
    </section>


</asp:Content>
