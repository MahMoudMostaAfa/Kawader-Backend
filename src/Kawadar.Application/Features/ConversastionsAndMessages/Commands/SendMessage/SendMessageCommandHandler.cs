using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Conversations.Events;
using Kawadar.Domain.Conversations.Messages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Commands.SendMessage;


public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result<MessageDto>>
{

  private readonly IUnitOfWork _unitOfWork;
  private readonly IConversationsRepository _conversationsRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly IUser _user;

  private readonly IStorageClient _storageClient;
  private readonly ILogger<SendMessageCommandHandler> _logger;
  public SendMessageCommandHandler(IConversationsRepository conversationsRepository, IUnitOfWork unitOfWork, IUsersRepository usersRepository, IUser user, IStorageClient storageClient, ILogger<SendMessageCommandHandler> logger)
  {
    _unitOfWork = unitOfWork;
    _conversationsRepository = conversationsRepository;
    _usersRepository = usersRepository;
    _user = user;
    _storageClient = storageClient;
    _logger = logger;
  }


  public async Task<Result<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
  {
    var userId = request.SenderId;
    if (userId is null) userId = _user.Id;

    if (userId is null) return ApplicationErrors.UnauthorizedAccess;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var conversationResult = await _conversationsRepository.GetConversationByIdAsync(request.conversationId);
    if (conversationResult.IsError) return conversationResult.Errors;
    var conversation = conversationResult.Value;

    // CHECK THAT USER IS A PARTICIPANT IN THE CONVERSATION
    if (conversation.ReceiverUserId != userProfile.Id && conversation.SenderUserId != userProfile.Id)
    {
      return ApplicationErrors.UnauthorizedAccess;
    }

    // upload attachments to storage and get their URLs
    var MessageFiles = new List<MessageFile>();
    foreach (var file in request.AttachmentFiles ?? [])
    {
      var fileUrlResult = await _storageClient.UploadFileAsync(file.OpenReadStream(), file.FileName, Containers.MessageAttachements, cancellationToken);
      if (fileUrlResult.IsError) return fileUrlResult.Errors;

      var fileInfo = new Kawadar.Domain.Common.ValueObjects.FileInfo()
      {
        FileName = file.FileName,
        FileUrl = fileUrlResult.Value,
        FileSizeInBytes = file.Length,
        MimeType = file.ContentType
      };

      var messageFileResult = MessageFile.Create(fileInfo);
      if (messageFileResult.IsError) return messageFileResult.Errors;
      MessageFiles.Add(messageFileResult.Value);
    }

    foreach (var link in request.AttachmentLinks ?? [])
    {
      var fileInfo = new Kawadar.Domain.Common.ValueObjects.FileInfo()
      {
        FileName = link,
        FileUrl = link,
        MimeType = "link"
      };

      var messageFileResult = MessageFile.Create(fileInfo);
      if (messageFileResult.IsError) return messageFileResult.Errors;
      MessageFiles.Add(messageFileResult.Value);
    }



    // check if the message is a reply to another message in the same conversation
    if (request.replyToMessageId != null)
    {

      var repliedMessageResult = await _conversationsRepository.GetMessageByIdAsync(request.replyToMessageId.Value);
      if (repliedMessageResult.IsError) return repliedMessageResult.Errors;
      var repliedMessage = repliedMessageResult.Value;

      if (repliedMessage.ConversationId != conversation.Id)
      {
        return Error.Validation("InvalidReply", "The message you are replying to does not belong to the same conversation.");
      }
    }



    // create message entity and save to database 
    var messageResult = Message.Create(conversation.Id, userProfile.Id, request.content, request.replyToMessageId, MessageFiles);
    if (messageResult.IsError) return messageResult.Errors;
    var message = messageResult.Value;

    await _conversationsRepository.AddMessageAsync(message);

    // get recipient user id
    var recipientProfileId = conversation.SenderUserId == userProfile.Id ? conversation.ReceiverUserId : conversation.SenderUserId;

    var recipientProfileResult = await _usersRepository.GetUserProfileByIdAsync(recipientProfileId);
    if (recipientProfileResult.IsError) return recipientProfileResult.Errors;
    var recipientProfile = recipientProfileResult.Value;

    // add domain event for real-time notification
    message.AddDomainEvent(new CreatedMessageEvent(message, conversation.Id, recipientProfile.UserId, recipientProfile.Id, request.connectionId));

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("User {userID} created a message successfully.", userId);

    return new MessageDto
    {
      Id = message.Id,
      Content = message.Content,
      SenderId = message.SenderUserId,
      SentAt = message.CreatedAt,
      ConversationId = message.ConversationId,
      Attachments = message.Files?.Select(a => new MessageAttachmentDto
      {
        Id = a.Id,
        FileName = a.File.FileName,
        FileUrl = a.File.FileUrl,
        ContentType = a.File.MimeType,
        FileSizeInBytes = a.File.FileSizeInBytes
      }).ToList()
    };
  }

}