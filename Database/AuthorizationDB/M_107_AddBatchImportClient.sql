START TRANSACTION;

DO $$
DECLARE clientId integer;
BEGIN

INSERT INTO public."Client"(
	"Name", "Description", "DisplayUrl", "LogoUrl", "RequireConsent")
	VALUES ('VokabularBatchImport', 'Vokabular Webovy - aplikace pro hromadny import del', 'http://vokabular.ujc.cas.cz', 'http://vokabular.ujc.cas.cz', false);

		   
SELECT "Id"	INTO clientId FROM public."Client" WHERE "Name" = 'VokabularBatchImport';

INSERT INTO public."Uri"(
	"Uri", "ClientId")
	VALUES ('https://127.0.0.1:7890/', clientId);

INSERT INTO public."Uri_UriType"("UriId", "UriTypeId")
	VALUES ((SELECT "Id" FROM public."Uri" WHERE "Uri" = 'https://127.0.0.1:7890/'), (SELECT "Id" FROM public."UriType" WHERE "Value" = 'Redirect')),
	((SELECT "Id" FROM public."Uri" WHERE "Uri" = 'https://127.0.0.1:7890/logout'), (SELECT "Id" FROM public."UriType" WHERE "Value" = 'FrontChannelLogout'));
	
INSERT INTO public."Client_ApiResource"(
	"ClientId", "ApiResourceId")
	VALUES (clientId, (SELECT "Id" FROM public."Resource" WHERE "Name" = 'authorization-provider'));

INSERT INTO public."Client_GrantType"(
	"ClientId", "GrantTypeId")
	VALUES (clientId, (SELECT "Id" FROM public."GrantType" WHERE "Value" = 'hybrid'));

INSERT INTO public."Client_IdentityResource"(
	"ClientId", "IdentityResourceId")
	VALUES (clientId, (SELECT "Id" FROM public."Resource" WHERE "Name" = 'openid')),
	(clientId, (SELECT "Id" FROM public."Resource" WHERE "Name" = 'profile'));

END $$;

INSERT INTO public."VersionInfo"("Version", "AppliedOn", "Description") 
VALUES 
(107, now(), 'M_107_AddBatchImportClient');

COMMIT;