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
('card-file-nla-excerpce', 'Permission to view card file NLA - exceprce'),
('card-file-gebauer-excerpce', 'Permission to view card file Gebauer – excerpce'),
('card-file-gebauer-prameny', 'Permission to view card file Gebauer – prameny'),
('card-file-staroceska-excerpce', 'Permission to view card file Staročeská – excerpce'),
('card-file-zubaty-excerpce', 'Permission to view card file Zubatý – excerpce'),
('card-file-archiv-lidoveho-jazyka', 'Permission to view card file Archiv lidového jazyka'),
('card-file-excerpce-textu-z-16-stoleti', 'Permission to view card file Excerpce textu z 16. století'),
('card-file-latinsko-ceska-kartoteka', 'Permission to view card file Latinsko-česká kartotéka'),
('card-file-rukopisy', 'Permission to view card file Rukopisy'),
('card-file-justitia', 'Permission to view card file Justitia'),
('card-file-tyl-excerpce', 'Permission to view card file Tyl – excerpce'),
('card-file-nla-prameny', 'Permission to view card file NLA - prameny'),
('card-file-svoboda', 'Permission to view card file Svoboda'),
('card-file-excerpce-pomistnich-jmen', 'Permission to view card file Excerpce pomístních jmen'),
('card-file-tereziansky-katastr', 'Permission to view card file Tereziánský katastr'),
('card-file-archivy-muzea', 'Permission to view card file Archivy, muzea'),
('card-file-stabilni-katastr', 'Permission to view card file Stabilní katastr'),
('card-file-sajtl', 'Permission to view card file Sajtl'),
('card-file-dodatky-psjc', 'Permission to view card file Dodatky PSJC'),
('card-file-grepl-archiv', 'Permission to view card file Grepl - archiv');

INSERT INTO public."VersionInfo"("Version", "AppliedOn", "Description") 
VALUES 
(100, now(), 'M_100_AddPermissions');

COMMIT;