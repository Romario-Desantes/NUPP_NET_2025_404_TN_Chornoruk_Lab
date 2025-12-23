using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FigureProj.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDomainIdToFigures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DomainId",
                table: "Figures",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Figures_DomainId_Unique",
                table: "Figures",
                column: "DomainId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Figures_DomainId_Unique",
                table: "Figures");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "Figures");
        }
    }
}
