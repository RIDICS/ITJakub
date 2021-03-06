﻿namespace ITJakub.Web.Hub.Constants
{
    public static class PageSizes
    {
        public const int BibliographyModule = 20;
        public const int CorpusSearch = 30;
        public const int CorpusSearchBooks = 10;
        public const int ProjectList = 20;
        public const int ProjectListInDialog = 12;
        public const int SnapshotList = 10;
        public const int CooperationList = 10;
        public const int Headwords = 50;
        public const int SearchHeadwords = 25;

        public const int NewsFeed = 10;
        public const int News = 5;
        public const int Feedback = 10;
        public const int KeyTable = 14;

        public const int Users = 20;
        public const int Roles = 12;
        public const int UsersInSublist = 12;
        public const int Permissions = 15;
        public const int ProjectsInSublist = 16;

        public const int MinCorpusSearch = 1;
        public const int MaxCorpusSearch = 100;
        public const int DefaultContextLength = 100;
        public const int MinContextLength = 40; //search backend may crash if context is too short
        public const int MaxContextLength = 500;

        // Missing favorite items here
    }
}
