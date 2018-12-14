START TRANSACTION;

INSERT INTO public."Role"("Name", "Description", "AuthenticationServiceOnly")
VALUES 
('Admin', 'Role for administrators', 'false'),
('Unregistered', 'default user group', 'false');

INSERT INTO public."VersionInfo"("Version", "AppliedOn", "Description") 
VALUES 
(101, now(), 'M_101_AddRoles');

COMMIT;