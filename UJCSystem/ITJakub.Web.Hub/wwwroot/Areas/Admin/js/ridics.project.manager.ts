class ProjectManager {
    private ajax(type: string, urlPath: string, data: Object|string, success: (response: any) => void, error: (status: HttpStatusCode) => void) {
        $.ajax({
            type: type,
            traditional: true,
            url: getBaseUrl() + urlPath,
            data: data,
            dataType: "json",
            contentType: "application/json",
            success: response => {
                success(response);
            },
            error: (jqXhr) => {
                error(jqXhr.status);
            }
        });
    }

    private getAjax(urlPath: string, data: Object, callback: (response: any, errorCode: HttpStatusCode) => void) {
        this.ajax("GET", urlPath, data, response => callback(response, null), status => callback(null, status));
    }

    private postAjax(urlPath: string, data: Object, callback: (response: any, errorCode: HttpStatusCode) => void) {
        this.ajax("POST", urlPath, JSON.stringify(data), response => callback(response, null), status => callback(null, status));
    }

    public createProject(name: string, callback: (id: number, error: HttpStatusCode) => void) {
        this.postAjax("Admin/Project/CreateProject", {name: name}, callback);
    }

    public deleteProject(id: number, callback: (error: HttpStatusCode) => void) {
        this.postAjax("Admin/Project/DeleteProject", {id: id}, (response, errorCode) => callback(errorCode));
    }

    public getResourceList(projectId: number, resourceType: ResourceType, callback: (list: IProjectResource[], errorCode: HttpStatusCode) => void) {
        var data = {
            projectId: projectId,
            resourceType: resourceType
        }
        this.getAjax("Admin/Project/GetResourceList", data, callback);
    }

    public processUploadedResources(projectId: number, sessionId: string, comment: string, callback: (errorCode: HttpStatusCode) => void) {
        var data = {
            projectId: projectId,
            sessionId: sessionId,
            comment: comment
        };
        this.postAjax("Admin/Project/ProcessUploadedResources", data, (response, errorCode) => callback(errorCode));
    }

    public processUploadedResourceVersion(resourceId: number, sessionId: string, comment: string, callback: (errorCode: HttpStatusCode) => void) {
        var data = {
            resourceId: resourceId,
            sessionId: sessionId,
            comment: comment
        };
        this.postAjax("Admin/Project/ProcessUploadResourceVersion", data, (response, errorCode) => callback(errorCode));
    }

    public deleteResource(resourceId: number, callback: (errorCode: HttpStatusCode) => void) {
        this.postAjax("Admin/Project/DeleteResource", {resourceId: resourceId}, (response, errorCode) => callback(errorCode));
    }

    public renameResource(resourceId: number, newName: string, callback: (errorCode: HttpStatusCode) => void) {
        var data = {
            resourceId: resourceId,
            newName: newName
        }
        this.postAjax("Admin/Project/RenameResource", data, (response, errorCode) => callback(errorCode));
    }

    public duplicateResource(resourceId: number, callback: (newResourceId: number, errorCode: HttpStatusCode) => void) {
        this.postAjax("Admin/Project/DuplicateResource", {resourceId: resourceId}, callback);
    }

    public createPublisher(name: string, email: string, callback: (newPublisherId: number, errorCode: HttpStatusCode) => void) {
        var data = {
            text: name,
            email: email
        };
        this.postAjax("Admin/Project/CreatePublisher", data, callback);
    }

    public createLiteraryKind(name: string, callback: (newKindId: number, errorCode: HttpStatusCode) => void) {
        var data = {
            name: name
        };
        this.postAjax("Admin/Project/CreateLiteraryKind", data, callback);
    }

    public createLiteraryGenre(name: string, callback: (newGenreId: number, errorCode: HttpStatusCode) => void) {
        var data = {
            name: name
        };
        this.postAjax("Admin/Project/CreateLiteraryGenre", data, callback);
    }

    public createAuthor(firstName: string, lastName: string, callback: (newAuthorId: number, errorCode: HttpStatusCode) => void) {
        var data = {
            firstName: firstName,
            lastName: lastName
        };
        this.postAjax("Admin/Project/CreateAuthor", data, callback);
    }

    public createResponsiblePerson(firstName: string, lastName: string, responsibleTypeId: number, callback: (newResponsibleId: number, errorCode: HttpStatusCode) => void) {
        var data = {
            firstName: firstName,
            lastName: lastName,
            responsibleTypeId: responsibleTypeId
        }
        this.postAjax("Admin/Project/CreateAuthor", data, callback);
    }
}