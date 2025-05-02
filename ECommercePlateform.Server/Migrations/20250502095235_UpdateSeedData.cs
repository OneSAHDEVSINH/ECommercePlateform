using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommercePlateform.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e65a3a8a-2407-4965-9b71-b9a1d8e2c34f"),
                columns: new[] { "CreatedOn", "ModifiedOn" },
                values: new object[] { new DateTime(2025, 5, 2, 3, 18, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 5, 2, 3, 18, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e65a3a8a-2407-4965-9b71-b9a1d8e2c34f"),
                columns: new[] { "CreatedOn", "ModifiedOn" },
                values: new object[] { new DateTime(2025, 5, 2, 14, 56, 57, 234, DateTimeKind.Local).AddTicks(4047), new DateTime(2025, 5, 2, 14, 56, 57, 234, DateTimeKind.Local).AddTicks(4297) });
        }
    }
}
