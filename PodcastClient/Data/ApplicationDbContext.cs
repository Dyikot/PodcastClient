using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PodcastClient.Data
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Podcast> Podcasts { get; set; }
		public DbSet<Episode> Episodes { get; set; }
		public DbSet<UserEpisode> UserEpisodes { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite("Data Source=test.db");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>(ConfigureUser);
			modelBuilder.Entity<Podcast>(ConfigurePodcast);
			modelBuilder.Entity<Episode>(ConfigureEpisode);
			modelBuilder.Entity<UserEpisode>(ConfigureUserEpisode);
		}

		private void ConfigureUser(EntityTypeBuilder<User> builder)
		{
			builder.HasIndex(u => u.UserName).IsUnique();
		}

		private void ConfigurePodcast(EntityTypeBuilder<Podcast> builder)
		{
			builder.HasIndex(p => p.Rss).IsUnique();
			builder.HasIndex(p => p.Title);
		}

		private void ConfigureEpisode(EntityTypeBuilder<Episode> builder)
		{
			builder.HasKey(e => new { e.EpisodeNumber, e.PodcastId });
		}

		private void ConfigureUserEpisode(EntityTypeBuilder<UserEpisode> builder)
		{
			builder.HasKey(ep => new { ep.UserId, ep.EpisodeNumber, ep.PodcastId });
			builder.HasOne(ep => ep.Episode)
				   .WithMany()
				   .HasForeignKey(ep => new { ep.EpisodeNumber, ep.PodcastId })
				   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}
