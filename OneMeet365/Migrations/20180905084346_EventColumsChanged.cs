using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OneMeet365.Migrations
{
    public partial class EventColumsChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WhereToMeet",
                table: "OneMeetEvent",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "WhereToGo",
                table: "OneMeetEvent",
                newName: "MeetingPlace");

            migrationBuilder.RenameColumn(
                name: "WhenToMeet",
                table: "OneMeetEvent",
                newName: "EventPlace");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "OneMeetEvent",
                newName: "WhereToMeet");

            migrationBuilder.RenameColumn(
                name: "MeetingPlace",
                table: "OneMeetEvent",
                newName: "WhereToGo");

            migrationBuilder.RenameColumn(
                name: "EventPlace",
                table: "OneMeetEvent",
                newName: "WhenToMeet");
        }
    }
}
