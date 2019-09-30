﻿class ProjectClient {
    private ajax(type: string, urlPath: string, data: Object|string, success: (response: any) => void, error: (status: HttpStatusCode) => void) {
        $.ajax({
            type: type,
            traditional: true,
            url: getBaseUrl() + urlPath,
            data: data as JQuery.PlainObject,
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

    createAuthor(firstName: string, lastName: string): JQuery.jqXHR<number> {
        const data: IOriginalAuthor = {
            id:0,
            firstName: firstName,
            lastName: lastName
        };
        return $.post(`${getBaseUrl()}Admin/KeyTable/CreateAuthor`, { request: data } as JQuery.PlainObject);
    }

    createResponsiblePerson(firstName: string, lastName: string): JQuery.jqXHR<number> {
        const data: IResponsiblePerson = {
            id:0,
            firstName: firstName,
            lastName: lastName
        };
        return $.post(`${getBaseUrl()}Admin/KeyTable/CreateResponsiblePerson`, { request: data } as JQuery.PlainObject);
    }

    createResponsibleType(type: ResponsibleTypeEnum, text: string): JQuery.jqXHR<number> {
        const data:IResponsibleType = {
            id: 0,
            type: type,
            text: text
        };
        return $.post(`${getBaseUrl()}Admin/KeyTable/CreateResponsibleType`, { request: data } as JQuery.PlainObject);
    }

    saveMetadata(projectId: number, data: ISaveMetadataResource): JQuery.jqXHR<IMetadataSaveResult> {
        return $.ajax({
            url: `${getBaseUrl()}Admin/Project/SaveMetadata?projectId=${projectId}`,
            type: "POST",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            cache: false,
            async: true,
            dataType: "json"
        });
    }

    saveCategorization(projectId: number, data: ISaveCategorization): JQuery.jqXHR {
        return $.ajax({
            url: `${getBaseUrl()}Admin/Project/SaveCategorization?projectId=${projectId}`,
            type: "POST",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            cache: false,
            async: true,
            dataType: "json"
        });
    }

    getProjectsByAuthor(authorId: number, start?: number, count?: number): JQuery.jqXHR<IPagedResult<IProjectDetailContract>> {
        return $.get(`${getBaseUrl()}Admin/Project/GetProjectsByAuthor?authorId=${authorId}&start=${start}&count=${count}`);
    }

    getProjectsByResponsiblePerson(responsiblePersonId: number, start?: number, count?: number): JQuery.jqXHR<IPagedResult<IProjectDetailContract>> {
        return $.get(`${getBaseUrl()}Admin/Project/GetProjectsByResponsiblePerson?responsiblePersonId=${responsiblePersonId}&start=${start}&count=${count}`);
    }

    createForum(projectId: number): JQuery.jqXHR<IForumViewModel> {
        return $.post(`${getBaseUrl()}Admin/Project/CreateForum?projectId=${projectId}`, {});
    }
}