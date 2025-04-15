using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fuse8.BackendInternship.PublicApi.Migrations
{
    /// <inheritdoc />
    public partial class user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "user");

            migrationBuilder.CreateTable(
                name: "selected_exchange_rates",
                schema: "user",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    currency_code = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    base_currency = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_selected_exchange_rates", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_selected_exchange_rates_currency_code_base_currency",
                schema: "user",
                table: "selected_exchange_rates",
                columns: new[] { "currency_code", "base_currency" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_selected_exchange_rates_name",
                schema: "user",
                table: "selected_exchange_rates",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "selected_exchange_rates",
                schema: "user");
        }
    }
}
