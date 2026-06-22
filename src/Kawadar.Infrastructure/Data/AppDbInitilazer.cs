
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.UserProfiles.Enums;
using Kawadar.Domain.Skills;
using Kawadar.Domain.Skills.FreelancerSkill;
using Kawadar.Domain.Skills.FreelancerSkill.Enum;
using Kawadar.Domain.Specilizations;
using Kawadar.Domain.Jobs;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Domain.Reviews;
using Kawadar.Domain.Reviews.Enums;
using Kawadar.Domain.Portfolios.Project;
using Kawadar.Domain.Portfolios.Items;
using Kawadar.Domain.Portfolios.Items.Enum;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using static MassTransit.ValidationResultExtensions;
namespace Kawadar.Infrastructure.Data;


public class ApplicationDbContextInitialiser(
    ILogger<ApplicationDbContextInitialiser> logger,
    AppDbContext context,
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IRecommendationService recommendationService,
    IFreelancerVectorStore freelancerVectorStore)
{
  private readonly ILogger<ApplicationDbContextInitialiser> _logger = logger;
  private readonly AppDbContext _context = context;
  private readonly UserManager<AppUser> _userManager = userManager;
  private readonly RoleManager<IdentityRole> _roleManager = roleManager;
  private readonly IRecommendationService _recommendationService = recommendationService;
  private readonly IFreelancerVectorStore _freelancerVectorStore = freelancerVectorStore;

  public async Task InitialiseAsync()
  {
    try
    {
      await _context.Database.EnsureCreatedAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while initialising the database.");
      throw;
    }
  }

  public async Task SeedAsync()
  {
    try
    {
      await TrySeedAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while seeding the database.");
      throw;
    }
  }

  public async Task TrySeedAsync()
  {
    // seeds role 
    _logger.LogInformation("Seeding default roles");

    if (!await _roleManager.RoleExistsAsync(DefaultRoles.User))
      await _roleManager.CreateAsync(new IdentityRole(DefaultRoles.User));

    if (!await _roleManager.RoleExistsAsync(DefaultRoles.Admin))
      await _roleManager.CreateAsync(new IdentityRole(DefaultRoles.Admin));

    // seeds default admin user
    _logger.LogInformation("Seeding default admin user");

    if (await _userManager.FindByEmailAsync("admin@kawadar.com") == null)
    {
      var admin = new AppUser
      {
        UserName = "masteradmin",
        Email = "admin@kawadar.com",
        EmailConfirmed = true,
      };

      var result = await _userManager.CreateAsync(admin, "Admin@123");
      if (result.Succeeded)
      {
        await _userManager.AddToRoleAsync(admin, DefaultRoles.Admin);
        foreach (var permission in Permissions.GetAllPermissions())
        {
          await _userManager.AddClaimAsync(admin, new Claim("Permission", permission));
        }
      }
    }

    //  seed default user
    _logger.LogInformation("Seeding default user");

    if (await _userManager.FindByEmailAsync("user@kawadar.com") == null)
    {
      var user = new AppUser
      {
        UserName = "defaultuser",
        Email = "user@kawadar.com",
        EmailConfirmed = true,
      };

      var result = await _userManager.CreateAsync(user, "User@123");
      if (result.Succeeded)
      {
        await _userManager.AddToRoleAsync(user, DefaultRoles.User);
      }
    }

    if (await _userManager.FindByEmailAsync("omartamer244@gmail.com") == null)
    {
      var coAdmin = new AppUser
      {
        UserName = "Omar244",
        Email = "omartamer244@gmail.com",
        EmailConfirmed = true,
      };
      var result = await _userManager.CreateAsync(coAdmin, "Omar@123");
      if (result.Succeeded)
      {
        await _userManager.AddToRoleAsync(coAdmin, DefaultRoles.Admin);
        foreach (var permission in Permissions.GetAllPermissions())
        {
          await _userManager.AddClaimAsync(coAdmin, new Claim("Permission", permission));
        }
      }
    }

    if (await _userManager.FindByEmailAsync("Omartamer2445@gmail.com") == null)
    {
      var coAdmin2 = new AppUser
      {
        UserName = "Omar24455",
        Email = "Omartamer2445@gmail.com",
        EmailConfirmed = true,
      };

      var result = await _userManager.CreateAsync(coAdmin2, "Omar@123");
      if (result.Succeeded)
      {
        await _userManager.AddToRoleAsync(coAdmin2, DefaultRoles.Admin);
        var Profile = UserProfile.create(coAdmin2.Id, "Omar", "Tamer", ProfileType.Admin);
        await _context.UserProfiles.AddAsync(Profile.Value);
        foreach (var permission in Permissions.GetAllPermissions())
        {
          await _userManager.AddClaimAsync(coAdmin2, new Claim("Permission", permission));
        }
        await _context.SaveChangesAsync();
      }
    }

    // Seed specialized admins with scoped permission categories
    await SeedSpecializedAdminsAsync();

    if (await _userManager.FindByEmailAsync("Ahmed12345@gmail.com") == null)
    {
      var client = new AppUser
      {
        UserName = "Ahmed123",
        Email = "Ahmed12345@gmail.com",
        EmailConfirmed = true
      };
      var result = await _userManager.CreateAsync(client, "Ahmed@123");
      if (result.Succeeded)
      {
        await _userManager.AddToRoleAsync(client, DefaultRoles.User);
        var Profile = UserProfile.create(client.Id, "Ahmed", "Tarek", ProfileType.Client);
        await _context.UserProfiles.AddAsync(Profile.Value);
        await _context.SaveChangesAsync();
      }
    }

    if (await _userManager.FindByEmailAsync("Youssef123@gmail.com") == null)
    {
      var client = new AppUser
      {
        UserName = "Youssef123",
        Email = "Youssef123@gmail.com",
        EmailConfirmed = true
      };
      var result = await _userManager.CreateAsync(client, "Youssef@123");
      if (result.Succeeded)
      {
        await _userManager.AddToRoleAsync(client, DefaultRoles.User);
        var Profile = UserProfile.create(client.Id, "Youssef", "Amin", ProfileType.Client);
        await _context.UserProfiles.AddAsync(Profile.Value);
        await _context.SaveChangesAsync();
      }
    }

    // seed skills
    await SeedSkillsAsync();

    // seed specializations
    await SeedSpecilizationsAsync();

    // seed random users (freelancers + clients)
    await SeedRandomUsersAsync();

    // seed wallets
    await SeedWalletsAsync();

    // seed reviews
    await SeedReviewsAsync();

    // seed jobs
    await SeedJobsAsync();

    // seed jobs to recommendation engine
    await SeedJobsToRecommendationEngineAsync();

    // seed users to recommendation engine
    await SeedUsersToRecommendationEngineAsync();
  }

  private async Task SeedSpecializedAdminsAsync()
  {
    // Each entry: (email, username, firstName, lastName, permissions[])
    var specializedAdmins = new List<(string Email, string Username, string FirstName, string LastName, string[] Permissions)>
    {
      // Policy & Violations Admin — manages policy violations and disputes
      (
        "violations.admin@kawadar.com", "violations_admin", "Violations", "Admin",
        new[]
        {
          Permissions.ViewUsers,
          Permissions.BanUsers,
          Permissions.ViewViolations,
          Permissions.SolveViolations,
          Permissions.ViewDisbutes,
          Permissions.SolveDisbutes,
          Permissions.ViewConversations,
          Permissions.ViewJobReports,
          Permissions.UpdateJobReports,
        }
      ),

      // Financial Admin — manages wallets, withdrawals and transactions
      (
        "finance.admin@kawadar.com", "finance_admin", "Finance", "Admin",
        new[]
        {
          Permissions.ViewWallets,
          Permissions.ViewWithdrawals,
          Permissions.ApproveWithdrawals,
          Permissions.RejectWithdrawals,
          Permissions.ViewTransactions,
          Permissions.ViewStatistics,
        }
      ),

      // User Management Admin — approves and manages user accounts
      (
        "users.admin@kawadar.com", "users_admin", "Users", "Admin",
        new[]
        {
          Permissions.ViewUsers,
          Permissions.EditUsers,
          Permissions.DeleteUsers,
          Permissions.ApproveUsers,
          Permissions.BanUsers,
          Permissions.ViewStatistics,
        }
      ),

      // Content Moderation Admin — moderates jobs, reports and badges
      (
        "content.admin@kawadar.com", "content_admin", "Content", "Admin",
        new[]
        {
          Permissions.ViewUsers,
          Permissions.DeleteJobs,
          Permissions.ViewJobReports,
          Permissions.UpdateJobReports,
          Permissions.ViewBadges,
          Permissions.CreateBadges,
          Permissions.EditBadges,
          Permissions.DeleteBadges,
          Permissions.ViewProposals,
        }
      ),

      // Analytics Admin — read-only access to platform statistics
      (
        "analytics.admin@kawadar.com", "analytics_admin", "Analytics", "Admin",
        new[]
        {
          Permissions.ViewUsers,
          Permissions.ViewStatistics,
          Permissions.ViewWallets,
          Permissions.ViewTransactions,
          Permissions.ViewProposals,
          Permissions.ViewJobReports,
        }
      ),
    };

    foreach (var (email, username, firstName, lastName, permissions) in specializedAdmins)
    {
      if (await _userManager.FindByEmailAsync(email) != null) continue;

      _logger.LogInformation("Seeding specialized admin: {Email}", email);

      var adminUser = new AppUser
      {
        UserName = username,
        Email = email,
        EmailConfirmed = true,
      };

      var result = await _userManager.CreateAsync(adminUser, "Admin@123");
      if (!result.Succeeded) continue;

      await _userManager.AddToRoleAsync(adminUser, DefaultRoles.Admin);

      foreach (var permission in permissions)
        await _userManager.AddClaimAsync(adminUser, new Claim("Permission", permission));

      var profile = UserProfile.create(adminUser.Id, firstName, lastName, ProfileType.Admin);
      if (profile.IsSuccess)
        await _context.UserProfiles.AddAsync(profile.Value);

      await _context.SaveChangesAsync();
    }
  }

  private async Task SeedSkillsAsync()
  {
    if (await _context.Skills.AnyAsync()) return;

    _logger.LogInformation("Seeding skills...");

    // Business logic uses UserProfile ID, not Identity user ID
    var adminUser = await _userManager.FindByEmailAsync("Omartamer2445@gmail.com")
                 ?? await _userManager.FindByEmailAsync("admin@kawadar.com");

    var adminProfile = adminUser is not null
      ? await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == adminUser.Id)
      : null;

    var createdBy = adminProfile?.Id ?? Guid.Empty;

    var skillNames = new List<string>
    {
      // Web Development
      "HTML", "CSS", "JavaScript", "TypeScript", "React.js", "Next.js", "Vue.js",
      "Angular", "Svelte", "Tailwind CSS", "Bootstrap", "SASS/SCSS",
      // Backend
      "Node.js", "ASP.NET Core", "Django", "FastAPI", "Laravel", "Ruby on Rails",
      "Spring Boot", "Express.js", "NestJS", "GraphQL", "REST API Design",
      // Mobile
      "Flutter", "React Native", "Swift", "Kotlin", "Dart", "Android Development",
      "iOS Development", "Xamarin",
      // Databases
      "PostgreSQL", "MySQL", "Microsoft SQL Server", "MongoDB", "Redis",
      "SQLite", "Oracle DB", "Cassandra", "Elasticsearch", "Firebase Firestore",
      // DevOps & Cloud
      "Docker", "Kubernetes", "CI/CD Pipelines", "GitHub Actions", "Azure DevOps",
      "AWS", "Google Cloud Platform", "Azure", "Terraform", "Ansible", "Linux Administration",
      "Nginx", "Apache",
      // Programming Languages
      "Python", "Java", "C#", "C++", "Go", "Rust", "PHP", "Ruby", "Scala",
      "Kotlin", "Swift", "R", "MATLAB",
      // Data & AI
      "Machine Learning", "Deep Learning", "TensorFlow", "PyTorch", "Scikit-learn",
      "Pandas", "NumPy", "Data Analysis", "Data Visualization", "Power BI",
      "Tableau", "Apache Spark", "Hadoop", "Natural Language Processing",
      "Computer Vision", "LangChain", "OpenAI API", "Hugging Face",
      // UI/UX Design
      "Figma", "Adobe XD", "Sketch", "UI Design", "UX Research",
      "Wireframing", "Prototyping", "Design Systems", "Adobe Photoshop",
      "Adobe Illustrator", "Motion Design", "3D Modeling",
      // Cybersecurity
      "Penetration Testing", "Network Security", "OWASP", "Ethical Hacking",
      "Cryptography", "SIEM", "SOC Analysis", "Vulnerability Assessment",
      // Blockchain
      "Solidity", "Smart Contracts", "Ethereum", "Web3.js", "DeFi Development",
      "NFT Development",
      // Testing
      "Unit Testing", "Integration Testing", "Selenium", "Cypress", "Jest",
      "xUnit", "Playwright", "Load Testing",
      // Game Development
      "Unity", "Unreal Engine", "C++ Game Dev", "Game Design",
      // Other Technical
      "Microservices Architecture", "System Design", "RabbitMQ", "Apache Kafka",
      "SignalR", "WebSockets", "gRPC", "OAuth2 / OpenID Connect",
      "Search Engine Optimization", "WordPress", "Shopify", "Magento",
      // Soft / Business
      "Technical Writing", "Project Management", "Agile / Scrum", "Git",
      "Code Review", "Software Architecture"
    };

    var skills = skillNames
      .Distinct(StringComparer.OrdinalIgnoreCase)
      .Take(200)
      .Select(name => Skill.Create(name, true, createdBy))
      .Where(r => r.IsSuccess)
      .Select(r => r.Value)
      .ToList();

    await _context.Skills.AddRangeAsync(skills);
    await _context.SaveChangesAsync();

    _logger.LogInformation("Seeded {Count} skills.", skills.Count);
  }

  private async Task SeedSpecilizationsAsync()
  {
    if (await _context.Specilizations.AnyAsync()) return;

    _logger.LogInformation("Seeding specializations...");

    var specilizationNames = new List<string>
    {
      "Web Development",
      "Mobile App Development",
      "UI/UX Design",
      "Data Science & Analytics",
      "Artificial Intelligence & Machine Learning",
      "DevOps & Cloud Engineering",
      "Cybersecurity",
      "Blockchain & Web3",
      "Game Development",
      "Backend Development",
      "Frontend Development",
      "Full Stack Development",
      "Quality Assurance & Testing",
      "Database Administration",
      "Embedded Systems & IoT",
      "Digital Marketing & SEO",
      "Content Writing & Copywriting",
      "Graphic Design",
      "Video & Animation",
      "IT Support & Networking"
    };

    var specilizations = specilizationNames
      .Select(name => Specilization.Create(name, true))
      .Where(r => r.IsSuccess)
      .Select(r => r.Value)
      .ToList();

    await _context.Specilizations.AddRangeAsync(specilizations);
    await _context.SaveChangesAsync();

    _logger.LogInformation("Seeded {Count} specializations.", specilizations.Count);
  }

  private async Task SeedJobsAsync()
  {
    if (await _context.Jobs.AnyAsync()) return;

    _logger.LogInformation("Seeding ~100 random Arabic jobs...");

    var clients = await _context.UserProfiles
      .Where(p => p.ProfileType == ProfileType.Client)
      .ToListAsync();

    var specs = await _context.Specilizations.ToListAsync();
    var allSkills = await _context.Skills.ToListAsync();

    if (!clients.Any() || !specs.Any() || !allSkills.Any()) return;

    var rnd = new Random(42);

    var jobAdjectives = new[] { "مطلوب", "نبحث عن", "فرصة لـ", "مشروع جديد لـ" };
    var jobRoles = new[] { "مطور ويب", "مصمم واجهات", "مطور تطبيقات", "مهندس بيانات", "مسوق إلكتروني", "كاتب محتوى", "خبير أمن سيبراني", "مستشار قانوني", "محاسب", "مترجم", "محرر فيديو", "مساعد افتراضي" };
    var jobScopes = new[] { "لمشروع ناشئ", "لشركة تقنية", "لتطبيق جوال", "لمنصة تجارة إلكترونية", "لعمل عن بعد", "بعقد حر", "لمشروع طويل الأمد", "لمهمة سريعة" };
    var jobReqs = new[] { "بخبرة لا تقل عن سنتين.", "مطلوب مهارات تواصل ممتازة.", "العمل عن بعد بالكامل.", "الالتزام بالمواعيد النهائية.", "القدرة على العمل ضمن فريق.", "يجب إرفاق سابقة الأعمال." };

    var jobsToAdd = new List<Job>();

    for (int i = 0; i < 100; i++)
    {
      var client = clients[rnd.Next(clients.Count)];
      var spec = specs[rnd.Next(specs.Count)];

      var title = $"{jobAdjectives[rnd.Next(jobAdjectives.Length)]} {jobRoles[rnd.Next(jobRoles.Length)]} {jobScopes[rnd.Next(jobScopes.Length)]}";
      var description = $"نحن {jobScopes[rnd.Next(jobScopes.Length)]} {jobAdjectives[rnd.Next(jobAdjectives.Length)]} {jobRoles[rnd.Next(jobRoles.Length)]} ذو كفاءة عالية. {jobReqs[rnd.Next(jobReqs.Length)]} التفاصيل سيتم مناقشتها لاحقاً.";

      var jobType = (JobType)rnd.Next(1, 3);
      var budget = (BudgetRange)rnd.Next(1, 5); 
      var rate = (HourlyRateRange)rnd.Next(1, 5);
      var expLevel = (JobExperienceLevel)rnd.Next(1, 4);
      var duration = rnd.Next(7, 90);

      var slugResult = Job.GenerateSlug(title);
      var slug = slugResult.IsSuccess ? slugResult.Value : $"job-{Guid.NewGuid().ToString().Substring(0, 8)}";

      var numSkills = rnd.Next(2, 6);
      var jobSkills = allSkills.OrderBy(x => rnd.Next()).Take(numSkills).ToList();

      var jobResult = Job.Create(
        client.Id,
        spec.Id,
        title,
        description,
        jobType,
        budget,
        rate,
        duration,
        expLevel,
        slug,
        [], 
        jobSkills, 
        []  
      );

      if (jobResult.IsSuccess)
      {
        jobsToAdd.Add(jobResult.Value);
      }
    }

    if (jobsToAdd.Any())
    {
      await _context.Jobs.AddRangeAsync(jobsToAdd);
      await _context.SaveChangesAsync();
      _logger.LogInformation("Seeded {Count} jobs.", jobsToAdd.Count);
    }
  }

  private async Task SeedReviewsAsync()
  {
    if (await _context.Reviews.AnyAsync()) return;

    _logger.LogInformation("Seeding reviews...");

    var seededEmails = new[]
    {
      "khalid.dev@kawadar.com", "sara.design@kawadar.com", "mohammed.ai@kawadar.com",
      "nora.mobile@kawadar.com", "faisal.devops@kawadar.com", "omar.backend@kawadar.com",
      "lina.data@kawadar.com", "yazeed.security@kawadar.com", "ali.fullstack@kawadar.com",
      "reem.qa@kawadar.com"
    };

    var freelancers = new List<UserProfile>();
    foreach (var email in seededEmails)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user != null)
      {
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id && p.ProfileType == ProfileType.Freelancer);
        if (profile != null) freelancers.Add(profile);
      }
    }

    var client = await _context.UserProfiles
      .FirstOrDefaultAsync(p => p.ProfileType == ProfileType.Client);

    if (client == null || !freelancers.Any()) return;

    var spec = await _context.Specilizations.FirstOrDefaultAsync();
    if (spec == null) return;

    // Ensure we have a job to attach reviews to
    var job = await _context.Jobs.FirstOrDefaultAsync(j => j.PostedById == client.Id);
    if (job == null)
    {
      var jobSlugResult = Job.GenerateSlug("Dummy Job for Reviews");
      string slug = jobSlugResult.IsSuccess ? jobSlugResult.Value : "dummy-job-reviews";

      var jobResult = Job.Create(
        client.Id,
        spec.Id,
        "Dummy Job for Reviews",
        "This is a dummy job created to hold reviews.",
        (JobType)1,
        (BudgetRange)1,
        (HourlyRateRange)1,
        30,
        (JobExperienceLevel)1,
        slug,
        [],
        [],
        []
      );

      if (jobResult.IsSuccess)
      {
        job = jobResult.Value;
        await _context.Jobs.AddAsync(job);
        await _context.SaveChangesAsync();
      }
      else
      {
        _logger.LogWarning("Failed to create dummy job for reviews.");
        return;
      }
    }

    var rnd = new Random(42);
    var comments = new[]
    {
      "Excellent work, highly recommended!",
      "Great communication and fast delivery.",
      "Good quality, but slightly delayed.",
      "Very professional and skilled.",
      "Exceeded my expectations."
    };

    foreach (var freelancer in freelancers)
    {
      var rating = rnd.Next(3, 6); // 4 or 5
      var comment = comments[rnd.Next(comments.Length)];

      var reviewResult = Review.Create(
        job.Id,
        client.Id,
        freelancer.Id,
        ReviewType.ClientFreelancer,
        rating,
        comment
      );

      if (reviewResult.IsSuccess)
      {
        await _context.Reviews.AddAsync(reviewResult.Value);
      }
    }

    await _context.SaveChangesAsync();
    _logger.LogInformation("Seeded reviews for {Count} freelancers.", freelancers.Count);
  }

  private async Task SeedWalletsAsync()
  {
    _logger.LogInformation("Seeding wallets...");

    var profilesWithoutWallets = await _context.UserProfiles
      .Where(p => !_context.Wallets.Any(w => w.UserId == p.Id))
      .ToListAsync();

    if (!profilesWithoutWallets.Any())
    {
      return;
    }

    foreach (var profile in profilesWithoutWallets)
    {
      var walletResult = Wallet.Create(profile.Id);
      if (walletResult.IsSuccess)
      {
        await _context.Wallets.AddAsync(walletResult.Value);
      }
    }

    await _context.SaveChangesAsync();
    _logger.LogInformation("Seeded {Count} wallets.", profilesWithoutWallets.Count);
  }

  private async Task SeedRandomUsersAsync()
  {
    // Skip if enough users already seeded
    if (await _context.UserProfiles.CountAsync(p => p.ProfileType == ProfileType.Freelancer) > 25) return;

    _logger.LogInformation("Seeding random users...");

    // Load seeded skills and specializations for assignment
    var allSkills = await _context.Skills.ToListAsync();
    var allSpecilizations = await _context.Specilizations.ToListAsync();

    if (!allSkills.Any() || !allSpecilizations.Any())
    {
      _logger.LogWarning("Skills or Specializations not seeded yet. Skipping random users.");
      return;
    }

    var rnd = new Random(42); // fixed seed for reproducibility

    // Mock profile pictures from randomuser.me (real HTTP-accessible URLs)
    var malePics = new[]
    {
      "https://randomuser.me/api/portraits/men/1.jpg",
      "https://randomuser.me/api/portraits/men/2.jpg",
      "https://randomuser.me/api/portraits/men/3.jpg",
      "https://randomuser.me/api/portraits/men/4.jpg",
      "https://randomuser.me/api/portraits/men/5.jpg",
      "https://randomuser.me/api/portraits/men/6.jpg",
      "https://randomuser.me/api/portraits/men/7.jpg",
    };
    var femalePics = new[]
    {
      "https://randomuser.me/api/portraits/women/1.jpg",
      "https://randomuser.me/api/portraits/women/2.jpg",
      "https://randomuser.me/api/portraits/women/3.jpg",
    };

    // ── Freelancer seed data ──────────────────────────────────
    var freelancers = new[]
    {
      new
      {
        Email = "khalid.dev@kawadar.com", Username = "khalid_dev",
        FirstName = "Khaled", LastName = "Mansour",
        Title = "Full Stack Web Developer",
        Bio = "مطور ويب متكامل بخبرة تزيد عن خمس سنوات في بناء تطبيقات الويب باستخدام React و ASP.NET Core. أحب حل المشكلات المعقدة وتقديم تجارب مستخدم سلسة واحترافية.",
        Phone = "+201012345678", PicUrl = malePics[0],
        Experience = ExperienceYear.FiveToTenYears,
        SpecializationIndex = 0, // Web Development
        SkillIndices = new[] { 0, 1, 2, 3, 4, 5 } // HTML, CSS, JS, TS, React, Next
      },
      new
      {
        Email = "sara.design@kawadar.com", Username = "sara_design",
        FirstName = "Sara", LastName = "Hassan",
        Title = "Senior UI/UX Designer",
        Bio = "مصممة واجهات مستخدم شغوفة بتحويل الأفكار إلى تجارب بصرية جذابة. أعمل بـ Figma وAdobe XD وأحرص على أن يكون كل تصميم سهل الاستخدام وجميل في آنٍ واحد.",
        Phone = "+201198765432", PicUrl = femalePics[0],
        Experience = ExperienceYear.ThreeToFiveYears,
        SpecializationIndex = 2, // UI/UX
        SkillIndices = new[] { 60, 61, 62, 63, 64 } // Figma, AdobeXD, Sketch, UI, UX
      },
      new
      {
        Email = "mohammed.ai@kawadar.com", Username = "mohammed_ai",
        FirstName = "Mohammed", LastName = "Ibrahim",
        Title = "AI & Data Engineer",
        Bio = "متخصص في تطوير نماذج التعلم الآلي ومعالجة اللغات الطبيعية. عملت على مشاريع ضخمة في مجال التوصيات والتنبؤ بالبيانات باستخدام Python وTensorFlow وPyTorch.",
        Phone = "+201123456789", PicUrl = malePics[1],
        Experience = ExperienceYear.ThreeToFiveYears,
        SpecializationIndex = 4, // AI & ML
        SkillIndices = new[] { 46, 47, 48, 49, 50 } // ML, DL, TF, PyTorch, sklearn
      },
      new
      {
        Email = "nora.mobile@kawadar.com", Username = "nora_mobile",
        FirstName = "Nour", LastName = "El-Sayed",
        Title = "Mobile App Developer",
        Bio = "أطور تطبيقات الجوال بتقنية Flutter منذ أربع سنوات. أهتم بجودة الكود وتجربة المستخدم وأعمل على أنظمة iOS وAndroid في آنٍ واحد بكفاءة عالية.",
        Phone = "+201234567890", PicUrl = femalePics[1],
        Experience = ExperienceYear.ThreeToFiveYears,
        SpecializationIndex = 1, // Mobile
        SkillIndices = new[] { 23, 24, 25, 26, 27 } // Flutter, RN, Swift, Kotlin, Dart
      },
      new
      {
        Email = "faisal.devops@kawadar.com", Username = "faisal_devops",
        FirstName = "Fares", LastName = "Mostafa",
        Title = "DevOps & Cloud Engineer",
        Bio = "مهندس بنية تحتية سحابية معتمد على AWS وAzure. أبني خطوط CI/CD وأدير كتل Kubernetes وأضمن استمرارية الخدمات بمعدل توفر يبلغ 99.9%.",
        Phone = "+201345678901", PicUrl = malePics[2],
        Experience = ExperienceYear.FiveToTenYears,
        SpecializationIndex = 5, // DevOps
        SkillIndices = new[] { 37, 38, 39, 40, 41, 42 } // Docker, K8s, CI/CD, GHA, AWS, GCP
      },
      new
      {
        Email = "omar.backend@kawadar.com", Username = "omar_backend",
        FirstName = "Omar", LastName = "Abdallah",
        Title = ".NET Backend Developer",
        Bio = "مطور خلفي بخبرة سبع سنوات في ASP.NET Core وSQL Server. أصمم APIs موثوقة وقابلة للتوسع، وأطبق أنماط Clean Architecture وDDD في مشاريعي.",
        Phone = "+201056789012", PicUrl = malePics[3],
        Experience = ExperienceYear.FiveToTenYears,
        SpecializationIndex = 9, // Backend
        SkillIndices = new[] { 13, 14, 15, 30, 31 } // Node, ASP.NET, Django, PG, MySQL
      },
      new
      {
        Email = "lina.data@kawadar.com", Username = "lina_data",
        FirstName = "Lina", LastName = "Kamal",
        Title = "Data Analyst & BI Developer",
        Bio = "أحوّل البيانات الخام إلى قصص بصرية مؤثرة باستخدام Power BI وTableau وPython. ساعدت أكثر من عشرين شركة على اتخاذ قرارات مبنية على البيانات.",
        Phone = "+201567890123", PicUrl = femalePics[2],
        Experience = ExperienceYear.ThreeToFiveYears,
        SpecializationIndex = 3, // Data Science
        SkillIndices = new[] { 51, 52, 53, 54, 55 } // Pandas, NumPy, Data Analysis, Viz, PowerBI
      },
      new
      {
        Email = "yazeed.security@kawadar.com", Username = "yazeed_sec",
        FirstName = "Yasser", LastName = "Salah",
        Title = "Cybersecurity Specialist",
        Bio = "خبير في اختبار الاختراق وتحليل الثغرات الأمنية. حاصل على شهادات CEH وOSCP، وعملت مع شركات كبرى لتأمين بنيتها التحتية من التهديدات الإلكترونية.",
        Phone = "+201678901234", PicUrl = malePics[4],
        Experience = ExperienceYear.FiveToTenYears,
        SpecializationIndex = 6, // Cybersecurity
        SkillIndices = new[] { 70, 71, 72, 73 } // PenTest, NetSec, OWASP, EthHack
      },
      new
      {
        Email = "ali.fullstack@kawadar.com", Username = "ali_fullstack",
        FirstName = "Ali", LastName = "Farouk",
        Title = "Full Stack Developer (React + Node)",
        Bio = "أبني تطبيقات ويب متكاملة من الفكرة حتى الإنتاج. أتقن React في الواجهة الأمامية وNode.js وExpress في الخلفية، مع إدارة قواعد بيانات SQL وNoSQL.",
        Phone = "+201789012345", PicUrl = malePics[5],
        Experience = ExperienceYear.OneToThreeYears,
        SpecializationIndex = 11, // Full Stack
        SkillIndices = new[] { 2, 3, 4, 13, 18, 30 } // JS, TS, React, Node, NestJS, PG
      },
      new
      {
        Email = "reem.qa@kawadar.com", Username = "reem_qa",
        FirstName = "Reem", LastName = "Tawfik",
        Title = "QA Engineer",
        Bio = "متخصصة في اختبار البرمجيات يدويًا وآليًا. أستخدم Cypress وPlaywright وSelenium لكتابة اختبارات شاملة تضمن جودة المنتج قبل كل إصدار.",
        Phone = "+201890123456", PicUrl = femalePics[2],
        Experience = ExperienceYear.OneToThreeYears,
        SpecializationIndex = 12, // QA
        SkillIndices = new[] { 81, 82, 83, 84, 85 } // Unit, Integration, Selenium, Cypress, Jest
      },
    };

    // ── Client seed data ──────────────────────────────────────
    var clients = new[]
    {
      new { Email = "tech.startup@kawadar.com", Username = "tech_startup", FirstName = "Tarek",  LastName = "Naguib",   PicUrl = malePics[6] },
      new { Email = "ecommerce.client@kawadar.com", Username = "ecom_client", FirstName = "Samy",   LastName = "Khalil",   PicUrl = malePics[5] },
      new { Email = "fintech.co@kawadar.com", Username = "fintech_co",  FirstName = "Nadia",  LastName = "Ragab",    PicUrl = malePics[4] },
      new { Email = "health.app@kawadar.com", Username = "health_app",  FirstName = "Yousef", LastName = "Hamdy",    PicUrl = malePics[3] },
      new { Email = "edu.platform@kawadar.com", Username = "edu_platform", FirstName = "Layla",  LastName = "Elmasry",  PicUrl = malePics[2] },
    };

    // ── Seed Clients ──────────────────────────────────────────
    foreach (var c in clients)
    {
      if (await _userManager.FindByEmailAsync(c.Email) != null) continue;

      var appUser = new AppUser { UserName = c.Username, Email = c.Email, EmailConfirmed = true };
      var result = await _userManager.CreateAsync(appUser, "User@123");
      if (!result.Succeeded) continue;

      await _userManager.AddToRoleAsync(appUser, DefaultRoles.User);

      var profile = UserProfile.create(appUser.Id, c.FirstName, c.LastName, ProfileType.Client);
      if (!profile.IsSuccess) continue;

      profile.Value.UpdateProfilePicture(c.PicUrl);
      await _context.UserProfiles.AddAsync(profile.Value);
      await _context.SaveChangesAsync();

      _logger.LogInformation("Seeded client: {Email}", c.Email);
    }

    // ── Seed Freelancers ─────────────────────────────────────
    foreach (var f in freelancers)
    {
      if (await _userManager.FindByEmailAsync(f.Email) != null) continue;

      var appUser = new AppUser { UserName = f.Username, Email = f.Email, EmailConfirmed = true };
      var result = await _userManager.CreateAsync(appUser, "User@123");
      if (!result.Succeeded) continue;

      await _userManager.AddToRoleAsync(appUser, DefaultRoles.User);

      // Create profile
      var profileResult = UserProfile.create(appUser.Id, f.FirstName, f.LastName, ProfileType.Freelancer);
      if (!profileResult.IsSuccess) continue;

      var profile = profileResult.Value;

      // Set profile details (triggers IsActivated = true when all required fields are set)
      profile.UpdateProfile(f.FirstName, f.LastName, f.Title, f.Bio, f.Experience, true, ProfileType.Freelancer, f.Phone);
      profile.UpdateProfilePicture(f.PicUrl);

      // Assign specialization
      if (f.SpecializationIndex < allSpecilizations.Count)
        profile.updateSpecilization(allSpecilizations[f.SpecializationIndex].Id);

      await _context.UserProfiles.AddAsync(profile);
      await _context.SaveChangesAsync();

      // Assign skills via FreelancerSkill join entity
      var freelancerSkills = f.SkillIndices
        .Where(i => i < allSkills.Count)
        .Select(i => FreelancerSkill.Create(profile.Id, allSkills[i].Id, SkillType.Predefined, null))
        .Where(r => r.IsSuccess)
        .Select(r => r.Value)
        .ToList();

      if (freelancerSkills.Any())
      {
        await _context.FreelacnerSkills.AddRangeAsync(freelancerSkills);
        await _context.SaveChangesAsync();
      }

      // Mark identity as verified and ensure profile is activated
      // Egyptian national ID: 14 digits, starts with birth century digit
      profile.UpdateIdentityInfo(
        identityNumber: "29206151234567",
        dateOfBirth: new DateOnly(1992, 6, 15),
        identityLocation: "Cairo, Egypt",
        identityName: profile.FullName);
      await _context.SaveChangesAsync();

      // Reload profile with skills + specialization for embedding
      var fullProfile = await _context.UserProfiles
        .Include(p => p.Skills)
        .Include(p => p.Specialization)
        .FirstOrDefaultAsync(p => p.Id == profile.Id);

      if (fullProfile is null) continue;


      // Register in Qdrant vector store
      try
      {
        await _freelancerVectorStore.AddFreelancerAsync(fullProfile);
        _logger.LogInformation("Registered freelancer {Name} in Qdrant.", fullProfile.FullName);
      }
      catch (Exception ex)
      {
        _logger.LogWarning(ex, "Failed to register freelancer {Name} in Qdrant.", fullProfile.FullName);
      }

      _logger.LogInformation("Seeded freelancer: {Email}", f.Email);

      // Add portfolio projects for the first 4 freelancers
      var freelancerIndex = Array.IndexOf(freelancers, f);
      if (freelancerIndex < 4)
        await SeedPortfolioAsync(profile.Id, allSpecilizations, freelancerIndex);
    }
  }

  private async Task SeedPortfolioAsync(Guid freelancerId, List<Specilization> specializations, int freelancerIndex)
  {
    // Mock project images from Unsplash (stable embed URLs)
    var projectImages = new[]
    {
      "https://images.unsplash.com/photo-1547658719-da2b51169166?w=800",
      "https://images.unsplash.com/photo-1531297484001-80022131f5a1?w=800",
      "https://images.unsplash.com/photo-1555949963-ff9fe0c870eb?w=800",
      "https://images.unsplash.com/photo-1504639725590-34d0984388bd?w=800",
      "https://images.unsplash.com/photo-1508739773434-c26b3d09e071?w=800",
      "https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=800",
      "https://images.unsplash.com/photo-1551650975-87deedd944c3?w=800",
      "https://images.unsplash.com/photo-1581291518857-4e27b48ff24e?w=800",
    };

    // Portfolio data per freelancer index
    var portfolioData = new[]
    {
      // Index 0 – Khalid (Full Stack Web Developer)
      new[]
      {
        new
        {
          Title = "E-Commerce Platform",
          Description = "A full-featured e-commerce platform built with React and ASP.NET Core, supporting product management, cart, checkout, and order tracking.",
          SpecIndex = 0,
          ImgUrl = projectImages[0],
          ProjectUrl = "https://github.com/demo/ecommerce",
          ItemTexts = new[] { "Built with React 18, Redux Toolkit, and ASP.NET Core 8.", "Integrated Stripe payment gateway and real-time order notifications via SignalR." },
          ItemImages = new[] { projectImages[1] },
        },
        new
        {
          Title = "Real-Time Chat App",
          Description = "A scalable real-time messaging application using SignalR and Redis pub/sub for horizontal scaling.",
          SpecIndex = 0,
          ImgUrl = projectImages[2],
          ProjectUrl = "https://github.com/demo/chat",
          ItemTexts = new[] { "Supports group chats, file sharing, and read receipts.", "Deployed on Azure with auto-scaling container groups." },
          ItemImages = new[] { projectImages[3] },
        },
      },
      // Index 1 – Sara (UI/UX Designer)
      new[]
      {
        new
        {
          Title = "Mobile Banking App UI",
          Description = "A modern mobile banking interface designed in Figma with a focus on accessibility and clarity, adhering to WCAG 2.1 standards.",
          SpecIndex = 2,
          ImgUrl = projectImages[4],
          ProjectUrl = "https://www.figma.com/demo/banking",
          ItemTexts = new[] { "Designed 40+ screens including onboarding, dashboard, transfers, and statements.", "Conducted usability testing with 15 participants and iterated based on feedback." },
          ItemImages = new[] { projectImages[5] },
        },
      },
      // Index 2 – Mohammed (AI & Data Engineer)
      new[]
      {
        new
        {
          Title = "Product Recommendation Engine",
          Description = "An ML-powered recommendation system using collaborative filtering and content-based approaches, deployed as a REST API.",
          SpecIndex = 4,
          ImgUrl = projectImages[6],
          ProjectUrl = "https://github.com/demo/recommender",
          ItemTexts = new[] { "Trained on 1M+ user interactions using PyTorch and served via FastAPI.", "Achieved 35% increase in click-through rate in A/B testing." },
          ItemImages = new[] { projectImages[7] },
        },
      },
      // Index 3 – Nora (Mobile App Developer)
      new[]
      {
        new
        {
          Title = "Health Tracking App",
          Description = "A cross-platform Flutter health app that integrates with wearables for real-time vitals monitoring, meal logging, and progress analytics.",
          SpecIndex = 1,
          ImgUrl = projectImages[0],
          ProjectUrl = "https://github.com/demo/healthapp",
          ItemTexts = new[] { "Single codebase deployed on iOS and Android with 4.8★ app store rating.", "Integrated Bluetooth LE for wearable device pairing." },
          ItemImages = new[] { projectImages[2] },
        },
      },
    };

    if (freelancerIndex >= portfolioData.Length) return;

    var projects = portfolioData[freelancerIndex];
    int displayOrder = 1;

    foreach (var p in projects)
    {
      var specId = p.SpecIndex < specializations.Count
        ? specializations[p.SpecIndex].Id
        : specializations[0].Id;

      var projectResult = PortfolioProject.Create(
        p.Title,
        p.Description,
        specId,
        freelancerId,
        p.ImgUrl,
        displayOrder++,
        p.ProjectUrl);

      if (!projectResult.IsSuccess) continue;

      var project = projectResult.Value;
      await _context.PortfolioProjects.AddAsync(project);
      await _context.SaveChangesAsync();

      // Add text items
      int itemOrder = 1;
      foreach (var text in p.ItemTexts)
      {
        var textItem = PortfolioItem.Create(ItemType.Text, text, itemOrder++, project.Id);
        if (textItem.IsSuccess)
          await _context.PortfolioItems.AddAsync(textItem.Value);
      }

      // Add image items
      foreach (var imgUrl in p.ItemImages)
      {
        var imgItem = PortfolioItem.Create(ItemType.Image, imgUrl, itemOrder++, project.Id);
        if (imgItem.IsSuccess)
          await _context.PortfolioItems.AddAsync(imgItem.Value);
      }

      // Add link item
      var linkItem = PortfolioItem.Create(ItemType.Link, p.ProjectUrl, itemOrder++, project.Id);
      if (linkItem.IsSuccess)
        await _context.PortfolioItems.AddAsync(linkItem.Value);

      await _context.SaveChangesAsync();
      _logger.LogInformation("Seeded portfolio project '{Title}' for freelancer {Id}.", p.Title, freelancerId);
    }
  }

  private async Task SeedJobsToRecommendationEngineAsync()
  {
    _logger.LogInformation("Seeding jobs to Gorse recommendation engine...");

    var jobs = await _context.Jobs
      .Include(j => j.Skills)
      .Include(j => j.Specilization)
      .ToListAsync();

    foreach (var job in jobs)
    {
      try
      {
        var labels = job.Skills.Select(s => s.Name.ToLower())
          .Concat(new[] { job.JobType.ToString().ToLower(), job.ExperienceLevel.ToString().ToLower() })
          .ToArray();

        var categories = job.Specilization != null ? new[] { job.Specilization.Name } : Array.Empty<string>();

        await _recommendationService.InsertItemAsync(
          job.Id.ToString(),
          categories: categories,
          labels: labels,
          comment: job.Title);

        _logger.LogInformation("Registered job {Title} in Gorse.", job.Title);
      }
      catch (Exception ex)
      {
        _logger.LogWarning(ex, "Failed to register job {Title} in Gorse.", job.Title);
      }
    }
  }

  private async Task SeedUsersToRecommendationEngineAsync()
  {
    

    var freelancers = await _context.UserProfiles
      .Where(p => p.ProfileType == ProfileType.Freelancer)
      .ToListAsync();

    foreach (var freelancer in freelancers)
    {
      try
      {
        await _recommendationService.InsertUserAsync(freelancer.Id, comment: freelancer.FullName);
        _logger.LogInformation("Registered freelancer {Name} in Gorse.", freelancer.FullName);
      }
      catch (Exception ex)
      {
        _logger.LogWarning(ex, "Failed to register freelancer {Name} in Gorse.", freelancer.FullName);
      }
    }
  }
}

public static class InitialiserExtensions
{
  public static async Task InitialiseDatabaseAsync(this WebApplication app)
  {
    using var scope = app.Services.CreateScope();

    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

    await initialiser.InitialiseAsync();

    await initialiser.SeedAsync();
  }
}