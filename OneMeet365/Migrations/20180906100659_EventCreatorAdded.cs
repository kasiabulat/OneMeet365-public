using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OneMeet365.Migrations
{
    public partial class EventCreatorAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "Events",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_ResourceResponseId",
                table: "Events",
                column: "ResourceResponseId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_ResourceResponseId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "Events");
        }
    }
}
