using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockChatter.API.Migrations
{
    public partial class CreatesMessagesTableSenAtFieldIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IDX_Messages_SentAt",
                table: "UserMassages",
                column: "SentAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_Messages_SentAt",
                table: "UserMassages");
        }
    }
}
