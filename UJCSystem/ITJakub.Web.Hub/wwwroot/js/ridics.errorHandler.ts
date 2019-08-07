class ErrorHandler {

    public getErrorMessage(ajaxResponse: JQuery.jqXHR, defaultMessage?: string): string {
        const response = ajaxResponse.responseText;
        const statusCode = ajaxResponse.status;
        try {
            const parsedResponse = JSON.parse(response);

            if (this.isErrorContract(parsedResponse)) {
                return  parsedResponse.errorMessage;
            } else { // valid json, but unexpected object
                return this.getDefaultOrErrorByCode(statusCode, defaultMessage);
            }
        } catch (e) { // response is not a valid json, show generic error
            return this.getDefaultOrErrorByCode(statusCode, defaultMessage);
        }
    }

    private isErrorContract(error: any): error is IErrorContract {
        if (!error) {
            return false;
        }
        return typeof (error as IErrorContract).errorMessage !== "undefined";
    }

    private getDefaultOrErrorByCode(statusCode: number, defaultMessage?: string): string {
        if (defaultMessage) {
            return defaultMessage;
        } else {
            return this.getMessagesByStatusCode(statusCode);
        }
    }

    getMessagesByStatusCode(statusCode: number): string {
        let errorMessage;
        switch (statusCode) {
            case StatusCodes.Status400BadRequest:
                errorMessage = localization.translate("bad-request-detail", "Error").value;
                break;
            case StatusCodes.Status401Unauthorized:
                errorMessage = localization.translate("unauthorized-detail", "Error").value;
                break;
            case StatusCodes.Status403Forbidden:
                errorMessage = localization.translate("forbidden-detail", "Error").value;
                break;
            case StatusCodes.Status404NotFound:
                errorMessage = localization.translate("not-found-detail", "Error").value;
                break;
            case StatusCodes.Status500InternalServerError:
                errorMessage = localization.translate("internal-server-error-detail", "Error").value;
                break;
            case StatusCodes.Status502BadGateway:
                errorMessage = localization.translate("bad-gateway-detail", "Error").value;
                break;
            case StatusCodes.Status504GatewayTimeout:
                errorMessage = localization.translate("gateway-timeout-detail", "Error").value;
                break;
            default:
                errorMessage = localization.translate("generic-error", "Error").value;
        }

        return errorMessage;
    }
}

enum StatusCodes {
    Status400BadRequest = 400,
    Status401Unauthorized = 401,
    Status403Forbidden = 403,
    Status404NotFound = 404,
    Status500InternalServerError = 500,
    Status502BadGateway = 502,
    Status503ServiceUnavailable = 503,
    Status504GatewayTimeout = 504,
}