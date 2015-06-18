<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DocProjectStorageWeb.WebModels.NewProjectModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    NewProject
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  

    <h2>NewProject</h2>
    <section id="newProjectForm">
        <% using (Html.BeginForm("EstablishProject", "Project", FormMethod.Post, new { enctype = "multipart/form-data" }))
           { %>
        <%: Html.AntiForgeryToken() %>
        <%: Html.ValidationMessageFor(m => m) %>
        <ol>
            <li>
                <%: Html.LabelFor(m => m.Title)%>
                <%: Html.TextBoxFor(m => m.Title, new { @id = "title" } ) %>
                <%: Html.ValidationMessageFor(m => m.Title) %>
            </li>
            <li>
                <%: Html.LabelFor(m => m.Author) %>
                <%: Html.TextBoxFor(m => m.Author) %>
                <%: Html.ValidationMessageFor(m => m.Author) %>
            </li>
          
             <li>
                 <%: Html.LabelFor(m => m.DocTypeValue) %>
                 <%: Html.DropDownList("DocTypeValue" , Model.DocTypes, "-- Select an Option --") %>
            </li>
            <li>
                <input type="file" name="file" />
            </li>
            
        </ol>
        <input type="submit" value="Založit" />
        <% } %>
    </section>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsSection" runat="server">
</asp:Content>
