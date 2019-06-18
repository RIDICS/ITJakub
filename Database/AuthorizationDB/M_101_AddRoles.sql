START TRANSACTION;

INSERT INTO public."Role"("Name", "Description", "AuthenticationServiceOnly")
VALUES 
('PortalAdmin', 'Role for Vokabular administrators', 'false'),
('Unregistered', 'Default user group', 'false');

INSERT INTO public."VersionInfo"("Version", "AppliedOn", "Description") 
VALUES 
(101, now(), 'M_101_AddRoles');

COMMIT;