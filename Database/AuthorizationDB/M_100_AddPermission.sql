START TRANSACTION;

INSERT INTO public."Permission"("Name", "Description") 
VALUES 
('upload-book', 'Permission to upload books'),
('manage-permissions', 'Permission to manage permissions'),
('add-news', 'Permission to add news'),
('manage-feedbacks', 'Permission to manage feedbacks'),
('edit-lemmatization', 'Permission to edit lemmatization'),
('read-lemmatization', 'Permission to read lemmatization'),
('derivate-lemmatization', 'Permission to derivate lemmatization'),
('edition-print-text', 'Permission to '),
('edit-static-text', 'Permission to edit static text'),
('card-file:1', 'Permission to view card file NLA - exceprce'),
('card-file:2', 'Permission to view card file Gebauer – excerpce'),
('card-file:3', 'Permission to view card file Gebauer – prameny'),
('card-file:4', 'Permission to view card file Staročeská – excerpce'),
('card-file:5', 'Permission to view card file Zubatý – excerpce'),
('card-file:6', 'Permission to view card file Archiv lidového jazyka'),
('card-file:7', 'Permission to view card file Excerpce textu z 16. století'),
('card-file:8', 'Permission to view card file Latinsko-česká kartotéka'),
('card-file:9', 'Permission to view card file Rukopisy'),
('card-file:10', 'Permission to view card file Justitia'),
('card-file:11', 'Permission to view card file Tyl – excerpce'),
('card-file:12', 'Permission to view card file NLA - prameny'),
('card-file:13', 'Permission to view card file Svoboda'),
('card-file:14', 'Permission to view card file Excerpce pomístních jmen'),
('card-file:15', 'Permission to view card file Tereziánský katastr'),
('card-file:16', 'Permission to view card file Archivy, muzea'),
('card-file:17', 'Permission to view card file Stabilní katastr'),
('card-file:18', 'Permission to view card file Sajtl'),
('card-file:19', 'Permission to view card file Dodatky PSJC'),
('card-file:20', 'Permission to view card file Grepl - archiv');

INSERT INTO public."VersionInfo"("Version", "AppliedOn", "Description") 
VALUES 
(100, now(), 'M_100_AddPermissions');

COMMIT;