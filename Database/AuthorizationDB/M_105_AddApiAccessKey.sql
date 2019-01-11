START TRANSACTION;

DO $$
DECLARE apiAccessKeyId integer;
BEGIN

INSERT INTO public."ApiAccessKey"(
	"ApiKeyHash", "HashAlgorithm", "Name")
	VALUES
   ('2BB80D537B1DA3E38BD30361AA855686BDE0EACD7162FEF6A25FE97BF527A25B' -- ApiKeyHash (hash is 'secret')
   ,'sha256' 
    ,'Vokabular');

		   
SELECT "Id"	INTO apiAccessKeyId FROM public."ApiAccessKey" WHERE "Name" = 'Vokabular';



INSERT INTO public."ApiAccessKeyPermission"(
	"ApiAccessKeyId", "Permission")
	VALUES 
(apiAccessKeyId, 1);

INSERT INTO public."ApiAccessKeyPermission"(
	"ApiAccessKeyId", "Permission")
	VALUES 
(apiAccessKeyId, 2);

END $$;

INSERT INTO public."VersionInfo"("Version", "AppliedOn", "Description") 
VALUES 
(105, now(), 'M_105_AddApiAccessKey');

COMMIT;