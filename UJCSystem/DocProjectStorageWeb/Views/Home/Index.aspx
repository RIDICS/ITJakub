<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DocProjectStorageWeb.WebModels.DashboardModel>" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Document storage
</asp:Content>

<asp:Content ID="indexFeatured" ContentPlaceHolderID="FeaturedContent" runat="server">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h2><%: ViewBag.Message %></h2>
            </hgroup>


            <div class="inner">
                <div class="buttonDiv"><%: Html.ActionLink("Nový projekt...", "New", "Project", new object { } ,new { @class = "button" }) %></div>
                <div class="buttonDiv"><%: Html.ActionLink("Nastavení aplikace", "Index", "Setup", new object { } ,new { @class = "button" }) %></div>
            </div>
            <div class="dash-wrapper">
                <h1>Poslední události</h1>
            </div>

            <div class="dash-wrapper">
                <h1>Seznam projektů</h1>
            
                    <% foreach (var item in Model.Projects)
                       { %>
                            <div class="dashItem"> <%: Html.ActionLink(item.Title, "Index", "Project", new { id = item.Id } ,new { @class = "dashLink" }) %></div>
                    <% } %>
                
            </div>
            <div class="dash-wrapper">
                <h1>Vaše aktivita</h1>
            </div>



        </div>
    </section>

</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsSection" runat="server">
    <script src="<%= Url.Content("~/Scripts/Home.js") %>" type="text/javascript"></script>
</asp:Content>
