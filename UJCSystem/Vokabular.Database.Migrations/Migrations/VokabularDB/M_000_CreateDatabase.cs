﻿using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.Data, CoreMigrationTypeTagTypes.All)]
    [Migration(000)]
    public class M_000_CreateDatabase : ForwardOnlyMigration
    {
        public override void Up()
        {
        }
    }
}