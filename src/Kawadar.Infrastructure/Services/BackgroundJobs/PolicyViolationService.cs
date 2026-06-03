using System.Text.Json;
using System.Text.Json.Serialization;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Violations;
using Kawadar.Domain.Violations.Enums;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace Kawadar.Infrastructure.Services.BackgroundJobs;

public class PolicyViolationService : IPolicyViolationService
{
  private readonly ILogger<PolicyViolationService> _logger;
  private readonly IUnitOfWork _unitOfWork;
  private readonly AppDbContext _dbContext;
  private readonly IViolationRepository _violationRepository;
  private readonly IChatCompletionService _chatCompletionService;
  private const string SystemPrompt = """
    You are a moderator for a freelancing platform where clients and freelancers 
    discuss work, projects, prices, deadlines, and deliverables.
    
    You will receive a single chat message. Analyze it and determine if it violates 
    platform rules.
    
    Violations are LIMITED to these 2 things ONLY:
    
    === VIOLATION 1: Sharing contact information ===
    The message contains or asks for:
    
    Phone numbers in ANY of these formats:
    - Numeric: 01012345678 / 010-123-45678 / +20 101 234 5678 / (010) 12345678
    - With country code: 00201012345678 / +201012345678
    - Spelled out in English: "zero one zero one two three four five"
    - Spelled out in Arabic: "صفر واحد صفر" / "زيرو واحد زيرو"
    - Written in Arabic digits: ٠١٠١٢٣٤٥٦٧٨
    - Partially hidden: "010 *** 5678" / "010xxxx678" / "my number starts with 010"
    - Arabizi: "rakamy 010" / "nemrty 010" / "raqami zero one"

    Email addresses in ANY of these formats:
    - Standard: ahmed@gmail.com / ahmed.ali@company.com
    - Dot/at tricks: "ahmed dot ali at gmail dot com"
    - Arabic tricks: "ايميلي ahmed تاء gmail نقطة com"
    - Spaces: "ahmed @ gmail . com"
    - Arabizi: "emaili howa ahmed at gmail"
    - Any variation of writing @ as "at" or "عند" or "آت"

    Social media and messaging:
    - Usernames: @ahmed_dev / "username is ahmed_dev" / "اسمي على انستا ahmed"
    - Facebook / Instagram / Twitter / TikTok / LinkedIn profiles
    - WhatsApp / واتساب / Telegram / تيليجرام / Signal / Viber / Skype
      WITH a number, username, or clear intent to move conversation outside
    - Arabizi: "wa5udny 3la whatsapp" / "kalmny 3la telegram"

    NOT a violation:
    - Mentioning WhatsApp/Telegram without sharing a number or username
    - Asking to share work files, designs, documents, or project links
    - Any work-related discussion (prices, deadlines, offers, feedback, questions)
    
    === VIOLATION 2: Offensive or inappropriate language ===
    The message contains insults, threats, or sexual content.
    
    Egyptian Examples Arabic (flag these):
    يلعن / يلعن ابوك / يلعن امك / يلعن دينك / كس / كس امك / كس اختك /
    متناك / ابن متناكة / ابن الشرموطة / ابن الكلب / ابن الوسخة /
    احا / اتنيك / انيكك / نيك / هنيك / هنيك امك /
    كلب / حيوان / خنزير / قرد / شرموط / شرموطة / عرص / معرص / خول /
    وسخ / قذر / زبالة / حقير / تافه /
    هضربك / هكسرك / هعمل فيك حاجة / هاخد حقي منك / دمك هيتسال /
    روح متناك / نيك امك / اللي خلفك / عيل متناك


    Arabic  Examples formal (flag these):
    ابن العاهرة / ابن القحبة / عاهرة / قحبة / زانية / زاني /
    لعين / ملعون / يلعنك / يلعن والديك /
    سأقتلك / سأضربك / سأؤذيك / سأنتقم منك /
    كافر / ابن الحرام / حرامي / لص / نصاب

    English Examples (flag these):
    fuck / fucking / fucker / bitch / asshole / bastard / motherfucker /
    son of a bitch / piece of shit / dickhead / cunt / whore / slut /
    idiot / moron / retard / stupid (when used as insult) /
    i will kill you / i will hurt you / i will find you /
    kill yourself / go to hell / i know where you live
    
    NOT a violation:
    - Formal or informal work conversation in any language or dialect
    - Expressing frustration about work without insults ("الشغل ده صعب" / "I'm frustrated")
    - Negotiating, complaining, or disagreeing professionally
    - Using the word "hell" or "damn" casually without targeting someone
    
    === IMPORTANT ===
    - When in doubt → the message is clean
    - Short messages with no clear violation = clean
    - Polite Arabic or Egyptian dialect = ALWAYS clean
    - Discussing offers, prices, work, deadlines = ALWAYS clean
    - A violation must be CLEAR and OBVIOUS, not assumed
    
    Respond ONLY with this exact JSON format, no extra text:
    {
      "detectedType": "contact_info" | "offensive_language" | "clean",
      "severity": "low" | "medium" | "high"
      "reason": "string"
      "isViolation": true | false
    }
    """;

  public PolicyViolationService(ILogger<PolicyViolationService> logger,
  IUnitOfWork unitOfWork, AppDbContext dbContext, IViolationRepository violationRepository
  , IChatCompletionService chatCompletionService
  )
  {
    _logger = logger;
    _unitOfWork = unitOfWork;
    _dbContext = dbContext;
    _violationRepository = violationRepository;
    _chatCompletionService = chatCompletionService;

  }
  public async Task ProcessPolicyViolationAsync()
  {
    _logger.LogInformation("Starting policy violation processing...");


    var settings = new OllamaPromptExecutionSettings
    {

      ExtensionData = new Dictionary<string, object> { { "think", false } }
    };


    // first get all messages in the last 3 minutes 
    var threeMinutesAgo = DateTime.UtcNow.AddMinutes(-3);
    var recentMessages = await _dbContext.Messages
        .Where(m => m.CreatedAt >= threeMinutesAgo)
        .ToListAsync();
    // check each message for policy violation using semantic kernel

    foreach (var message in recentMessages)
    {
      var history = new ChatHistory();
      history.AddSystemMessage(SystemPrompt);
      history.AddUserMessage(message.Content);
      var response = await _chatCompletionService.GetChatMessageContentAsync(
          history,
          executionSettings: settings,
          cancellationToken: default);
      var violationResult = JsonSerializer.Deserialize<OpenAIResponse>(response.Content!);

      if (violationResult != null && violationResult.IsViolation)
      {
        // save violation to database
        var violation = Violation.Create(message.SenderUserId, violationResult.Reason,
        violationResult.DetectedType == "contact_info" ? ViolationType.ContactSharing : ViolationType.EthicalBreach, message.Id, $"messages/{message.Id}", "messages");
        await _violationRepository.AddViolation(violation.Value);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Violation detected and saved for message {MessageId}: {Reason}", message.Id, violationResult.Reason);
      }
    }


    //second check job proposals in the last 3 minutes
    var recentProposals = await _dbContext.JobProposals
        .Where(p => p.CreatedAt >= threeMinutesAgo)
        .ToListAsync();

    foreach (var proposal in recentProposals)
    {
      var history = new ChatHistory();
      history.AddSystemMessage(SystemPrompt);
      history.AddUserMessage(proposal.CoverLetter);
      var response = await _chatCompletionService.GetChatMessageContentAsync(
          history,
          executionSettings: settings,
          cancellationToken: default);
      var violationResult = JsonSerializer.Deserialize<OpenAIResponse>(response.Content!);

      if (violationResult != null && violationResult.IsViolation)
      {
        // save violation to database
        var violation = Violation.Create(proposal.FreelancerId, violationResult.Reason,
        violationResult.DetectedType == "contact_info" ? ViolationType.ContactSharing : ViolationType.EthicalBreach, proposal.Id, $"proposals/{proposal.Id}", "job_proposals");
        await _violationRepository.AddViolation(violation.Value);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Violation detected and saved for proposal {ProposalId}: {Reason}", proposal.Id, violationResult.Reason);
      }
    }


    // third check jobs in the last 3 minutes

    var recentJobs = await _dbContext.Jobs
        .Where(j => j.CreatedAt >= threeMinutesAgo)
        .ToListAsync();

    foreach (var job in recentJobs)
    {
      var history = new ChatHistory();
      history.AddSystemMessage(SystemPrompt);
      history.AddUserMessage(job.Description);
      var response = await _chatCompletionService.GetChatMessageContentAsync(
          history,
          executionSettings: settings,
          cancellationToken: default);
      var violationResult = JsonSerializer.Deserialize<OpenAIResponse>(response.Content!);

      if (violationResult != null && violationResult.IsViolation)
      {
        // save violation to database
        var violation = Violation.Create(job.PostedById, violationResult.Reason,
        violationResult.DetectedType == "contact_info" ? ViolationType.ContactSharing : ViolationType.EthicalBreach, job.Id, $"jobs/{job.Id}", "jobs");
        await _violationRepository.AddViolation(violation.Value);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Violation detected and saved for job {JobId}: {Reason}", job.Id, violationResult.Reason);
      }
    }
    _logger.LogInformation("Finished processing policy violations.");

  }


  private class OpenAIResponse
  {
    [JsonPropertyName("detectedType")]
    public string DetectedType { get; set; } = "clean";
    [JsonPropertyName("severity")]
    public string Severity { get; set; } = "low";

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = "No violation detected.";
    [JsonPropertyName("isViolation")]
    public bool IsViolation { get; set; }
  }
}