START TRANSACTION;

DO $$
DECLARE clientId integer;
BEGIN

INSERT INTO public."Client"(
	"Name", "Description", "DisplayUrl", "LogoUrl", "RequireConsent")
	VALUES ('VokabularForum', 'Vokabular Webovy - Forum', 'http://vokabular.ujc.cas.cz', 'http://vokabular.ujc.cas.cz', false);

		   
SELECT "Id"	INTO clientId FROM public."Client" WHERE "Name" = 'VokabularForum';

INSERT INTO public."Uri"(
	"Uri", "ClientId")
	VALUES ('https://localhost:50165/auth.aspx?auth=vokabular', clientId);

INSERT INTO public."Uri_UriType"("UriId", "UriTypeId")
	VALUES ((SELECT "Id" FROM public."Uri" WHERE "Uri" = 'http://localhost:50165/auth.aspx?auth=vokabular'), (SELECT "Id" FROM public."UriType" WHERE "Value" = 'Redirect'));

INSERT INTO public."Secret"(
	"Value", "Description", "Expiration", "ResourceId", "ClientId", "Discriminator")
	VALUES ('secret', 'secret', null, null, clientId, 'ClientSecret');

INSERT INTO public."Client_ApiResource"(
	"ClientId", "ApiResourceId")
	VALUES (clientId, (SELECT "Id" FROM public."Resource" WHERE "Name" = 'authorization-provider'));

INSERT INTO public."Client_GrantType"(
	"ClientId", "GrantTypeId")
	VALUES (clientId, (SELECT "Id" FROM public."GrantType" WHERE "Value" = 'authorization_code'));

INSERT INTO public."Client_IdentityResource"(
	"ClientId", "IdentityResourceId")
	VALUES (clientId, (SELECT "Id" FROM public."Resource" WHERE "Name" = 'offline_access')),
	(clientId, (SELECT "Id" FROM public."Resource" WHERE "Name" = 'openid')),
	(clientId, (SELECT "Id" FROM public."Resource" WHERE "Name" = 'profile'));

END $$;

INSERT INTO public."VersionInfo"("Version", "AppliedOn", "Description") 
VALUES 
(106, now(), 'M_106_AddForumClient');

COMMIT;