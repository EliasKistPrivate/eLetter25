using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eLetter25.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "eletter25");

            migrationBuilder.CreateTable(
                name: "Letters",
                schema: "eletter25",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SentDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SenderReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecipientReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SenderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SenderStreet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SenderPostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SenderCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SenderCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SenderEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    SenderPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RecipientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RecipientStreet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RecipientPostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RecipientCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RecipientCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RecipientEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    RecipientPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Letters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LetterTags",
                schema: "eletter25",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LetterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LetterTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LetterTags_Letters_LetterId",
                        column: x => x.LetterId,
                        principalSchema: "eletter25",
                        principalTable: "Letters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LetterTags_LetterId",
                schema: "eletter25",
                table: "LetterTags",
                column: "LetterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LetterTags",
                schema: "eletter25");

            migrationBuilder.DropTable(
                name: "Letters",
                schema: "eletter25");
        }
    }
}
