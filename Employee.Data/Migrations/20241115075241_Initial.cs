using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Employee.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    LastName = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    BirthDay = table.Column<DateOnly>(type: "date", nullable: false),
                    Sex = table.Column<int>(type: "integer", nullable: false),
                    HaveChildren = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employers_LastName_FirstName",
                table: "Employers",
                columns: new[] { "LastName", "FirstName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employers");
        }
    }
}
