﻿START TRANSACTION;

DO $$
DECLARE roleId integer;
DECLARE userId integer;
BEGIN

INSERT INTO public."User"(
"Username", "FirstName", "LastName", "PasswordHash", "SecurityStamp", "TwoFactorEnabled", "LockoutEndDateUtc", "LockoutEnabled", "AccessFailedCount", "LastChange", "MasterUserId")
VALUES
   ('Admin' -- Username
	,'Admin' -- FirstName
           ,'Admin' -- LastName
           ,'AQAAAAEAACcQAAAAEMmJB8Wd5WUJlww2+qO1ClJ7+nrFKxt5gm73qaiDqUmBSyjTXv2Vjw87lsG14XldAw==' -- PasswordHash (password is 'bob')
           ,'7cda8f60-01de-447f-91e9-a497aa6b2146' -- SecurityStamp
           ,false --TwoFactorEnabled
           ,NULL -- LockoutEndDateUtc
		   ,false --LockoutEnabled
           ,0 -- AccessFailedCount
           ,'2019-01-01 00:00:00.000' -- LastChange
		   ,'f19d060a-42e6-49b4-8655-993378fed3c1'); -- MasterUserId

		   
SELECT "Id"	INTO roleId FROM public."Role" WHERE "Name" = 'Admin';

SELECT "Id"	INTO userId FROM public."User" WHERE "Username" = 'Admin';

INSERT INTO public."User_Role"(
	"UserId", "RoleId")
	VALUES 
(userId, roleId);

INSERT INTO public."UserContact"(
"UserId", "Type", "Value", "Confirmed", "CreateTime", "ConfirmCodeChangeTime")
VALUES
   (userId
	,'Email' 
           ,'admin@example.com' 
		   ,true
		   ,'2019-01-01 00:00:00.000'
           ,NULL);

END $$;

INSERT INTO public."VersionInfo"("Version", "AppliedOn", "Description") 
VALUES 
(103, now(), 'M_103_AddDefaultAdminUser');

COMMIT;