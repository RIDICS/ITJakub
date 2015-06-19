<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DocProjectStorageWeb.WebModels.UserRolesModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    UserRolesSetup
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>UserRolesSetup</h2>
      <h3><%: Model.UserName %></h3>

                    <table>
                         <% foreach (var role in Model.Roles) %>
                        <% { %>           
                        <tr>
                            <td>
                                <div class="propertyName"><%: role.RoleName  %></div>
                            </td>
                            <td>
                                <div class="propertyValue"><%: Html.CheckBox(role.RoleName, role.IsSet)  %></div>
                            </td>
                        </tr>
                         <% } %>
                    </table>
                <input type="submit" value="Potvrdit změny" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsSection" runat="server">
</asp:Content>
