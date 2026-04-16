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
  private readonly IConversationsRepository _conversationsRepository;

  private readonly IUnitOfWork _unitOfWork;
  private readonly IIdentityService _identityService;

  private readonly ILogger<CreateConversationCommandHandler> _logger;

  public CreateConversationCommandHandler(IUser user, IUsersRepository usersRepository, IJobsRepository jobsRepository, IConversationsRepository conversationsRepository, IUnitOfWork unitOfWork, ILogger<CreateConversationCommandHandler> logger, IIdentityService identityService)
  {
    _user = user;
    _usersRepository = usersRepository;
    _jobsRepository = jobsRepository;
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


    if (request.JobId is not null)
    {

      var jobResult = await _jobsRepository.GetJobByIdAsync(request.JobId.Value);
      if (jobResult.IsError) return jobResult.Errors;
      if (jobResult.Value.PostedById != senderUserResult.Value.Id) return ApplicationErrors.UnauthorizedAccess;

    }

    // create the conversation

    var ConversationResult = Conversation.Create(request.Title, senderUserResult.Value.Id, receiverUserProfile.Id, request.JobId);
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