<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DocProjectStorageWeb.WebModels.SetupModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Nastavení aplikace</h2>
    <div class="OverviewCell">
        <div id="UserRoles">
            <div class="inner">
                <div class="buttonDiv"><%: Html.TextBox( "UserName", "" ,new { @id = "UserNameEdit" } ) %></div>
                <div class="buttonDiv">
                    <input id="UserRoleBtn" onclick="UserRoleBtn()" type="button" value="Nastavení uživatele" class="button" />
                </div>
            </div>
        </div>
        <div id="Project Types">
            <ol>
            <% foreach (var docType in Model.DocumentTypeNames) %>
            <% { %>
            <li><%: docType %> </li>    
            <% } %>
            </ol>
            <div class="inner">
                <div class="buttonDiv"><%: Html.TextBox( "DocType", "" ,new { @id = "AddDocTypeEdit" } ) %></div>
                <div class="buttonDiv">
                    <input id="AddDocTypeBtn" onclick="AddDocTypeBtn()" type="button" value="Přidat typ dokumentu" class="button" />
                </div>
            </div>

        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsSection" runat="server">
    <script src="<%= Url.Content("~/Scripts/Setup.js") %>" type="text/javascript"></script>
</asp:Content>
