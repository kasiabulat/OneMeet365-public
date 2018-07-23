using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OneMeet365.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OneMeetEvent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Activity = table.Column<string>(nullable: true),
                    MaxNumberOfPeople = table.Column<int>(nullable: false),
                    WhenToMeet = table.Column<string>(nullable: true),
                    WhereToGo = table.Column<string>(nullable: true),
                    WhereToMeet = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneMeetEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    ResourceResponseId = table.Column<string>(nullable: false),
                    EventDataId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.ResourceResponseId);
                    table.ForeignKey(
                        name: "FK_Events_OneMeetEvent_EventDataId",
                        column: x => x.EventDataId,
                        principalTable: "OneMeetEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Atendee",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventCardDataResourceResponseId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atendee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Atendee_Events_EventCardDataResourceResponseId",
                        column: x => x.EventCardDataResourceResponseId,
                        principalTable: "Events",
                        principalColumn: "ResourceResponseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Atendee_EventCardDataResourceResponseId",
                table: "Atendee",
                column: "EventCardDataResourceResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventDataId",
                table: "Events",
                column: "EventDataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Atendee");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "OneMeetEvent");
        }
    }
}
