<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DocProjectStorageWeb.WebModels.ProjectModel>" %>

<script runat="server">
    
    
</script>



<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Projekt> <%: Model.Title %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="Wrapper">
        <div id="Container">
            <div id="ProjectOverview" class="mainBox">
                <div id="ProjectHeader" class="OverviewCell">

                    <table>
                        <tr>
                            <td>
                                <div class="propertyName"><%: Html.LabelFor(m => m.Id) %></div>
                            </td>
                            <td>
                                <div class="propertyValue"><%: Html.ValueFor(m => m.Id) %></div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="propertyName"><%: Html.LabelFor(m => m.Title) %></div>
                            </td>
                            <td>
                                <div class="propertyValue"><%: Html.ValueFor(m => m.Title) %></div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="propertyName"><%: Html.LabelFor(m => m.Author) %></div>
                            </td>
                            <td>
                                <div class="propertyValue"><%: Html.ValueFor(m => m.Author) %></div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="propertyName"><%: Html.LabelFor(m => m.DocType) %></div>
                            </td>
                            <td>
                                <div class="propertyValue"><%: Html.ValueFor(m => m.DocType) %></div>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="ProjectPeople" class="OverviewCell">
                    <table>
                        <tr>
                            <td>
                                <div class="propertyName">Editoři</div>
                                <div id="EditorsWrapper">
                                    <div id="Editors">
                                        <% foreach (var editor in Model.Editors) %>
                                        <% { %>
                                        <div class="editor"><%: editor %></div>
                                        <% } %>
                                    </div>
                                </div>
                                <div id="AddEditorWrapper">
                                    <div class="inner">
                                        <div class="buttonDiv"><%: Html.TextBox( "new message", "" ,new { @id = "AddEditorEdit" } ) %></div>
                                        <div class="buttonDiv">
                                            <input id="AddEditorBtn" onclick="AddEditor()" type="button" value="Přidat" class="button" />
                                        </div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="propertyName"><%: Html.LabelFor(m => m.ResponsibleRedactors) %></div>
                            </td>
                            <td>
                                <div class="propertyValue">
                                    <% foreach (var item in Model.ResponsibleRedactors)
                                       { %>
                                    <%: item %>
                                    <% } %>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="propertyName"><%: Html.LabelFor(m => m.ResponsibleTechRedactors) %></div>
                            </td>
                            <td>
                                <div class="propertyValue">
                                    <% foreach (var item in Model.ResponsibleTechRedactors)
                                       { %>
                                    <%: item %>
                                    <% } %>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
                

                <div id="ProjectControl" class="OverviewCell">
                    <div>
                        <h3>Stav projektu</h3>
                    </div>
                    <table>
                        <tr>
                            <td>
                                <div class="propertyName"><%: Html.LabelFor(m => m.ProjectState) %></div>
                            </td>
                            <td>
                                <div class="propertyValue"><%: Html.ValueFor(m => m.ProjectState) %></div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="propertyName"><%: Html.LabelFor(m => m.LockingPerson) %></div>
                            </td>
                            <td>
                                <div class="propertyValue"><%: Html.ValueFor(m => m.LockingPerson) %></div>
                            </td>
                        </tr>
                    </table>
                    <div>
                        <div class="inner">
                            <div class="buttonDiv"><%: Html.ActionLink("Zamknout", "", "", new object { } ,new { @class = "button" }) %></div>
                            <div class="buttonDiv"><%: Html.ActionLink("Postoupit tech redaktorovi", "", "", new object { } ,new { @class = "button" }) %></div>
                            <div class="buttonDiv"><%: Html.ActionLink("Vratit editorovi", "", "", new object { } ,new { @class = "button" }) %></div>
                        </div>
                    </div>
                </div>

                <div id="ProjectRevisions" class="OverviewCell">
                    <div>
                        <div class="propertyName">Revize projektu</div>
                        <select id="Select1">
                            <% foreach (var item in Model.Revisions)
                               { %>
                            <option class="Message"><%: item.RevNumber %> - <%: item.ReleaseDate %></option>
                            <% } %>
                        </select>

                        <div class="inner">
                            <div class="buttonDiv"><%: Html.ActionLink("Stáhnout revizi...", "", "", new object { } ,new { @class = "button" }) %></div>
                            <div class="buttonDiv"><%: Html.ActionLink("Nahrát revizi...", "", "", new object { } ,new { @class = "button" }) %></div>
                        </div>
                    </div>
                </div>

                <div id="ProjectPublishing" class="OverviewCell">
                    <div>
                        <h3>Publikace projektu</h3>
                    </div>
                    <div>
                        <div class="inner">
                            <div class="buttonDiv"><%: Html.ActionLink("Nahrat do VV", "", "", new object { } ,new { @class = "button" }) %></div>
                            <div class="buttonDiv"><%: Html.ActionLink("Verejne publikovat", "", "", new object { } ,new { @class = "button" }) %></div>
                            <div class="buttonDiv"><%: Html.ActionLink("Pro interni pouziti", "", "", new object { } ,new { @class = "button" }) %></div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="ProjectDiscussion" class="mainBox">

                <div id="DiscussionWarpper">
                    <div id="MessagesWrapper">
                        <div id="Messages">
                        </div>
                    </div>
                    <div id="NewMessage">
                        <div class="inner">
                            <div class="buttonDiv"><%: Html.TextBox( "new message", "" ,new { @id = "NewMessageEdit" } ) %></div>
                            <div class="buttonDiv">
                                <input id="NewMessageBtn" onclick="sendMessage()" type="button" value="Odeslat" class="button" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsSection" runat="server">
    <script type="text/javascript">
        $projectID = "<%: Model.Id %>";
    </script>
    <script src="<%= Url.Content("~/Scripts/Project.js") %>" type="text/javascript"></script>
</asp:Content>
