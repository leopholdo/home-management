using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShoppingSuggestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS unaccent;");
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Suggestion",
                table: "Suggestion");

            migrationBuilder.RenameTable(
                name: "Suggestion",
                newName: "ShoppingSuggestions");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ShoppingSuggestions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "NameNormalized",
                table: "ShoppingSuggestions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShoppingSuggestions",
                table: "ShoppingSuggestions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingSuggestions_Name",
                table: "ShoppingSuggestions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingSuggestions_NameNormalized",
                table: "ShoppingSuggestions",
                column: "NameNormalized")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingSuggestions_UsageCount_LastUsedAt",
                table: "ShoppingSuggestions",
                columns: new[] { "UsageCount", "LastUsedAt" });

            migrationBuilder.Sql(@"
                CREATE INDEX ix_shopping_suggestions_name_trgm
                ON ""ShoppingSuggestions""
                USING GIN (""NameNormalized"" gin_trgm_ops);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_shopping_suggestions_name_trgm;");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShoppingSuggestions",
                table: "ShoppingSuggestions");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingSuggestions_Name",
                table: "ShoppingSuggestions");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingSuggestions_NameNormalized",
                table: "ShoppingSuggestions");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingSuggestions_UsageCount_LastUsedAt",
                table: "ShoppingSuggestions");

            migrationBuilder.DropColumn(
                name: "NameNormalized",
                table: "ShoppingSuggestions");

            migrationBuilder.RenameTable(
                name: "ShoppingSuggestions",
                newName: "Suggestion");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Suggestion",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Suggestion",
                table: "Suggestion",
                column: "Id");
        }
    }
}
