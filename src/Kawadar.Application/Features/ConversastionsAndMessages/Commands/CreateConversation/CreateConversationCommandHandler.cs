using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Conversations;
using Kawadar.Domain.Conversations.Events;
using Kawadar.Domain.Conversations.Messages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Commands.CreateConversation;


public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, Result<Guid>>
{
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;

  private readonly IJobsRepository _jobsRepository;
  private readonly IProposalsRepository _proposalsRepository;
  private readonly IConversationsRepository _conversationsRepository;

  private readonly IUnitOfWork _unitOfWork;
  private readonly IIdentityService _identityService;

  private readonly ILogger<CreateConversationCommandHandler> _logger;

  public CreateConversationCommandHandler(IUser user, IUsersRepository usersRepository, IJobsRepository jobsRepository, IProposalsRepository proposalsRepository, IConversationsRepository conversationsRepository, IUnitOfWork unitOfWork, ILogger<CreateConversationCommandHandler> logger, IIdentityService identityService)
  {
    _user = user;
    _usersRepository = usersRepository;
    _jobsRepository = jobsRepository;
    _proposalsRepository = proposalsRepository;
    _conversationsRepository = conversationsRepository;
    _unitOfWork = unitOfWork;
    _logger = logger;
    _identityService = identityService;
  }
  public async Task<Result<Guid>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;

    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;
    // get sender and receiver user profiles to ensure they exist and to use their data in the conversation creation if needed
    var senderUserResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (senderUserResult.IsError) return senderUserResult.Errors;

    var receiverUserResult = await _identityService.GetUserByUserNameAsync(request.ReceiverUserName);
    if (receiverUserResult.IsError) return receiverUserResult.Errors;
    var receiverUserProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(receiverUserResult.Value.Id);
    if (receiverUserProfileResult.IsError) return receiverUserProfileResult.Errors;
    var receiverUserProfile = receiverUserProfileResult.Value;


    var proposalResult = await _proposalsRepository.GetByIdAsync(request.ProposalId, cancellationToken);
    if (proposalResult.IsError) return proposalResult.Errors;
    var proposal = proposalResult.Value;

    var jobResult = await _jobsRepository.GetJobByIdAsync(proposal.JobId);
    if (jobResult.IsError) return jobResult.Errors;
    var job = jobResult.Value;

    var isSenderJobOwner = job.PostedById == senderUserResult.Value.Id;
    var isSenderFreelancer = proposal.FreelancerId == senderUserResult.Value.Id;
    var isReceiverJobOwner = job.PostedById == receiverUserProfile.Id;
    var isReceiverFreelancer = proposal.FreelancerId == receiverUserProfile.Id;

    if (!((isSenderJobOwner && isReceiverFreelancer) || (isSenderFreelancer && isReceiverJobOwner)))
    {
      return ApplicationErrors.UnauthorizedAccess;
    }

    var conversationExistsResult = await _conversationsRepository.ConversationExistsForProposalAsync(request.ProposalId, cancellationToken);
    if (conversationExistsResult.IsError) return conversationExistsResult.Errors;
    if (conversationExistsResult.Value) return ConversationErrors.ProposalConversationAlreadyExists;

    // create the conversation

    var ConversationResult = Conversation.Create(request.Title, senderUserResult.Value.Id, receiverUserProfile.Id, request.ProposalId, job.Id);
    if (ConversationResult.IsError) return ConversationResult.Errors;

    var conversation = ConversationResult.Value;

    // save the conversation first to prevent circular dependency with the message
    await _conversationsRepository.AddConversationAsync(conversation, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    // create the initial message
    var messageResult = Message.Create(conversation.Id, senderUserResult.Value.Id, request.InitialMessageContent, null, null);
    if (messageResult.IsError) return messageResult.Errors;
    var message = messageResult.Value;

    //set the conversation's last message to the initial message
    var lastMessageResult = conversation.SetLastMessage(message);
    if (lastMessageResult.IsError) return lastMessageResult.Errors;

    // save the message in the database and update conversation
    await _conversationsRepository.AddMessageAsync(message, cancellationToken);

    _logger.LogInformation("User {UserId} created a new conversation {ConversationId} with user {ReceiverUserId}", senderUserResult.Value.Id, conversation.Id, receiverUserProfile.Id);

    conversation.AddDomainEvent(new CreatedConversationEvent(conversation.Id, conversation.SenderUserId, conversation.ReceiverUserId, conversation.JobId));

    await _unitOfWork.SaveChangesAsync(cancellationToken);
    return conversation.Id;

  }
}