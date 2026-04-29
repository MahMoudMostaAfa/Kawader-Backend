
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Kawadar.Api.Controllers.V1;


[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/test")]
public class TestController : ApiController
{

  private readonly AppDbContext appDbContext;
  private readonly IIdentityService _identityService;
  private readonly ILogger<TestController> logger;
  public TestController(AppDbContext appDbContext, ILogger<TestController> logger, IIdentityService identityService)
  {
    this.appDbContext = appDbContext;
    this.logger = logger;
    _identityService = identityService;
  }

  [HttpPost]
  public async Task<IActionResult> Test(CancellationToken ct)
  {
    // id1 = 456E78BD-0A16-4355-B7EC-7370BCD4DC8B
    // id2 = DA0E58F5-E9E0-4369-8C91-1388BF9C7966
    // var wallet1Result = Wallet.Create(Guid.Parse("456E78BD-0A16-4355-B7EC-7370BCD4DC8B"));
    // var wallet1 = wallet1Result.Value;
    // await appDbContext.Wallets.AddAsync(wallet1, ct);

    // var wallet2Result = Wallet.Create(Guid.Parse("DA0E58F5-E9E0-4369-8C91-1388BF9C7966"));
    // var wallet = wallet2Result.Value;

    var wallet1Result = await appDbContext.Wallets.FirstOrDefaultAsync(W => W.UserId == Guid.Parse("456E78BD-0A16-4355-B7EC-7370BCD4DC8B"), ct);


    var wallet2Result = await appDbContext.Wallets.FirstOrDefaultAsync(W => W.UserId == Guid.Parse("DA0E58F5-E9E0-4369-8C91-1388BF9C7966"), ct);

    // // logger.LogInformation("Wallet 1: {Wallet1}", wallet1Result);
    // // logger.LogInformation("Wallet 2: {Wallet2}", wallet2Result);

    // wallet1Result!.Deposit(1000m);
    // wallet2Result!.Deposit(1000m);


    // wallet2Result!.Hold(500m);
    // wallet1Result!.Hold(500m);

    // var tokenResult = await _identityService.GeneratePasswordResetTokenAsync("d66db919-d2d9-4bec-8553-f7670aa53765");
    // var token = tokenResult.Value;

    // await _identityService.ResetPasswordAsync("d66db919-d2d9-4bec-8553-f7670aa53765", token, "Mahmoud12345");

    await appDbContext.SaveChangesAsync(ct);

    return Ok();

  }
}