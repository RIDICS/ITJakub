START TRANSACTION;

DO $$
DECLARE apiAccessKeyId integer;
BEGIN

INSERT INTO public."ApiAccessKey"(
	"ApiKeyHash", "HashAlgorithm", "Name")
	VALUES
   ('D55CE0C2E0594B71636FAF04DE7E686120562459914CC30E291256A66420AB88' -- ApiKeyHash (hash is 'secret')
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
(104, now(), 'M_104_AddApiAccessKey');

COMMIT;