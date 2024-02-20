using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Migrations
{
    /// <inheritdoc />
    public partial class MoveToTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "TransactionalData",
                newName: "Amount");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "TransactionalData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "TransactionalData",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "TransactionalData");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TransactionalData");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "TransactionalData",
                newName: "Value");
        }
    }
}
