using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RandomBoxAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPictureofItemToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxItems_Items_BoxId",
                table: "BoxItems");

            migrationBuilder.DropIndex(
                name: "IX_BoxItems_BoxId",
                table: "BoxItems");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Items",
                type: "varbinary(max)",
                nullable: true,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Items");

            migrationBuilder.CreateIndex(
                name: "IX_BoxItems_BoxId",
                table: "BoxItems",
                column: "BoxId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoxItems_Items_BoxId",
                table: "BoxItems",
                column: "BoxId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
