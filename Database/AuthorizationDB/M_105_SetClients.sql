START TRANSACTION;

DO $$
DECLARE vokabularId integer;
DECLARE vokabularForumId integer;
DECLARE vokabularImportId integer;
BEGIN

INSERT INTO public."Client"(
	"Name", "DisplayUrl", "LogoUrl", "RequireConsent")
	VALUES ('vokabular','http://vokabular.ujc.cas.cz/','http://vokabular.ujc.cas.cz/','false'),
	('forum-client','http://vokabular.ujc.cas.cz/','http://vokabular.ujc.cas.cz/','false'),
	('vokabular-batchimport','http://vokabular.ujc.cas.cz/','http://vokabular.ujc.cas.cz/','false');

SELECT "Id"	INTO vokabularId FROM public."Client" WHERE "Name" = 'vokabular';
SELECT "Id"	INTO vokabularForumId FROM public."Client" WHERE "Name" = 'forum-client';
SELECT "Id"	INTO vokabularImportId FROM public."Client" WHERE "Name" = 'vokabular-batchimport';


//*secrets
//uri
identity
api
//*grant types**/*/

END $$;

INSERT INTO public."VersionInfo"("Version", "AppliedOn", "Description") 
VALUES 
(105, now(), 'M_105_SetClients');

COMMIT;