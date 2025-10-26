using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZorgmeldSysteem.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPriorityAndUrgentFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUrgent",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DefaultPriority",
                table: "Objects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefaultReactionTime",
                table: "Objects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUrgent",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DefaultPriority",
                table: "Objects");

            migrationBuilder.DropColumn(
                name: "DefaultReactionTime",
                table: "Objects");
        }
    }
}
