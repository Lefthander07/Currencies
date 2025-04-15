using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fuse8.BackendInternship.InternalApi.Migrations
{
    /// <inheritdoc />
    public partial class cur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cur");

            migrationBuilder.CreateTable(
                name: "currency_caches",
                schema: "cur",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    base_currency = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    cache_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_currency_caches", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "currency_exchange_rates",
                schema: "cur",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    currency_cache_id = table.Column<int>(type: "integer", nullable: false),
                    currency_code = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    exchange_rate = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_currency_exchange_rates", x => x.id);
                    table.ForeignKey(
                        name: "fk_currency_exchange_rates_currency_caches_currency_cache_id",
                        column: x => x.currency_cache_id,
                        principalSchema: "cur",
                        principalTable: "currency_caches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_currency_exchange_rates_currency_cache_id",
                schema: "cur",
                table: "currency_exchange_rates",
                column: "currency_cache_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "currency_exchange_rates",
                schema: "cur");

            migrationBuilder.DropTable(
                name: "currency_caches",
                schema: "cur");
        }
    }
}
