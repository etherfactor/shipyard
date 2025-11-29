using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Base;
using FluentMigrator.Runner.Generators.Postgres;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace EtherGizmos.Shipyard.Services;

internal class CustomPostgresGenerator : Postgres15_0Generator
{
    [Obsolete]
    public CustomPostgresGenerator(
        Postgres15_0Generator inner,
        IPostgresTypeMap typeMap,
        IOptions<GeneratorOptions> generatorOptions)
        : this(GetColumn(inner, typeMap), GetQuoter(inner), generatorOptions)
    {
    }

    protected CustomPostgresGenerator(IColumn column, PostgresQuoter quoter, IOptions<GeneratorOptions> generatorOptions) : base(column, quoter, generatorOptions)
    {
    }

    [Obsolete]
    private static ColumnBase<IPostgresTypeMap> GetColumn(
        GeneratorBase inner,
        IPostgresTypeMap typeMap)
    {
        var column = (ColumnBase<IPostgresTypeMap>)typeof(GeneratorBase)
            .GetProperty(nameof(Column), BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(inner)!;

        return new CustomPostgresColumn(column, typeMap, GetQuoter(inner));
    }

    [Obsolete]
    private static PostgresQuoter GetQuoter(
        GeneratorBase inner)
    {
        var quoter = (PostgresQuoter)typeof(GeneratorBase)
            .GetProperty(nameof(Quoter), BindingFlags.Public | BindingFlags.Instance)!
            .GetValue(inner)!;

        return quoter;
    }

    [ExcludeFromCodeCoverage]
    public override string Generate(AlterColumnExpression expression)
    {
        var alterStatement = new StringBuilder();
        alterStatement.AppendFormat(
            AlterColumn,
            Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
            ((CustomPostgresColumn)Column).GenerateAlterClauses(expression.Column));

        AppendSqlStatementEndToken(alterStatement);

        var descriptionStatement = DescriptionGenerator.GenerateDescriptionStatement(expression);

        if (!string.IsNullOrEmpty(descriptionStatement))
        {
            alterStatement.Append(descriptionStatement);
            AppendSqlStatementEndToken(alterStatement);
        }

        return alterStatement.ToString();
    }

    [ExcludeFromCodeCoverage]
    public override string Generate(AlterDefaultConstraintExpression expression)
    {
        return string.Format(
            "ALTER TABLE {0} ALTER {1} DROP DEFAULT, ALTER {1} {2};",
            Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
            Quoter.QuoteColumnName(expression.ColumnName),
            ((CustomPostgresColumn)Column).FormatAlterDefaultValue(expression.ColumnName, expression.DefaultValue));
    }
}
