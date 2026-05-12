using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MyTasks_UserId",
                table: "MyTasks");

            migrationBuilder.CreateIndex(
                name: "IX_MyTasks_Priority",
                table: "MyTasks",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_MyTasks_Status",
                table: "MyTasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MyTasks_UserId_IsDeleted_CreatedAt",
                table: "MyTasks",
                columns: new[] { "UserId", "IsDeleted", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MyTasks_Priority",
                table: "MyTasks");

            migrationBuilder.DropIndex(
                name: "IX_MyTasks_Status",
                table: "MyTasks");

            migrationBuilder.DropIndex(
                name: "IX_MyTasks_UserId_IsDeleted_CreatedAt",
                table: "MyTasks");

            migrationBuilder.CreateIndex(
                name: "IX_MyTasks_UserId",
                table: "MyTasks",
                column: "UserId");
        }
    }
}
