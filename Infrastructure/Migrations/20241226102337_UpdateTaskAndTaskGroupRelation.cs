using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GKTodoManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskAndTaskGroupRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskGroups_Tasks_TaskId",
                table: "TaskGroups");

            migrationBuilder.DropIndex(
                name: "IX_TaskGroups_TaskId",
                table: "TaskGroups");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "TaskGroups");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 26, 10, 23, 35, 979, DateTimeKind.Utc).AddTicks(4898),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 12, 13, 14, 38, 24, 548, DateTimeKind.Utc).AddTicks(5178));

            migrationBuilder.AddColumn<int>(
                name: "TaskGroupId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskGroupId",
                table: "Tasks",
                column: "TaskGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskGroups_TaskGroupId",
                table: "Tasks",
                column: "TaskGroupId",
                principalTable: "TaskGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskGroups_TaskGroupId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TaskGroupId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TaskGroupId",
                table: "Tasks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 13, 14, 38, 24, 548, DateTimeKind.Utc).AddTicks(5178),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 12, 26, 10, 23, 35, 979, DateTimeKind.Utc).AddTicks(4898));

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "TaskGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TaskGroups_TaskId",
                table: "TaskGroups",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskGroups_Tasks_TaskId",
                table: "TaskGroups",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
