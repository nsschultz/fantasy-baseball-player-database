using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FantasyBaseball.PlayerServiceDatabase.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MlbTeams",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 3, nullable: false),
                    AlternativeCode = table.Column<string>(maxLength: 3, nullable: true),
                    MlbLeagueId = table.Column<string>(maxLength: 2, nullable: true),
                    City = table.Column<string>(maxLength: 20, nullable: true),
                    Nickname = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("MlbTeam_PK", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 3, nullable: false),
                    FullName = table.Column<string>(maxLength: 20, nullable: true),
                    PlayerType = table.Column<int>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Position_PK", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BhqId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 20, nullable: true),
                    LastName = table.Column<string>(maxLength: 20, nullable: true),
                    Age = table.Column<int>(nullable: false),
                    Team = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    DraftRank = table.Column<int>(nullable: false),
                    AverageDraftPick = table.Column<int>(nullable: false),
                    HighestPick = table.Column<int>(nullable: false),
                    DraftedPercentage = table.Column<double>(nullable: false),
                    Reliability = table.Column<double>(nullable: false),
                    MayberryMethod = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Player_PK", x => x.Id);
                    table.UniqueConstraint("Player_Bhq_AK", x => new { x.BhqId, x.Type });
                    table.ForeignKey(
                        name: "Player_MlbTeam_FK",
                        column: x => x.Team,
                        principalTable: "MlbTeams",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BattingStats",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(nullable: false),
                    StatsType = table.Column<int>(nullable: false),
                    AtBats = table.Column<int>(nullable: false),
                    Runs = table.Column<int>(nullable: false),
                    Hits = table.Column<int>(nullable: false),
                    Doubles = table.Column<int>(nullable: false),
                    Triples = table.Column<int>(nullable: false),
                    HomeRuns = table.Column<int>(nullable: false),
                    RunsBattedIn = table.Column<int>(nullable: false),
                    BaseOnBalls = table.Column<int>(nullable: false),
                    StrikeOuts = table.Column<int>(nullable: false),
                    StolenBases = table.Column<int>(nullable: false),
                    CaughtStealing = table.Column<int>(nullable: false),
                    Power = table.Column<double>(nullable: false),
                    Speed = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("BattingStats_PK", x => new { x.PlayerId, x.StatsType });
                    table.ForeignKey(
                        name: "BattingStats_Player_FK",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeagueStatuses",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(nullable: false),
                    LeagueId = table.Column<int>(nullable: false),
                    LeagueStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("LeagueStatus_PK", x => new { x.PlayerId, x.LeagueId });
                    table.ForeignKey(
                        name: "LeagueStatus_Player_FK",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PitchingStats",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(nullable: false),
                    StatsType = table.Column<int>(nullable: false),
                    Wins = table.Column<int>(nullable: false),
                    Losses = table.Column<int>(nullable: false),
                    QualityStarts = table.Column<int>(nullable: false),
                    Saves = table.Column<int>(nullable: false),
                    BlownSaves = table.Column<int>(nullable: false),
                    Holds = table.Column<int>(nullable: false),
                    InningsPitched = table.Column<double>(nullable: false),
                    HitsAllowed = table.Column<int>(nullable: false),
                    EarnedRuns = table.Column<int>(nullable: false),
                    HomeRunsAllowed = table.Column<int>(nullable: false),
                    BaseOnBallsAllowed = table.Column<int>(nullable: false),
                    StrikeOuts = table.Column<int>(nullable: false),
                    FlyBallRate = table.Column<double>(nullable: false),
                    GroundBallRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PitchingStats_PK", x => new { x.PlayerId, x.StatsType });
                    table.ForeignKey(
                        name: "PitchingStats_Player_FK",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerPositionEntity",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(nullable: false),
                    PositionCode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPositionEntity", x => new { x.PlayerId, x.PositionCode });
                    table.ForeignKey(
                        name: "PlayerPosition_Player_FK",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "PlayerPosition_Position_FK",
                        column: x => x.PositionCode,
                        principalTable: "Positions",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MlbTeams",
                columns: new[] { "Code", "AlternativeCode", "City", "MlbLeagueId", "Nickname" },
                values: new object[,]
                {
                    { "", null, "Free Agent", "", "Free Agent" },
                    { "SF", null, "San Francisco", "NL", "Giants" },
                    { "SD", null, "San Diego", "NL", "Padres" },
                    { "LAD", "LA", "Los Angeles", "NL", "Dodgers" },
                    { "COL", null, "Colorado", "NL", "Rockies" },
                    { "ARZ", "ARI", "Arizona", "NL", "Diamondbacks" },
                    { "STL", null, "St. Louis", "NL", "Cardinals" },
                    { "PIT", null, "Pittsburgh", "NL", "Pirates" },
                    { "MIL", null, "Milwaukee", "NL", "Brewers" },
                    { "CHC", null, "Chicago", "NL", "Cubs" },
                    { "WAS", null, "Washington", "NL", "Nationals" },
                    { "PHI", null, "Philadelphia", "NL", "Phillies" },
                    { "NYM", null, "New York", "NL", "Mets" },
                    { "MIA", null, "Miami", "NL", "Marlins" },
                    { "ATL", null, "Atlanta", "NL", "Braves" },
                    { "CIN", null, "Cincinnati", "NL", "Reds" },
                    { "SEA", null, "Seattle", "AL", "Mariners" },
                    { "TEX", null, "Texas", "AL", "Rangers" },
                    { "BOS", null, "Boston", "AL", "Red Sox" },
                    { "NYY", null, "New York", "AL", "Yankees" },
                    { "TB", "TAM", "Tampa Bay", "AL", "Rays" },
                    { "TOR", null, "Toronto", "AL", "Blue Jays" },
                    { "CWS", "CHW", "Chicago", "AL", "White Sox" },
                    { "CLE", null, "Cleveland", "AL", "Indians" },
                    { "BAL", null, "Baltimore", "AL", "Orioles" },
                    { "KC", null, "Kansas City", "AL", "Royals" },
                    { "MIN", null, "Minnesota", "AL", "Twins" },
                    { "HOU", null, "Houston", "AL", "Astros" },
                    { "LAA", null, "Los Angeles", "AL", "Angels" },
                    { "OAK", null, "Oakland", "AL", "Athletics" },
                    { "DET", null, "Detriot", "AL", "Tigers" }
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "Code", "FullName", "PlayerType", "SortOrder" },
                values: new object[,]
                {
                    { "RP", "Relief Pitcher", 1, 12 },
                    { "SP", "Starting Pitcher", 1, 11 },
                    { "DH", "Designated Hitter", 1, 10 },
                    { "OF", "Outfielder", 1, 9 },
                    { "RF", "Right Fielder", 1, 8 },
                    { "CF", "Center Feilder", 1, 7 },
                    { "LF", "Left Fielder", 1, 6 },
                    { "3B", "Third Baseman", 1, 3 },
                    { "SS", "Shortstop", 1, 4 },
                    { "2B", "Second Baseman", 1, 2 },
                    { "1B", "First Baseman", 1, 1 },
                    { "C", "Catcher", 1, 0 },
                    { "", "Unknown", 0, 2147483647 },
                    { "IF", "Infielder", 1, 5 },
                    { "P", "Pitcher", 1, 13 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPositionEntity_PositionCode",
                table: "PlayerPositionEntity",
                column: "PositionCode");

            migrationBuilder.CreateIndex(
                name: "IX_Players_Team",
                table: "Players",
                column: "Team");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_SortOrder",
                table: "Positions",
                column: "SortOrder",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BattingStats");

            migrationBuilder.DropTable(
                name: "LeagueStatuses");

            migrationBuilder.DropTable(
                name: "PitchingStats");

            migrationBuilder.DropTable(
                name: "PlayerPositionEntity");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "MlbTeams");
        }
    }
}
