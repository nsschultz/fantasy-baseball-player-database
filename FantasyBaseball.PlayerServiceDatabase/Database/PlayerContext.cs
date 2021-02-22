using System.Threading.Tasks;
using FantasyBaseball.Common.Enums;
using FantasyBaseball.PlayerServiceDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace FantasyBaseball.PlayerServiceDatabase.Database
{
    /// <summary>The context object for players and their related entities.</summary>
    public class PlayerContext : DbContext, IPlayerContext
    {
        private IDbContextTransaction _transaction;

        /// <summary>
        ///     Initializes a new instance of the Microsoft.EntityFrameworkCore.DbContext class using the specified options. 
        ///     The Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)
        ///     method will still be called to allow further configuration of the options.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public PlayerContext(DbContextOptions<PlayerContext> options) : base(options) { }

        /// <summary>A collection of batting stats.</summary>
        public DbSet<BattingStatsEntity> BattingStats { get; set; }

        /// <summary>A collection of league statuses.</summary>
        public DbSet<PlayerLeagueStatusEntity> LeagueStatuses { get; set; }

        /// <summary>A collection of mlb teams statuses.</summary>
        public DbSet<MlbTeamEntity> MlbTeams { get; set; }

        /// <summary>A collection of pitching stats.</summary>
        public DbSet<PitchingStatsEntity> PitchingStats { get; set; }

        /// <summary>A collection of players.</summary>
        public DbSet<PlayerEntity> Players { get; set; }

        /// <summary>A collection of positions.</summary>
        public DbSet<PositionEntity> Positions { get; set; }
        
        /// <summary>A collection of teams.</summary>
        public DbSet<MlbTeamEntity> Teams { get; set; }
 
        /// <summary>Starts a new database transaction.</summary>
        public async Task BeginTransaction() => _transaction = await Database.BeginTransactionAsync();
        
        /// <summary>Commits the database transaction.</summary>
        public async Task Commit()
        {
            try { await SaveAndCommit(); }
            finally { await _transaction.DisposeAsync(); }        
        }

        /// <summary>Rolls the database transaction back.</summary>
        public async Task Rollback()
        { 
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            BuildPlayerModel(modelBuilder.Entity<PlayerEntity>());
            BuildPlayerLeagueStatusModel(modelBuilder.Entity<PlayerLeagueStatusEntity>());
            BuildBattingStatsModel(modelBuilder.Entity<BattingStatsEntity>());
            BuildPitchingStatsModel(modelBuilder.Entity<PitchingStatsEntity>());
            BuildMlbTeamModel(modelBuilder.Entity<MlbTeamEntity>());
            BuildPositionModel(modelBuilder.Entity<PositionEntity>());
            BuildPlayerPositionModel(modelBuilder.Entity<PlayerPositionEntity>());
        }

        private static void BuildBattingStatsModel(EntityTypeBuilder<BattingStatsEntity> builder)
        {
            builder.HasKey(b => new { b.PlayerId, b.StatsType }).HasName("BattingStats_PK");
            builder.HasOne(b => b.Player)
                .WithMany(p => p.BattingStats)
                .HasForeignKey(b => b.PlayerId)
                .HasConstraintName("BattingStats_Player_FK");
        }

        private static void BuildMlbTeamModel(EntityTypeBuilder<MlbTeamEntity> builder)  
        {
            builder.HasKey(b => b.Code).HasName("MlbTeam_PK");
            builder.Property(b => b.Code).HasMaxLength(3);
            builder.Property(b => b.AlternativeCode).HasMaxLength(3);
            builder.Property(b => b.MlbLeagueId).HasMaxLength(2);
            builder.Property(b => b.City).HasMaxLength(20);
            builder.Property(b => b.Nickname).HasMaxLength(20);
            builder.HasData(
                new MlbTeamEntity { Code = ""   , MlbLeagueId = ""  , City = "Free Agent"   , Nickname = "Free Agent"                            },
                new MlbTeamEntity { Code = "BAL", MlbLeagueId = "AL", City = "Baltimore"    , Nickname = "Orioles"                               },
                new MlbTeamEntity { Code = "BOS", MlbLeagueId = "AL", City = "Boston"       , Nickname = "Red Sox"                               },
                new MlbTeamEntity { Code = "NYY", MlbLeagueId = "AL", City = "New York"     , Nickname = "Yankees"                               },
                new MlbTeamEntity { Code = "TB" , MlbLeagueId = "AL", City = "Tampa Bay"    , Nickname = "Rays"       ,  AlternativeCode = "TAM" },
                new MlbTeamEntity { Code = "TOR", MlbLeagueId = "AL", City = "Toronto"      , Nickname = "Blue Jays"                             },
                new MlbTeamEntity { Code = "CWS", MlbLeagueId = "AL", City = "Chicago"      , Nickname = "White Sox"  ,  AlternativeCode = "CHW" },
                new MlbTeamEntity { Code = "CLE", MlbLeagueId = "AL", City = "Cleveland"    , Nickname = "Indians"                               },
                new MlbTeamEntity { Code = "DET", MlbLeagueId = "AL", City = "Detriot"      , Nickname = "Tigers"                                },
                new MlbTeamEntity { Code = "KC" , MlbLeagueId = "AL", City = "Kansas City"  , Nickname = "Royals"                                },
                new MlbTeamEntity { Code = "MIN", MlbLeagueId = "AL", City = "Minnesota"    , Nickname = "Twins"                                 },
                new MlbTeamEntity { Code = "HOU", MlbLeagueId = "AL", City = "Houston"      , Nickname = "Astros"                                },
                new MlbTeamEntity { Code = "LAA", MlbLeagueId = "AL", City = "Los Angeles"  , Nickname = "Angels"                                },
                new MlbTeamEntity { Code = "OAK", MlbLeagueId = "AL", City = "Oakland"      , Nickname = "Athletics"                             },
                new MlbTeamEntity { Code = "SEA", MlbLeagueId = "AL", City = "Seattle"      , Nickname = "Mariners"                              },
                new MlbTeamEntity { Code = "TEX", MlbLeagueId = "AL", City = "Texas"        , Nickname = "Rangers"                               },
                new MlbTeamEntity { Code = "ATL", MlbLeagueId = "NL", City = "Atlanta"      , Nickname = "Braves"                                },
                new MlbTeamEntity { Code = "MIA", MlbLeagueId = "NL", City = "Miami"        , Nickname = "Marlins"                               },
                new MlbTeamEntity { Code = "NYM", MlbLeagueId = "NL", City = "New York"     , Nickname = "Mets"                                  },
                new MlbTeamEntity { Code = "PHI", MlbLeagueId = "NL", City = "Philadelphia" , Nickname = "Phillies"                              },
                new MlbTeamEntity { Code = "WAS", MlbLeagueId = "NL", City = "Washington"   , Nickname = "Nationals"                             },
                new MlbTeamEntity { Code = "CHC", MlbLeagueId = "NL", City = "Chicago"      , Nickname = "Cubs"                                  },
                new MlbTeamEntity { Code = "CIN", MlbLeagueId = "NL", City = "Cincinnati"   , Nickname = "Reds"                                  },
                new MlbTeamEntity { Code = "MIL", MlbLeagueId = "NL", City = "Milwaukee"    , Nickname = "Brewers"                               },
                new MlbTeamEntity { Code = "PIT", MlbLeagueId = "NL", City = "Pittsburgh"   , Nickname = "Pirates"                               },
                new MlbTeamEntity { Code = "STL", MlbLeagueId = "NL", City = "St. Louis"    , Nickname = "Cardinals"                             },
                new MlbTeamEntity { Code = "ARZ", MlbLeagueId = "NL", City = "Arizona"      , Nickname = "Diamondbacks", AlternativeCode = "ARI" },
                new MlbTeamEntity { Code = "COL", MlbLeagueId = "NL", City = "Colorado"     , Nickname = "Rockies"                               },
                new MlbTeamEntity { Code = "LAD", MlbLeagueId = "NL", City = "Los Angeles"  , Nickname = "Dodgers"     , AlternativeCode = "LA"  },
                new MlbTeamEntity { Code = "SD" , MlbLeagueId = "NL", City = "San Diego"    , Nickname = "Padres"                                },
                new MlbTeamEntity { Code = "SF" , MlbLeagueId = "NL", City = "San Francisco", Nickname = "Giants"                                }
            );
        }
        
        private static void BuildPitchingStatsModel(EntityTypeBuilder<PitchingStatsEntity> builder)
        {
            builder.HasKey(b => new { b.PlayerId, b.StatsType }).HasName("PitchingStats_PK");
            builder.HasOne(b => b.Player)
                .WithMany(p => p.PitchingStats)
                .HasForeignKey(b => b.PlayerId)
                .HasConstraintName("PitchingStats_Player_FK");
        }

        private static void BuildPlayerLeagueStatusModel(EntityTypeBuilder<PlayerLeagueStatusEntity> builder)
        {
            builder.HasKey(b => new { b.PlayerId, b.LeagueId }).HasName("LeagueStatus_PK");
            builder.HasOne(b => b.Player)
                .WithMany(p => p.LeagueStatuses)
                .HasForeignKey(b => b.PlayerId)
                .HasConstraintName("LeagueStatus_Player_FK");
        }

        private static void BuildPlayerModel(EntityTypeBuilder<PlayerEntity> builder) 
        {
            builder.HasKey(b => b.Id).HasName("Player_PK");
            builder.Property(b => b.Id).ValueGeneratedOnAdd();
            builder.Property(b => b.FirstName).HasMaxLength(20);
            builder.Property(b => b.LastName).HasMaxLength(20);
            builder.HasAlternateKey(b => new { b.BhqId, b.Type }).HasName("Player_Bhq_AK");
            builder.HasOne(b => b.MlbTeam)
                .WithMany(t => t.Players)
                .HasForeignKey(b => b.Team)
                .HasConstraintName("Player_MlbTeam_FK");
        }

        private static void BuildPlayerPositionModel(EntityTypeBuilder<PlayerPositionEntity> builder)
        {
            builder.HasKey(pp => new { pp.PlayerId, pp.PositionCode });  
            builder.HasOne(pp => pp.Player)
                .WithMany(p => p.Positions)
                .HasForeignKey(pp => pp.PlayerId)
                .HasConstraintName("PlayerPosition_Player_FK");  
            builder.HasOne(pp => pp.Position)
                .WithMany(p => p.Players)
                .HasForeignKey(pp => pp.PositionCode)
                .HasConstraintName("PlayerPosition_Position_FK");  
        }

        private static void BuildPositionModel(EntityTypeBuilder<PositionEntity> builder)  
        {
            builder.HasKey(b => b.Code).HasName("Position_PK");
            builder.Property(b => b.Code).HasMaxLength(3);
            builder.Property(b => b.FullName).HasMaxLength(20);
            builder.HasIndex(b => b.SortOrder).IsUnique();
            builder.HasData(
                new PositionEntity { Code = ""  , FullName = "Unknown"          , PlayerType = PlayerType.U, SortOrder = int.MaxValue },
                new PositionEntity { Code = "C" , FullName = "Catcher"          , PlayerType = PlayerType.B, SortOrder = 0            },
                new PositionEntity { Code = "1B", FullName = "First Baseman"    , PlayerType = PlayerType.B, SortOrder = 1            },
                new PositionEntity { Code = "2B", FullName = "Second Baseman"   , PlayerType = PlayerType.B, SortOrder = 2            },
                new PositionEntity { Code = "3B", FullName = "Third Baseman"    , PlayerType = PlayerType.B, SortOrder = 3            },
                new PositionEntity { Code = "SS", FullName = "Shortstop"        , PlayerType = PlayerType.B, SortOrder = 4            },
                new PositionEntity { Code = "IF", FullName = "Infielder"        , PlayerType = PlayerType.B, SortOrder = 5            },
                new PositionEntity { Code = "LF", FullName = "Left Fielder"     , PlayerType = PlayerType.B, SortOrder = 6            },
                new PositionEntity { Code = "CF", FullName = "Center Feilder"   , PlayerType = PlayerType.B, SortOrder = 7            },
                new PositionEntity { Code = "RF", FullName = "Right Fielder"    , PlayerType = PlayerType.B, SortOrder = 8            },
                new PositionEntity { Code = "OF", FullName = "Outfielder"       , PlayerType = PlayerType.B, SortOrder = 9            },
                new PositionEntity { Code = "DH", FullName = "Designated Hitter", PlayerType = PlayerType.B, SortOrder = 10           },
                new PositionEntity { Code = "SP", FullName = "Starting Pitcher" , PlayerType = PlayerType.B, SortOrder = 11           },
                new PositionEntity { Code = "RP", FullName = "Relief Pitcher"   , PlayerType = PlayerType.B, SortOrder = 12           },
                new PositionEntity { Code = "P" , FullName = "Pitcher"          , PlayerType = PlayerType.B, SortOrder = 13           }
            );
        }

        private async Task SaveAndCommit()
        {
            await SaveChangesAsync();
            await _transaction.CommitAsync();
        }
    }
}