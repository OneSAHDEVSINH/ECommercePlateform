using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommercePlateform.Server.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class CleanArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedOn", "ModifiedOn" },
                values: new object[] { new DateTime(2025, 5, 12, 0, 34, 41, 866, DateTimeKind.Local).AddTicks(3662), new DateTime(2025, 5, 12, 0, 34, 41, 866, DateTimeKind.Local).AddTicks(3846) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedOn", "ModifiedOn" },
                values: new object[] { new DateTime(2025, 5, 12, 0, 27, 8, 473, DateTimeKind.Local).AddTicks(7472), new DateTime(2025, 5, 12, 0, 27, 8, 473, DateTimeKind.Local).AddTicks(7620) });
        }
    }
}
