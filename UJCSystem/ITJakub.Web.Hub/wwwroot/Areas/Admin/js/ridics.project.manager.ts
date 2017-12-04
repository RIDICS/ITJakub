class ProjectClient {
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

    createAuthor(firstName: string, lastName: string):JQueryXHR {
        const data: IOriginalAuthor = {
            id:0,
            firstName: firstName,
            lastName: lastName
        };
        return $.post(`${getBaseUrl()}Admin/KeyTable/CreateAuthor`, { request:data });
    }

    public createResponsiblePerson(firstName: string, lastName: string, responsibleTypeId: number, callback: (newResponsibleId: number, errorCode: HttpStatusCode) => void) {
        var data = {
            firstName: firstName,
            lastName: lastName,
            responsibleTypeIdList: [responsibleTypeId]
        }
        throw "TODO update UI required, because DB model changed.";
        //this.postAjax("Admin/Project/CreateResponsiblePerson", data, callback);
    }

    public createResponsibleType(type: ResponsibleTypeEnum, text: string, callback: (newResponsibleTypeId: number, errorCode: HttpStatusCode) => void) {
        var data = {
            type: type,
            text: text
        };
        this.postAjax("Admin/Project/CreateResponsibleType", data, callback);
    }

    saveMetadata(projectId: number, data: ISaveMetadataResource):JQueryXHR {
        return $.ajax({
            url: `${getBaseUrl()}Admin/Project/SaveMetadata?projectId=${projectId}`,
            type: "POST",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });
    }
}