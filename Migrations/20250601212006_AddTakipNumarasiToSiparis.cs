using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EticaretUygulamasi.Migrations
{
    /// <inheritdoc />
    public partial class AddTakipNumarasiToSiparis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TakipNumarasi",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TakipNumarasi",
                table: "Siparisler");
        }
    }
}
