START TRANSACTION;

DO $$
DECLARE roleId integer;
BEGIN

SELECT "Id"	INTO roleId FROM public."Role" WHERE "Name" = 'PortalAdmin';

INSERT INTO public."Role_Permission"(
	"RoleId", "PermissionId")
	VALUES 
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'manage-permissions')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'upload-book')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'add-news')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'manage-feedbacks')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'edit-lemmatization')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'read-lemmatization')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'derivate-lemmatization')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'edition-print-text')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'edit-static-text')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:1')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:2')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:3')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:4')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:5')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:6')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:7')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:8')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:9')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:10')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:11')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:12')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:13')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:14')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:15')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:16')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:17')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:18')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:19')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file:20')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'autoimport:0')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'autoimport:1')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'autoimport:2')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'autoimport:3')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'autoimport:4')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'autoimport:5')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'autoimport:6')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'autoimport:7'));

END $$;

INSERT INTO public."VersionInfo"("Version", "AppliedOn", "Description") 
VALUES 
(102, now(), 'M_102_AddPermissions');

COMMIT;