//using DataAccess.Migrations;

//using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;
using StudentAutomation.Domain.Entities;



namespace StudentAutomation.Infrastructure.Persistence.Context
{
    public class DataContext : DbContext
    {


        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        // DbSets
        public DbSet<User> Users => Set<User>();
        public DbSet<OperationClaim> OperationClaims => Set<OperationClaim>();
        public DbSet<UserOperationClaim> UserOperationClaims => Set<UserOperationClaim>();

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<Grade> Grades => Set<Grade>();
        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<StudentFeedback> StudentFeedbacks => Set<StudentFeedback>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        // Global Query Filter içinde erişeceğimiz property




            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // =========================
                // TABLE NAMES
                // =========================
                modelBuilder.Entity<User>().ToTable("Users");
                modelBuilder.Entity<OperationClaim>().ToTable("OperationClaims");
                modelBuilder.Entity<UserOperationClaim>().ToTable("UserOperationClaims");

                modelBuilder.Entity<Student>().ToTable("Students");
                modelBuilder.Entity<Teacher>().ToTable("Teachers");
                modelBuilder.Entity<Course>().ToTable("Courses");
                modelBuilder.Entity<Enrollment>().ToTable("Enrollments");
                modelBuilder.Entity<Grade>().ToTable("Grades");
                modelBuilder.Entity<Attendance>().ToTable("Attendances");
            modelBuilder.Entity<StudentFeedback>().ToTable("StudentFeedbacks");

            modelBuilder.Entity<AuditLog>().ToTable("AuditLogs");

                // =========================
                // USER / CLAIMS
                // =========================
                modelBuilder.Entity<User>(e =>
                {
                    e.HasKey(x => x.Id);
                    e.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
                    e.Property(x => x.LastName).HasMaxLength(100).IsRequired();
                    e.Property(x => x.Email).HasMaxLength(256).IsRequired();
                    e.HasIndex(x => x.Email).IsUnique();
                    e.Property(x => x.Status).HasDefaultValue(true);
                });

                modelBuilder.Entity<OperationClaim>(e =>
                {
                    e.HasKey(x => x.Id);
                    e.Property(x => x.Name).HasMaxLength(128).IsRequired();
                    e.HasIndex(x => x.Name).IsUnique();
                });

                modelBuilder.Entity<UserOperationClaim>(e =>
                {
                    e.HasKey(x => x.Id);

                    e.HasOne(uoc => uoc.User)
                     .WithMany(u => u.UserOperationClaims)
                     .HasForeignKey(uoc => uoc.UserId)
                     .OnDelete(DeleteBehavior.Cascade);

                    e.HasOne(uoc => uoc.OperationClaim)
                     .WithMany(oc => oc.UserOperationClaims)
                     .HasForeignKey(uoc => uoc.OperationClaimId)
                     .OnDelete(DeleteBehavior.Cascade);

                    // aynı kullanıcıya aynı claim bir kez
                    e.HasIndex(x => new { x.UserId, x.OperationClaimId }).IsUnique();
                });

            // =========================
            // STUDENT
            // =========================
            modelBuilder.Entity<Student>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.StudentNumber)
                 .HasMaxLength(32)
                 .IsRequired();

                e.HasIndex(x => x.StudentNumber).IsUnique();

                e.Property(x => x.Department)
                 .HasMaxLength(128);

                e.Property(x => x.Status)
                 .HasDefaultValue(true);

                // Doğum tarihi: saf tarih (time zone yok)
                e.Property(x => x.BirthDate)
                 .HasColumnType("date"); // <-- timestamptz değil

                e.HasOne(x => x.User)
                 .WithMany() // User -> Student tek yön
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // =========================
            // TEACHER
            // =========================
            modelBuilder.Entity<Teacher>(e =>
                {
                    e.HasKey(x => x.Id);

                    e.Property(x => x.Title).HasMaxLength(32).HasDefaultValue("Lecturer");
                    e.Property(x => x.Department).HasMaxLength(128);
                    e.Property(x => x.Status).HasDefaultValue(true);

                    e.HasOne(x => x.User)
                     .WithMany()
                     .HasForeignKey(x => x.UserId)
                     .OnDelete(DeleteBehavior.Restrict);
                });

                // =========================
                // COURSE
                // =========================
                modelBuilder.Entity<Course>(e =>
                {
                    e.HasKey(x => x.Id);

                    e.Property(x => x.Code).HasMaxLength(16).IsRequired();
                    e.HasIndex(x => x.Code).IsUnique();

                    e.Property(x => x.Name).HasMaxLength(128).IsRequired();
                    e.Property(x => x.Credits).HasDefaultValue(3);
                    e.Property(x => x.IsActive).HasDefaultValue(true);

                    e.HasOne(x => x.Teacher)
                     .WithMany(t => t.Courses)
                     .HasForeignKey(x => x.TeacherId)
                     .OnDelete(DeleteBehavior.SetNull);
                });

            // =========================
            // ENROLLMENT (Many-to-Many with payload)
            // =========================
            modelBuilder.Entity<Enrollment>(e =>
            {
                // Composite PK
                e.HasKey(x => new { x.StudentId, x.CourseId });

                e.HasOne(x => x.Student)
                 .WithMany(s => s.Enrollments)
                 .HasForeignKey(x => x.StudentId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Course)
                 .WithMany(c => c.Enrollments)
                 .HasForeignKey(x => x.CourseId)
                 .OnDelete(DeleteBehavior.Cascade);

                // ⬇⬇⬇ BURAYI DEĞİŞTİR
                e.Property(x => x.EnrolledAt)
                 .HasColumnType("date")          // PostgreSQL'de DateOnly için doğru tip
                 .IsRequired(false)              // EnrolledAt nullable ise
                 .HasDefaultValueSql("CURRENT_DATE"); // (opsiyonel) DB default'u bugün
            });


            // =========================
            // GRADE
            // =========================
            modelBuilder.Entity<Grade>(e =>
            {
                e.HasKey(x => x.Id);

                // 0..100 aralığı (PostgreSQL)
                e.Property(x => x.Value).HasColumnType("numeric(5,2)");

                // 🔧 CHECK ifadesinde TIRNAK YOK ve sütun adları küçük/snake!
                e.HasCheckConstraint("ck_grades_value_0_100", "value >= 0 AND value <= 100");

                e.Property(x => x.Term).HasMaxLength(32);

                // İstersen DateTime yerine DateTimeOffset kullan + "timestamptz"
                // DateTime kullanacaksan da bu satır çalışır:
                e.Property(x => x.CreatedAt).HasColumnType("timestamptz");

                e.HasOne(x => x.Student)
                 .WithMany(s => s.Grades)
                 .HasForeignKey(x => x.StudentId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Course)
                 .WithMany(c => c.Grades)
                 .HasForeignKey(x => x.CourseId)
                 .OnDelete(DeleteBehavior.Cascade);

                // aynı öğrenci+ders+tip için tek kayıt
                e.HasIndex(x => new { x.StudentId, x.CourseId, x.Type }).IsUnique();
            });


            // =========================
            // ATTENDANCE
            // =========================
            modelBuilder.Entity<Attendance>(e =>
                {
                    e.HasKey(x => x.Id);

                    // DateOnly -> PostgreSQL 'date'
                    e.Property(x => x.Date).HasColumnType("date");
                    e.Property(x => x.Note).HasMaxLength(512);

                    e.HasOne(x => x.Student)
                     .WithMany(s => s.Attendances)
                     .HasForeignKey(x => x.StudentId)
                     .OnDelete(DeleteBehavior.Cascade);

                    e.HasOne(x => x.Course)
                     .WithMany(c => c.Attendances)
                     .HasForeignKey(x => x.CourseId)
                     .OnDelete(DeleteBehavior.Cascade);

                    // Aynı gün aynı ders için tek kayıt
                    e.HasIndex(x => new { x.StudentId, x.CourseId, x.Date }).IsUnique();
                });

                // =========================
                // AUDIT LOG (varsayılan Id PK kabul edilmiştir)
                // =========================
                modelBuilder.Entity<AuditLog>(e =>
                {
                    e.HasKey(x => x.Id);
                    // Alan adlarını bilmiyorum; sadece güvenli varsayılanlar:
                    // e.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
                    // e.Property(x => x.EntityName).HasMaxLength(128);
                    // e.Property(x => x.Action).HasMaxLength(64);
                    // e.HasIndex(x => x.CreatedAt);
                });
            modelBuilder.Entity<StudentFeedback>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Comment).HasMaxLength(2000).IsRequired();
                e.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
                e.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone");

                e.HasOne(x => x.Course).WithMany().HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Teacher).WithMany().HasForeignKey(x => x.TeacherId).OnDelete(DeleteBehavior.Restrict);

                // Sık aranan kombinasyonlar için index
                e.HasIndex(x => new { x.CourseId, x.StudentId });
                e.HasIndex(x => new { x.StudentId });
                e.HasIndex(x => new { x.TeacherId });
            });
          /*  modelBuilder.Entity<OperationClaim>().HasData(
       new OperationClaim { Id = 1, Name = "Admin" },
       new OperationClaim { Id = 2, Name = "Teacher" },
       new OperationClaim { Id = 3, Name = "Student" }
   );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Deneme",
                    LastName = "Deneme",
                    Email = "deneme@gmail.com",
                    PasswordHash = Convert.FromBase64String("gzpOoqpwh9gUnQU9s8r+8d82wvzOKFOpZByUwDCuk4lM/dn+iNzQbA4isBdS5EpyJ3xeYXN6ox3O7VsJlXLH5w=="),
                    PasswordSalt = Convert.FromBase64String("qYkF9cEJg27h4Egf13rRxZcASZgeL7MEJW1pOnLepmo4lm1N1q+W5MtnMSRDrBaqm6FIKmlMybbzA2xnDKwC1A=="),
                    Status = true
                }

    );

            modelBuilder.Entity<UserOperationClaim>().HasData(
                new UserOperationClaim { Id = 1, UserId = 1, OperationClaimId = 1 },
                new UserOperationClaim { Id = 2, UserId = 1, OperationClaimId = 2 },
                new UserOperationClaim { Id = 3, UserId = 1, OperationClaimId = 3 }
                
            );*/
        }
        }
   }

//dotnet ef migrations add InitialCreate -p StudentAutomation.Infrastructure -s StudentAutomation.WebAPI
//dotnet ef database update -p StudentAutomation.Infrastructure -s StudentAutomation.WebAPI
//update-database "InitialCreate"
//tree /f /a > project-structure.txt
//dotnet ef migrations add InitialCreate -p StudentAutomation.Infrastructure -s StudentAutomation.WebAPI