using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailSender1.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "adresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "emailRecivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Surename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAdress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    csvInformations_Property1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    csvInformations_Property2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    csvInformations_Property3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    csvInformations_Property4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    csvInformations_Property5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdressesBookId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emailRecivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_emailRecivers_adresses_AdressesBookId",
                        column: x => x.AdressesBookId,
                        principalTable: "adresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_emailRecivers_AdressesBookId",
                table: "emailRecivers",
                column: "AdressesBookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "emailRecivers");

            migrationBuilder.DropTable(
                name: "adresses");
        }
    }
}
