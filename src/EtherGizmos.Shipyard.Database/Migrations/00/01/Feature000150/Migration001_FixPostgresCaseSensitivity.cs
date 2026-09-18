using EtherGizmos.Shipyard.Migrations.Core;
using FluentMigrator;

namespace EtherGizmos.Shipyard.Migrations._00._01.Feature000150;

[CreatedAt(year: 2025, month: 11, day: 29, hour: 13, minute: 30, description: "Fix PostgreSQL case sensitivity", trackingId: 150)]
public class Migration001_FixPostgresCaseSensitivity : MigrationExtension
{
    public override void Up()
    {
        IfDatabase(ProcessorIdConstants.PostgreSQL)
            .Execute.Sql("""
                do $$
                declare r record;
                begin
                  for r in
                    select table_schema,
                      table_name,
                      column_name
                      from information_schema.columns
                      where table_schema in ( 'public', 'oauth2', 'acl' )
                        and data_type in ( 'character varying', 'text' )
                        and table_name not in ( 'migration_history' )
                  loop
                    execute format(
                      'alter table %I.%I alter column %I type citext',
                      r.table_schema, r.table_name, r.column_name
                    );
                  end loop;
                end $$;
                """);
    }

    public override void Down()
    {
    }
}
