using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailSender1.Migrations
{
    /// <inheritdoc />
    public partial class init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    csvInformations_Property5 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emailRecivers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "emailRecivers");
        }
    }
}
