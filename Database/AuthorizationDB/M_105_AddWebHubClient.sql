START TRANSACTION;

DO $$
DECLARE clientId integer;
BEGIN

INSERT INTO public."Client"(
	"Name", "Description", "DisplayUrl", "LogoUrl", "RequireConsent")
	VALUES ('Vokabular', 'Vokabular Webovy', 'http://vokabular.ujc.cas.cz', 'http://vokabular.ujc.cas.cz', false);

		   
SELECT "Id"	INTO clientId FROM public."Client" WHERE "Name" = 'Vokabular';

INSERT INTO public."Uri"(
	"Uri", "ClientId")
	VALUES ('https://localhost:44368/signin-oidc', clientId),
	('https://localhost:44368/', clientId),
	('https://localhost:44368/signout-callback-oidc', clientId),
	('https://localhost:44368/account/clientlogout', clientId);

INSERT INTO public."Uri_UriType"("UriId", "UriTypeId")
	VALUES ((SELECT "Id" FROM public."Uri" WHERE "Uri" = 'https://localhost:44368/signin-oidc'), (SELECT "Id" FROM public."UriType" WHERE "Value" = 'Redirect')),
	((SELECT "Id" FROM public."Uri" WHERE "Uri" = 'https://localhost:44368/'), (SELECT "Id" FROM public."UriType" WHERE "Value" = 'PostLogoutRedirect')),
	((SELECT "Id" FROM public."Uri" WHERE "Uri" = 'https://localhost:44368/signout-callback-oidc'), (SELECT "Id" FROM public."UriType" WHERE "Value" = 'CorsOrigin')),
	((SELECT "Id" FROM public."Uri" WHERE "Uri" = 'https://localhost:44368/account/clientlogout'), (SELECT "Id" FROM public."UriType" WHERE "Value" = 'FrontChannelLogout'));

INSERT INTO public."Secret"(
	"Value", "Description", "Expiration", "ResourceId", "ClientId", "Discriminator")
	VALUES ('secret', 'secret', null, null, clientId, 'ClientSecret');

INSERT INTO public."Client_ApiResource"(
	"ClientId", "ApiResourceId")
	VALUES (clientId, (SELECT "Id" FROM public."Resource" WHERE "Name" = 'authorization-provider'));

INSERT INTO public."Client_GrantType"(
	"ClientId", "GrantTypeId")
	VALUES (clientId, (SELECT "Id" FROM public."GrantType" WHERE "Value" = 'client_credentials')),
	(clientId, (SELECT "Id" FROM public."GrantType" WHERE "Value" = 'hybrid')),
	(clientId, (SELECT "Id" FROM public."GrantType" WHERE "Value" = 'password'));

INSERT INTO public."Client_IdentityResource"(
	"ClientId", "IdentityResourceId")
	VALUES (clientId, (SELECT "Id" FROM public."Resource" WHERE "Name" = 'offline_access')),
	(clientId, (SELECT "Id" FROM public."Resource" WHERE "Name" = 'openid')),
	(clientId, (SELECT "Id" FROM public."Resource" WHERE "Name" = 'profile'));

END $$;

INSERT INTO public."VersionInfo"("Version", "AppliedOn", "Description") 
VALUES 
(105, now(), 'M_105_AddWebHubClient');

COMMIT;