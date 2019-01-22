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
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-nla-excerpce')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-gebauer-excerpce')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-gebauer-prameny')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-staroceska-excerpce')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-zubaty-excerpce')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-archiv-lidoveho-jazyka')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-excerpce-textu-z-16-stoleti')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-latinsko-ceska-kartoteka')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-rukopisy')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-justitia')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-tyl-excerpce')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-nla-prameny')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-svoboda')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-excerpce-pomistnich-jmen')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-tereziansky-katastr')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-archivy-muzea')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-stabilni-katastr')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-sajtl')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-dodatky-psjc')),
(roleId ,(SELECT "Id" FROM public."Permission" WHERE "Name" = 'card-file-grepl-archiv')),
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