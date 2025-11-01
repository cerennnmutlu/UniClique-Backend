using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UniCliqueBackend.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsBanned = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    Expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP + interval '30 days'"),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_consents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ConsentType = table.Column<int>(type: "integer", nullable: false),
                    IsAccepted = table.Column<bool>(type: "boolean", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_consents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_consents_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "BirthDate", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "PhoneNumber", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(1995, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 1, 13, 27, 24, 902, DateTimeKind.Utc).AddTicks(9170), "admin@uniclique.com", "Admin User", true, "admin123", "5555555555", 2, "admin" },
                    { 2, new DateTime(1990, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 1, 13, 27, 24, 902, DateTimeKind.Utc).AddTicks(9180), "business@uniclique.com", "Business Owner", true, "biz123", "5551112233", 1, "business1" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "BirthDate", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "PhoneNumber", "Username" },
                values: new object[] { 3, new DateTime(2000, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 1, 13, 27, 24, 902, DateTimeKind.Utc).AddTicks(9180), "user@uniclique.com", "Regular User", true, "user123", "5552223344", "user1" });

            migrationBuilder.InsertData(
                table: "refresh_tokens",
                columns: new[] { "Id", "CreatedAt", "Expiration", "Token", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 1, 13, 27, 24, 902, DateTimeKind.Utc).AddTicks(9270), new DateTime(2025, 12, 1, 13, 27, 24, 902, DateTimeKind.Utc).AddTicks(9270), "sample_refresh_token_admin", 1 },
                    { 2, new DateTime(2025, 11, 1, 13, 27, 24, 902, DateTimeKind.Utc).AddTicks(9270), new DateTime(2025, 12, 1, 13, 27, 24, 902, DateTimeKind.Utc).AddTicks(9270), "sample_refresh_token_user", 3 }
                });

            migrationBuilder.InsertData(
                table: "user_consents",
                columns: new[] { "Id", "AcceptedAt", "ConsentType", "IsAccepted", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 1, 13, 27, 24, 902, DateTimeKind.Utc).AddTicks(9220), 0, true, 1 },
                    { 2, new DateTime(2025, 11, 1, 13, 27, 24, 902, DateTimeKind.Utc).AddTicks(9220), 1, true, 1 },
                    { 3, new DateTime(2025, 11, 1, 13, 27, 24, 902, DateTimeKind.Utc).AddTicks(9220), 0, true, 2 },
                    { 4, new DateTime(2025, 11, 1, 13, 27, 24, 902, DateTimeKind.Utc).AddTicks(9220), 2, true, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_Token",
                table: "refresh_tokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_UserId",
                table: "refresh_tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_consents_UserId_ConsentType",
                table: "user_consents",
                columns: new[] { "UserId", "ConsentType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Username",
                table: "users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "user_consents");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
