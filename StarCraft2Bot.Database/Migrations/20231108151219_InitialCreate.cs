using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarCraft2Bot.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MyRace = table.Column<int>(type: "INTEGER", nullable: false),
                    EnemyRace = table.Column<int>(type: "INTEGER", nullable: false),
                    Result = table.Column<int>(type: "INTEGER", nullable: false),
                    GameStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GameLength = table.Column<int>(type: "INTEGER", nullable: false),
                    MapName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Datapoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentBuild = table.Column<string>(type: "TEXT", nullable: false),
                    IngameSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalMinerals = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentMinerals = table.Column<int>(type: "INTEGER", nullable: false),
                    LostMinerals = table.Column<int>(type: "INTEGER", nullable: false),
                    KilledMinerals = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalVespene = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentVespene = table.Column<int>(type: "INTEGER", nullable: false),
                    LostVespene = table.Column<int>(type: "INTEGER", nullable: false),
                    KilledVespene = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkerCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Supply = table.Column<int>(type: "INTEGER", nullable: false),
                    LostUnits = table.Column<int>(type: "INTEGER", nullable: false),
                    KilledUnits = table.Column<int>(type: "INTEGER", nullable: false),
                    LostBuildings = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datapoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Datapoints_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameValues_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Datapoints_GameId",
                table: "Datapoints",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameValues_GameId",
                table: "GameValues",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Datapoints");

            migrationBuilder.DropTable(
                name: "GameValues");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
