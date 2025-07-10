using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MES_ReportForms.Core.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LogType",
                table: "OrderLog",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "订单日志类型",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "From",
                table: "OrderLog",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "来自某用户或系统的名称",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AfterOrderStatus",
                table: "OrderLog",
                type: "int",
                nullable: true,
                comment: "后来状态");

            migrationBuilder.AddColumn<int>(
                name: "BeforeOrderStatus",
                table: "OrderLog",
                type: "int",
                nullable: true,
                comment: "原来状态");

            migrationBuilder.AddColumn<int>(
                name: "FromUserType",
                table: "OrderLog",
                type: "int",
                nullable: true,
                comment: "用户类型");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLog_LogType",
                table: "OrderLog",
                column: "LogType");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLog_OrderNo",
                table: "OrderLog",
                column: "OrderNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderLog_LogType",
                table: "OrderLog");

            migrationBuilder.DropIndex(
                name: "IX_OrderLog_OrderNo",
                table: "OrderLog");

            migrationBuilder.DropColumn(
                name: "AfterOrderStatus",
                table: "OrderLog");

            migrationBuilder.DropColumn(
                name: "BeforeOrderStatus",
                table: "OrderLog");

            migrationBuilder.DropColumn(
                name: "FromUserType",
                table: "OrderLog");

            migrationBuilder.AlterColumn<int>(
                name: "LogType",
                table: "OrderLog",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "订单日志类型");

            migrationBuilder.AlterColumn<string>(
                name: "From",
                table: "OrderLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "来自某用户或系统的名称");
        }
    }
}
