using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Contracts.Disbutes
{
    public class DisbuteErrors
    {
        public static Error ContractIdIsRequired => Error.Validation("Contract Id is required");
        public static Error RaisedByIdIsRequired => Error.Validation("Raised By Id is required");
        public static Error ReasonIsRequired => Error.Validation("Reason is required");
    }
}
