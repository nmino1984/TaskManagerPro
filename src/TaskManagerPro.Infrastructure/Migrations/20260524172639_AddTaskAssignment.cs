using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedToUserId",
                table: "MyTasks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MyTasks_AssignedToUserId",
                table: "MyTasks",
                column: "AssignedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MyTasks_Users_AssignedToUserId",
                table: "MyTasks",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyTasks_Users_AssignedToUserId",
                table: "MyTasks");

            migrationBuilder.DropIndex(
                name: "IX_MyTasks_AssignedToUserId",
                table: "MyTasks");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "MyTasks");
        }
    }
}
